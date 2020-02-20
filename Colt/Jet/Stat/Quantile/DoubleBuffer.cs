using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// A buffer holding <tt>double</tt> elements; internally used for computing approximate quantiles.
    /// </summary>
    public class DoubleBuffer : Buffer
    {

        #region Local Variables
        protected List<Double> values;
        protected Boolean isSorted;
        #endregion

        #region Implement Property
        /// <summary>
        /// Returns whether the receiver is empty.
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return values.Count == 0;
            }
        }

        /// <summary>
        /// Returns whether the receiver is empty.
        /// </summary>
        public override bool IsFull
        {
            get
            {
                return values.Count == k;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the receiver.
        /// </summary>
        public override int Size
        {
            get
            {
                return values.Count;
            }
        }

        public Boolean IsSorted
        {
            get { return isSorted; }
            set { isSorted = value; }
        }
        #endregion

        #region Property
        public List<Double> Values
        {
            get { return values; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="k"></param>
        public DoubleBuffer(int k) : base(k)
        {
            this.values = new List<Double>(0);
            this.isSorted = false;
        }


        #endregion

        #region Implement Methods

        /// <summary>
        /// Clears the receiver.
        /// </summary>
        public override void Clear()
        {
            values.Clear();
        }

        /// <summary>
        /// Sorts the receiver.
        /// </summary>
        public override void Sort()
        {
            if (!this.isSorted)
            {
                // IMPORTANT: TO DO : replace mergeSort with quickSort!
                // currently it is mergeSort only for debugging purposes (JDK 1.2 can't be imported into VisualAge).
                values.Sort();
                //values.mergeSort();
                this.isSorted = true;
            }
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return "k=" + this.k +
                    ", w=" + Weight.ToString() +
                    ", l=" + Level.ToString() +
                    ", size=" + values.Count;
            //", v=" + values.ToString();
        }
        #endregion

        #region Local Public Methods
        /// <summary>
        /// Adds a value to the receiver.
        /// </summary>
        /// <param name="value"></param>
        public void Add(double value)
        {
            if (!isAllocated) Allocate(); // lazy buffer allocation can safe memory.
            values.Add(value);
            this.isSorted = false;
        }

        /// <summary>
        /// Adds a value to the receiver.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddAllOfFromTo(List<Double> elements, int from, int to)
        {
            if (!isAllocated) Allocate(); // lazy buffer allocation can safe memory.
            values.AddRange(elements.GetRange(from, to)); //addAllOfFromTo(elements, from, to);
            this.isSorted = false;
        }

        /// <summary>
        /// Returns a deep copy of the receiver.
        /// </summary>
        /// <returns></returns>
        public override Object Clone()
        {
            DoubleBuffer copy = (DoubleBuffer)base.Clone();
            if (this.values != null) copy.values = this.values.ToList();
            return copy;
        }

        /// <summary>
        /// Returns whether the specified element is contained in the receiver.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Boolean Contains(double element)
        {
            this.Sort();
            return values.Contains(element);
        }

        /// <summary>
        /// Returns the number of elements currently needed to store all contained elements.
        /// This number usually differs from the results of method <tt>size()</tt>, according to the underlying algorithm.
        /// </summary>
        /// <returns></returns>
        public int Memory()
        {
            return values.ToArray().Length;
        }

        /// <summary>
        /// Returns the rank of a given element within the sorted sequence of the receiver.
        /// A rank is the number of elements &lt;= element.
        /// Ranks are of the form {1,2,...size()}.
        /// If no element is &lt;= element, then the rank is zero.
        /// If the element lies in between two contained elements, then uses linear interpolation.
        /// </summary>
        /// <param name="element">the element to search for</param>
        /// <returns>the rank of the element.</returns>
        public double Rank(double element)
        {
            this.Sort();
            return Cern.Jet.Stat.Descriptive.RankInterpolated(this.values, element);
        }

        #endregion

        #region Local Internal Methods
        /// <summary>
        /// Allocates the receiver.
        /// </summary>
        internal void Allocate()
        {
            isAllocated = true;
            values.EnsureCapacity(k);
        }

        #endregion


        #region Local Private Methods

        #endregion
    }
}
