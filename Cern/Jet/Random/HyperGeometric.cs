// <copyright file="HyperGeometric.cs" company="CERN">
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
    /// HyperGeometric distribution; See the <A HREF="http://library.advanced.org/10030/6atpdvah.htm"> math definition</A>
    /// 
    /// The hypergeometric distribution with parameters <i>N</i>, <i>n</i> and <i>s</i> is the probability distribution of the random variable X, 
    /// whose value is the number of successes in a sample of <i>n</i> items from a population of size <i>N</i> that has <i>s</i> 'success' items and <i>N - s</i> 'failure' items.
    /// <p>            
    /// <i>p(k) = C(s,k) * C(N-s,n-k) / C(N,n)</i> where <i>C(a,b) = a! / (b! * (a-b)!)</i>.
    /// <p>
    /// valid for N >= 2, s,n <= Nd 
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> High performance implementation.
    /// Patchwork Rejection/Inversion method.
    /// <dt>This is a port of <i>hprsc.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>
    /// Hd Zechner (1994): Efficient sampling from continuous and discrete unimodal distributions,
    /// Doctoral Dissertation, 156 ppd, Technical University Graz, Austria.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class HyperGeometric : AbstractDiscreteDistribution
    {
        protected int my_N;
        protected int my_s;
        protected int my_n;

        // cached vars shared by hmdu(..d) and hprs(..d)
        private int N_last = -1, M_last = -1, n_last = -1;
        private int N_Mn, m;

        // cached vars for hmdu(..d)
        private int mp, b;
        private double Mp, np, fm;

        // cached vars for hprs(..d)
        private int k2, k4, k1, k5;
        private double dl, dr, r1, r2, r4, r5, ll, lr, c_pm,
                        f1, f2, f4, f5, p1, p2, p3, p4, p5, p6;


        // The uniform random number generated shared by all <b>static</b> methods.
        protected static HyperGeometric shared = new HyperGeometric(1, 1, 1, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a HyperGeometric distribution.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="s"></param>
        /// <param name="n"></param>
        /// <param name="randomGenerator"></param>
        public HyperGeometric(int N, int s, int n, RandomEngine randomGenerator)
        {
            base.RandomGenerator = randomGenerator;
            SetState(N, s, n);
        }

        private static double Fc_lnpk(int k, int N_Mn, int M, int n)
        {
            return (Arithmetic.LogFactorial(k) + Arithmetic.LogFactorial(M - k) + Arithmetic.LogFactorial(n - k) + Arithmetic.LogFactorial(N_Mn + k));
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="M"></param>
        /// <param name="n"></param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        protected int Hmdu(int N, int M, int n, RandomEngine randomGenerator)
        {

            int I, K;
            double p, nu, c, d, U;

            if (N != N_last || M != M_last || n != n_last)
            {   // set-up           */
                N_last = N;
                M_last = M;
                n_last = n;

                Mp = (double)(M + 1);
                np = (double)(n + 1); N_Mn = N - M - n;

                p = Mp / (N + 2.0);
                nu = np * p;                             /* mode, real       */
                if ((m = (int)nu) == nu && p == 0.5)
                {     /* mode, int    */
                    mp = m--;
                }
                else
                {
                    mp = m + 1;                           /* mp = m + 1       */
                }

                /* mode probability, using the external function flogfak(k) = ln(k!)    */
                fm = System.Math.Exp(Arithmetic.LogFactorial(N - M) - Arithmetic.LogFactorial(N_Mn + m) - Arithmetic.LogFactorial(n - m)
                    + Arithmetic.LogFactorial(M) - Arithmetic.LogFactorial(M - m) - Arithmetic.LogFactorial(m)
                    - Arithmetic.LogFactorial(N) + Arithmetic.LogFactorial(N - n) + Arithmetic.LogFactorial(n));

                /* safety bound  -  guarantees at least 17 significant decimal digits   */
                /*                  b = min(n, (long int)(nu + k*c')) */
                b = (int)(nu + 11.0 * System.Math.Sqrt(nu * (1.0 - p) * (1.0 - n / (double)N) + 1.0));
                if (b > n) b = n;
            }

            for (; ; )
            {
                if ((U = randomGenerator.Raw() - fm) <= 0.0) return (m);
                c = d = fm;

                /* down- and upward search from the mode                                */
                for (I = 1; I <= m; I++)
                {
                    K = mp - I;                                   /* downward search  */
                    c *= (double)K / (np - K) * ((double)(N_Mn + K) / (Mp - K));
                    if ((U -= c) <= 0.0) return (K - 1);

                    K = m + I;                                    /* upward search    */
                    d *= (np - K) / (double)K * ((Mp - K) / (double)(N_Mn + K));
                    if ((U -= d) <= 0.0) return (K);
                }

                /* upward search from K = 2m + 1 to K = b                               */
                for (K = mp + m; K <= b; K++)
                {
                    d *= (np - K) / (double)K * ((Mp - K) / (double)(N_Mn + K));
                    if ((U -= d) <= 0.0) return (K);
                }
            }
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// Uses the Patchwork Rejection method of Heinz Zechner (HPRS)
        /// </summary>
        /// <param name="N"></param>
        /// <param name="M"></param>
        /// <param name="n"></param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        protected int Hprs(int N, int M, int n, RandomEngine randomGenerator)
        {
            int Dk, X, V;
            double Mp, np, p, nu, U, Y, W;       /* (X, Y) <-> (V, W) */

            if (N != N_last || M != M_last || n != n_last)
            {  /* set-up            */
                N_last = N;
                M_last = M;
                n_last = n;

                Mp = (double)(M + 1);
                np = (double)(n + 1); N_Mn = N - M - n;

                p = Mp / (N + 2.0); nu = np * p;              // main parameters   

                // approximate deviation of reflection points k2, k4 from nu - 1/2      
                U = System.Math.Sqrt(nu * (1.0 - p) * (1.0 - (n + 2.0) / (N + 3.0)) + 0.25);

                // mode m, reflection points k2 and k4, and points k1 and k5, which    
                // delimit the centre region of h(x)                                   
                // k2 = ceil (nu - 1/2 - U),    k1 = 2*k2 - (m - 1 + delta_ml)          
                // k4 = floor(nu - 1/2 + U),    k5 = 2*k4 - (m + 1 - delta_mr)         

                m = (int)nu;
                k2 = (int)System.Math.Ceiling(nu - 0.5 - U); if (k2 >= m) k2 = m - 1;
                k4 = (int)(nu - 0.5 + U);
                k1 = k2 + k2 - m + 1;                           // delta_ml = 0      
                k5 = k4 + k4 - m;                               // delta_mr = 1      

                // range width of the critical left and right centre region             
                dl = (double)(k2 - k1);
                dr = (double)(k5 - k4);

                // recurrence constants r(k) = p(k)/p(k-1) at k = k1, k2, k4+1, k5+1    
                r1 = (np / (double)k1 - 1.0) * (Mp - k1) / (double)(N_Mn + k1);
                r2 = (np / (double)k2 - 1.0) * (Mp - k2) / (double)(N_Mn + k2);
                r4 = (np / (double)(k4 + 1) - 1.0) * (M - k4) / (double)(N_Mn + k4 + 1);
                r5 = (np / (double)(k5 + 1) - 1.0) * (M - k5) / (double)(N_Mn + k5 + 1);

                // reciprocal values of the scale parameters of expond tail envelopes   
                ll = System.Math.Log(r1);                                  // expond tail left  //
                lr = -System.Math.Log(r5);                                  // expond tail right //

                // hypergeomd constant, necessary for computing function values f(k)    
                c_pm = Fc_lnpk(m, N_Mn, M, n);

                // function values f(k) = p(k)/p(m)  at  k = k2, k4, k1, k5             
                f2 = System.Math.Exp(c_pm - Fc_lnpk(k2, N_Mn, M, n));
                f4 = System.Math.Exp(c_pm - Fc_lnpk(k4, N_Mn, M, n));
                f1 = System.Math.Exp(c_pm - Fc_lnpk(k1, N_Mn, M, n));
                f5 = System.Math.Exp(c_pm - Fc_lnpk(k5, N_Mn, M, n));

                // area of the two centre and the two exponential tail regions  
                // area of the two immediate acceptance regions between k2, k4
                p1 = f2 * (dl + 1.0);                           // immedd left       
                p2 = f2 * dl + p1;                      // centre left       
                p3 = f4 * (dr + 1.0) + p2;                      // immedd right      
                p4 = f4 * dr + p3;                      // centre right      
                p5 = f1 / ll + p4;                      // expond tail left  
                p6 = f5 / lr + p5;                      // expond tail right 
            }

            for (; ; )
            {
                // generate uniform number U -- U(0, p6)                                
                // case distinction corresponding to U                                  
                if ((U = randomGenerator.Raw() * p6) < p2)
                {    // centre left       

                    // immediate acceptance region R2 = [k2, m) *[0, f2),  X = k2, ..d m -1 
                    if ((W = U - p1) < 0.0) return (k2 + (int)(U / f2));
                    // immediate acceptance region R1 = [k1, k2)*[0, f1),  X = k1, ..d k2-1 
                    if ((Y = W / dl) < f1) return (k1 + (int)(W / f1));

                    // computation of candidate X < k2, and its counterpart V > k2          
                    // either squeeze-acceptance of X or acceptance-rejection of V          
                    Dk = (int)(dl * randomGenerator.Raw()) + 1;
                    if (Y <= f2 - Dk * (f2 - f2 / r2))
                    {            // quick accept of   
                        return (k2 - Dk);                          // X = k2 - Dk       
                    }
                    if ((W = f2 + f2 - Y) < 1.0)
                    {                // quick reject of V 
                        V = k2 + Dk;
                        if (W <= f2 + Dk * (1.0 - f2) / (dl + 1.0))
                        { // quick accept of   
                            return (V);                              // V = k2 + Dk       
                        }
                        if (System.Math.Log(W) <= c_pm - Fc_lnpk(V, N_Mn, M, n))
                        {
                            return (V);               // accept of V 
                        }
                    }
                    X = k2 - Dk;
                }
                else if (U < p4)
                {                              // centre right      

                    // immediate acceptance region R3 = [m, k4+1)*[0, f4), X = m, ..d k4    
                    if ((W = U - p3) < 0.0) return (k4 - (int)((U - p2) / f4));
                    // immediate acceptance region R4 = [k4+1, k5+1)*[0, f5)              
                    if ((Y = W / dr) < f5) return (k5 - (int)(W / f5));

                    // computation of candidate X > k4, and its counterpart V < k4          
                    // either squeeze-acceptance of X or acceptance-rejection of V          
                    Dk = (int)(dr * randomGenerator.Raw()) + 1;
                    if (Y <= f4 - Dk * (f4 - f4 * r4))
                    {            // quick accept of   
                        return (k4 + Dk);                          // X = k4 + Dk     
                    }
                    if ((W = f4 + f4 - Y) < 1.0)
                    {                // quick reject of V
                        V = k4 - Dk;
                        if (W <= f4 + Dk * (1.0 - f4) / dr)
                        {       // quick accept of   
                            return (V);                            // V = k4 - Dk       
                        }
                        if (System.Math.Log(W) <= c_pm - Fc_lnpk(V, N_Mn, M, n))
                        {
                            return (V);                            // accept of V 
                        }
                    }
                    X = k4 + Dk;
                }
                else
                {
                    Y = randomGenerator.Raw();
                    if (U < p5)
                    {                                 // expond tail left  
                        Dk = (int)(1.0 - System.Math.Log(Y) / ll);
                        if ((X = k1 - Dk) < 0) continue;         // 0 <= X <= k1 - 1  
                        Y *= (U - p4) * ll;                       // Y -- U(0, h(x))   
                        if (Y <= f1 - Dk * (f1 - f1 / r1))
                        {
                            return (X);                            // quick accept of X 
                        }
                    }
                    else
                    {                                        // expond tail right 
                        Dk = (int)(1.0 - System.Math.Log(Y) / lr);
                        if ((X = k5 + Dk) > n) continue;        // k5 + 1 <= X <= n  
                        Y *= (U - p5) * lr;                       // Y -- U(0, h(x))   /
                        if (Y <= f5 - Dk * (f5 - f5 * r5))
                        {
                            return (X);                            // quick accept of X 
                        }
                    }
                }

                // acceptance-rejection test of candidate X from the original area     
                // test, whether  Y <= f(X),    with  Y = U*h(x)  and  U -- U(0, 1)    
                // log f(X) = log( m! (M - m)! (n - m)! (N - M - n + m)! )             
                //          - log( X! (M - X)! (n - X)! (N - M - n + X)! )              
                // by using an external function for log k!                             
                if (System.Math.Log(Y) <= c_pm - Fc_lnpk(X, N_Mn, M, n)) return (X);
            }
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override int NextInt()
        {
            return NextInt(this.my_N, this.my_s, this.my_n, this.RandomGenerator);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="s"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public int NextInt(int N, int s, int n)
        {
            return NextInt(N, s, n, this.RandomGenerator);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="M"></param>
        /// <param name="n"></param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        protected int NextInt(int N, int M, int n, RandomEngine randomGenerator)
        {
            /******************************************************************
             *                                                                *
             * Hypergeometric Distribution - Patchwork Rejection/Inversion    *
             *                                                                *
             ******************************************************************
             *                                                                *
             * The basic algorithms work for parameters 1 <= n <= M <= N/2d   *
             * Otherwise parameters are re-defined in the set-up step and the *
             * random number K is adapted before deliveringd                  *
             * For l = m-max(0,n-N+M) < 10  Inversion method hmdu is applied: *
             * The random numbers are generated via modal down-up search,     *
             * starting at the mode md The cumulative probabilities           *
             * are avoided by using the technique of chop-downd               *
             * For l >= 10  the Patchwork Rejection method  hprs is employed: *
             * The area below the histogram function f(x) in its              *
             * body is rearranged by certain point reflectionsd Within a      *
             * large center interval variates are sampled efficiently by      *
             * rejection from uniform hatsd Rectangular immediate acceptance  *
             * regions speed up the generationd The remaining tails are       *
             * covered by exponential functionsd                              *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - hprsc samples a random number from the          *
             *                Hypergeometric distribution with parameters     *
             *                N (number of red and black balls), M (number    *
             *                of red balls) and n (number of trials)          *
             *                valid for N >= 2, M,n <= Nd                     *
             * REFERENCE :  - Hd Zechner (1994): Efficient sampling from      *
             *                continuous and discrete unimodal distributions, *
             *                Doctoral Dissertation, 156 ppd, Technical       *
             *                University Graz, Austriad                       *
             * SUBPROGRAMS: - flogfak(k)  ..d log(k!) with long int k     *
             *              - drand(seed) ..d (0,1)-Uniform generator with    *
             *                unsigned long int *seedd                    *
             *              - hmdu(seed,N,M,n) ..d Hypergeometric generator   *
             *                for l<10                                        *
             *              - hprs(seed,N,M,n) ..d Hypergeometric generator   *
             *                for l>=10 with unsigned long int *seed,     *
             *                long int  N , M , nd                        *
             *                                                                *
             ******************************************************************/
            int Nhalf, n_le_Nhalf, M_le_Nhalf, K;

            Nhalf = N / 2;
            n_le_Nhalf = (n <= Nhalf) ? n : N - n;
            M_le_Nhalf = (M <= Nhalf) ? M : N - M;

            if ((n * M / N) < 10)
            {
                K = (n_le_Nhalf <= M_le_Nhalf)
                    ? Hmdu(N, M_le_Nhalf, n_le_Nhalf, randomGenerator)
                    : Hmdu(N, n_le_Nhalf, M_le_Nhalf, randomGenerator);
            }
            else
            {
                K = (n_le_Nhalf <= M_le_Nhalf)
                    ? Hprs(N, M_le_Nhalf, n_le_Nhalf, randomGenerator)
                    : Hprs(N, n_le_Nhalf, M_le_Nhalf, randomGenerator);
            }

            if (n <= Nhalf)
            {
                return (M <= Nhalf) ? K : n - K;
            }
            else
            {
                return (M <= Nhalf) ? M - K : n - N + M + K;
            }
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(int k)
        {
            return Arithmetic.Binomial(my_s, k) * Arithmetic.Binomial(my_N - my_s, my_n - k)
                / Arithmetic.Binomial(my_N, my_n);
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="s"></param>
        /// <param name="n"></param>
        public void SetState(int N, int s, int n)
        {
            this.my_N = N;
            this.my_s = s;
            this.my_n = n;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="M"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double StaticNextInt(int N, int M, int n)
        {
            return shared.NextInt(N, M, n);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + my_N + "," + my_s + "," + my_n + ")";
        }

        /// <summary>
        /// Sets the uniform random number generated shared by all <b>static</b> methods.
        /// </summary>
        /// <param name="randomGenerator">the new uniform random number generator to be shared.</param>
        private static void xStaticSetRandomGenerator(RandomEngine randomGenerator)
        {
            shared.RandomGenerator = randomGenerator;
        }
    }
}
