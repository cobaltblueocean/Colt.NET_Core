// <copyright file="Distributions.cs" company="CERN">
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
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random
{
    /// <summary>
    /// Contains methods for conveniently generating pseudo-random numbers from special distributions such as the Burr, Cauchy, Erlang, Geometric, Lambda, Laplace, Logistic, Weibull, etc.
    /// <p>
    /// <b>About this class:</b>
    /// <dt>All distributions are obtained by using a <b>uniform</b> pseudo-random number generator.
    /// followed by a transformation to the desired distribution.
    /// <p>
    /// <b>Example usage:</b><pre>
    /// cern.jet.random.engine.RandomEngine generator;
    /// generator = new cern.jet.random.engine.MersenneTwister(new java.util.Date());
    /// //generator = new edu.cornell.lassp.houle.RngPack.Ranecu(new java.util.Date());
    /// //generator = new edu.cornell.lassp.houle.RngPack.Ranmar(new java.util.Date());
    /// //generator = new edu.cornell.lassp.houle.RngPack.Ranlux(new java.util.Date());
    /// //generator = AbstractDistribution.makeDefaultGenerator();
    /// for (int i=1000000; --i >=0; ) {
    ///    int cauchy = Distributions.nextCauchy(generator);
    ///    ...
    /// }
    /// </pre>
    ///
    /// @see cern.jet.random.engine.MersenneTwister
    /// @see java.util.Random
    /// @see java.lang.Math
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public static class Distributions
    {
        /// <summary>
        /// Returns the probability distribution function of the discrete geometric distribution.
        /// <p>
        /// <i>p(k) = p * (1-p)^k</i> for <i> k &gt;= 0</i>.
        /// <p>
        /// </summary>
        /// <param name="k">the argument to the probability distribution function.</param>
        /// <param name="p">the parameter of the probability distribution function.</param>
        /// <returns></returns>
        public static double GeometricProbalityDistributionFunction(int k, double p)
        {
            if (k < 0) throw new ArgumentException();
            return p * System.Math.Pow(1 - p, k);
        }

        /// <summary>
        /// Returns a random number from the Burr II, VII, VIII, X Distributions.
        /// <p>
        /// <b>Implementation:</b> Inversion method.
        /// This is a port of <i>burr1.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
        /// C-RAND's implementation, in turn, is based upon
        /// <p>
        /// Ld Devroye (1986): Non-Uniform Random Variate Generation, Springer Verlag, New Yorkd                                      
        /// <p>
        /// </summary>
        /// <param name="r">must be &gt; 0.</param>
        /// <param name="nr">the number of the burr distribution (e.gd 2,7,8,10).</param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextBurr1(double r, int nr, RandomEngine randomGenerator)
        {
            /******************************************************************
             *                                                                *
             *        Burr II, VII, VIII, X Distributions - Inversion         *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - burr1 samples a random number from one of the   *
             *                Burr II, VII, VIII, X distributions with        *
             *                parameter  r > 0 , where the nod of the         *
             *                distribution is indicated by a pointer          *
             *                variabled                                       *
             * REFERENCE :  - Ld Devroye (1986): Non-Uniform Random Variate   *
             *                Generation, Springer Verlag, New Yorkd          *
             * SUBPROGRAM : - drand(seed) ..d (0,1)-uniform generator with    *
             *                unsigned long int *seedd                    *
             *                                                                *
             ******************************************************************/

            double y;
            y = System.Math.Exp(System.Math.Log(randomGenerator.Raw()) / r);                                /* y=u^(1/r) */
            switch (nr)
            {
                // BURR II   
                case 2: return (-System.Math.Log(1 / y - 1));

                // BURR VII 
                case 7: return (System.Math.Log(2 * y / (2 - 2 * y)) / 2);

                // BURR VIII 
                case 8: return (System.Math.Log(System.Math.Tan(y * System.Math.PI / 2.0)));

                // BURR X    
                case 10: return (System.Math.Sqrt(-System.Math.Log(1 - y)));
            }
            return 0;
        }

        /// <summary>
         /// Returns a random number from the Burr III, IV, V, VI, IX, XII distributions.
         /// <p>
         /// <b>Implementation:</b> Inversion method.
         /// This is a port of <i>burr2.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         /// C-RAND's implementation, in turn, is based upon
         /// <p>
         /// Ld Devroye (1986): Non-Uniform Random Variate Generation, Springer Verlag, New Yorkd                                      
         /// <p>
        /// </summary>
        /// <param name="r">must be &gt; 0.</param>
        /// <param name="k">must be &gt; 0.</param>
        /// <param name="nr">the number of the burr distribution (e.gd 3,4,5,6,9,12).</param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextBurr2(double r, double k, int nr, RandomEngine randomGenerator)
        {
            /******************************************************************
             *                                                                *
             *      Burr III, IV, V, VI, IX, XII Distribution - Inversion     *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - burr2 samples a random number from one of the   *
             *                Burr III, IV, V, VI, IX, XII distributions with *
             *                parameters r > 0 and k > 0, where the nod of    *
             *                the distribution is indicated by a pointer      *
             *                variabled                                       *
             * REFERENCE :  - Ld Devroye (1986): Non-Uniform Random Variate   *
             *                Generation, Springer Verlag, New Yorkd          *
             * SUBPROGRAM : - drand(seed) ..d (0,1)-Uniform generator with    *
             *                unsigned long int *seedd                    *
             *                                                                *
             ******************************************************************/
            double y, u;
            u = randomGenerator.Raw();                     // U(0/1)       
            y = System.Math.Exp(-System.Math.Log(u) / r) - 1.0;              // u^(-1/r) - 1 
            switch (nr)
            {
                case 3:               // BURR III 
                    return (System.Math.Exp(-System.Math.Log(y) / k));      // y^(-1/k) 

                case 4:               // BURR IV  
                    y = System.Math.Exp(k * System.Math.Log(y)) + 1.0;         // y^k + 1 
                    y = k / y;
                    return (y);

                case 5:               // BURR V  
                    y = System.Math.Atan(-System.Math.Log(y / k));           // arctan[log(y/k)] 
                    return (y);

                case 6:               // BURR VI  
                    y = -System.Math.Log(y / k) / r;
                    y = System.Math.Log(y + System.Math.Sqrt(y * y + 1.0));
                    return (y);

                case 9:               // BURR IX  
                    y = 1.0 + 2.0 * u / (k * (1.0 - u));
                    y = System.Math.Exp(System.Math.Log(y) / r) - 1.0;         // y^(1/r) -1 
                    return System.Math.Log(y);

                case 12:               // BURR XII 
                    return System.Math.Exp(System.Math.Log(y) / k);        // y^(1/k) 
            }
            return 0;
        }

        /// <summary>
         /// Returns a cauchy distributed random number from the standard Cauchy distribution C(0,1)d  
         /// <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node25.html#SECTION000250000000000000000"> math definition</A>
         /// and <A HREF="http://www.statsoft.com/textbook/glosc.html#Cauchy Distribution"> animated definition</A>d 
         /// <p>
         /// <i>p(x) = 1/ (mean*pi * (1+(x/mean)^2))</i>.
         /// <p>
         /// <b>Implementation:</b>
         /// This is a port of <i>cin.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         /// <p>
        /// </summary>
        /// <param name="randomGenerator"></param>
        /// <returns>a number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).</returns>
        public static double NextCauchy(RandomEngine randomGenerator)
        {
            return System.Math.Tan(System.Math.PI * randomGenerator.Raw());
        }

        /// <summary>
        /// Returns an erlang distributed random number with the given variance and mean.
        /// </summary>
        /// <param name="variance"></param>
        /// <param name="mean"></param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextErlang(double variance, double mean, RandomEngine randomGenerator)
        {
            int k = (int)((mean * mean) / variance + 0.5);
            k = (k > 0) ? k : 1;
            double a = k / mean;

            double prod = 1.0;
            for (int i = 0; i < k; i++) prod *= randomGenerator.Raw();
            return -System.Math.Log(prod) / a;
        }

        /// <summary>
        /// Returns a discrete geometric distributed random number; <A HREF="http://www.statsoft.com/textbook/glosf.html#Geometric Distribution">Definition</A>.
        /// <p>
        /// <i>p(k) = p * (1-p)^k</i> for <i> k &gt;= 0</i>.
        /// <p>
        /// <b>Implementation:</b> Inversion method.
        /// This is a port of <i>geo.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
        /// <p>
        /// </summary>
        /// <param name="p">must satisfy <i>0 &lt; p &lt; 1</i>.</param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static int NextGeometric(double p, RandomEngine randomGenerator)
        {
            /******************************************************************
             *                                                                *
             *              Geometric Distribution - Inversion                *
             *                                                                *
             ******************************************************************
             *                                                                *
             * On generating random numbers of a discrete distribution by     *
             * Inversion normally sequential search is necessary, but in the  *
             * case of the Geometric distribution a direct transformation is  *
             * possible because of the special parallel to the continuous     *
             * Exponential distribution Exp(t):                               *
             *    X - Exp(t): G(x)=1-exp(-tx)                                 *
             *        Geo(p): pk=G(k+1)-G(k)=exp(-tk)*(1-exp(-t))             *
             *                p=1-exp(-t)                                     *
             * A random number of the Geometric distribution Geo(p) is        *
             * obtained by k=(long int)x, where x is from Exp(t) with         *
             * parameter t=-log(1-p)d                                         *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION:    - geo samples a random number from the Geometric  *
             *                distribution with parameter 0<p<1d              *
             * SUBPROGRAMS: - drand(seed) ..d (0,1)-Uniform generator with    *
             *                unsigned long int *seedd                    *
             *                                                                *
             ******************************************************************/
            double u = randomGenerator.Raw();
            return (int)(System.Math.Log(u) / System.Math.Log(1.0 - p));
        }

        /// <summary>
         /// Returns a lambda distributed random number with parameters l3 and l4.
         /// <p>
         /// <b>Implementation:</b> Inversion method.
         /// This is a port of <i>lamin.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         /// C-RAND's implementation, in turn, is based upon
         /// <p>
         /// J.Sd Ramberg, B:Wd Schmeiser (1974): An approximate method for generating asymmetric variables, Communications ACM 17, 78-82.
         /// <p>
        /// </summary>
        /// <param name="l3"></param>
        /// <param name="l4"></param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextLambda(double l3, double l4, RandomEngine randomGenerator)
        {
            double l_sign;
            if ((l3 < 0) || (l4 < 0)) l_sign = -1.0;                          // sign(l) 
            else l_sign = 1.0;

            double u = randomGenerator.Raw();                           // U(0/1) 
            double x = l_sign * (System.Math.Exp(System.Math.Log(u) * l3) - System.Math.Exp(System.Math.Log(1.0 - u) * l4));
            return x;
        }

        /// <summary>
         /// Returns a Laplace (Double Exponential) distributed random number from the standard Laplace distribution L(0,1)d  
         /// <p>
         /// <b>Implementation:</b> Inversion method.
         /// This is a port of <i>lapin.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         /// <p>
        /// </summary>
        /// <param name="randomGenerator"></param>
        /// <returns>a number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).</returns>
        public static double NextLaplace(RandomEngine randomGenerator)
        {
            double u = randomGenerator.Raw();
            u = u + u - 1.0;
            if (u > 0) return -System.Math.Log(1.0 - u);
            else return System.Math.Log(1.0 + u);
        }

        /// <summary>
        /// Returns a random number from the standard Logistic distribution Log(0,1).
        /// <p>
        /// <b>Implementation:</b> Inversion method.
        /// This is a port of <i>login.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
        /// </summary>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextLogistic(RandomEngine randomGenerator)
        {
            double u = randomGenerator.Raw();
            return (-System.Math.Log(1.0 / u - 1.0));
        }

        /// <summary>
        /// Returns a power-law distributed random number with the given exponent and lower cutoff.
        /// </summary>
        /// <param name="alpha">the exponent </param>
        /// <param name="cut">the lower cutoff</param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextPowLaw(double alpha, double cut, RandomEngine randomGenerator)
        {
            return cut * System.Math.Pow(randomGenerator.Raw(), 1.0 / (alpha + 1.0));
        }

        /// <summary>
        /// Returns a random number from the standard Triangular distribution in (-1,1).
        /// <p>
        /// <b>Implementation:</b> Inversion method.
        /// This is a port of <i>tra.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
        /// <p>
        /// </summary>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextTriangular(RandomEngine randomGenerator)
        {
            /******************************************************************
             *                                                                *
             *     Triangular Distribution - Inversion: x = +-(1-sqrt(u))     *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - tra samples a random number from the            *
             *                standard Triangular distribution in (-1,1)      *
             * SUBPROGRAM : - drand(seed) ..d (0,1)-Uniform generator with    *
             *                unsigned long int *seedd                    *
             *                                                                *
             ******************************************************************/

            double u;
            u = randomGenerator.Raw();
            if (u <= 0.5) return (System.Math.Sqrt(2.0 * u) - 1.0);                      /* -1 <= x <= 0 */
            else return (1.0 - System.Math.Sqrt(2.0 * (1.0 - u)));                 /*  0 <= x <= 1 */
        }

        /// <summary>
         /// Returns a weibull distributed random numberd 
         /// Polar method.
         /// See Simulation, Modelling & Analysis by Law & Kelton, pp259
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        public static double NextWeibull(double alpha, double beta, RandomEngine randomGenerator)
        {
            // Polar method.
            // See Simulation, Modelling & Analysis by Law & Kelton, pp259
            return System.Math.Pow(beta * (-System.Math.Log(1.0 - randomGenerator.Raw())), 1.0 / alpha);
        }

        /// <summary>
        /// Returns a zipfian distributed random number with the given skew.
        /// <p>
        /// Algorithm from page 551 of:
        /// Devroye, Luc (1986) `Non-uniform random variate generation',
        /// Springer-Verlag: Berlind   ISBN 3-540-96305-7 (also 0-387-96305-7)
        /// </summary>
        /// <param name="z">the skew of the distribution (must be &gt;1.0).</param>
        /// <param name="randomGenerator"></param>
        /// <returns>a zipfian distributed number in the closed interval <i>[1,int.MaxValue]</i>.</returns>
        public static int NextZipfInt(double z, RandomEngine randomGenerator)
        {
            /* Algorithm from page 551 of:
             * Devroye, Luc (1986) `Non-uniform random variate generation',
             * Springer-Verlag: Berlind   ISBN 3-540-96305-7 (also 0-387-96305-7)
             */
            double b = System.Math.Pow(2.0, z - 1.0);
            double constant = -1.0 / (z - 1.0);

            int result = 0;
            for (; ; )
            {
                double u = randomGenerator.Raw();
                double v = randomGenerator.Raw();
                result = (int)(System.Math.Floor(System.Math.Pow(u, constant)));
                double t = System.Math.Pow(1.0 + 1.0 / result, z - 1.0);
                if (v * result * (t - 1.0) / (b - 1.0) <= t / b) break;
            }
            return result;
        }
    }
}
