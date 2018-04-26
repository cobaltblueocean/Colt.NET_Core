using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// A buffer holding elements; internally used for computing approximate quantiles.
    /// </summary>
    public abstract class Buffer: Cern.Colt.PersistentObject
    {

        #region Local Variables
        protected int weight;
        protected int level;
        protected int k;
        protected Boolean isAllocated;
        #endregion

        #region Property
        /// <summary>
        /// Gets whether the receiver's level, or sets the receiver's level.
        /// </summary>
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public Boolean IsAllocated
        {
            get { return isAllocated; }
        }

        /// <summary>
        /// Gets the number of elements within a buffer.
        /// </summary>
        public int NumberOfElements
        {
            get { return k; }
        }

        /// <summary>
        /// Gets the number of elements within a buffer.
        /// </summary>
        [Obsolete("K is deprecated, please use NumberOfElements property instead.")]
        public int K
        {
            get { return k; }
        }

        /// <summary>
        /// Gets whether the receiver is partial.
        /// </summary>
        public Boolean IsPartial
        {
            get { return !(IsEmpty || IsFull); }
        }

        /// <summary>
        /// Gets whether the receiver's weight, or sets the receiver's weight.
        /// </summary>
        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="k"></param>
        public Buffer(int k)
        {
            this.k = k;
            this.weight = 1;
            this.level = 0;
            this.isAllocated = false;
        }
        #endregion

        #region Abstract Property

        /// <summary>
        /// Gets whether the receiver is empty.
        /// </summary>
        public abstract Boolean IsEmpty { get; }

        /// <summary>
        /// Gets whether the receiver is empty.
        /// </summary>
        public abstract Boolean IsFull { get; }

        /// <summary>
        /// Gets the number of elements contained in the receiver.
        /// </summary>
        public abstract int Size { get; }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Clears the receiver.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Sorts the receiver.
        /// </summary>
        public abstract void Sort();

        #endregion
    }
}
