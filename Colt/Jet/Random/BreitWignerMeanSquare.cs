// <copyright file="BreitWignerMeanSquare.cs" company="CERN">
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
    /// Mean-square BreitWigner distribution; See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node23.html#SECTION000230000000000000000"> math definition</A>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronizedd 
    /// <p>
    /// <b>Implementation:</b> This is a port of <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep/manual/RefGuide/Random/RandBreitWigner.html">RandBreitWigner</A> used in <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep">CLHEP 1.4.0</A> (C++).
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class BreitWignerMeanSquare : BreitWigner
    {
        protected Uniform uniform; // helper

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static BreitWigner sharedSquare = new BreitWignerMeanSquare(1.0, 0.2, 1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a mean-squared BreitWigner distribution.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="gamma"></param>
        /// <param name="cut">cut==Double.NegativeInfinity indicates "don't cut".</param>
        /// <param name="randomGenerator"></param>
        public BreitWignerMeanSquare(double mean, double gamma, double cut, RandomEngine randomGenerator) : base(mean, gamma, cut, randomGenerator)
        {
            this.uniform = new Uniform(randomGenerator);
        }

        /// <summary>
        /// Returns a deep copy of the receiver; the copy will produce identical sequences.
        /// After this call has returned, the copy and the receiver have equal but separate state.
        /// </summary>
        /// <returns>a copy of the receiver.</returns>
        public new Object Clone()
        {
            BreitWignerMeanSquare copy = (BreitWignerMeanSquare)base.Clone();
            if (this.uniform != null) copy.uniform = new Uniform(copy.randomGenerator);
            return copy;
        }

        /// <summary>
        /// Returns a mean-squared random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="gamma"></param>
        /// <param name="cut">cut==Double.NegativeInfinity indicates "don't cut".</param>
        /// <returns></returns>
        public new double NextDouble(double mean, double gamma, double cut)
        {
            if (gamma == 0.0) return mean;
            if (cut == Double.NegativeInfinity)
            { // don't cut
                double val = System.Math.Atan(-mean / gamma);
                double rval = this.uniform.NextDoubleFromTo(val, System.Math.PI / 2.0);
                double displ = gamma * System.Math.Tan(rval);
                return System.Math.Sqrt(mean * mean + mean * displ);
            }
            else
            {
                double tmp = System.Math.Max(0.0, mean - cut);
                double lower = System.Math.Atan((tmp * tmp - mean * mean) / (mean * gamma));
                double upper = System.Math.Atan(((mean + cut) * (mean + cut) - mean * mean) / (mean * gamma));
                double rval = this.uniform.NextDoubleFromTo(lower, upper);

                double displ = gamma * System.Math.Tan(rval);
                return System.Math.Sqrt(System.Math.Max(0.0, mean * mean + mean * displ));
            }
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="gamma"></param>
        /// <param name="cut">cut==Double.NegativeInfinity indicates "don't cut".</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public new static double StaticNextDouble(double mean, double gamma, double cut)
        {
            return sharedSquare.NextDouble(mean, gamma, cut);
        }

        /// <summary>
        /// Sets the uniform random number generated shared by all <b>static</b> methods.
        /// </summary>
        /// <param name="randomGenerator">the new uniform random number generator to be shared.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void xStaticSetRandomGenerator(RandomEngine randomGenerator)
        {
            ((BreitWignerMeanSquare)sharedSquare).RandomGenerator = randomGenerator;
        }
    }
}
