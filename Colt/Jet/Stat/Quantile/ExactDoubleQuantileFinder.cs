﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// Exact quantile finding algorithm for known and unknown <tt>N</tt> requiring large main memory; computes quantiles over a sequence of <tt>double</tt> elements.
 /// The folkore algorithm: Keeps all elements in main memory, sorts the list, then picks the quantiles.
     /// </summary>
    public class ExactDoubleQuantileFinder : Cern.Colt.PersistentObject, IDoubleQuantileFinder
    {

        #region Local Variables
        private List<Double> buffer;
        private Boolean isSorted;
        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the buffer
        /// </summary>
        public List<Double> Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        /// <summary>
        /// Gets or sets the value is sorted or not
        /// </summary>
        public Boolean IsSorted
        {
            get { return isSorted; }
            set { isSorted = value; }
        }
        #endregion

        #region Implement Property
        /// <summary>
        /// Gets the number of elements currently contained in the receiver (identical to the number of values added so far).
        /// </summary>
        public long Size
        {
            get
            {
                return buffer.Count;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs an empty exact quantile finder.
        /// </summary>
        public ExactDoubleQuantileFinder()
        {
            this.buffer = new List<Double>(0);
            this.Clear();
        }
        #endregion

        #region Implement Methods
        /// <summary>
        /// Adds a value to the receiver.
        /// </summary>
        /// <param name="value">the value to add.</param>
        public void Add(double value)
        {
            this.buffer.Add(value);
            this.isSorted = false;
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
            //buffer.AddAllOfFromTo(values, from, to);
            buffer.AddRange(values.GetRange(from, to)); 

            this.isSorted = false;
        }

        /// <summary>
        /// Removes all elements from the receiver.  The receiver will be empty after this call returns, and its memory requirements will be close to zero.
        /// </summary>
        public void Clear()
        {
            this.buffer.Clear();
            this.buffer.TrimExcess();
            this.isSorted = false;
        }

        /// <summary>
        /// Returns a deep copy of the receiver.
        /// </summary>
        /// <returns>a deep copy of the receiver.</returns>
        public new Object Clone()
        {
            ExactDoubleQuantileFinder copy = (ExactDoubleQuantileFinder)base.Clone();
            if (this.buffer != null) copy.buffer = copy.buffer.ToList();
            return copy;
        }

        /// <summary>
        /// Applies a procedure to each element of the receiver, if any.
        /// Iterates over the receiver in no particular order.
        /// </summary>
        /// <param name="procedure">the procedure to be applied. Stops iteration if the procedure returns <tt>false</tt>, otherwise continues. </param>
        /// <returns><tt>false</tt> if the procedure stopped before all elements where iterated over, <tt>true</tt> otherwise.</returns>
        public bool ForEach(DoubleProcedure procedure)
        {
            double[] theElements = buffer.ToArray();
            int theSize = (int)Size;

            for (int i = 0; i < theSize;) if (!procedure(theElements[i++])) return false;
            return true;
        }

        /// <summary>
        /// Returns the number of elements currently needed to store all contained elements.
        /// This number usually differs from the results of method <tt>size()</tt>, according to the underlying datastructure.
        /// </summary>
        /// <returns></returns>
        public long Memory()
        {
            return buffer.ToArray().Length;
        }

        /// <summary>
        /// Returns how many percent of the elements contained in the receiver are <tt>&lt;= element</tt>.
        /// Does linear interpolation if the element is not contained but lies in between two contained elements.
        /// </summary>
        /// <param name="element">the element to search for.</param>
        /// <returns>the percentage <tt>p</tt> of elements <tt>&lt;= element</tt> (<tt>0.0 &lt;= p &lt;=1.0)</tt>.</returns>
        public double Phi(double element)
        {
            this.Sort();
            return Cern.Jet.Stat.Descriptive.RankInterpolated(buffer, element) / this.Size;
        }

        /// <summary>
        /// Computes the specified quantile elements over the values previously added.
        /// </summary>
        /// <param name="phis">the quantiles for which elements are to be computed. Each phi must be in the interval [0.0,1.0]. <tt>phis</tt> must be sorted ascending.</param>
        /// <returns>the exact quantile elements.</returns>
        public List<double> QuantileElements(List<double> phis)
        {
            this.Sort();
            return Cern.Jet.Stat.Descriptive.Quantiles(this.buffer, phis);
            /*
            int bufferSize = (int) this.Count;
            double[] quantileElements = new double[phis.Count];
            for (int i=phis.Count; --i >=0;) {
                int rank=(int)Utils.epsilonCeiling(phis.Get(i)*bufferSize) -1;
                quantileElements[i]=buffer.Get(rank);
            }
            return new List<Double>(quantileElements);
            */
        }

        /// <summary>
        /// Returns the number of elements currently needed to store all contained elements.
        /// This number usually differs from the results of method <tt>size()</tt>, according to the underlying datastructure.
        /// </summary>
        /// <returns></returns>
        public long TotalMemory()
        {
            return Memory();
        }

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns whether the specified element is contained in the receiver.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Boolean Contains(double element)
        {
            this.Sort();
            return buffer.BinarySearch(element) >= 0;
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public new String ToString()
        {
            String s = this.GetType().Name;
            s = s.Substring(s.LastIndexOf('.') + 1);
            return s + "(mem=" + Memory() + ", size=" + Size + ")";
        }

        #endregion

        #region Local Protcted Methods
        /// <summary>
        /// Sorts the receiver.
        /// </summary>
        protected void Sort()
        {
            if (!isSorted)
            {
                // IMPORTANT: TO DO : replace mergeSort with quickSort!
                // currently it is mergeSort because JDK 1.2 can't be imported into VisualAge.
                buffer.Sort();
                //this.buffer.mergeSort();
                this.isSorted = true;
            }
        }
        #endregion

    }
}
