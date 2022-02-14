// <copyright file="StudentT.cs" company="CERN">
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
    /// 
    /// </summary>
    public class StudentT : AbstractContinousDistribution
    {
        protected double freedom;

        protected double TERM; // performance cache for pdf()
                               // The uniform random number generated shared by all <b>static</b> methodsd 
        protected static StudentT shared = new StudentT(1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a StudentT distribution.
        /// Example: freedom=1.0.
        /// </summary>
        /// <param name="freedom">degrees of freedom.</param>
        /// <param name="randomGenerator"></param>
        /// <exception cref="ArgumentException">if <i>freedom &lt;= 0.0</i>.</exception>
        public StudentT(double freedom, RandomEngine randomGenerator)
        {
            base.RandomGenerator = randomGenerator;
            SetState(freedom);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double CumulativeDistributionFunction(double x)
        {
            return Probability.StudentT(freedom, x);
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            return NextDouble(this.freedom);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="degreesOfFreedom">a degrees of freedom.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>a &lt;= 0.0</i>.</exception>
        public double NextDouble(double degreesOfFreedom)
        {
            /*
             * The polar method of Box/Muller for generating Normal variates 
             * is adapted to the Student-t distributiond The two generated   
             * variates are not independent and the expected nod of uniforms 
             * per variate is 2.5464.
             *
             * REFERENCE :  - R.Wd Bailey (1994): Polar generation of random  
             *                variates with the t-distribution, Mathematics   
             *                of Computation 62, 779-781.
             */
            if (degreesOfFreedom <= 0.0) throw new ArgumentException();
            double u, v, w;

            do
            {
                u = 2.0 * RandomGenerator.Raw() - 1.0;
                v = 2.0 * RandomGenerator.Raw() - 1.0;
            }
            while ((w = u * u + v * v) > 1.0);

            return (u * System.Math.Sqrt(degreesOfFreedom * (System.Math.Exp(-2.0 / degreesOfFreedom * System.Math.Log(w)) - 1.0) / w));
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(double x)
        {
            return this.TERM * System.Math.Pow((1 + x * x / freedom), -(freedom + 1) * 0.5);
        }

        /// <summary>
        /// Sets the distribution parameter.
        /// </summary>
        /// <param name="freedom">degrees of freedom.</param>
        /// <exception cref="ArgumentException">if <i>freedom &lt;= 0.0</i>.</exception>
        public void SetState(double freedom)
        {
            if (freedom <= 0.0) throw new ArgumentException();
            this.freedom = freedom;

            double val = Fun.LogGamma((freedom + 1) / 2) - Fun.LogGamma(freedom / 2);
            this.TERM = System.Math.Exp(val) / System.Math.Sqrt(System.Math.PI * freedom);
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="freedom">degrees of freedom.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>freedom &lt;= 0.0</i>.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double StaticNextDouble(double freedom)
        {
                return shared.NextDouble(freedom);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + freedom + ")";
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
