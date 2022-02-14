// <copyright file="PoissonSlow.cs" company="CERN">
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
    /// Poisson distribution; See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node208.html#SECTION0002080000000000000000"> math definition</A>
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
    /// <b>Implementation:</b> 
    /// This is a port of <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep/manual/RefGuide/Random/RandPoisson.html">RandPoisson</A> used in <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep">CLHEP 1.4.0</A> (C++).
    /// CLHEP's implementation, in turn, is based upon "W.H.Press et ald, Numerical Recipes in C, Second Edition".
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class PoissonSlow : AbstractDiscreteDistribution
    {
        protected double mean;

        // precomputed and cached values (for performance only)
        protected double cached_sq;
        protected double cached_alxm;
        protected double cached_g;

        protected static double MEAN_MAX = int.MaxValue; // for all means larger than that, we don't try to compute a poisson deviation, but return the mean.
        protected static double SWITCH_MEAN = 12.0; // switch from method A to method B

        protected static double[] cof = { // for method logGamma() 
		76.18009172947146,-86.50532032941677,
        24.01409824083091, -1.231739572450155,
        0.1208650973866179e-2, -0.5395239384953e-5};

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static PoissonSlow shared = new PoissonSlow(0.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a poisson distribution.
        /// Example: mean=1.0.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="randomGenerator"></param>
        public PoissonSlow(double mean, RandomEngine randomGenerator)
        {
            base.RandomGenerator = randomGenerator;
            SetMean(mean);
        }

        /// <summary>
        /// Returns the value ln(Gamma(xx) for xx > 0d  Full accuracy is obtained for 
        /// xx > 1d For 0 &lt; xx &lt; 1d the reflection formula (6.1.4) can be used first.
        /// (Adapted from Numerical Recipes in C)
        /// </summary>
        /// <param name="xx"></param>
        /// <returns></returns>
        public static double LogGamma(double xx)
        {
            double x = xx - 1.0;
            double tmp = x + 5.5;
            tmp -= (x + 0.5) * System.Math.Log(tmp);
            double ser = 1.000000000190015;

            double[] coeff = cof;
            for (int j = 0; j <= 5; j++)
            {
                x++;
                ser += coeff[j] / x;
            }
            return -tmp + System.Math.Log(2.5066282746310005 * ser);
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
        private int NextInt(double theMean)
        {
            /* 
             * Adapted from "Numerical Recipes in C".
             */
            double xm = theMean;
            double g = this.cached_g;

            if (xm == -1.0) return 0; // not defined
            if (xm < SWITCH_MEAN)
            {
                int poisson = -1;
                double product = 1;
                do
                {
                    poisson++;
                    product *= RandomGenerator.Raw();
                } while (product >= g);
                // bug in CLHEP 1.4.0: was "} while ( product > g );"
                return poisson;
            }
            else if (xm < MEAN_MAX)
            {
                double t;
                double em;
                double sq = this.cached_sq;
                double alxm = this.cached_alxm;

                RandomEngine rand = this.RandomGenerator;
                do
                {
                    double y;
                    do
                    {
                        y = System.Math.Tan(System.Math.PI * rand.Raw());
                        em = sq * y + xm;
                    } while (em < 0.0);
                    em = (double)(int)(em); // faster than em = System.Math.Floor(em); (em>=0.0)
                    t = 0.9 * (1.0 + y * y) * System.Math.Exp(em * alxm - LogGamma(em + 1.0) - g);
                } while (rand.Raw() > t);
                return (int)em;
            }
            else
            { // mean is too large
                return (int)xm;
            }
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        protected int NextIntSlow()
        {
            double bound = System.Math.Exp(-mean);
            int count = 0;
            double product;
            for (product = 1.0; product >= bound && product > 0.0; count++)
            {
                product *= RandomGenerator.Raw();
            }
            if (product <= 0.0 && bound > 0.0) return (int)System.Math.Round(mean); // detected endless loop due to rounding errors
            return count - 1;
        }

        /// <summary>
        /// Sets the mean.
        /// </summary>
        /// <param name="mean"></param>
        public void SetMean(double mean)
        {
            if (mean != this.mean)
            {
                this.mean = mean;
                if (mean == -1.0) return; // not defined
                if (mean < SWITCH_MEAN)
                {
                    this.cached_g = System.Math.Exp(-mean);
                }
                else
                {
                    this.cached_sq = System.Math.Sqrt(2.0 * mean);
                    this.cached_alxm = System.Math.Log(mean);
                    this.cached_g = mean * cached_alxm - LogGamma(mean + 1.0);
                }
            }
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
