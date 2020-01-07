// <copyright file="NegativeBinomial.cs" company="CERN">
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
    /// Negative Binomial distribution; See the <A HREF="http://www.statlets.com/usermanual/glossary2.htm"> math definition</A>.
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> High performance implementationd Compound methodd 
    /// <dt>
    /// This is a port of <i>nbp.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>
    /// J.Hd Ahrens, Ud Dieter (1974): Computer methods for sampling from gamma, beta, Poisson and binomial distributions, Computing 12, 223--246.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class NegativeBinomial : AbstractDiscreteDistribution
    {
        protected int n;
        protected double p;

        protected Gamma gamma;
        protected Poisson poisson;

        // The uniform random number generated shared by all <b>static</b> methodsd 
        protected static NegativeBinomial shared = new NegativeBinomial(1, 0.5, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a Negative Binomial distribution.
        /// Example: n=1, p=0.5.
        /// </summary>
        /// <param name="n">the number of trials.</param>
        /// <param name="p">the probability of success.</param>
        /// <param name="randomGenerator">a uniform random number generator.</param>
        public NegativeBinomial(int n, double p, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetNandP(n, p);
            this.gamma = new Gamma(n, 1.0, randomGenerator);
            this.poisson = new Poisson(0.0, randomGenerator);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double CumulativeDistributionFunction(int k)
        {
            return Probability.NegativeBinomial(k, n, p);
        }

        /// <summary>
        /// Returns a deep copy of the receiver; the copy will produce identical sequences.
        /// After this call has returned, the copy and the receiver have equal but separate state.
        /// </summary>
        /// <returns>a copy of the receiver.</returns>
        public new Object Clone()
        {
            NegativeBinomial copy = (NegativeBinomial)base.Clone();
            if (this.poisson != null) copy.poisson = (Poisson)this.poisson.Clone();
            copy.poisson.RandomGenerator = copy.RandomGenerator;
            if (this.gamma != null) copy.gamma = (Gamma)this.gamma.Clone();
            copy.gamma.RandomGenerator = copy.RandomGenerator;
            return copy;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override int NextInt()
        {
            return NextInt(n, p);
        }

        /// <summary>
        /// Returns a random number from the distribution; bypasses the internal state.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public int NextInt(int n, double p)
        {
            /******************************************************************
             *                                                                *
             *        Negative Binomial Distribution - Compound method        *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION:    - nbp  samples a random number from the Negative  *
             *                Binomial distribution with parameters r (nod of *
             *                failures given) and p (probability of success)  *
             *                valid for  r > 0, 0 < p < 1d                    *
             *                If G from Gamma(r) then K  from Poiss(pG/(1-p)) *
             *                is NB(r,p)--distributedd                        *
             * REFERENCE:   - J.Hd Ahrens, Ud Dieter (1974): Computer methods *
             *                for sampling from gamma, beta, Poisson and      *
             *                binomial distributions, Computing 12, 223--246d *
             * SUBPROGRAMS: - drand(seed) ..d (0,1)-Uniform generator with    *
             *                unsigned long int *seed                     *
             *              - Gamma(seed,a) ..d Gamma generator for a > 0     *
             *                unsigned long *seed, double a                   *
             *              - Poisson(seed,a) ...Poisson generator for a > 0  *
             *                unsigned long *seed, double ad                  *
             *                                                                *
             ******************************************************************/

            double x = p / (1.0 - p);
            double p1 = p;
            double y = x * this.gamma.NextDouble(n, 1.0);
            return this.poisson.NextInt(y);
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(int k)
        {
            if (k > n) throw new ArgumentException();
            return Cern.Jet.Math.Arithmetic.Binomial(n, k) * System.Math.Pow(p, k) * System.Math.Pow(1.0 - p, n - k);
        }

        /// <summary>
        /// Sets the parameters number of trials and the probability of success.
        /// </summary>
        /// <param name="n">the number of trials</param>
        /// <param name="p">the probability of success.</param>
        public void SetNandP(int n, double p)
        {
            this.n = n;
            this.p = p;
        }

        /// <summary>
        /// Returns a random number from the distribution with the given parameters n and p.
        /// </summary>
        /// <param name="n">the number of trials</param>
        /// <param name="p">the probability of success.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int StaticNextInt(int n, double p)
        {
                return shared.NextInt(n, p);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + n + "," + p + ")";
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
