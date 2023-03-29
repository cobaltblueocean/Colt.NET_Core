// <copyright file="SparseDoubleMatrix3D.cs" company="CERN">
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
    /// Sparse hashed 3-d matrix holding <i>double</i> elements.
    ///
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Note that this implementation is not synchronized.
    /// Uses a {@link cern.colt.map.OpenIntDoubleHashMap}, which is a compact and performant hashing technique.
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
    /// Thus, a 100 x 100 x 100 matrix with minLoadFactor=0.25 and maxLoadFactor=0.5 and 1000000 non-zero cells consumes between 25 MB and 50 MB.
    /// The same 100 x 100 x 100 matrix with 1000 non-zero cells consumes between 25 and 50 KB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// This class offers <i>expected</i> time complexity <i>O(1)</i> (i.ed constant time) for the basic operations
    /// <i>get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>
    /// assuming the hash function disperses the elements properly among the buckets.
    /// Otherwise, pathological cases, although highly improbable, can occur, degrading performance to <i>O(N)</i> in the worst case.
    /// As such this sparse class is expected to have no worse time complexity than its dense counterpart {@link DenseDoubleMatrix2D}.
    /// However, constant factors are considerably larger.
    /// <p>
    /// Cells are internally addressed in (in decreasing order of significance): slice major, row major, column major.
    /// Applications demanding utmost speed can exploit this fact.
    /// Setting/getting values in a loop slice-by-slice, row-by-row, column-by-column is quicker than, for example, column-by-column, row-by-row, slice-by-slice.
    /// Thus
    /// <pre>
    ///    for (int slice=0; slice &lt; slices; slice++) {
    /// 	  for (int row=0; row &lt; rows; row++) {
    /// 	     for (int column=0; column &lt; columns; column++) {
    /// 			matrix.setQuick(slice,row,column,someValue);
    /// 		 }		    
    /// 	  }
    ///    }
    /// </pre>
    /// is quicker than
    /// <pre>
    ///    for (int column=0; column &lt; columns; column++) {
    /// 	  for (int row=0; row &lt; rows; row++) {
    /// 	     for (int slice=0; slice &lt; slices; slice++) {
    /// 			matrix.setQuick(slice,row,column,someValue);
    /// 		 }
    /// 	  }
    ///    }
    /// </pre>
    /// 
    /// @see cern.colt.map
    /// @see cern.colt.map.OpenIntDoubleHashMap
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class SparseDoubleMatrix3D : DoubleMatrix3D
    {
        /// <summary>
        /// Gets he elements of the matrix.
        /// </summary>
        internal IDictionary<int, double> Elements { get; private set; }


        private double MinLoadFactor;
        private double MaxLoadFactor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public override double this[int slice, int row, int column]
        {
            get
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //return elements.Get(index(slice,row,column));
                //manually inlined:
                // return Elements[SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride];
                return Elements[Index(slice, row, column)];
            }

            set
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //int index =	index(slice,row,column);
                //manually inlined:
                //int index = SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride;
                int index = Index(slice, row, column);
                if (value == 0)
                    this.Elements.Remove(index);
                else
                    this.Elements.Add(index, value);
            }
        }

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of rows in in every slice and exactly the same number of columns in in every row.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        ///
        /// @param values The values to be filled into the new matrix.
        /// @throws ArgumentException if <i>for any 1 &lt;= slice &lt; values.Length: values[slice].Length != values[slice-1].Length</i>.
        /// @throws ArgumentException if <i>for any 1 &lt;= row &lt; values.GetLength(1): values[slice][row].Length != values[slice][row-1].Length</i>.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <exception cref=""></exception>
        /// <exception cref=""></exception>
        public SparseDoubleMatrix3D(double[][][] values):this(values.Length, (values.Length == 0 ? 0 : values.GetLength(1)), (values.Length == 0 ? 0 : values.GetLength(1) == 0 ? 0 : values[0].GetLength(1)))
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of slices, rows and columns and default memory usage.
        /// All entries are initially <i>0</i>.
        /// @param slices the number of slices the matrix shall have.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @throws	ArgumentException if <i>(double)slices*columns*rows > int.MaxValue</i>.
        /// @throws	ArgumentException if <i>slices<0 || rows<0 || columns<0</i>.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <exception cref=""></exception>
        /// <exception cref=""></exception>
        public SparseDoubleMatrix3D(int slices, int rows, int columns):this(slices, rows, columns, slices * rows * (columns / 1000), 0.2, 0.5)
        {
        }

        /// <summary>
        /// Constructs a matrix with a given number of slices, rows and columns using memory as specified.
        /// All entries are initially <i>0</i>.
        /// For details related to memory usage see {@link cern.colt.map.OpenIntDoubleHashMap}.
        /// 
        /// @param slices the number of slices the matrix shall have.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @param initialCapacity   the initial capacity of the hash map.
        ///                          If not known, set <i>initialCapacity=0</i> or smalld     
        /// @param minLoadFactor        the minimum load factor of the hash map.
        /// @param maxLoadFactor        the maximum load factor of the hash map.
        /// @throws	ArgumentException if <i>initialCapacity < 0 || (minLoadFactor < 0.0 || minLoadFactor >= 1.0) || (maxLoadFactor <= 0.0 || maxLoadFactor >= 1.0) || (minLoadFactor >= maxLoadFactor)</i>.
        /// @throws	ArgumentException if <i>(double)columns*rows > int.MaxValue</i>.
        /// @throws	ArgumentException if <i>slices<0 || rows<0 || columns<0</i>.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="initialCapacity"></param>
        /// <param name="minLoadFactor"></param>
        /// <param name="maxLoadFactor"></param>
        /// <exception cref=""></exception>
        /// <exception cref=""></exception>
        /// <exception cref=""></exception>
        public SparseDoubleMatrix3D(int slices, int rows, int columns, int initialCapacity, double minLoadFactor, double maxLoadFactor)
        {
            Setup(slices, rows, columns);
            this.MinLoadFactor = minLoadFactor;
            this.MaxLoadFactor = maxLoadFactor;

            var capacity = PrimeFinder.NextPrime(initialCapacity);
            this.Elements = new Dictionary<int, Double>(capacity);
        }

        /// <summary>
        /// Constructs a view with the given parameters.
        /// @param slices the number of slices the matrix shall have.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @param elements the cells.
        /// @param sliceZero the position of the first element.
        /// @param rowZero the position of the first element.
        /// @param columnZero the position of the first element.
        /// @param sliceStride the number of elements between two slices, i.ed <i>index(k+1,i,j)-index(k,i,j)</i>.
        /// @param rowStride the number of elements between two rows, i.ed <i>index(k,i+1,j)-index(k,i,j)</i>.
        /// @param columnnStride the number of elements between two columns, i.ed <i>index(k,i,j+1)-index(k,i,j)</i>.
        /// @throws	ArgumentException if <i>(double)slices*columns*rows > int.MaxValue</i>.
        /// @throws	ArgumentException if <i>slices<0 || rows<0 || columns<0</i>.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slices"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="elements"></param>
        /// <param name="sliceZero"></param>
        /// <param name="rowZero"></param>
        /// <param name="columnZero"></param>
        /// <param name="sliceStride"></param>
        /// <param name="rowStride"></param>
        /// <param name="columnStride"></param>
        protected SparseDoubleMatrix3D(int slices, int rows, int columns, IDictionary<int, Double> elements, int sliceZero, int rowZero, int columnZero, int sliceStride, int rowStride, int columnStride)
        {
            Setup(slices, rows, columns, sliceZero, rowZero, columnZero, sliceStride, rowStride, columnStride);
            this.Elements = elements;
            this.IsView = true;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>value</i>.
        /// @param    value the value to be filled into the cells.
        /// @return <i>this</i> (for convenience only).
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override IDoubleMatrix3D Assign(double value)
        {
            // overriden for performance only
            if (!this.IsView && value == 0) this.Elements.Clear();
            else base.Assign(value);
            return this;
        }

        /// <summary>
        /// Returns the number of cells having non-zero values.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int Cardinality()
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
        ///
        /// @param   minNonZeros   the desired minimum number of non-zero cells.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minCapacity"></param>
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
        ///
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @return    the value at the specified coordinate.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        [Obsolete("GetQuick(int slice, int row, int column) is deprecated, please use indexer instead.")]
        public override Double GetQuick(int slice, int row, int column)
        {
            return this[slice, row, column];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        public override Boolean HaveSharedCellsRaw(IDoubleMatrix3D other)
        {
            if (other is SelectedSparseDoubleMatrix3D)
            {
                SelectedSparseDoubleMatrix3D otherMatrix = (SelectedSparseDoubleMatrix3D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is SparseDoubleMatrix3D)
            {
                SparseDoubleMatrix3D otherMatrix = (SparseDoubleMatrix3D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional arrayd 
        ///
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the third-coordinate.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        protected new int Index(int slice, int row, int column)
        {
            //return _sliceOffset(_sliceRank(slice)) + _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
            //manually inlined:
            return SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of slices, rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix3D</i> the new matrix must also be of type <i>DenseDoubleMatrix3D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix3D</i> the new matrix must also be of type <i>SparseDoubleMatrix3D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        ///
        /// @param slices the number of slices the matrix shall have.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @return  a new empty matrix of the same dynamic type.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Slices"></param>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public override IDoubleMatrix3D Like(int Slices, int Rows, int Columns)
        {
            return new SparseDoubleMatrix3D(Slices, Rows, Columns);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix3D</i> the new matrix must also be of type <i>DenseDoubleMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix3D</i> the new matrix must also be of type <i>SparseDoubleMatrix2D</i>, etc.
        ///
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @param rowZero the position of the first element.
        /// @param columnZero the position of the first element.
        /// @param rowStride the number of elements between two rows, i.ed <i>index(i+1,j)-index(i,j)</i>.
        /// @param columnStride the number of elements between two columns, i.ed <i>index(i,j+1)-index(i,j)</i>.
        /// @return  a new matrix of the corresponding dynamic type.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <param name="rowZero"></param>
        /// <param name="columnZero"></param>
        /// <param name="Rowstride"></param>
        /// <param name="Columnstride"></param>
        /// <returns></returns>
        protected override IDoubleMatrix2D Like2D(int Rows, int Columns, int rowZero, int columnZero, int Rowstride, int Columnstride)
        {
            return new SparseDoubleMatrix2D(Rows, Columns, this.Elements, rowZero, columnZero, RowStride, ColumnStride);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=slices() || row&lt;0 || row&gt;=rows() || column&lt;0 || column&gt;=column()</i>.
        ///
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @param    value the value to be filled into the specified cell.
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        [Obsolete("SetQuick(int slice, int row, int column, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int slice, int row, int column, double value)
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
        public void TrimToSize()
        {
            this.Elements.TrimExcess();
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="sliceOffsets">the offsets of the visible elements.</param>
        /// <param name="rowOffsets">the offsets of the visible elements.</param>
        /// <param name="columnOffsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override IDoubleMatrix3D ViewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedSparseDoubleMatrix3D(this.Elements, sliceOffsets, rowOffsets, columnOffsets, 0);
        }

        public override string ToString(int slice, int row, int column)
        {
            return this[slice, row, column].ToString();
        }
    }
}
