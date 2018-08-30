// <copyright file="Empirical.cs" company="CERN">
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
    /// Empirical distribution.
    /// <p>
    /// The probability distribution function (pdf) must be provided by the user as an array of positive real numbersd 
    /// The pdf does not need to be provided in the form of relative probabilities, absolute probabilities are also accepted.
    /// <p>
    /// If <i>interpolationType == LINEAR_INTERPOLATION</i> a linear interpolation within the bin is computed, resulting in a constant density within each bin.
    /// <dt>
    /// If <i>interpolationType == NO_INTERPOLATION</i> no interpolation is performed and the result is a discrete distributiond  
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b>
    /// A uniform random number is generated using a user supplied generator.
    /// The uniform number is then transformed to the user's distribution using the cumulative probability distribution constructed from the pdf.
    /// The cumulative distribution is inverted using a binary search for the nearest bin boundaryd 
    /// <p>
    /// This is a port of <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep/manual/RefGuide/Random/RandGeneral.html">RandGeneral</A> used in <A HREF="http://wwwinfo.cern.ch/asd/lhc++/clhep">CLHEP 1.4.0</A> (C++).
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class Empirical : AbstractContinousDistribution
    {
        protected double[] _cdf; // cumulative distribution function
        protected int interpolationType;

        public static int LINEAR_INTERPOLATION = 0;
        public static int NO_INTERPOLATION = 1;

        /// <summary>
        /// Constructs an Empirical distribution.
        /// The probability distribution function (pdf) is an array of positive real numbersd 
        /// It need not be provided in the form of relative probabilities, absolute probabilities are also accepted.
        /// The <i>pdf</i> must satisfy both of the following conditions
        /// <ul>
        /// <li><i>0.0 &lt;= pdf[i] : 0&lt;=i&lt;=pdf.Length-1</i>
        /// <li><i>0.0 &lt; Sum(pdf[i]) : 0&lt;=i&lt;=pdf.Length-1</i>
        /// </ul>
        /// </summary>
        /// <param name="pdf">the probability distribution function.</param>
        /// <param name="interpolationType">can be either <i>Empirical.NO_INTERPOLATION</i> or <i>Empirical.LINEAR_INTERPOLATION</i>.</param>
        /// <param name="randomGenerator">a uniform random number generator.</param>
        /// <exception cref="ArgumentException">if at least one of the three conditions above is violated.</exception>
        public Empirical(double[] pdf, int interpolationType, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(pdf, interpolationType);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public double CumulativeDistributionFunction(int k)
        {
            if (k < 0) return 0.0;
            if (k >= _cdf.Length - 1) return 1.0;
            return _cdf[k];
        }

        /// <summary>
        /// Returns a deep copy of the receiver; the copy will produce identical sequences.
        /// After this call has returned, the copy and the receiver have equal but separate state.
        /// </summary>
        /// <returns>a copy of the receiver.</returns>
        public new Object Clone()
        {
            Empirical copy = (Empirical)base.Clone();
            if (this._cdf != null) copy._cdf = (double[])this._cdf.Clone();
            return copy;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override double NextDouble()
        {
            double rand = randomGenerator.Raw();
            if (this._cdf == null) return rand; // Non-existing pdf

            // binary search in cumulative distribution function:
            int nBins = _cdf.Length - 1;
            int nbelow = 0;     // largest k such that I[k] is known to be <= rand
            int nabove = nBins; // largest k such that I[k] is known to be >  rand

            while (nabove > nbelow + 1)
            {
                int middle = (nabove + nbelow + 1) >> 1; // div 2
                if (rand >= _cdf[middle]) nbelow = middle;
                else nabove = middle;
            }
            // after this binary search, nabove is always nbelow+1 and they straddle rand:

            if (this.interpolationType == NO_INTERPOLATION)
            {
                return ((double)nbelow) / nBins;
            }
            else if (this.interpolationType == LINEAR_INTERPOLATION)
            {
                double binMeasure = _cdf[nabove] - _cdf[nbelow];
                // binMeasure is always aProbFunc[nbelow], 
                // but we don't have aProbFunc any more so we subtract.

                if (binMeasure == 0.0)
                {
                    // rand lies right in a bin of measure 0d  Simply return the center
                    // of the range of that bind  (Any value between k/N and (k+1)/N is
                    // equally good, in this rare cased)
                    return (nbelow + 0.5) / nBins;
                }

                double binFraction = (rand - _cdf[nbelow]) / binMeasure;
                return (nbelow + binFraction) / nBins;
            }
            else throw new InvalidOperationException(); // illegal interpolation type
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(double x)
        {
            throw new NotImplementedException();
            //if (x < 0 || x > cdf.Length-2) return 0.0;
            //int k = (int) x;
            //return cdf[k-1] - cdf[k];
        }

        /// <summary>
        /// Returns the probability distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityDistributionFunction(int k)
        {
            if (k < 0 || k >= _cdf.Length - 1) return 0.0;
            return _cdf[k - 1] - _cdf[k];
        }

        /// <summary>
        /// Sets the distribution parameters.
        /// The <i>pdf</i> must satisfy both of the following conditions
        /// <ul>
        /// <li><i>0.0 &lt;= pdf[i] : 0 &lt; =i &lt;= pdf.Length-1</i>
        /// <li><i>0.0 &lt; Sum(pdf[i]) : 0 &lt;=i &lt;= pdf.Length-1</i>
        /// </ul>
        /// </summary>
        /// <param name="pdf">probability distribution function.</param>
        /// <param name="interpolationType">can be either <i>Empirical.NO_INTERPOLATION</i> or <i>Empirical.LINEAR_INTERPOLATION</i>.</param>
        /// <exception cref="ArgumentException">if at least one of the three conditions above is violated.</exception>
        public void SetState(double[] pdf, int interpolationType)
        {
            if (interpolationType != LINEAR_INTERPOLATION &&
                interpolationType != NO_INTERPOLATION)
            {
                throw new ArgumentException("Illegal Interpolation Type");
            }
            this.interpolationType = interpolationType;

            if (pdf == null || pdf.Length == 0)
            {
                this._cdf = null;
                //throw new ArgumentException("Non-existing pdf");
                return;
            }

            // compute cumulative distribution function (cdf) from probability distribution function (pdf)
            int nBins = pdf.Length;
            this._cdf = new double[nBins + 1];

            _cdf[0] = 0;
            for (int ptn = 0; ptn < nBins; ++ptn)
            {
                double prob = pdf[ptn];
                if (prob < 0.0) throw new ArgumentException("Negative probability");
                _cdf[ptn + 1] = _cdf[ptn] + prob;
            }
            if (_cdf[nBins] <= 0.0) throw new ArgumentException("At leat one probability must be > 0.0");
            for (int ptn = 0; ptn < nBins + 1; ++ptn)
            {
                _cdf[ptn] /= _cdf[nBins];
            }
            // cdf is now cached...
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String interpolation = null;
            if (interpolationType == NO_INTERPOLATION) interpolation = "NO_INTERPOLATION";
            if (interpolationType == LINEAR_INTERPOLATION) interpolation = "LINEAR_INTERPOLATION";
            return this.GetType().Name + "(" + ((_cdf != null) ? _cdf.Length : 0) + "," + interpolation + ")";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int xnBins()
        {
            return _cdf.Length - 1;
        }
    }
}
