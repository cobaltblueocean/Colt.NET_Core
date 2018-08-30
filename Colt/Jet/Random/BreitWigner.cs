// <copyright file="BreitWigner.cs" company="CERN">
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Math;
using Cern.Jet.Stat;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random
{
     /// <summary>
     /// BreitWigner (aka Lorentz) distribution; See the <A HREF="http://www.cern.ch/RD11/rkb/AN16pp/node23.html#SECTION000230000000000000000"> math definition</A>.
     /// A more general form of the Cauchy distribution.
     /// <p>
     /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronizedd 
     /// <dt>
     /// Static methods operate on a default uniform random number generator; they are synchronized.
     /// <p>
     /// <b>Implementation:</b> This is a port of <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep/manual/RefGuide/Random/RandBreitWigner.html">RandBreitWigner</A> used in <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep">CLHEP 1.4.0</A> (C++).
     ///
     /// @author wolfgang.hoschek@cern.ch
     /// @version 1.0, 09/24/99
           /// </summary>
    public class BreitWigner : AbstractContinousDistribution
    {
        protected double mean;
        protected double gamma;
        protected double cut;

        // The uniform random number generated shared by all <b>static</b> methods.
        protected static BreitWigner shared = new BreitWigner(1.0, 0.2, 1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a BreitWigner distribution.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="gamma"></param>
        /// <param name="cut">cut==Double.NegativeInfinity indicates "don't cut".</param>
        /// <param name="randomGenerator"></param>
        public BreitWigner(double mean, double gamma, double cut, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(mean, gamma, cut);
        }

        /// <summary>
                   /// Returns a random number from the distribution.
                   /// </summary>
                   /// <returns></returns>
        public override double NextDouble()
        {
            return NextDouble(mean, gamma, cut);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="gamma"></param>
        /// <param name="cut">cut==Double.NegativeInfinity indicates "don't cut".</param>
        /// <returns></returns>
        public double NextDouble(double mean, double gamma, double cut)
        {
            double val, rval, displ;

            if (gamma == 0.0) return mean;
            if (cut == Double.NegativeInfinity)
            { // don't cut
                rval = 2.0 * randomGenerator.Raw() - 1.0;
                displ = 0.5 * gamma * System.Math.Tan(rval * (System.Math.PI / 2.0));
                return mean + displ;
            }
            else
            {
                val = System.Math.Atan(2.0 * cut / gamma);
                rval = 2.0 * randomGenerator.Raw() - 1.0;
                displ = 0.5 * gamma * System.Math.Tan(rval * val);

                return mean + displ;
            }
        }

        /// <summary>
        /// Sets the mean, gamma and cut parameters.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="gamma"></param>
        /// <param name="cut">cut==Double.NegativeInfinity indicates "don't cut".</param>
        public void SetState(double mean, double gamma, double cut)
        {
            this.mean = mean;
            this.gamma = gamma;
            this.cut = cut;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="gamma"></param>
        /// <param name="cut">cut==Double.NegativeInfinity indicates "don't cut".</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double StaticNextDouble(double mean, double gamma, double cut)
        {
            return shared.NextDouble(mean, gamma, cut);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + mean + "," + gamma + "," + cut + ")";
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
