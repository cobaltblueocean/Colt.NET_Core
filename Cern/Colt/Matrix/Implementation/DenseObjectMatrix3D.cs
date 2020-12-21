// <copyright file="DenseObjectMatrix3D.cs" company="CERN">
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
using Cern.Colt.Matrix;

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Dense 3-d matrix holding <i>Object</i> elements.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Internally holds one single contigous one-dimensional array, addressed in (in decreasing order of significance): slice major, row major, column major.
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory [bytes] = 8*Slices*Rows*Columns</i>.
    /// Thus, a 100*100*100 matrix uses 8 MB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// <i>O(1)</i> (i.ed constant time) for the basic operations
    /// <i>get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>,
    /// <p>
    /// Applications demanding utmost speed can exploit knowledge about the internal addressing.
    /// Setting/getting values in a loop slice-by-slice, row-by-row, column-by-column is quicker than, for example, column-by-column, row-by-row, slice-by-slice.
    /// Thus
    /// <pre>
    ///    for (int slice=0; slice &lt; Slices; slice++) {
    /// 	  for (int row=0; row &lt; Rows; row++) {
    /// 	     for (int column=0; column &lt; Columns; column++) {
    /// 			matrix[slice,row,column] = someValue;
    /// 		 }		    
    /// 	  }
    ///    }
    /// </pre>
    /// is quicker than
    /// <pre>
    ///    for (int column=0; column &lt; Columns; column++) {
    /// 	  for (int row=0; row &lt; Rows; row++) {
    /// 	     for (int slice=0; slice &lt; Slices; slice++) {
    /// 			matrix[slice,row,column] = someValue;
    /// 		 }
    /// 	  }
    ///    }
    /// </pre>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class DenseObjectMatrix3D : ObjectMatrix3D
    {
        /// <summary>
        /// The elements of this matrix.
        /// elements are stored in slice major, then row major, then column major, in order of significance, i.e.
        /// index==slice*SliceStride+ row*RowStride + column*ColumnStride
        /// i.ed {slice0 row0..m}, {slice1 row0..m}, ..d, {sliceN row0..m}
        /// with each row storead as 
        /// {row0 column0..m}, {row1 column0..m}, ..d, {rown column0..m}
        /// </summary>
        protected internal Object[] Elements { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices || row&lt;0 || row&gt;=Rows || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        public override object this[int slice, int row, int column]
        {
            get
            {
                //if (debug) if (slice<0 || slice>=Slices || row<0 || row>=Rows || column<0 || column>=Columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //return elements[index(slice,row,column)];
                //manually inlined:
                return Elements[Index(slice, row, column)];
            }
            set
            {
                //if (debug) if (slice<0 || slice>=Slices || row<0 || row>=Rows || column<0 || column>=Columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //elements[index(slice,row,column)] = value;
                //manually inlined:
                Elements[Index(slice, row, column)] = value;
            }
        }

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of Rows in in every slice and exactly the same number of Columns in in every row.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= slice &lt; values.Length: values[slice].Length != values[slice-1].Length</i>.</exception>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= row &lt; values.GetLength(1): values[slice][row].Length != values[slice][row-1].Length</i>.</exception>
        public DenseObjectMatrix3D(Object[][][] values) : this(values.Length, (values.Length == 0 ? 0 : values[0].Length), (values.Length == 0 ? 0 : values[0].Length == 0 ? 0 : values[0][0].Length))
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of Slices, Rows and Columns.
        /// All entries are initially <i>0</i>.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>(Object)Slices*Columns*Rows > int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>Slices &lt; 0 || Rows &lt; 0 || Columns &lt; 0</i>.</exception>
        public DenseObjectMatrix3D(int Slices, int Rows, int Columns)
        {
            Setup(Slices, Rows, Columns);
            this.Elements = new Object[Slices * Rows * Columns];
        }

        /// <summary>
        /// Constructs a view with the given parameters.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="SliceZero">the position of the first element.</param>
        /// <param name="RowZero">the position of the first element.</param>
        /// <param name="ColumnZero">the position of the first element.</param>
        /// <param name="SliceStride">the number of elements between two Slices, i.ed <i>index(k+1,i,j)-index(k,i,j)</i>.</param>
        /// <param name="RowStride">the number of elements between two Rows, i.ed <i>index(k,i+1,j)-index(k,i,j)</i>.</param>
        /// <param name="ColumnStride">the number of elements between two Columns, i.ed <i>index(k,i,j+1)-index(k,i,j)</i>.</param>
        /// <exception cref="ArgumentException">if <i>(Object)Slices*Columns*Rows > int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>Slices &lt; 0 || Rows &lt; 0 || Columns &lt; 0</i>.</exception>
        protected DenseObjectMatrix3D(int Slices, int Rows, int Columns, Object[] elements, int SliceZero, int RowZero, int ColumnZero, int SliceStride, int RowStride, int ColumnStride)
        {
            Setup(Slices, Rows, Columns, SliceZero, RowZero, ColumnZero, SliceStride, RowStride, ColumnStride);
            this.Elements = elements;
            this.IsView = true;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>values</i>.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of Slices, Rows and Columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != Slices || for any 0 &lt;= slice &lt; Slices: values[slice].Length != Rows</i>.</exception>
        /// <exception cref="ArgumentException">if <i>for any 0 &lt;= column &lt; Columns: values[slice][row].Length != Columns</i>.</exception>
        public override ObjectMatrix3D Assign(Object[][][] values)
        {
            if (!this.IsView)
            {
                if (values.Length != Slices) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Matrix_MustHaveSameNumberOfSlices, values.Length , Slices));
                int i = Slices * Rows * Columns - Columns;
                for (int slice = Slices; --slice >= 0;)
                {
                    Object[][] currentSlice = values[slice];
                    if (currentSlice.Length != Rows) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Matrix_MustHaveSameNumberOfRowsInEverySlice, currentSlice.Length , Rows));
                    for (int row = Rows; --row >= 0;)
                    {
                        Object[] currentRow = currentSlice[row];
                        if (currentRow.Length != Columns) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Matrix_MustHaveSameNumberOfColumnsInEveryRow, currentRow.Length , Columns));
                        Array.Copy(currentRow, 0, this.Elements, i, Columns);
                        i -= Columns;
                    }
                }
            }
            else
            {
                base.Assign(values);
            }
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same number of Slices, Rows and Columns.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        /// </summary>
        /// <param name="source">the source matrix to copy from (may be identical to the receiver).</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>Slices != source.Slices || Rows != source.Rows || Columns != source.Columns</i></exception>
        public override ObjectMatrix3D Assign(ObjectMatrix3D source)
        {
            // overriden for performance only
            if (!(source is DenseObjectMatrix3D))
            {
                return base.Assign(source);
            }
            DenseObjectMatrix3D other = (DenseObjectMatrix3D)source;
            if (other == this) return this;
            CheckShape(other);
            if (HaveSharedCells(other))
            {
                ObjectMatrix3D c = other.Copy();
                if (!(c is DenseObjectMatrix3D))
                { // should not happen
                    return base.Assign(source);
                }
                other = (DenseObjectMatrix3D)c;
            }

            if (!this.IsView && !other.IsView)
            { // quickest
                Array.Copy(other.Elements, 0, this.Elements, 0, this.Elements.Length);
                return this;
            }
            return base.Assign(other);
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>[slice,row,column]</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices || row&lt;0 || row&gt;=Rows || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        [Obsolete("GetQuick(int slice, int index, int column) is deprecated, please use indexer instead.")]
        public override Object GetQuick(int slice, int row, int column)
        {
            return this[slice, row, column];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share common cells.
        /// More formally, returns <i>true</i> if <i>other != null</i> and at least one of the following conditions is met
        /// <ul>
        /// <li>the receiver is a view of the other matrix
        /// <li>the other matrix is a view of the receiver
        /// <li><i>this == other</i>
        /// </ul>
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix3D other)
        {
            if (other is SelectedDenseObjectMatrix3D)
            {
                SelectedDenseObjectMatrix3D otherMatrix = (SelectedDenseObjectMatrix3D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is DenseObjectMatrix3D)
            {
                DenseObjectMatrix3D otherMatrix = (DenseObjectMatrix3D)other;
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
        /// <returns>The value of the position</returns>
        protected new int Index(int slice, int row, int column)
        {
            //return _sliceOffset(_sliceRank(slice)) + _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
            //manually inlined:
            return SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of Slices, Rows and Columns.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix3D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix3D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override ObjectMatrix3D Like(int Slices, int Rows, int Columns)
        {
            return new DenseObjectMatrix3D(Slices, Rows, Columns);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix2D</i>, etc.
        /// </summary>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <param name="RowZero">the position of the first element.</param>
        /// <param name="ColumnZero">the position of the first element.</param>
        /// <param name="RowStride">the number of elements between two Rows, i.ed <i>index(i+1,j)-index(i,j)</i>.</param>
        /// <param name="ColumnStride">the number of elements between two Columns, i.ed <i>index(i,j+1)-index(i,j)</i>.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        protected override ObjectMatrix2D Like2D(int Rows, int Columns, int RowZero, int ColumnZero, int RowStride, int ColumnStride)
        {
            return new DenseObjectMatrix2D(Rows, Columns, this.Elements, RowZero, ColumnZero, RowStride, ColumnStride);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices || row&lt;0 || row&gt;=Rows || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int slice, int index, int column) is deprecated, please use indexer instead.")]
        public override void SetQuick(int slice, int row, int column, Object value)
        {
            this[slice, row, column] = value;
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
            return new SelectedDenseObjectMatrix3D(this.Elements, sliceOffsets, rowOffsets, columnOffsets, 0);
        }

        public override string ToString(int slice, int row, int column)
        {
            return this[slice, row, column].ToString();
        }
    }
}
