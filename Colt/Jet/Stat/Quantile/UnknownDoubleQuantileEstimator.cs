using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Cern.Jet.Random.Engine;
using Cern.Jet.Random.Sampling;
using Cern.Colt.List;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// Approximate quantile finding algorithm for unknown <i>N</i> requiring only one pass and little main memory; computes quantiles over a sequence of <i>double</i> elements.
    /// This algorithm requires at most two times the memory of a corresponding approxd quantile finder knowing <i>N</i>.
    ///
    /// <p>Needs as input the following parameters:<p>
    /// <dt>1d <i>quantiles</i> - the number of quantiles to be computed.
    /// <dt>2d <i>epsilon</i> - the allowed approximation error on quantilesd The approximation guarantee of this algorithm is explicit.
    ///
    /// <p>It is also possible to couple the approximation algorithm with random sampling to further reduce memory requirementsd 
    /// With sampling, the approximation guarantees are explicit but probabilistic, i.ed they apply with respect to a (user controlled) confidence parameter "delta".
    ///
    /// <dt>3d <i>delta</i> - the probability allowed that the approximation error fails to be smaller than epsilond Set <i>delta</i> to zero for explicit non probabilistic guarantees.
    ///
    /// You usually don't instantiate quantile finders by using the constructord Instead use the factory <i>QuantileFinderFactor</i> to do sod It will set up the right parametrization for you.
    /// 
    /// <p>After Gurmeet Singh Manku, Sridhar Rajagopalan and Bruce Gd Lindsay,
    /// Random Sampling Techniques for Space Efficient Online Computation of Order Statistics of Large Datasets.
    /// Accepted for Procd of the 1999 ACM SIGMOD Intd Confd on Management of Data,
    /// Paper (soon) available <A HREF="http://www-cad.eecs.berkeley.edu/~manku"> here</A>.
    /// </summary>
    public class UnknownDoubleQuantileEstimator : DoubleQuantileEstimator
    {

        #region Local Variables
        protected int currentTreeHeight;
        protected int treeHeightStartingSampling;
        protected WeightedRandomSampler sampler;
        protected double precomputeEpsilon;
        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs an approximate quantile finder with b buffers, each having k elements.
        /// </summary>
        /// <param name="b">the number of buffers</param>
        /// <param name="k">the number of elements per buffer</param>
        /// <param name="h">the tree height at which sampling shall start.</param>
        /// <param name="precomputeEpsilon">the epsilon for which quantiles shall be precomputed; set this value <=0.0 if nothing shall be precomputed.</param>
        /// <param name="generator">a uniform random number generator.</param>
        public UnknownDoubleQuantileEstimator(int b, int k, int h, double precomputeEpsilon, RandomEngine generator)
        {
            this.sampler = new WeightedRandomSampler(1, generator);
            SetUp(b, k);
            this.treeHeightStartingSampling = h;
            this.precomputeEpsilon = precomputeEpsilon;
            this.Clear();
        }
        #endregion

        #region Implement Methods
        protected override void NewBuffer()
        {
            CurrentBufferToFill = BufferSet.GetFirstEmptyBuffer();
            if (CurrentBufferToFill == null) throw new NullReferenceException("Oops, no empty buffer.");

            CurrentBufferToFill.Level = (currentTreeHeight - 1);
            CurrentBufferToFill.Weight = (sampler.Weight);
        }

        protected override void PostCollapse(DoubleBuffer[] toCollapse)
        {
            if (toCollapse.Length == BufferSet.BufferSize)
            { //delta for unknown finder
                currentTreeHeight++;
                if (currentTreeHeight >= treeHeightStartingSampling)
                {
                    sampler.Weight = (sampler.Weight * 2);
                }
            }

        }

        protected override bool SampleNextElement()
        {
            return sampler.SampleNextElement();
        }

        #endregion

        #region Local Public Methods

        /// <summary>
        /// Removes all elements from the receiver.  The receiver will be empty after this call returns, and its memory requirements will be close to zero.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Clear()
        {
            base.Clear();
            this.currentTreeHeight = 1;
            this.sampler.Weight = 1;
        }

        /// <summary>
        /// Returns a deep copy of the receiver.
        /// </summary>
        /// <returns></returns>
        public override Object Clone()
        {
            UnknownDoubleQuantileEstimator copy = (UnknownDoubleQuantileEstimator)base.Clone();
            if (this.sampler != null) copy.sampler = (WeightedRandomSampler)copy.sampler.Clone();
            return copy;
        }

        /// <summary>
        /// Computes the specified quantile elements over the values previously added.
        /// </summary>
        /// <param name="phis">the quantiles for which elements are to be computed. Each phi must be in the interval (0.0,1.0]. <tt>phis</tt> must be sorted ascending.</param>
        /// <returns>the approximate quantile elements.</returns>
        public override DoubleArrayList QuantileElements(DoubleArrayList phis)
        {
            if (precomputeEpsilon <= 0.0) return base.QuantileElements(phis);

            int quantilesToPrecompute = (int)Utils.EpsilonCeiling(1.0 / precomputeEpsilon);
            /*
            if (phis.Count > quantilesToPrecompute) {
                // illegal use case!
                // we compute results, but loose explicit approximation guarantees.
                return base.QuantileElements(phis);
            }
            */

            //select that quantile from the precomputed set that corresponds to a position closest to phi.
            phis = phis.Copy();
            double e = precomputeEpsilon;
            for (int index = phis.Size; --index >= 0;)
            {
                double phi = phis[index];
                int i = (int)System.Math.Round(((2.0 * phi / e) - 1.0) / 2.0); // finds closest
                i = System.Math.Min(quantilesToPrecompute - 1, System.Math.Max(0, i));
                double augmentedPhi = (e / 2.0) * (1 + 2 * i);
                phis[index] = augmentedPhi;
            }

            return base.QuantileElements(phis);
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder(base.ToString());
            buf.Length = (buf.Length - 1);
            return buf + ", h=" + currentTreeHeight + ", hStartSampling=" + treeHeightStartingSampling + ", precomputeEpsilon=" + precomputeEpsilon + ")";
        }
        #endregion

        #region Local Protected Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected new DoubleBuffer[] BuffersToCollapse()
        {
            DoubleBuffer[] fullBuffers = BufferSet.GetFullOrPartialBuffers();

            SortAscendingByLevel(fullBuffers);

            // if there is only one buffer at the lowest level, then increase its level so that there are at least two at the lowest level.
            int minLevel = fullBuffers[1].Level;
            if (fullBuffers[0].Level < minLevel)
            {
                fullBuffers[0].Level = minLevel;
            }

            return BufferSet.GetFullOrPartialBuffersWithLevel(minLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullBuffers"></param>
        protected static void SortAscendingByLevel(DoubleBuffer[] fullBuffers)
        {
            new List<DoubleBuffer>(fullBuffers).Sort(0, fullBuffers.Length - 1, new DoubleQuantileEstimatorComparer());
        }

        #endregion

        #region Local Private Methods

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DoubleQuantileEstimatorComparer : IComparer<DoubleBuffer>
    {
        public int Compare(DoubleBuffer n1, DoubleBuffer n2)
        {
            int l1 = ((DoubleBuffer)n1).Level;
            int l2 = ((DoubleBuffer)n2).Level;
            return l1 < l2 ? -1 : l1 == l2 ? 0 : +1;
        }
    }
}
