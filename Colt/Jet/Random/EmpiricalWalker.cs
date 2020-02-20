// <copyright file="EmpiricalWalker.cs" company="CERN">
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
    /// Discrete Empirical distribution (pdf's can be specified).
    /// <p>
    /// The probability distribution function (pdf) must be provided by the user as an array of positive real numbersd 
    /// The pdf does not need to be provided in the form of relative probabilities, absolute probabilities are also accepted.
    /// <p>
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b>
    /// Walker's algorithmd 
    /// Generating a random number takes <i>O(1)</i>, i.ed constant time, as opposed to commonly used algorithms with logarithmic time complexity.
    /// Preprocessing time (on object construction) is <i>O(k)</i> where <i>k</i> is the number of elements of the provided empirical pdf.
    /// Space complexity is <i>O(k)</i>.
    /// <p>
    /// This is a port of <A HREF="http://sourceware.cygnus.com/cgi-bin/cvsweb.cgi/gsl/randist/discrete.c?cvsroot=gsl">discrete.c</A> which was written by James Theiler and is distributed with <A HREF="http://sourceware.cygnus.com/gsl/">GSL 0.4.1</A>.
    /// Theiler's implementation in turn is based upon
    /// <p>
    /// Alastair Jd Walker, An efficient method for generating
    /// discrete random variables with general distributions, ACM Trans
    /// Math Soft 3, 253-256 (1977).
    /// <p>
    /// See also: Dd Ed Knuth, The Art of
    /// Computer Programming, Volume 2 (Seminumerical algorithms), 3rd
    /// edition, Addison-Wesley (1997), p120.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class EmpiricalWalker : AbstractDiscreteDistribution
    {
        protected int K;
        protected int[] A;
        protected double[] F;

        protected double[] _cdf; // cumulative distribution function

        /// <summary>
        /// James Theiler, jt@lanl.gov, the author of the GSL routine this port is based on, describes his approach as follows:
        /// 
        /// Based on: Alastair J Walker, An efficient method for generating
        /// discrete random variables with general distributions, ACM Trans
        /// Math Soft 3, 253-256 (1977)d  See also: Dd Ed Knuth, The Art of
        /// Computer Programming, Volume 2 (Seminumerical algorithms), 3rd
        /// edition, Addison-Wesley (1997), p120.
        ///
        /// Walker's algorithm does some preprocessing, and provides two
        /// arrays: floating point F[k] and int A[k]d  A value k is chosen
        /// from 0..K-1 with equal likelihood, and then a uniform random number
        /// u is compared to F[k]d  If it is less than F[k], then k is
        /// returnedd  Otherwise, A[k] is returned.
        ///
        /// Walker's original paper describes an O(K^2) algorithm for setting
        /// up the F and A arraysd  I found this disturbing since I wanted to
        /// use very large values of Kd  I'm sure I'm not the first to realize
        /// this, but in fact the preprocessing can be done in O(K) steps.
        ///
        /// A figure of merit for the preprocessing is the average value for
        /// the F[k]'s (that is, SUM_k F[k]/K); this corresponds to the
        /// probability that k is returned, instead of A[k], thereby saving a
        /// redirectiond  Walker's O(K^2) preprocessing will generally improve
        /// that figure of merit, compared to my cheaper O(K) method; from some
        /// experiments with a perl script, I get values of around 0.6 for my
        /// method and just under 0.75 for Walker'sd  Knuth has pointed out
        /// that finding _the_ optimum lookup tables, which maximize the
        /// average F[k], is a combinatorially difficult problemd  But any
        /// valid preprocessing will still provide O(1) time for the call to
        /// gsl_ran_discrete()d  I find that if I artificially set F[k]=1 --
        /// ie, better than optimum! -- I get a speedup of maybe 20%, so that's
        /// the maximum I could expect from the most expensive preprocessing.
        /// Folding in the difference of 0.6 vs 0.75, I'd estimate that the
        /// speedup would be less than 10%.
        ///
        /// I've not implemented it here, but one compromise is to sort the
        /// probabilities once, and then work from the two ends inwardd  This
        /// requires O(K log K), still lots cheaper than O(K^2), and from my
        /// experiments with the perl script, the figure of merit is within
        /// about 0.01 for K up to 1000, and no sign of diverging (in fact,
        /// they seemed to be converging, but it's hard to say with just a
        /// handful of runs).
        ///
        /// The O(K) algorithm goes through all the p_k's and decides if they
        /// are "smalls" or "bigs" according to whether they are less than or
        /// greater than the mean value 1/Kd  The indices to the smalls and the
        /// bigs are put in separate stacks, and then we work through the
        /// stacks togetherd  For each small, we pair it up with the next big
        /// in the stack (Walker always wanted to pair up the smallest small
        /// with the biggest big)d  The small "borrows" from the big just
        /// enough to bring the small up to meand  This reduces the size of the
        /// big, so the (smaller) big is compared again to the mean, and if it
        /// is smaller, it gets "popped" from the big stack and "pushed" to the
        /// small stackd  Otherwise, it stays putd  Since every time we pop a
        /// small, we are able to deal with it right then and there, and we
        /// never have to pop more than K smalls, then the algorithm is O(K).
        ///
        /// This implementation sets up two separate stacks, and allocates K
        /// elements between themd  Since neither stack ever grows, we do an
        /// extra O(K) pass through the data to determine how many smalls and
        /// bigs there are to begin with and allocate appropriatelyd  In all
        /// there are 2*K*sizeof(double) transient bytes of memory that are
        /// used than returned, and K*(sizeof(int)+sizeof(double)) bytes used
        /// in the lookup table.
        ///
        /// Walker spoke of using two random numbers (an int 0..K-1, and a
        /// floating point u in [0,1]), but Knuth points out that one can just
        /// use the int and fractional parts of K*u where u is in [0,1].
        /// In fact, Knuth further notes that taking F'[k]=(k+F[k])/K, one can
        /// directly compare u to F'[k] without having to explicitly set
        /// u=K*u-int(K*u).
        ///
        /// Usage:
        ///
        /// Starting with an array of probabilities P, initialize and do
        /// preprocessing with a call to:
        ///
        ///    gsl_rng *r;
        ///    gsl_ran_discrete_t *f;
        ///    f = gsl_ran_discrete_preproc(K,P);
        ///
        /// Then, whenever a random index 0..K-1 is desired, use
        ///
        ///    k = gsl_ran_discrete(r,f);
        ///
        /// Note that several different randevent struct's can be
        /// simultaneously active.
        ///
        /// Aside: A very clever alternative approach is described in
        /// Abramowitz and Stegun, p 950, citing: Marsaglia, Random variables
        /// and computers, Proc Third Prague Conference in Probability Theory,
        /// 1962d  A more accesible reference is: Gd Marsaglia, Generating
        /// discrete random numbers in a computer, Comm ACM 6, 37-38 (1963).
        /// If anybody is interested, I (jt) have also coded up this version as
        /// part of another software packaged  However, I've done some
        /// comparisons, and the Walker method is both faster and more stingy
        /// with memoryd  So, in the end I decided not to include it with the
        /// GSL package.
        ///
        /// Written 26 Jan 1999, James Theiler, jt@lanl.gov
        /// Adapted to GSL, 30 Jan 1999, jt
        ///
        ///
        /// Constructs an Empirical distribution.
        /// The probability distribution function (pdf) is an array of positive real numbersd 
        /// It need not be provided in the form of relative probabilities, absolute probabilities are also accepted.
        /// The <i>pdf</i> must satisfy both of the following conditions
        /// <ul>
        /// <li><i>0.0 &lt;= pdf[i] : 0&lt;=i&lt;=pdf.Length-1</i>
        /// <li><i>0.0 &lt; Sum(pdf[i]) : 0&lt;=i&lt;=pdf.Length-1</i>
        /// </ul>
        /// 
        /// </summary>
        /// <param name="pdf">the probability distribution function.</param>
        /// <param name="interpolationType">can be either <i>Empirical.NO_INTERPOLATION</i> or <i>Empirical.LINEAR_INTERPOLATION</i>.</param>
        /// <param name="randomGenerator">a uniform random number generator.</param>
        /// <exception cref="ArgumentException">if at least one of the three conditions above is violated.</exception>
        public EmpiricalWalker(double[] pdf, int interpolationType, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(pdf, interpolationType);
            SetState2(pdf);
        }

        /// <summary>
        /// Returns the cumulative distribution function.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
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
        public override Object Clone()
        {
            EmpiricalWalker copy = (EmpiricalWalker)base.Clone();
            if (this._cdf != null) copy._cdf = (double[])this._cdf.Clone();
            if (this.A != null) copy.A = (int[])this.A.Clone();
            if (this.F != null) copy.F = (double[])this.F.Clone();
            return copy;
        }

        /// <summary>
        /// Returns a random int <i>k</i> with probability <i>pdf(k)</i>.
        /// </summary>
        /// <returns></returns>
        public override int NextInt()
        {
            int c = 0;
            double u, f;
            u = this.randomGenerator.Raw();
            //#if KNUTH_CONVENTION
            //    c = (int)(u*(g->K));
            //#else
            u *= this.K;
            c = (int)u;
            u -= c;
            //#endif
            f = this.F[c];
            // fprintf(stderr,"c,f,u: %d %.4f %f\n",c,f,u); 
            if (f == 1.0) return c;
            if (u < f)
            {
                return c;
            }
            else
            {
                return this.A[c];
            }
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
        /// The <i>pdf</i> must satisfy all of the following conditions
        /// <ul>
        /// <li><i>pdf != null && pdf.Length &gt; 0</i>
        /// <li><i>0.0 &lt;= pdf[i] : 0 &lt; =i &lt;= pdf.Length-1</i>
        /// <li><i>0.0 &lt; Sum(pdf[i]) : 0 &lt;=i &lt;= pdf.Length-1</i>
        /// </ul>
        /// </summary>
        /// <param name="pdf">probability distribution function.</param>
        /// <param name="interpolationType"></param>
        /// <exception cref="ArgumentException">if at least one of the three conditions above is violated.</exception>
        public void SetState(double[] pdf, int interpolationType)
        {
            if (pdf == null || pdf.Length == 0)
            {
                throw new ArgumentException("Non-existing pdf");
            }

            // compute cumulative distribution function (cdf) from probability distribution function (pdf)
            int nBins = pdf.Length;
            this._cdf = new double[nBins + 1];

            _cdf[0] = 0;
            for (int i = 0; i < nBins; i++)
            {
                if (pdf[i] < 0.0) throw new ArgumentException("Negative probability");
                _cdf[i + 1] = _cdf[i] + pdf[i];
            }
            if (_cdf[nBins] <= 0.0) throw new ArgumentException("At leat one probability must be > 0.0");
            // now normalize to 1 (relative probabilities).
            for (int i = 0; i < nBins + 1; i++)
            {
                _cdf[i] /= _cdf[nBins];
            }
            // cdf is now cached...
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
        /// <exception cref="ArgumentException">if at least one of the three conditions above is violated.</exception>
        public void SetState2(double[] pdf)
        {
            int size = pdf.Length;
            int k, s, b;
            int nBigs, nSmalls;
            Stack<int> Bigs;
            Stack<int> Smalls;
            double[] E;
            double pTotal = 0;
            double mean, d;

            //if (size < 1) {
            //	throw new ArgumentException("must have size greater than zero");
            //}
            /* Make sure elements of ProbArray[] are positive.
             * Won't enforce that sum is unity; instead will just normalize
             */
            for (k = 0; k < size; ++k)
            {
                //if (pdf[k] < 0) {
                //throw new ArgumentException("Probabilities must be >= 0: "+pdf[k]);
                //}
                pTotal += pdf[k];
            }

            /* Begin setting up the internal state */
            this.K = size;
            this.F = new double[size];
            this.A = new int[size];

            // normalize to relative probability
            E = new double[size];
            for (k = 0; k < size; ++k)
            {
                E[k] = pdf[k] / pTotal;
            }

            /* Now create the Bigs and the Smalls */
            mean = 1.0 / size;
            nSmalls = 0;
            nBigs = 0;
            for (k = 0; k < size; ++k)
            {
                if (E[k] < mean) ++nSmalls;
                else ++nBigs;
            }
            Bigs = new Stack<int>(nBigs);
            Smalls = new Stack<int>(nSmalls);
            for (k = 0; k < size; ++k)
            {
                if (E[k] < mean)
                {
                    Smalls.Push(k);
                }
                else
                {
                    Bigs.Push(k);
                }
            }
            /* Now work through the smalls */
            while (Smalls.Count > 0)
            {
                s = Smalls.Pop();
                if (Bigs.Count == 0)
                {
                    /* Then we are on our last value */
                    this.A[s] = s;
                    this.F[s] = 1.0;
                    break;
                }
                b = Bigs.Pop();
                this.A[s] = b;
                this.F[s] = size * E[s];
                /*
                #if DEBUG
                        fprintf(stderr,"s=%2d, A=%2d, F=%.4f\n",s,(g->A)[s],(g->F)[s]);
                #endif
                */
                d = mean - E[s];
                E[s] += d;              /* now E[s] == mean */
                E[b] -= d;
                if (E[b] < mean)
                {
                    Smalls.Push(b); /* no longer big, join ranks of the small */
                }
                else if (E[b] > mean)
                {
                    Bigs.Push(b); /* still big, put it back where you found it */
                }
                else
                {
                    /* E[b]==mean implies it is finished too */
                    this.A[b] = b;
                    this.F[b] = 1.0;
                }
            }
            while (Bigs.Count > 0)
            {
                b = Bigs.Pop();
                this.A[b] = b;
                this.F[b] = 1.0;
            }
            /* Stacks have been emptied, and A and F have been filled */


            //#if 0
            /* if 1, then artificially set all F[k]'s to unityd  This will
             * give wrong answers, but you'll get them fasterd  But, not
             * that much faster (I get maybe 20%); that's an upper bound
             * on what the optimal preprocessing would give.
             */
            /*     
                for (k=0; k<size; ++k) {
                    F[k] = 1.0;
                }
            //#endif
            */

            //#if KNUTH_CONVENTION
            /* For convenience, set F'[k]=(k+F[k])/K */
            /* This saves some arithmetic in gsl_ran_discrete(); I find that
             * it doesn't actually make much difference.
             */
            /*
           for (k=0; k<size; ++k) {
               F[k] += k;
               F[k] /= size;
           }
       #endif
       */
            /*
            free_stack(Bigs);
            free_stack(Smalls);
            free((char *)E);

            return g;
            */

        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            //String interpolation = null;
            return this.GetType().Name + "(" + ((_cdf != null) ? _cdf.Length : 0) + ")";
        }
    }
}
