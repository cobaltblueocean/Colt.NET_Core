// <copyright file="ChiSquare.cs" company="CERN">
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
using Cern.Jet.Stat;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random
{

    /// <summary>
    /// ChiSquare distribution; See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node31.html#SECTION000310000000000000000"> math definition</A>
    /// and <A HREF="http://www.statsoft.com/textbook/glosc.html#Chi-square Distribution"> animated definition</A>.
    /// <dt>A special case of the Gamma distribution.
    /// <p>
    /// <i>p(x) = (1/g(f/2)) * (x/2)^(f/2-1) * exp(-x/2)</i> with <i>g(a)</i> being the gamma function and <i>f</i> being the degrees of freedom.
    /// <p>
    /// Valid parameter ranges: <i>freedom &gt; 0</i>.
    /// <p> 
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> 
    /// <dt>
    /// Method: Ratio of Uniforms with shift.
    /// <dt>
    /// High performance implementationd This is a port of <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep/manual/RefGuide/Random/RandChiSquare.html">RandChiSquare</A> used in <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep">CLHEP 1.4.0</A> (C++).
    /// CLHEP's implementation, in turn, is based on <i>chru.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>J.Fd Monahan (1987): An algorithm for generating chi random variables, ACM Transd Mathd Software 13, 168-172.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class ChiSquare : AbstractContinousDistribution
    {
        protected double freedom;

        // cached vars for method nextDouble(a)(for performance only)
        private double freedom_in = -1.0, b, vm, vp, vd;

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static ChiSquare shared = new ChiSquare(1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a ChiSquare distribution.
        /// Example: freedom=1.0.
        /// </summary>
        /// <param name="freedom">degrees of freedom.</param>
        /// <param name="randomGenerator"></param>
        /// <exception cref="ArgumentException">if <i>freedom &lt; 1.0</i>.</exception>
        public ChiSquare(double freedom, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(freedom);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double CumulativeDistributionFunction(double x)
        {
            return Probability.ChiSquare(freedom, x);
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
        /// <param name="freedom">degrees of freedom. It should hold <i>freedom &lt; 1.0</i>.</param>
        /// <returns></returns>
        public double NextDouble(double freedom)
        {
            /******************************************************************
             *                                                                *
             *        Chi Distribution - Ratio of Uniforms  with shift        *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - chru samples a random number from the Chi       *
             *                distribution with parameter  a > 1d             *
             * REFERENCE :  - J.Fd Monahan (1987): An algorithm for           *
             *                generating chi random variables, ACM Transd     *
             *                Mathd Software 13, 168-172d                     *
             * SUBPROGRAM : - anEngine  ..d pointer to a (0,1)-Uniform        *
             *                engine                                          *
             *                                                                *
             * Implemented by Rd Kremer, 1990                                 *
             ******************************************************************/

            double u, v, z, zz, r;

            //if( a < 1 )  return (-1.0); // Check for invalid input value

            if (freedom == 1.0)
            {
                for (; ; )
                {
                    u = randomGenerator.Raw();
                    v = randomGenerator.Raw() * 0.857763884960707;
                    z = v / u;
                    if (z < 0) continue;
                    zz = z * z;
                    r = 2.5 - zz;
                    if (z < 0.0) r = r + zz * z / (3.0 * z);
                    if (u < r * 0.3894003915) return (z * z);
                    if (zz > (1.036961043 / u + 1.4)) continue;
                    if (2.0 * System.Math.Log(u) < (-zz * 0.5)) return (z * z);
                }
            }
            else
            {
                if (freedom != freedom_in)
                {
                    b = System.Math.Sqrt(freedom - 1.0);
                    vm = -0.6065306597 * (1.0 - 0.25 / (b * b + 1.0));
                    vm = (-b > vm) ? -b : vm;
                    vp = 0.6065306597 * (0.7071067812 + b) / (0.5 + b);
                    vd = vp - vm;
                    freedom_in = freedom;
                }
                for (; ; )
                {
                    u = randomGenerator.Raw();
                    v = randomGenerator.Raw() * vd + vm;
                    z = v / u;
                    if (z < -b) continue;
                    zz = z * z;
                    r = 2.5 - zz;
                    if (z < 0.0) r = r + zz * z / (3.0 * (z + b));
                    if (u < r * 0.3894003915) return ((z + b) * (z + b));
                    if (zz > (1.036961043 / u + 1.4)) continue;
                    if (2.0 * System.Math.Log(u) < (System.Math.Log(1.0 + z / b) * b * b - zz * 0.5 - z * b)) return ((z + b) * (z + b));
                }
            }
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(double x)
        {
            if (x <= 0.0) throw new ArgumentException();
            double logGamma = Fun.LogGamma(freedom / 2.0);
            return System.Math.Exp((freedom / 2.0 - 1.0) * System.Math.Log(x / 2.0) - x / 2.0 - logGamma) / 2.0;
        }

        /// <summary>
        /// Sets the distribution parameter.
        /// </summary>
        /// <param name="freedom">degrees of freedom.</param>
        /// <exception cref="ArgumentException">if <i>freedom &lt; 1.0</i>.</exception>
        public void SetState(double freedom)
        {
            if (freedom < 1.0) throw new ArgumentException();
            this.freedom = freedom;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="freedom">degrees of freedom.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>freedom &lt; 1.0</i>.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double staticNextDouble(double freedom)
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
        private static void xstaticSetRandomGenerator(RandomEngine randomGenerator)
        {
            shared.RandomGenerator = randomGenerator;
        }
    }
}
