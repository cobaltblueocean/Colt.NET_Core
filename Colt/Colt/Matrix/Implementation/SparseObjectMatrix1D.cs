// <copyright file=".cs" company="CERN">
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

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Sparse hashed 1-d matrix (aka <i>vector</i>) holding <see cref="Object"/> elements.
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Note that this implementation is not synchronized.
    /// Uses a <see cref="IDictionary{int, Object}"/>, which is a compact and performant hashing technique.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// Cells that
    /// <ul>
    /// <li>are never set to non-zero values do not use any memory.
    /// <li>switch from zero to non-zero state do use memory.
    /// <li>switch back from non-zero to zero state also do use memoryd However, their memory is automatically reclaimed from time to timed It can also manually be reclaimed by calling {@link #trimToSize()}.
    /// </ul>
    /// <p>
    /// worst case: <i>memory [bytes] = (1/minLoadFactor) * nonZeros * 13</i>.
    /// <br>best  case: <i>memory [bytes] = (1/maxLoadFactor) * nonZeros * 13</i>.
    /// <br>Where <i>nonZeros = cardinality()</i> is the number of non-zero cells.
    /// Thus, a 1000000 matrix with minLoadFactor=0.25 and maxLoadFactor=0.5 and 1000000 non-zero cells consumes between 25 MB and 50 MB.
    /// The same 1000000 matrix with 1000 non-zero cells consumes between 25 and 50 KB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// This class offers <i>expected</i> time complexity <i>O(1)</i> (i.ed constant time) for the basic operations
    /// <i>get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>
    /// assuming the hash function disperses the elements properly among the buckets.
    /// Otherwise, pathological cases, although highly improbable, can occur, degrading performance to <i>O(N)</i> in the worst case.
    /// As such this sparse class is expected to have no worse time complexity than its dense counterpart <see cref="DenseObjectMatrix1D"/>.
    /// However, constant factors are considerably larger.
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class SparseObjectMatrix1D : ObjectMatrix1D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal IDictionary<int, Object> Elements { get; private set; }

        private double MinLoadFactor;
        private double MaxLoadFactor;

        /// <summary>
        /// Get or set the value of Element specified with the index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override object this[int index]
        {
            get
            {
                //if (debug) if (index<0 || index>=size) checkIndex(index);
                //return this.elements.Get(index(index)); 
                // manually inlined:
                return Elements[Zero + index * Stride];
            }
            set
            {
                //if (debug) if (index<0 || index>=size) checkIndex(index);
                //int i =	index(index);
                // manually inlined:
                int i = Zero + index * Stride;
                if (value == null)
                    this.Elements.Remove(i);
                else
                    this.Elements.Add(i, value);
            }
        }

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        public SparseObjectMatrix1D(Object[] values) : this(values.Length)
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of cells.
        /// All entries are initially <i>null</i>.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <exception cref="ArgumentException">if size &lt; 0.</exception>
        public SparseObjectMatrix1D(int size) : this(size, size / 1000, 0.2, 0.5)
        {

        }

        /// <summary>
        /// Constructs a matrix with a given number of parameters.
        /// All entries are initially <i>null</i>.
        /// For details related to memory usage see {@link cern.colt.map.OpenIntObjectHashMap}.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <param name="initialCapacity">the initial capacity of the hash map.  If not known, set <i>initialCapacity=0</i> or smalld  </param>
        /// <param name="minLoadFactor">the minimum load factor of the hash map.</param>
        /// <param name="maxLoadFactor">the maximum load factor of the hash map.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>initialCapacity &lt; 0 || (minLoadFactor &lt; 0.0 || minLoadFactor >= 1.0) || (maxLoadFactor &lt;= 0.0 || maxLoadFactor >= 1.0) || (minLoadFactor >= maxLoadFactor)</i>.</exception>
        /// <exception cref="ArgumentException">if <i>size &lt; 0</i>.</exception>
        public SparseObjectMatrix1D(int size, int initialCapacity, double minLoadFactor, double maxLoadFactor)
        {
            Setup(size);
            //this.Elements = new OpenIntObjectHashMap(initialCapacity, minLoadFactor, maxLoadFactor);
            this.MaxLoadFactor = minLoadFactor;
            this.MaxLoadFactor = maxLoadFactor;

            var capacity = PrimeFinder.NextPrime(initialCapacity);
            this.Elements = new Dictionary<int, Object>(capacity);
        }

        /// <summary>
        /// Constructs a matrix view with a given number of parameters.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="offset">the index of the first element.</param>
        /// <param name="stride">the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.</param>
        /// <exception cref="ArgumentException">if <i>size &lt; 0</i>.</exception>
        public SparseObjectMatrix1D(int size, IDictionary<int, Object> elements, int offset, int stride)
        {
            Setup(size, offset, stride);
            this.Elements = elements;
            this.IsView = true;
        }

        /// <summary>
        /// Returns the number of cells having non-zero values.
        /// </summary>
        public new int Cardinality()
        {
            if (!this.IsView) return this.Elements.Count;
            else return base.Cardinality();
        }

        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of non-zero cells without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// <p>
        /// This method never need be called; it is for performance tuning only.
        /// Calling this method before tt>set()</i>ing a large number of non-zero values boosts performance,
        /// because the receiver will grow only once instead of potentially many times and hash collisions get less probable.
        /// </summary>
        /// <param name="minNonZeros">the desired minimum number of non-zero cells.</param>
        public void EnsureCapacity(int minCapacity)
        {
            this.Elements.EnsureCapacity(minCapacity);
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        [Obsolete("GetQuick(int index) is deprecated, please use indexer instead.")]
        public override Object GetQuick(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix1D other)
        {
            if (other is SelectedSparseObjectMatrix1D)
            {
                SelectedSparseObjectMatrix1D otherMatrix = (SelectedSparseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is SparseObjectMatrix1D)
            {
                SparseObjectMatrix1D otherMatrix = (SparseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// You may want to override this method for performance.
        /// </summary>
        /// <param name="rank">the rank of the element.</param>
        protected new int Index(int rank)
        {
            // overriden for manual inlining only
            //return _offset(_rank(rank));
            return Zero + rank * Stride;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="size">the number of cell the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override ObjectMatrix1D Like(int size)
        {
            return new SparseObjectMatrix1D(size);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <see cref="DenseObjectMatrix1D"/> the new matrix must be of type <see cref="DenseObjectMatrix2D"/>,
        /// if the receiver is an instance of type <see cref="SparseObjectMatrix1D"/> the new matrix must be of type <see cref="SparseObjectMatrix2D"/>, etc.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override ObjectMatrix2D Like2D(int rows, int columns)
        {
            return new SparseObjectMatrix2D(rows, columns);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>index</i> to the specified value.
        /// 
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int index, Object value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int index, Object value)
        {
            this[index] = value;
        }

        /// <summary>
        /// Releases any basefluous memory created by explicitly putting zero values into cells formerly having non-zero values; 
        /// An application can use this operation to minimize the
        /// storage of the receiver.
        /// <p>
        /// <b>Background:</b>
        /// <p>
        /// Cells that <ul>
        /// <li>are never set to non-zero values do not use any memory.
        /// <li>switch from zero to non-zero state do use memory.
        /// <li>switch back from non-zero to zero state also do use memoryd However, their memory can be reclaimed by calling <i>trimToSize()</i>.
        /// </ul>
        /// A sequence like <i>set(i,5); set(i,0);</i>
        /// sets a cell to non-zero state and later back to zero state.
        /// Such as sequence generates obsolete memory that is automatically reclaimed from time to time or can manually be reclaimed by calling <i>trimToSize()</i>.
        /// Putting zeros into cells already containing zeros does not generate obsolete memory since no memory was allocated to them in the first place.
        /// </summary>
        public void TrimToSize()
        {
            this.Elements.TrimExcess();
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override ObjectMatrix1D ViewSelectionLike(int[] offsets)
        {
            return new SelectedSparseObjectMatrix1D(this.Elements, offsets);
        }

        public override string ToString(int index)
        {
            return this[index].ToString();
        }
    }
}
