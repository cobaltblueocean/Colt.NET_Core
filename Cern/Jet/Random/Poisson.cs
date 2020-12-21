// <copyright file="Poisson.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Cern.Jet.Math;
using Cern.Jet.Stat;
using Cern.Jet.Random.Engine;
namespace Cern.Jet.Random
{
    /// <summary>
    /// Poisson distribution (quick); See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node208.html#SECTION0002080000000000000000"> math definition</A>
    /// and <A HREF="http://www.statsoft.com/textbook/glosp.html#Poisson Distribution"> animated definition</A>.
    /// <p>
    /// <i>p(k) = (mean^k / k!) * exp(-mean)</i> for <i>k &gt;= 0</i>.
    /// <p>
    /// Valid parameter ranges: <i>mean &gt; 0</i>.
    /// Note: if <i>mean &lt;= 0.0</i> then always returns zero.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> High performance implementation.
    /// Patchwork Rejection/Inversion method.
    /// <dt>This is a port of <i>pprsc.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>
    /// Hd Zechner (1994): Efficient sampling from continuous and discrete unimodal distributions,
    /// Doctoral Dissertation, 156 ppd, Technical University Graz, Austria.
    /// <p>
    /// Also see
    /// <p>
    /// Stadlober Ed, Hd Zechner (1999), <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">The patchwork rejection method for sampling from unimodal distributions</A>,
    /// to appear in ACM Transactions on Modelling and Simulation.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class Poisson : AbstractDiscreteDistribution
    {
        protected double mean;

        // precomputed and cached values (for performance only)
        // cache for < SWITCH_MEAN
        protected double my_old = -1.0;
        protected double p, q, p0;
        protected double[] pp = new double[36];
        protected int llll;

        // cache for >= SWITCH_MEAN
        protected double my_last = -1.0;
        protected double ll;
        protected int k2, k4, k1, k5;
        protected double dl, dr, r1, r2, r4, r5, lr, l_my, c_pm;
        protected double f1, f2, f4, f5, p1, p2, p3, p4, p5, p6;

        // cache for both;
        protected int m;

        protected static double MEAN_MAX = int.MaxValue; // for all means larger than that, we don't try to compute a poisson deviation, but return the mean.
        protected static double SWITCH_MEAN = 10.0; // switch from method A to method B

        // The uniform random number generated shared by all <b>static</b> methodsd 
        protected static Poisson shared = new Poisson(0.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a poisson distribution.
        /// Example: mean=1.0.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="randomGenerator"></param>
        public Poisson(double mean, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetMean(mean);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double CumulativeDistributionFunction(int k)
        {
            return Probability.Poisson(k, this.mean);
        }

        /// <summary>
        /// Returns a deep copy of the receiver; the copy will produce identical sequences.
        /// After this call has returned, the copy and the receiver have equal but separate state.
        /// </summary>
        /// <returns>a copy of the receiver.</returns>
        public override Object Clone()
        {
            Poisson copy = (Poisson)base.Clone();
            if (this.pp != null) copy.pp = (double[])this.pp.Clone();
            return copy;
        }

        private static double Function(int k, double l_nu, double c_pm)
        {
            return System.Math.Exp(k * l_nu - Arithmetic.LogFactorial(k) - c_pm);
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override int NextInt()
        {
            return NextInt(this.mean);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="theMean"></param>
        /// <returns></returns>
        public int NextInt(double theMean)
        {
            /******************************************************************
             *                                                                *
             * Poisson Distribution - Patchwork Rejection/Inversion           *
             *                                                                *
             ******************************************************************
             *                                                                *
             * For parameter  my < 10  Tabulated Inversion is appliedd        *
             * For my >= 10  Patchwork Rejection is employed:                 *
             * The area below the histogram function f(x) is rearranged in    *
             * its body by certain point reflectionsd Within a large center   *
             * interval variates are sampled efficiently by rejection from    *
             * uniform hatsd Rectangular immediate acceptance regions speed   *
             * up the generationd The remaining tails are covered by          *
             * exponential functionsd                                         *
             *                                                                *
             *****************************************************************/
            RandomEngine gen = this.RandomGenerator;
            double my = theMean;

            //double t, g, my_k;
            //double gx, gy, px, py, e, x, xx, delta, v;
            //int sign;

            //static double p,q,p0,pp[36];
            //static long ll,m;
            double u;
            int k, i;

            if (my < SWITCH_MEAN)
            { // CASE B: Inversion- start new table and calculate p0
                if (my != my_old)
                {
                    my_old = my;
                    llll = 0;
                    p = System.Math.Exp(-my);
                    q = p;
                    p0 = p;
                    //for (k=pp.Length; --k >=0; ) pp[k] = 0;
                }
                m = (my > 1.0) ? (int)my : 1;
                for (; ; )
                {
                    u = gen.Raw();           // Step Ud Uniform sample 
                    k = 0;
                    if (u <= p0) return (k);
                    if (llll != 0)
                    {              // Step Td Table comparison 
                        i = (u > 0.458) ? System.Math.Min(llll, m) : 1;
                        for (k = i; k <= llll; k++) if (u <= pp[k]) return (k);
                        if (llll == 35) continue;
                    }
                    for (k = llll + 1; k <= 35; k++)
                    { // Step Cd Creation of new probd 
                        p *= my / (double)k;
                        q += p;
                        pp[k] = q;
                        if (u <= q)
                        {
                            llll = k;
                            return (k);
                        }
                    }
                    llll = 35;
                }
            }     // end my < SWITCH_MEAN 
            else if (my < MEAN_MAX)
            { // CASE A: acceptance complement
              //static double        my_last = -1.0;
              //static long int      m,  k2, k4, k1, k5;
              //static double        dl, dr, r1, r2, r4, r5, ll, lr, l_my, c_pm,
              //  					 f1, f2, f4, f5, p1, p2, p3, p4, p5, p6;
                int Dk, X, Y;
                double Ds, U, V, W;

                m = (int)my;
                if (my != my_last)
                { //  set-up    
                    my_last = my;

                    // approximate deviation of reflection points k2, k4 from my - 1/2    
                    Ds = System.Math.Sqrt(my + 0.25);

                    // mode m, reflection points k2 and k4, and points k1 and k5, which    
                    // delimit the centre region of h(x)                                    
                    k2 = (int)System.Math.Ceiling(my - 0.5 - Ds);
                    k4 = (int)(my - 0.5 + Ds);
                    k1 = k2 + k2 - m + 1;
                    k5 = k4 + k4 - m;

                    // range width of the critical left and right centre region            
                    dl = (double)(k2 - k1);
                    dr = (double)(k5 - k4);

                    // recurrence constants r(k) = p(k)/p(k-1) at k = k1, k2, k4+1, k5+1    
                    r1 = my / (double)k1;
                    r2 = my / (double)k2;
                    r4 = my / (double)(k4 + 1);
                    r5 = my / (double)(k5 + 1);

                    // reciprocal values of the scale parameters of expond tail envelopes   
                    ll = System.Math.Log(r1);                     // expond tail left 
                    lr = -System.Math.Log(r5);                     // expond tail right

                    // Poisson constants, necessary for computing function values f(k)      
                    l_my = System.Math.Log(my);
                    c_pm = m * l_my - Arithmetic.LogFactorial(m);

                    // function values f(k) = p(k)/p(m) at k = k2, k4, k1, k5               
                    f2 = Function(k2, l_my, c_pm);
                    f4 = Function(k4, l_my, c_pm);
                    f1 = Function(k1, l_my, c_pm);
                    f5 = Function(k5, l_my, c_pm);

                    // area of the two centre and the two exponential tail regions          
                    // area of the two immediate acceptance regions between k2, k4         
                    p1 = f2 * (dl + 1.0);                    // immedd left    
                    p2 = f2 * dl + p1;               // centre left    
                    p3 = f4 * (dr + 1.0) + p2;               // immedd right     
                    p4 = f4 * dr + p3;               // centre right     
                    p5 = f1 / ll + p4;               // expond tail left 
                    p6 = f5 / lr + p5;               // expond tail right
                } // end set-up

                for (; ; )
                {
                    // generate uniform number U -- U(0, p6)                                
                    // case distinction corresponding to U                                  
                    if ((U = gen.Raw() * p6) < p2)
                    {         // centre left      

                        // immediate acceptance region R2 = [k2, m) *[0, f2),  X = k2, ..d m -1 
                        if ((V = U - p1) < 0.0) return (k2 + (int)(U / f2));
                        // immediate acceptance region R1 = [k1, k2)*[0, f1),  X = k1, ..d k2-1 
                        if ((W = V / dl) < f1) return (k1 + (int)(V / f1));

                        // computation of candidate X < k2, and its counterpart Y > k2          
                        // either squeeze-acceptance of X or acceptance-rejection of Y          
                        Dk = (int)(dl * gen.Raw()) + 1;
                        if (W <= f2 - Dk * (f2 - f2 / r2))
                        {            // quick accept of  
                            return (k2 - Dk);                          // X = k2 - Dk      
                        }
                        if ((V = f2 + f2 - W) < 1.0)
                        {                // quick reject of Y
                            Y = k2 + Dk;
                            if (V <= f2 + Dk * (1.0 - f2) / (dl + 1.0))
                            {// quick accept of  
                                return (Y);                             // Y = k2 + Dk      
                            }
                            if (V <= Function(Y, l_my, c_pm)) return (Y);    // accept of Y
                        }
                        X = k2 - Dk;
                    }
                    else if (U < p4)
                    {                                 // centre right     
                                                      // immediate acceptance region R3 = [m, k4+1)*[0, f4), X = m, ..d k4    
                        if ((V = U - p3) < 0.0) return (k4 - (int)((U - p2) / f4));
                        // immediate acceptance region R4 = [k4+1, k5+1)*[0, f5)                
                        if ((W = V / dr) < f5) return (k5 - (int)(V / f5));

                        // computation of candidate X > k4, and its counterpart Y < k4          
                        // either squeeze-acceptance of X or acceptance-rejection of Y          
                        Dk = (int)(dr * gen.Raw()) + 1;
                        if (W <= f4 - Dk * (f4 - f4 * r4))
                        {             // quick accept of  
                            return (k4 + Dk);                           // X = k4 + Dk      
                        }
                        if ((V = f4 + f4 - W) < 1.0)
                        {                 // quick reject of Y
                            Y = k4 - Dk;
                            if (V <= f4 + Dk * (1.0 - f4) / dr)
                            {       // quick accept of  
                                return (Y);                             // Y = k4 - Dk      
                            }
                            if (V <= Function(Y, l_my, c_pm)) return (Y);    // accept of Y
                        }
                        X = k4 + Dk;
                    }
                    else
                    {
                        W = gen.Raw();
                        if (U < p5)
                        {                                  // expond tail left 
                            Dk = (int)(1.0 - System.Math.Log(W) / ll);
                            if ((X = k1 - Dk) < 0) continue;          // 0 <= X <= k1 - 1 
                            W *= (U - p4) * ll;                        // W -- U(0, h(x))  
                            if (W <= f1 - Dk * (f1 - f1 / r1)) return (X); // quick accept of X
                        }
                        else
                        {                                         // expond tail right
                            Dk = (int)(1.0 - System.Math.Log(W) / lr);
                            X = k5 + Dk;                              // X >= k5 + 1      
                            W *= (U - p5) * lr;                        // W -- U(0, h(x))  
                            if (W <= f5 - Dk * (f5 - f5 * r5)) return (X); // quick accept of X
                        }
                    }

                    // acceptance-rejection test of candidate X from the original area   
                    // test, whether  W <= f(k),    with  W = U*h(x)  and  U -- U(0, 1)  
                    // log f(X) = (X - m)*log(my) - log X! + log m!                       
                    if (System.Math.Log(W) <= X * l_my - Arithmetic.LogFactorial(X) - c_pm) return (X);
                }
            }
            else
            { // mean is too large
                return (int)my;
            }
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(int k)
        {
            return System.Math.Exp(k * System.Math.Log(this.mean) - Arithmetic.LogFactorial(k) - this.mean);

            // Overflow sensitive:
            // return (System.Math.Pow(mean,k) / cephes.Arithmetic.factorial(k)) * System.Math.Exp(-this.mean);
        }

        /// <summary>
        /// Sets the mean.
        /// </summary>
        /// <param name="mean"></param>
        public void SetMean(double mean)
        {
            this.mean = mean;
        }

        /// <summary>
        /// Returns a random number from the distribution with the given mean.
        /// </summary>
        /// <param name="mean"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int StaticNextInt(double mean)
        {
                shared.SetMean(mean);
                return shared.NextInt();
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + mean + ")";
        }

        /// <summary>
        /// Sets the uniform random number generated shared by all <b>static</b> methods.
        /// </summary>
        /// <param name="randomGenerator">the new uniform random number generator to be shared.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void xStaticSetRandomGenerator(RandomEngine randomGenerator)
        {
                shared.RandomGenerator = randomGenerator;
        }
    }
}
