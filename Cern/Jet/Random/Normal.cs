// <copyright file="Normal.cs" company="CERN">
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
using Cern.Jet.Math;
using Cern.Jet.Stat;
using Cern.Jet.Random.Engine;
using System.Runtime.CompilerServices;

namespace Cern.Jet.Random
{
    /// <summary>
    /// Normal (aka Gaussian) distribution; See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node188.html#SECTION0001880000000000000000"> math definition</A>
    /// and <A HREF="http://www.statsoft.com/textbook/glosn.html#Normal Distribution"> animated definition</A>.
    /// <pre>                       
    /// 				   1                       2
    /// 	  pdf(x) = ---------    exp( - (x-mean) / 2v ) 
    /// 			   sqrt(2pi*v)
    ///
    /// 							x
    /// 							 -
    /// 				   1        | |                 2
    /// 	  cdf(x) = ---------    |    exp( - (t-mean) / 2v ) dt
    /// 			   sqrt(2pi*v)| |
    /// 						   -
    /// 						  -inf.
    /// </pre>
    /// where <i>v = variance = standardDeviation^2</i>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> Polar Box-Muller transformationd See 
    /// G.E.Pd Box, M.Ed Muller (1958): A note on the generation of random normal deviates, Annals Mathd Statistd 29, 610-611.
    /// <p>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class Normal : AbstractContinousDistribution
    {
        protected double mean;
        protected double variance;
        protected double standardDeviation;

        protected double cache; // cache for Box-Mueller algorithm 
        protected Boolean cacheFilled; // Box-Mueller

        protected double SQRT_INV; // performance cache

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static Normal shared = new Normal(0.0, 1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a normal (gauss) distribution.
        /// Example: mean=0.0, standardDeviation=1.0.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <param name="randomGenerator"></param>
        public Normal(double mean, double standardDeviation, RandomEngine randomGenerator)
        {
            this.RandomGenerator = randomGenerator;
            SetState(mean, standardDeviation);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double CumulativeDistributionFunction(double x)
        {
            return Probability.Normal(mean, variance, x);
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            return NextDouble(this.mean, this.standardDeviation);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        public double NextDouble(double mean, double standardDeviation)
        {
            // Uses polar Box-Muller transformation.
            if (cacheFilled && this.mean == mean && this.standardDeviation == standardDeviation)
            {
                cacheFilled = false;
                return cache;
            };

            double x, y, r, z;
            do
            {
                x = 2.0 * RandomGenerator.Raw() - 1.0;
                y = 2.0 * RandomGenerator.Raw() - 1.0;
                r = x * x + y * y;
            } while (r >= 1.0);

            z = System.Math.Sqrt(-2.0 * System.Math.Log(r) / r);
            cache = mean + standardDeviation * x * z;
            cacheFilled = true;
            return mean + standardDeviation * y * z;
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(double x)
        {
            double diff = x - mean;
            return SQRT_INV * System.Math.Exp(-(diff * diff) / (2.0 * variance));
        }

        /// <summary>
        /// Sets the uniform random generator internally used.
        /// </summary>
        /// <param name="randomGenerator"></param>
        public override RandomEngine RandomGenerator
        {
            get
            {
                return base.RandomGenerator;
            }
            set
            {
                base.RandomGenerator = value;
                this.cacheFilled = false;
            }
        }

        /// <summary>
        /// Sets the mean and variance.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        public void SetState(double mean, double standardDeviation)
        {
            if (mean != this.mean || standardDeviation != this.standardDeviation)
            {
                this.mean = mean;
                this.standardDeviation = standardDeviation;
                this.variance = standardDeviation * standardDeviation;
                this.cacheFilled = false;

                this.SQRT_INV = 1.0 / System.Math.Sqrt(2.0 * System.Math.PI * variance);
            }
        }

        /// <summary>
        /// Returns a random number from the distribution with the given mean and standard deviation.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double StaticNextDouble(double mean, double standardDeviation)
        {
            return shared.NextDouble(mean, standardDeviation);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + mean + "," + standardDeviation + ")";
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
