// <copyright file="Exponential.cs" company="CERN">
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
    /// Exponential Distribution (aka Negative Exponential Distribution); See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node78.html#SECTION000780000000000000000"> math definition</A>
    /// <A HREF="http://www.statsoft.com/textbook/glose.html#Exponential Distribution"> animated definition</A>.
    /// <p>
    /// <i>p(x) = lambda*exp(-x*lambda)</i> for <i>x &gt;= 0</i>, <i>lambda &gt; 0</i>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// 
    /// </summary>
    public class Exponential : AbstractContinousDistribution
    {
        protected double lambda;

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static Exponential shared = new Exponential(1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a Negative Exponential distribution.
        /// </summary>
        /// <param name="lambda"></param>
        /// <param name="randomGenerator"></param>
        public Exponential(double lambda, RandomEngine randomGenerator)
        {
            base.RandomGenerator = randomGenerator;
            SetState(lambda);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double CumulativeDistributionFunction(double x)
        {
            if (x <= 0.0) return 0.0;
            return 1.0 - System.Math.Exp(-x * lambda);
        }

        /// <summary>
        ///  Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            return NextDouble(lambda);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public double NextDouble(double lambda)
        {
            return -System.Math.Log(RandomGenerator.Raw()) / lambda;
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(double x)
        {
            if (x < 0.0) return 0.0;
            return lambda * System.Math.Exp(-x * lambda);
        }

        /// <summary>
        /// Sets the mean.
        /// </summary>
        /// <param name="lambda"></param>
        public void SetState(double lambda)
        {
            this.lambda = lambda;
        }

        /// <summary>
        /// Returns a random number from the distribution with the given lambda.
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double StaticNextDouble(double lambda)
        {
            return shared.NextDouble(lambda);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + lambda + ")";
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
