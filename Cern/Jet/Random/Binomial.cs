// <copyright file="Binomial.cs" company="CERN">
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
    /// Binomial distribution; See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node19.html#SECTION000190000000000000000"> math definition</A>
    /// and <A HREF="http://www.statsoft.com/textbook/glosb.html#Binomial Distribution"> animated definition</A>.
    /// <p>
    /// <i>p(x) = k * p^k * (1-p)^(n-k)</i> with <i>k = n! / (k! * (n-k)!)</i>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> High performance implementationd Acceptance Rejection/Inversion methodd 
    /// This is a port of <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep/manual/RefGuide/Random/RandBinomial.html">RandBinomial</A> used in <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep">CLHEP 1.4.0</A> (C++).
    /// CLHEP's implementation is, in turn, based on 
    /// <p>Vd Kachitvichyanukul, B.Wd Schmeiser (1988): Binomial random variate generation, Communications of the ACM 31, 216-222.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class Binomial : AbstractDiscreteDistribution
    {
        protected int n;
        protected double p;

        // cache vars for method generateBinomial(..d)
        private int n_last = -1, n_prev = -1;
        private double par, np, p0, q, p_last = -1.0, p_prev = -1.0;
        private int b, m, nm;
        private double pq, rc, ss, xm, xl, xr, ll, lr, c, p1, p2, p3, p4, ch;

        // cache vars for method pdf(..d)
        private double log_p, log_q, log_n;

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static Binomial shared = new Binomial(1, 0.5, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a binomial distribution.
        /// </summary>
        /// <param name="n">the number of trials (also known as <i>sample size</i>).</param>
        /// <param name="p">the probability of success.</param>
        /// <param name="randomGenerator">a uniform random number generator.</param>
        /// <exception cref="ArgumentException">if <i>n*System.Math.Min(p,1-p) &lt;= 0.0</i></exception>
        /// <example>
        /// n=1, p=0.5.
        /// </example>
        public Binomial(int n, double p, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetNandP(n, p);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double CumulativeDistributionFunction(int k)
        {
            return Probability.Binomial(k, n, p);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private double CumulativeDistributionFunctionSlow(int k)
        {
            if (k < 0) throw new ArgumentException();

            double sum = 0.0;
            for (int r = 0; r <= k; r++) sum += ProbabilityDistributionFunction(r);

            return sum;
        }

        /// <summary>
        /// Binomial-Distribution - Acceptance Rejection/Inversion
        /// 
        /// Acceptance Rejection method combined with Inversion for
        /// generating Binomial random numbers with parameters
        /// n (number of trials) and p (probability of success)d
        /// For  min(n*p,n*(1-p)) < 10  the Inversion method is applied:
        /// The random numbers are generated via sequential search,
        /// starting at the lowest index k=0d The cumulative probabilities
        /// are avoided by using the technique of chop-downd
        /// For  min(n*p,n*(1-p)) >= 10  Acceptance Rejection is used:
        /// The algorithm is based on a hat-function which is uniform in
        /// the centre region and exponential in the tailsd
        /// A triangular immediate acceptance region in the centre speeds
        /// up the generation of binomial variatesd
        /// If candidate k is near the mode, f(k) is computed recursively
        /// starting at the mode md
        /// The acceptance test by Stirling's formula is modified
        /// according to Wd Hoermann (1992): The generation of binomial
        /// random variates, to appear in Jd Statistd Computd Simuld
        /// If  p &lt; .5  the algorithm is applied to parameters n, pd
        /// Otherwise p is replaced by 1-p, and k is replaced by n - kd
        /// 
        /// ---------------------------------------------------------------
        /// FUNCTION:    - samples a random number from the binomial       
        ///                distribution with parameters n and p  and is    
        ///                valid for  n*min(p,1-p)  >  0d                  
        /// REFERENCE:   - Vd Kachitvichyanukul, B.Wd Schmeiser (1988):    
        ///                Binomial random variate generation,             
        ///                Communications of the ACM 31, 216-222d          
        /// SUBPROGRAMS: - StirlingCorrection()                            
        ///                            ..d Correction term of the Stirling 
        ///                                approximation for log(k!)       
        ///                                (series in 1/k or table values  
        ///                                for small k) with long int k    
        ///              - randomGenerator    ..d (0,1)-Uniform engine      
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        protected int GenerateBinomial(int n, double p)
        {
            double C1_3 = 0.33333333333333333;
            double C5_8 = 0.62500000000000000;
            double C1_6 = 0.16666666666666667;
            int DMAX_KM = 20;


            int bh, i, K, Km, nK;
            double f, rm, U, V, X, T, E;

            if (n != n_last || p != p_last)
            {                 // set-up 
                n_last = n;
                p_last = p;
                par = System.Math.Min(p, 1.0 - p);
                q = 1.0 - par;
                np = n * par;

                // Check for invalid input values

                if (np <= 0.0) return -1;

                rm = np + par;
                m = (int)rm;                              // mode, int 
                if (np < 10)
                {
                    p0 = System.Math.Exp(n * System.Math.Log(q));               // Chop-down
                    bh = (int)(np + 10.0 * System.Math.Sqrt(np * q));
                    b = System.Math.Min(n, bh);
                }
                else
                {
                    rc = (n + 1.0) * (pq = par / q);          // recurrd relat.
                    ss = np * q;                              // variance  
                    i = (int)(2.195 * System.Math.Sqrt(ss) - 4.6 * q); // i = p1 - 0.5
                    xm = m + 0.5;
                    xl = (double)(m - i);                    // limit left
                    xr = (double)(m + i + 1L);               // limit right
                    f = (rm - xl) / (rm - xl * par); ll = f * (1.0 + 0.5 * f);
                    f = (xr - rm) / (xr * q); lr = f * (1.0 + 0.5 * f);
                    c = 0.134 + 20.5 / (15.3 + (double)m);    // parallelogram
                                                              // height
                    p1 = i + 0.5;
                    p2 = p1 * (1.0 + c + c);                  // probabilities
                    p3 = p2 + c / ll;                           // of regions 1-4
                    p4 = p3 + c / lr;
                }
            }

            if (np < 10)
            {                                      //Inversion Chop-down
                double pk;

                K = 0;
                pk = p0;
                U = this.RandomGenerator.Raw();
                while (U > pk)
                {
                    ++K;
                    if (K > b)
                    {
                        U = this.RandomGenerator.Raw();
                        K = 0;
                        pk = p0;
                    }
                    else
                    {
                        U -= pk;
                        pk = (double)(((n - K + 1) * par * pk) / (K * q));
                    }
                }
                return ((p > 0.5) ? (n - K) : K);
            }

            for (; ; )
            {
                V = this.RandomGenerator.Raw();
                if ((U = this.RandomGenerator.Raw() * p4) <= p1)
                {    // triangular region
                    K = (int)(xm - U + p1 * V);
                    return (p > 0.5) ? (n - K) : K;  // immediate accept
                }
                if (U <= p2)
                {                                    // parallelogram
                    X = xl + (U - p1) / c;
                    if ((V = V * c + 1.0 - System.Math.Abs(xm - X) / p1) >= 1.0) continue;
                    K = (int)X;
                }
                else if (U <= p3)
                {                                // left tail
                    if ((X = xl + System.Math.Log(V) / ll) < 0.0) continue;
                    K = (int)X;
                    V *= (U - p2) * ll;
                }
                else
                {                                            // right tail
                    if ((K = (int)(xr - System.Math.Log(V) / lr)) > n) continue;
                    V *= (U - p3) * lr;
                }

                // acceptance test :  two cases, depending on |K - m|
                if ((Km = System.Math.Abs(K - m)) <= DMAX_KM || Km + Km + 2L >= ss)
                {

                    // computation of p(K) via recurrence relationship from the mode
                    f = 1.0;                              // f(m)
                    if (m < K)
                    {
                        for (i = m; i < K;)
                        {
                            if ((f *= (rc / ++i - pq)) < V) break;  // multiply  f
                        }
                    }
                    else
                    {
                        for (i = K; i < m;)
                        {
                            if ((V *= (rc / ++i - pq)) > f) break;  // multiply  V
                        }
                    }
                    if (V <= f) break;                               // acceptance test
                }
                else
                {

                    // lower and upper squeeze tests, based on lower bounds for log p(K)
                    V = System.Math.Log(V);
                    T = -Km * Km / (ss + ss);
                    E = (Km / ss) * ((Km * (Km * C1_3 + C5_8) + C1_6) / ss + 0.5);
                    if (V <= T - E) break;
                    if (V <= T + E)
                    {
                        if (n != n_prev || par != p_prev)
                        {
                            n_prev = n;
                            p_prev = par;

                            nm = n - m + 1;
                            ch = xm * System.Math.Log((m + 1.0) / (pq * nm)) +
                               Arithmetic.StirlingCorrection(m + 1) + Arithmetic.StirlingCorrection(nm);
                        }
                        nK = n - K + 1;

                        // computation of log f(K) via Stirling's formula
                        // acceptance-rejection test
                        if (V <= ch + (n + 1.0) * System.Math.Log((double)nm / (double)nK) +
                                     (K + 0.5) * System.Math.Log(nK * pq / (K + 1.0)) -
                                     Arithmetic.StirlingCorrection(K + 1) - Arithmetic.StirlingCorrection(nK)) break;
                    }
                }
            }
            return (p > 0.5) ? (n - K) : K;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override int NextInt()
        {
            return GenerateBinomial(n, p);
        }

        /// <summary>
        /// Returns a random number from the distribution with the given parameters n and p; bypasses the internal state.
        /// </summary>
        /// <param name="n">the number of trials</param>
        /// <param name="p">the probability of success.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>n*System.Math.Min(p,1-p) &lt;= 0.0</i></exception>
        public int NextInt(int n, double p)
        {
            if (n * System.Math.Min(p, 1 - p) <= 0.0) throw new ArgumentException();
            return GenerateBinomial(n, p);
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(int k)
        {
            if (k < 0) throw new ArgumentException();
            int r = this.n - k;
            return System.Math.Exp(this.log_n - Arithmetic.LogFactorial(k) - Arithmetic.LogFactorial(r) + this.log_p * k + this.log_q * r);
        }

        /// <summary>
        /// Sets the parameters number of trials and the probability of success.
        /// </summary>
        /// <param name="n">the number of trials</param>
        /// <param name="p">the probability of success.</param>
        /// <exception cref="ArgumentException">if <i>n*System.Math.Min(p,1-p) &lt;= 0.0</i></exception>
        public void SetNandP(int n, double p)
        {
            if (n * System.Math.Min(p, 1 - p) <= 0.0) throw new ArgumentException();
            this.n = n;
            this.p = p;

            this.log_p = System.Math.Log(p);
            this.log_q = System.Math.Log(1.0 - p);
            this.log_n = Arithmetic.LogFactorial(n);
        }

        /// <summary>
        /// Returns a random number from the distribution with the given parameters n and p.
        /// </summary>
        /// <param name="n">the number of trials</param>
        /// <param name="p">the probability of success.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>n*System.Math.Min(p,1-p) &lt;= 0.0</i></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int StaticNextInt(int n, double p)
        {
            return shared.NextInt(n, p);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + n + "," + p + ")";
        }

        /// <summary>
        /// Sets the uniform random number generated shared by all <b>static</b> methods.
        /// </summary>
        /// <param name="randomGenerator">the new uniform random number generator to be shared.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void xstaticSetRandomGenerator(RandomEngine randomGenerator)
        {
            shared.RandomGenerator = randomGenerator;
        }
    }
}
