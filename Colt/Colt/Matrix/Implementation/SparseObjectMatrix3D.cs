using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    ////Sparse hashed 3-d matrix holding <i>Object</i> elements.
    ////First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    ////<p>
    ////<b>Implementation:</b>
    ////<p>
    ////Note that this implementation is not synchronized.
    ////Uses a {@link cern.colt.map.OpenIntObjectHashMap}, which is a compact and performant hashing technique.
    ////<p>
    ////<b>Memory requirements:</b>
    ////<p>
    ////Cells that
    ////<ul>
    ////<li>are never set to non-zero values do not use any memory.
    ////<li>switch from zero to non-zero state do use memory.
    ////<li>switch back from non-zero to zero state also do use memoryd However, their memory is automatically reclaimed from time to timed It can also manually be reclaimed by calling {@link #trimToSize()}.
    ////</ul>
    ////<p>
    ////worst case: <i>memory [bytes] = (1/minLoadFactor) * nonZeros * 13</i>.
    ////<br>best  case: <i>memory [bytes] = (1/maxLoadFactor) * nonZeros * 13</i>.
    ////<br>Where <i>nonZeros = cardinality()</i> is the number of non-zero cells.
    ////Thus, a 100 x 100 x 100 matrix with minLoadFactor=0.25 and maxLoadFactor=0.5 and 1000000 non-zero cells consumes between 25 MB and 50 MB.
    ////The same 100 x 100 x 100 matrix with 1000 non-zero cells consumes between 25 and 50 KB.
    ////<p>
    ////<b>Time complexity:</b>
    ////<p>
    ////This class offers <i>expected</i> time complexity <i>O(1)</i> (i.ed constant time) for the basic operations
    ////<i>get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>
    ////assuming the hash function disperses the elements properly among the buckets.
    ////Otherwise, pathological cases, although highly improbable, can occur, degrading performance to <i>O(N)</i> in the worst case.
    ////As such this sparse class is expected to have no worse time complexity than its dense counterpart {@link DenseObjectMatrix2D}.
    ////However, constant factors are considerably larger.
    ////<p>
    ////Cells are internally addressed in (in decreasing order of significance): slice major, row major, column major.
    ////Applications demanding utmost speed can exploit this fact.
    ////Setting/getting values in a loop slice-by-slice, row-by-row, column-by-column is quicker than, for example, column-by-column, row-by-row, slice-by-slice.
    ////Thus
    ////<pre>
    ////   for (int slice=0; slice < slices; slice++) {
    ////	  for (int row=0; row < rows; row++) {
    ////	     for (int column=0; column < columns; column++) {
    ////			matrix.setQuick(slice,row,column,someValue);
    ////		 }		    
    ////	  }
    ////   }
    ////</pre>
    ////is quicker than
    ////<pre>
    ////   for (int column=0; column < columns; column++) {
    ////	  for (int row=0; row < rows; row++) {
    ////	     for (int slice=0; slice < slices; slice++) {
    ////			matrix.setQuick(slice,row,column,someValue);
    ////		 }
    ////	  }
    ////   }
    ////</pre>
    ////
    ////@see cern.colt.map
    ////@see cern.colt.map.OpenIntObjectHashMap
    ////@author wolfgang.hoschek@cern.ch
    ////@version 1.0, 09/24/99
    /// </summary>
    public class SparseObjectMatrix3D : ObjectMatrix3D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal IDictionary<int, Object> Elements { get; private set; }

        /// <summary>
        /// Get or set value coordinate with the indexes
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override object this[int slice, int row, int column]
        {
            get
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //return elements.Get(index(slice,row,column));
                //manually inlined:
                return Elements[offset + sliceOffsets[SliceZero + slice * SliceStride] + rowOffsets[RowZero + row * RowStride] + columnOffsets[ColumnZero + column * ColumnStride]];
            }
            set
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //int index =	index(slice,row,column);
                //manually inlined:
                int index = offset + sliceOffsets[SliceZero + slice * SliceStride] + rowOffsets[RowZero + row * RowStride] + columnOffsets[ColumnZero + column * ColumnStride];
                if (value == null)
                    this.Elements.Remove(index);
                else
                    this.Elements[index] = value;
            }
        }

        /// <summary>
        /// The offsets of the visible cells of this matrix.
        /// </summary>
        protected int[] sliceOffsets;
        protected int[] rowOffsets;
        protected int[] columnOffsets;

        private double MinLoadFactor;
        private double MaxLoadFactor;

        /// <summary>
        /// The offset.
        /// </summary>
        protected int offset;

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of rows in in every slice and exactly the same number of columns in in every row.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= slice &lt; values.Length: values[slice].Length != values[slice-1].Length</i>.</exception>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= row &lt; values.GetLength(1): values[slice][row].Length != values[slice][row-1].Length</i>.</exception>
        public SparseObjectMatrix3D(Object[][][] values) : this(values.Length, (values.Length == 0 ? 0 : values.GetLength(1)), (values.Length == 0 ? 0 : values.GetLength(1) == 0 ? 0 : values[0].GetLength(1)))
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of slices, rows and columns and default memory usage.
        /// All entries are initially <i>null</i>.
        /// </summary>
        /// <param name="slices">the number of slices the matrix shall have.</param>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>(double)slices * columns * rows > int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>slices &lt; 0 || rows &lt; 0 || columns &lt; 0</i>.</exception>
        public SparseObjectMatrix3D(int slices, int rows, int columns) : this(slices, rows, columns, slices * rows * (columns / 1000), 0.2, 0.5)
        {

        }

        /// <summary>
        /// Constructs a matrix with a given number of slices, rows and columns using memory as specified.
        /// All entries are initially <i>null</i>.
        /// For details related to memory usage see <see cref="IDictionary{int, Object} "/>.
        /// </summary>
        /// <param name="slices">the number of slices the matrix shall have.</param>
        /// <param name="rows">the number of rows the matrix shall have.s</param>
        /// <param name="column">the number of columns the matrix shall have.</param>
        /// <param name="initialCapacity">the initial capacity of the hash map.  If not known, set <i>initialCapacity=0</i> or smalld</param>
        /// <param name="minLoadFactor">the minimum load factor of the hash map.</param>
        /// <param name="maxLoadFactor">the maximum load factor of the hash map.</param>
        /// <exception cref="ArgumentException">if <i>initialCapacity &lt; 0 || (minLoadFactor &lt; 0.0 || minLoadFactor >= 1.0) || (maxLoadFactor &lt;= 0.0 || maxLoadFactor >= 1.0) || (minLoadFactor >= maxLoadFactor)</i>.</exception>
        /// <exception cref="ArgumentException">if <i>(double)slices * columns * rows > int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>slices &lt; 0 || rows &lt; 0 || columns &lt; 0</i>.</exception>
        public SparseObjectMatrix3D(int slices, int rows, int columns, int initialCapacity, double minLoadFactor, double maxLoadFactor)
        {
            Setup(slices, rows, columns);
            this.MaxLoadFactor = minLoadFactor;
            this.MaxLoadFactor = maxLoadFactor;

            var capacity = PrimeFinder.NextPrime(initialCapacity);
            this.Elements = new Dictionary<int, Object>(capacity);
        }

        /// <summary>
        /// Constructs a view with the given parameters.
        /// </summary>
        /// <param name="slices">the number of slices the matrix shall have.</param>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="sliceZero">the position of the first element.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="sliceStride">the number of elements between two slices, i.ed <i>index(k+1,i,j)-index(k,i,j)</i>.</param>
        /// <param name="rowStride">the number of elements between two rows, i.ed <i>index(k,i+1,j)-index(k,i,j)</i>.</param>
        /// <param name="columnnStride">the number of elements between two columns, i.ed <i>index(k,i,j+1)-index(k,i,j)</i>.</param>
        /// <exception cref="ArgumentException">if <i>(Object)slices * columns * rows > int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>slices &lt; 0 || rows &lt; 0 || columns &lt; 0</i>.</exception>
        protected SparseObjectMatrix3D(int slices, int rows, int columns, IDictionary<int, Object> elements, int sliceZero, int rowZero, int columnZero, int sliceStride, int rowStride, int columnStride)
        {
            Setup(slices, rows, columns, sliceZero, rowZero, columnZero, sliceStride, rowStride, columnStride);
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
        /// Returns the matrix cell value at coordinate <i>[slice,row,column]</i>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=slices() || row&lt;0 || row&gt;=rows() || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        [Obsolete("GetQuick(int slice, int row, int column) is deprecated, please use indexer instead.")]
        public Object GetQuick(int slice, int row, int column)
        {
            return this[slice, row, column];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix3D other)
        {
            if (other is SelectedSparseObjectMatrix3D)
            {
                SelectedSparseObjectMatrix3D otherMatrix = (SelectedSparseObjectMatrix3D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is SparseObjectMatrix3D)
            {
                SparseObjectMatrix3D otherMatrix = (SparseObjectMatrix3D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the third-coordinate.</param>
        protected new int Index(int slice, int row, int column)
        {
            //return _sliceOffset(_sliceRank(slice)) + _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
            //manually inlined:
            return SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride;
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        /// 
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=slices() || row&lt;0 || row&gt;=rows() || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int slice, int row, int column, Object value) is deprecated, please use indexer instead.")]
        public void SetQuick(int slice, int row, int column, Object value)
        {
            this[slice, row, column] = value;
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
        /// A sequence like <i>set(s,r,c,5); set(s,r,c,0);</i>
        /// sets a cell to non-zero state and later back to zero state.
        /// Such as sequence generates obsolete memory that is automatically reclaimed from time to time or can manually be reclaimed by calling <i>trimToSize()</i>.
        /// Putting zeros into cells already containing zeros does not generate obsolete memory since no memory was allocated to them in the first place.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public void TrimToSize()
        {
            this.Elements.TrimExcess();
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of slices, rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix3D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix3D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="slices">the number of slices the matrix shall have.</param>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override ObjectMatrix3D Like(int Slices, int Rows, int Columns)
        {
            return new SparseObjectMatrix3D(Slices, Rows, Columns);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix2D</i>, etc.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="rowStride">the number of elements between two rows, i.ed <i>index(i+1,j)-index(i,j)</i>.</param>
        /// <param name="columnStride">the number of elements between two columns, i.ed <i>index(i,j+1)-index(i,j)</i>.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        protected override ObjectMatrix2D Like2D(int Rows, int Columns, int RowZero, int columnZero, int Rowstride, int Columnstride)
        {
            return new SparseObjectMatrix2D(Rows, Columns, this.Elements, RowZero, columnZero, RowStride, ColumnStride);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="sliceOffsets">the offsets of the visible elements.</param>
        /// <param name="rowOffsets">the offsets of the visible elements.</param>
        /// <param name="columnOffsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override ObjectMatrix3D ViewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedSparseObjectMatrix3D(this.Elements, sliceOffsets, rowOffsets, columnOffsets, 0);
        }
    }
}
