using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// The abstract base class for approximate quantile finders computing quantiles over a sequence of <tt>double</tt> elements.
    /// </summary>
    [Serializable]
    public abstract class DoubleQuantileEstimator : Colt.PersistentObject, IDoubleQuantileFinder
    {

        #region Local Variables
        private DoubleBufferSet bufferSet;
        private DoubleBuffer currentBufferToFill;
        private int totalElementsFilled;
        #endregion

        #region Property
        /// <summary>
        /// 
        /// </summary>
        DoubleBufferSet BufferSet
        {
            get { return bufferSet; }
            set { bufferSet = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        DoubleBuffer CurrentBufferToFill
        {
            get { return currentBufferToFill; }
            set { currentBufferToFill = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        int TotalElementsFilled
        {
            get { return totalElementsFilled; }
        }
        #endregion

        #region Implemented Property
        /// <summary>
        /// 
        /// </summary>
        public long Size
        {
            get
            {
                return totalElementsFilled;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected DoubleQuantileEstimator() { }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// 
        /// </summary>
        protected abstract void NewBuffer();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toCollapse"></param>
        protected abstract void PostCollapse(DoubleBuffer[] toCollapse);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract Boolean SampleNextElement();

        #endregion

        #region Implement Methods
        /// <summary>
        /// Adds a value to the receiver.
        /// </summary>
        /// <param name="value">the value to add.</param>
        public void Add(double value)
        {
            totalElementsFilled++;
            if (!SampleNextElement()) return;

            //System.out.println("adding "+value);

            if (currentBufferToFill == null)
            {
                if (bufferSet.GetFirstEmptyBuffer() == null) Collapse();
                NewBuffer();
            }

            currentBufferToFill.Add(value);
            if (currentBufferToFill.IsFull) currentBufferToFill = null;
        }

        /// <summary>
        /// Adds all values of the specified list to the receiver.
        /// </summary>
        /// <param name="values">the list of which all values shall be added.</param>
        public void AddAllOf(List<double> values)
        {
            AddAllOfFromTo(values, 0, values.Count - 1);
        }

        /// <summary>
        /// Adds the part of the specified list between indexes <tt>from</tt> (inclusive) and <tt>to</tt> (inclusive) to the receiver.
        /// </summary>
        /// <param name="values">the list of which elements shall be added.</param>
        /// <param name="from">the index of the first element to be added (inclusive).</param>
        /// <param name="to">the index of the last element to be added (inclusive).</param>
        public void AddAllOfFromTo(List<double> values, int from, int to)
        {
            /*
            // the obvious version, but we can do quicker...
            double[] theValues = values.ToArray();
            int theSize=values.Count;
            for (int i=0; i<theSize; ) add(theValues[i++]);
            */

            double[] valuesToAdd = values.ToArray();
            int k = this.bufferSet.NumberOfElements;
            int bufferSize = k;
            double[] bufferValues = null;
            if (currentBufferToFill != null)
            {
                bufferValues = currentBufferToFill.Values.ToArray();
                bufferSize = currentBufferToFill.Size;
            }

            for (int i = from - 1; ++i <= to;)
            {
                if (SampleNextElement())
                {
                    if (bufferSize == k)
                    { // full
                        if (bufferSet.GetFirstEmptyBuffer() == null) Collapse();
                        NewBuffer();
                        if (!currentBufferToFill.IsAllocated) currentBufferToFill.Allocate();
                        currentBufferToFill.IsSorted = false;
                        bufferValues = currentBufferToFill.Values.ToArray();
                        bufferSize = 0;
                    }

                    bufferValues[bufferSize++] = valuesToAdd[i];
                    if (bufferSize == k)
                    { // full
                        currentBufferToFill.Values.SetSize(bufferSize);
                        currentBufferToFill = null;
                    }
                }
            }
            if (this.currentBufferToFill != null)
            {
                this.currentBufferToFill.Values.SetSize(bufferSize);
            }

            this.totalElementsFilled += to - from + 1;
        }

        /// <summary>
        /// Removes all elements from the receiver.  The receiver will
        /// be empty after this call returns, and its memory requirements will be close to zero.
        /// </summary>
        public void Clear()
        {
            this.totalElementsFilled = 0;
            this.currentBufferToFill = null;
            this.bufferSet.Clear();
        }

        /// <summary>
        /// Returns a deep copy of the receiver.
        /// </summary>
        /// <returns>a deep copy of the receiver.</returns>
        public new Object Clone()
        {
            DoubleQuantileEstimator copy = (DoubleQuantileEstimator)base.Clone();
            if (this.bufferSet != null)
            {
                copy.bufferSet = (DoubleBufferSet)copy.bufferSet.Clone();
                if (this.currentBufferToFill != null)
                {
                    int index = new List<Object> (this.bufferSet.Buffers).IndexOf(this.currentBufferToFill);
                    copy.currentBufferToFill = copy.bufferSet.Buffers[index];
                }
            }
            return copy;
        }

        /// <summary>
        /// Applies a procedure to each element of the receiver, if any.
        /// Iterates over the receiver in no particular order.
        /// </summary>
        /// <param name="procedure">the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues.</param>
        /// <returns><tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise. </returns>
        public bool ForEach(DoubleProcedure procedure)
        {
            return this.bufferSet.ForEach(procedure);
        }

        /// <summary>
        /// Returns the number of elements currently needed to store all contained elements.
        /// This number usually differs from the results of method <tt>size()</tt>, according to the underlying datastructure.
        /// </summary>
        /// <returns></returns>
        public long Memory()
        {
            return bufferSet.Memory();
        }

        /// <summary>
        /// Returns how many percent of the elements contained in the receiver are <tt>&lt;= element</tt>.
        /// Does linear interpolation if the element is not contained but lies in between two contained elements.
        /// </summary>
        /// <param name="element">the element to search for.</param>
        /// <returns>the percentage <tt>p</tt> of elements <tt>&lt;= element</tt> (<tt>0.0 &lt;= p &lt;=1.0)</tt>.</returns>
        public double Phi(double element)
        {
            return bufferSet.Phi(element);
        }

        /// <summary>
        /// Computes the specified quantile elements over the values previously added.
        /// </summary>
        /// <param name="phis">the quantiles for which elements are to be computed. Each phi must be in the interval [0.0,1.0]. <tt>phis</tt> must be sorted ascending.</param>
        /// <returns>the approximate quantile elements.</returns>
        public List<double> QuantileElements(List<double> phis)
        {
            /*
            //check parameter
            List<Double> sortedPhiList = phis.copy();
            sortedPhiList.sort();
            if (! phis.Equals(sortedPhiList)) {
                throw new ArgumentException("Phis must be sorted ascending.");
            }
            */

            //System.out.println("starting to augment missing values, if necessary...");

            phis = PreProcessPhis(phis);

            long[] triggerPositions = new long[phis.Count];
            long totalSize = this.bufferSet.TotalSize;
            for (int i = phis.Count; --i >= 0;)
            {
                triggerPositions[i] = Utils.EpsilonCeiling(phis[i] * totalSize) - 1;
            }

            //System.out.println("triggerPositions="+cern.colt.Arrays.ToString(triggerPositions));
            //System.out.println("starting to determine quantiles...");
            //System.out.println(bufferSet);

            DoubleBuffer[] fullBuffers = bufferSet.GetFullOrPartialBuffers();
            double[] quantileElements = new double[phis.Count];

            //do the main work: determine values at given positions in sorted sequence
            return new List<Double>(bufferSet.GetValuesAtPositions(fullBuffers, triggerPositions));
        }

        /// <summary>
        /// Returns the number of elements currently needed to store all contained elements.
        /// This number usually differs from the results of method <tt>size()</tt>, according to the underlying datastructure.
        /// </summary>
        /// <returns></returns>
        public long TotalMemory()
        {
            return bufferSet.BufferSize * bufferSet.NumberOfElements;
        }
        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public new String ToString()
        {
            String s = this.GetType().Name;
            s = s.Substring(s.LastIndexOf('.') + 1);
            int b = bufferSet.BufferSize;
            int k = bufferSet.NumberOfElements;
            return s + "(mem=" + Memory() + ", b=" + b + ", k=" + k + ", size=" + Size + ", totalSize=" + this.bufferSet.TotalSize + ")";
        }

        #endregion

        #region Local Protected Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DoubleBuffer[] BuffersToCollapse()
        {
            int minLevel = bufferSet.GetMinLevelOfFullOrPartialBuffers();
            return bufferSet.GetFullOrPartialBuffersWithLevel(minLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Collapse()
        {
            DoubleBuffer[] toCollapse = BuffersToCollapse();
            DoubleBuffer outputBuffer = bufferSet.Collapse(toCollapse);

            int minLevel = toCollapse[0].Level;
            outputBuffer.Level = minLevel + 1;

            PostCollapse(toCollapse);
        }

        /// <summary>
        /// Default implementation does nothing.
        /// </summary>
        /// <param name="phis"></param>
        /// <returns></returns>
        protected List<Double> PreProcessPhis(List<Double> phis)
        {
            return phis;
        }

        /// <summary>
        /// Initializes the receiver
        /// </summary>
        /// <param name="b"></param>
        /// <param name="k"></param>
        protected void SetUp(int b, int k)
        {
            if (!(b >= 2 && k >= 1))
            {
                throw new ArgumentException("Assertion: b>=2 && k>=1");
            }
            this.bufferSet = new DoubleBufferSet(b, k);
            this.Clear();
        }

        #endregion

        #region Local Private Methods

        #endregion
    }
}
