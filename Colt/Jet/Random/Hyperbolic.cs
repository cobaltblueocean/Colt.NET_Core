// <copyright file="Hyperbolic.cs" company="CERN">
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
    /// Hyperbolic distributiond 
    /// <p>
    /// Valid parameter ranges: <i>alpha &gt; 0</i> and <i>beta &gt; 0</i>d            
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b>
    /// <dt>Method: Non-Universal Rejection.
    /// High performance implementation.
    /// <dt>This is a port of <i>hyplc.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>
    /// Ld Devroye (1986): Non-Uniform Random Variate Generation, Springer Verlag, New York.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// 
    /// </summary>
    public class Hyperbolic : AbstractContinousDistribution
    {
        protected double alpha;
        protected double beta;

        // cached values shared for generateHyperbolic(..d)
        protected double a_setup = 0.0, b_setup = -1.0;
        protected double x, u, v, e;
        protected double hr, hl, s, pm, pr, samb, pmr, mpa_1, mmb_1;


        // The uniform random number generated shared by all <b>static</b> methods.
        protected static Hyperbolic shared = new Hyperbolic(10.0, 10.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a Beta distribution.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="randomGenerator"></param>
        public Hyperbolic(double alpha, double beta, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(alpha, beta);
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            return NextDouble(alpha, beta);
        }

        /// <summary>
        /// Returns a hyperbolic distributed random number; bypasses the internal state.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public double NextDouble(double alpha, double beta)
        {
            /******************************************************************
             *                                                                *
             *        Hyperbolic Distribution - Non-Universal Rejection       *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION   : - hyplc.c samples a random number from the        *
             *                hyperbolic distribution with shape parameter a  *
             *                and b valid for a>0 and |b|<a using the         *
             *                non-universal rejection method for log-concave  *
             *                densitiesd                                      *
             * REFERENCE :  - Ld Devroye (1986): Non-Uniform Random Variate   *
             *                Generation, Springer Verlag, New Yorkd          *
             * SUBPROGRAM : - drand(seed) ..d (0,1)-Uniform generator with    *
             *                unsigned long int *seedd                    *
             *                                                                *
             ******************************************************************/
            double a = alpha;
            double b = beta;

            if ((a_setup != a) || (b_setup != b))
            { // SET-UP 
                double mpa, mmb, mode;
                double amb;
                double a_, b_, a_1, b_1;  //, pl
                double help_1, help_2;
                amb = a * a - b * b;                                        // a^2 - b^2 
                samb = System.Math.Sqrt(amb);                                  // -log(f(mode)) 
                mode = b / samb;                                          // mode 
                help_1 = a * System.Math.Sqrt(2.0 * samb + 1.0);
                help_2 = b * (samb + 1.0);
                mpa = (help_2 + help_1) / amb;   // fr^-1(exp(-sqrt(a^2 - b^2) - 1.0)) 
                mmb = (help_2 - help_1) / amb;   // fl^-1(exp(-sqrt(a^2 - b^2) - 1.0))
                a_ = mpa - mode;
                b_ = -mmb + mode;
                hr = -1.0 / (-a * mpa / System.Math.Sqrt(1.0 + mpa * mpa) + b);
                hl = 1.0 / (-a * mmb / System.Math.Sqrt(1.0 + mmb * mmb) + b);
                a_1 = a_ - hr;
                b_1 = b_ - hl;
                mmb_1 = mode - b_1;                                     // lower border
                mpa_1 = mode + a_1;                                     // upper border 

                s = (a_ + b_);
                pm = (a_1 + b_1) / s;
                pr = hr / s;
                pmr = pm + pr;

                a_setup = a;
                b_setup = b;
            }

            // GENERATOR 
            for (; ; )
            {
                u = randomGenerator.Raw();
                v = randomGenerator.Raw();
                if (u <= pm)
                { // Rejection with a uniform majorizing function
                  // over the body of the distribution 
                    x = mmb_1 + u * s;
                    if (System.Math.Log(v) <= (-a * System.Math.Sqrt(1.0 + x * x) + b * x + samb)) break;
                }
                else
                {
                    if (u <= pmr)
                    {  // Rejection with an exponential envelope on the
                       // right side of the mode 
                        e = -System.Math.Log((u - pm) / pr);
                        x = mpa_1 + hr * e;
                        if ((System.Math.Log(v) - e) <= (-a * System.Math.Sqrt(1.0 + x * x) + b * x + samb)) break;
                    }
                    else
                    {           // Rejection with an exponential envelope on the
                                // left side of the mode 
                        e = System.Math.Log((u - pmr) / (1.0 - pmr));
                        x = mmb_1 + hl * e;
                        if ((System.Math.Log(v) + e) <= (-a * System.Math.Sqrt(1.0 + x * x) + b * x + samb)) break;
                    }
                }
            }

            return (x);
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        public void SetState(double alpha, double beta)
        {
            this.alpha = alpha;
            this.beta = beta;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double StaticNextDouble(double alpha, double beta)
        {
                return shared.NextDouble(alpha, beta);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + alpha + "," + beta + ")";
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
