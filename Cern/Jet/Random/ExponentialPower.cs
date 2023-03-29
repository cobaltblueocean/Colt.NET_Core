// <copyright file="ExponentialPower.cs" company="CERN">
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
    /// Exponential Power distribution.
    /// <p>
    /// Valid parameter ranges: <i>tau &gt;= 1</i>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b>
    /// <dt>Method: Non-universal rejection method for logconcave densities.
    /// <dt>This is a port of <i>epd.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>
    /// Ld Devroye (1986): Non-Uniform Random Variate Generation , Springer Verlag, New York.
    /// <p>
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class ExponentialPower : AbstractContinousDistribution
    {
        protected double tau;

        // cached vars for method nextDouble(tau)(for performance only)
        private double s, sm1, tau_set = -1.0;

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static ExponentialPower shared = new ExponentialPower(1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs an Exponential Power distribution.
        /// Example: tau=1.0.
        /// </summary>
        /// <param name="tau"></param>
        /// <param name="randomGenerator"></param>
        /// <exception cref="ArgumentException">if <i>tau &lt; 1.0</i>.</exception>
        public ExponentialPower(double tau, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(tau);
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            return NextDouble(this.tau);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="tau"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>tau &lt; 1.0</i>.</exception>
        public double NextDouble(double tau)
        {
            double u, u1, v, x, y;

            if (tau != tau_set)
            { // SET-UP 
                s = 1.0 / tau;
                sm1 = 1.0 - s;

                tau_set = tau;
            }

            // GENERATOR 
            do
            {
                u = this.RandomGenerator.Raw();                             // U(0/1)      
                u = (2.0 * u) - 1.0;                                     // U(-1.0/1.0) 
                u1 = System.Math.Abs(u);                                      // u1=|u|     
                v = this.RandomGenerator.Raw();                             // U(0/1) 

                if (u1 <= sm1)
                { // Uniform hat-function for x <= (1-1/tau)   
                    x = u1;
                }
                else
                { // Exponential hat-function for x > (1-1/tau) 
                    y = tau * (1.0 - u1);                                // U(0/1) 
                    x = sm1 - s * System.Math.Log(y);
                    v = v * y;
                }
            }

            // Acceptance/Rejection
            while (System.Math.Log(v) > -System.Math.Exp(System.Math.Log(x) * tau));

            // Random sign 
            if (u < 0.0)
                return x;
            else
                return -x;
        }

        /// <summary>
        /// Sets the distribution parameter.
        /// </summary>
        /// <param name="tau"></param>
        /// <exception cref="ArgumentException">if <i>tau &lt; 1.0</i>.</exception>
        public void SetState(double tau)
        {
            if (tau < 1.0) throw new ArgumentException();
            this.tau = tau;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="tau"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>tau &lt; 1.0</i>.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double StaticNextDouble(double tau)
        {
                return shared.NextDouble(tau);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + tau + ")";
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
