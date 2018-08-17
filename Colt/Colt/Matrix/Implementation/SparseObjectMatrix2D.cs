// <copyright file="SparseObjectMatrix2D.cs" company="CERN">
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
    /// Sparse hashed 2-d matrix holding <see cref="Object"/> elements.
    ///    <p>
    ///    <b>Implementation:</b>
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
    /// <li>switch back from non-zero to zero state also do use memoryd However, their memory is automatically reclaimed from time to timed It can also manually be reclaimed by calling {
    ///        @link #trimToSize()}.
    /// </ ul >
    /// < p >
    /// worst case: < tt > memory[bytes] = (1 / minLoadFactor) * nonZeros * 13 </ tt >.
    /// < br > best  case: < tt > memory[bytes] = (1 / maxLoadFactor) * nonZeros * 13 </ tt >.
    /// < br > Where < tt > nonZeros = cardinality() </ tt > is the number of non-zero cells.
    /// Thus, a 1000 x 1000 matrix with minLoadFactor = 0.25 and maxLoadFactor = 0.5 and 1000000 non - zero cells consumes between 25 MB and 50 MB.
    /// The same 1000 x 1000 matrix with 1000 non - zero cells consumes between 25 and 50 KB.
    /// < p >
    /// < b > Time complexity:</ b >
    /// < p >
    /// This class offers<i>expected</i> time complexity<i> O(1)</i> (i.e.constant time) for the basic operations
    /// <i> get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>
    /// assuming the hash function disperses the elements properly among the buckets.
    /// Otherwise, pathological cases, although highly improbable, can occur, degrading performance to<i> O(N)</i> in the worst case.
    /// As such this sparse class is expected to have no worse time complexity than its dense counterpart <see cref="DenseObjectMatrix2D"/>.
    /// However, constant factors are considerably larger.
    /// <p>
    /// Cells are internally addressed in row-major.
    /// Performance sensitive applications can exploit this fact.
    /// Setting values in a loop row-by-row is quicker than column-by-column, because fewer hash collisions occur.
    /// Thus
    /// <pre>
    /// 	for (int row= 0; row &lt; rows; row++) {
    ///    for (int column = 0; column &lt; columns; column++)
    ///    {
    ///        matrix.setQuick(row, column, someValue);
    ///    }
    /// }
    /// </pre>
    /// is quicker than
    /// <pre>
    /// 	for (int column=0; column &lt; columns; column++) {
    ///    for (int row = 0; row &lt; rows; row++)
    ///    {
    ///        matrix.setQuick(row, column, someValue);
    ///    }
    /// }
    /// </pre>
    /// 
    /// @see cern.colt.map
    /// @see cern.colt.map.OpenIntObjectHashMap
    /// @author wolfgang.hoschek @cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class SparseObjectMatrix2D : ObjectMatrix2D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal IDictionary<int, Object> Elements { get; private set; }

        public override object this[int rowindex, int colindex]
        {
            get
            {
                //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new IndexOutOfRangeException("row:"+row+", column:"+column);
                //return this.elements.Get(index(row,column));
                //manually inlined:
                return this.Elements[RowZero + rowindex * RowStride + ColumnZero + colindex * ColumnStride];
            }
            set
            {
                //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new IndexOutOfRangeException("row:"+row+", column:"+column);
                //int index =	index(row,column);
                //manually inlined:
                int index = RowZero + rowindex * RowStride + ColumnZero + colindex * ColumnStride;

                if (value == null)
                    this.Elements.Remove(index);
                else
                    this.Elements.Add(index, value);
            }
        }

        private double MinLoadFactor;
        private double MaxLoadFactor;

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// <i>values</i> is required to have the form <i>values[row][column]</i>
        /// and have exactly the same number of columns in every row.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= row &lt; values.Length: values[row].Length != values[row-1].Length</i>.</exception>
        public SparseObjectMatrix2D(Object[][] values) : this(values.Length, values.Length == 0 ? 0 : values.GetLength(1))
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of rows and columns and default memory usage.
        /// All entries are initially <i>null</i>.
        /// @param  
        /// @param  
        /// @throws	 
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>rows &lt; 0 || columns &lt; 0 || (double)columns * rows > int.MaxValue</i>.</exception>
        public SparseObjectMatrix2D(int rows, int columns) : this(rows, columns, rows * (columns / 1000), 0.2, 0.5)
        {

        }

        /// <summary>
        /// Constructs a matrix with a given number of rows and columns using memory as specified.
        /// All entries are initially <i>null</i>.
        /// For details related to memory usage see {@link cern.colt.map.OpenIntObjectHashMap}.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <param name="initialCapacity">the initial capacity of the hash map.  If not known, set <i>initialCapacity=0</i> or smalld</param>
        /// <param name="minLoadFactor">the minimum load factor of the hash map.</param>
        /// <param name="maxLoadFactor">the maximum load factor of the hash map.</param>
        /// <exception cref="ArgumentException">if <i>initialCapacity &lt; 0 || (minLoadFactor &lt; 0.0 || minLoadFactor >= 1.0) || (maxLoadFactor &lt;= 0.0 || maxLoadFactor >= 1.0) || (minLoadFactor >= maxLoadFactor)</i>.</exception>
        /// <exception cref="ArgumentException">if <i>rows &lt; 0 || columns &lt; 0 || (double)columns * rows > int.MaxValue</i>.</exception>
        public SparseObjectMatrix2D(int rows, int columns, int initialCapacity, double minLoadFactor, double maxLoadFactor)
        {
            Setup(rows, columns);
            this.MaxLoadFactor = minLoadFactor;
            this.MaxLoadFactor = maxLoadFactor;

            var capacity = PrimeFinder.NextPrime(initialCapacity);
            this.Elements = new Dictionary<int, Object>(capacity);
        }

        /// <summary>
        /// Constructs a view with the given parameters.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="rowStride">the number of elements between two rows, i.ed <i>index(i+1,j)-index(i,j)</i>.</param>
        /// <param name="columnStride">the number of elements between two columns, i.ed <i>index(i,j+1)-index(i,j)</i>.</param>
        /// <exception cref="ArgumentException">if <i>rows &lt; 0 || columns &lt; 0 || (double)columns * rows > int.MaxValue</i> or flip's are illegal.</exception>
        public SparseObjectMatrix2D(int rows, int columns, IDictionary<int, Object> elements, int rowZero, int columnZero, int rowStride, int columnStride)
        {
            Setup(rows, columns, rowZero, columnZero, rowStride, columnStride);
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
        /// Returns the matrix cell value at coordinate <i>[row,column]</i>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        [Obsolete("GetQuick(int index, int column) is deprecated, please use indexer instead.")]
        public override Object GetQuick(int row, int column)
        {
            return this[row, column];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share common cells.
        /// More formally, returns <i>true</i> if at least one of the following conditions is met
        /// <ul>
        /// <li>the receiver is a view of the other matrix
        /// <li>the other matrix is a view of the receiver
        /// <li><i>this == other</i>
        /// </ul>
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix2D other)
        {
            if (other is SelectedSparseObjectMatrix2D)
            {
                SelectedSparseObjectMatrix2D otherMatrix = (SelectedSparseObjectMatrix2D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is SparseObjectMatrix2D)
            {
                SparseObjectMatrix2D otherMatrix = (SparseObjectMatrix2D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name=column"">the index of the column-coordinate.</param>
        protected override int Index(int row, int column)
        {
            // return base.index(row,column);
            // manually inlined for speed:
            return RowZero + row * RowStride + ColumnZero + column * ColumnStride;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must also be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must also be of type <i>SparseObjectMatrix2D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override ObjectMatrix2D Like(int Rows, int Columns)
        {
            return new SparseObjectMatrix2D(Rows, Columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must be of type <i>SparseObjectMatrix1D</i>, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override ObjectMatrix1D Like1D(int size)
        {
            return new SparseObjectMatrix1D(size);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must be of type <i>SparseObjectMatrix1D</i>, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <param name="offset">the index of the first element.</param>
        /// <param name="stride">the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        /// <exception cref=""></exception>
        protected override ObjectMatrix1D Like1D(int size, int offset, int stride)
        {
            return new SparseObjectMatrix1D(size, this.Elements, offset, stride);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[row,column]</i> to the specified value.
        /// 
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int index, int column, Object value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int row, int column, Object value)
        {
            this[row, column] = value;
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
        /// A sequence like <i>set(r,c,5); set(r,c,0);</i>
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
        /// <param name="rowOffsets">the offsets of the visible elements.</param>
        /// <param name="columnOffsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override ObjectMatrix2D ViewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedSparseObjectMatrix2D(this.Elements, rowOffsets, columnOffsets, 0);
        }

        public override string ToString(int row, int column)
        {
            return this[row, column].ToString();
        }
    }
}
