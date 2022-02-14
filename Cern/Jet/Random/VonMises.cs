// <copyright file="VonMises.cs" company="CERN">
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
    /// Von Mises distribution.
    /// <p>
    /// Valid parameter ranges: <i>k &gt; 0</i>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> 
    /// <dt>
    /// Method: Acceptance Rejection.
    /// <dt>
    /// This is a port of <i>mwc.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>
    /// D.Jd Best, N.Id Fisher (1979): Efficient simulation of the von Mises distribution, Appld Statistd 28, 152-157.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class VonMises : AbstractContinousDistribution
    {
        protected double my_k;

        // cached vars for method nextDouble(a)(for performance only)
        private double k_set = -1.0;
        private double tau, rho, r;

        // The uniform random number generated shared by all <b>static</b> methodsd 
        protected static VonMises shared = new VonMises(1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a Von Mises distribution.
        /// Example: k=1.0.
        /// </summary>
        /// <param name="freedom"></param>
        /// <param name="randomGenerator"></param>
        /// <exception cref="ArgumentException">if <i>k &lt;= 0.0</i>.</exception>
        public VonMises(double freedom, RandomEngine randomGenerator)
        {
            base.RandomGenerator = randomGenerator;
            SetState(freedom);
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            return NextDouble(this.my_k);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>k &lt;= 0.0</i>.</exception>
        public double NextDouble(double k)
        {
            /******************************************************************
             *                                                                *
             *         Von Mises Distribution - Acceptance Rejection          *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :  - mwc samples a random number from the von Mises   *
             *               distribution ( -Pi <= x <= Pi) with parameter    *
             *               k > 0  via  rejection from the wrapped Cauchy    *
             *               distibutiond                                     *
             * REFERENCE:  - D.Jd Best, N.Id Fisher (1979): Efficient         *
             *               simulation of the von Mises distribution,        *
             *               Appld Statistd 28, 152-157d                      *
             * SUBPROGRAM: - drand(seed) ..d (0,1)-Uniform generator with     *
             *               unsigned long int *seedd                     *
             *                                                                *
             * Implemented by Fd Niederl, August 1992                         *
             ******************************************************************/
            double u, v, w, c, z;

            if (k <= 0.0) throw new ArgumentException();

            if (k_set != k)
            {                                               // SET-UP
                tau = 1.0 + System.Math.Sqrt(1.0 + 4.0 * k * k);
                rho = (tau - System.Math.Sqrt(2.0 * tau)) / (2.0 * k);
                r = (1.0 + rho * rho) / (2.0 * rho);
                k_set = k;
            }

            // GENERATOR 
            do
            {
                u = RandomGenerator.Raw();                                // U(0/1) 
                v = RandomGenerator.Raw();                                // U(0/1) 
                z = System.Math.Cos(System.Math.PI * u);
                w = (1.0 + r * z) / (r + z);
                c = k * (r - w);
            } while ((c * (2.0 - c) < v) && (System.Math.Log(c / v) + 1.0 < c));         // Acceptance/Rejection 

            return (RandomGenerator.Raw() > 0.5) ? System.Math.Acos(w) : -System.Math.Acos(w);        // Random sign //
                                                                                                      // 0 <= x <= Pi : -Pi <= x <= 0 //
        }

        /// <summary>
        /// Sets the distribution parameter.
        /// </summary>
        /// <param name="k"></param>
        /// <exception cref="ArgumentException">if <i>k &lt;= 0.0</i>.</exception>
        public void SetState(double k)
        {
            if (k <= 0.0) throw new ArgumentException();
            this.my_k = k;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="freedom"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>k &lt;= 0.0</i>.</exception>
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
            return this.GetType().Name + "(" + my_k + ")";
        }

        /// <summary>
        /// Sets the uniform random number generated shared by all <b>static</b> methods.
        /// </summary>
        /// <param name="randomGenerator">the new uniform random number generator to be shared.</param>
        ///   
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void xStaticSetRandomGenerator(RandomEngine randomGenerator)
        {
            shared.RandomGenerator = randomGenerator;
        }
    }
}
