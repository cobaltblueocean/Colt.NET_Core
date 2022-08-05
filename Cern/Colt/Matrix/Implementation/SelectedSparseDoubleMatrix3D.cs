// <copyright file="SelectedSparseDoubleMatrix3D.cs" company="CERN">
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
    /// Selection view on sparse 3-d matrices holding<i> double</i> elements.
    /// First see the<a href="package-summary.html"> package summary</a> and javadoc<a href="package-tree.html"> tree view</a> to get the broad picture.
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Objects of this class are typically constructed via<i> viewIndexes</i> methods on some source matrix.
    /// The interface introduced in abstract base classes defines everything a user can do.
    /// From a user point of view there is nothing special about this class; it presents the same functionality with the same signatures and semantics as its abstract baseclass(es) while introducing no additional functionality.
    /// Thus, this class need not be visible to users.
    /// By the way, the same principle applies to concrete DenseXXX and SparseXXX classes: they presents the same functionality with the same signatures and semantics as abstract baseclass(es) while introducing no additional functionality.
    /// Thus, they need not be visible to users, eitherd
    /// Factory methods could hide all these concrete types.
    /// <p>
    /// This class uses no delegationd
    /// Its instances point directly to the datad
    /// Cell addressing overhead is is 1 additional int addition and 3 additional array index accesses per get/set.
    /// <p>
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory[bytes] = 4 * (sliceIndexes.Length + rowIndexes.Length + columnIndexes.Length) </ i >.
    /// Thus, an index view with 100 x 100 x 100 indexes additionally uses 8 KB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// Depends on the parent view holding cells.
    /// <p>
    /// @author wolfgang.hoschek @cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class SelectedSparseDoubleMatrix3D : DoubleMatrix3D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal IDictionary<int, Double> Elements { get; private set; }

        /// <summary>
        /// The offsets of the visible cells of this matrix.
        /// </summary>
        protected int[] SliceOffsets;
        protected int[] RowOffsets;
        protected int[] ColumnOffsets;

        /// <summary>
        /// The offset.
        /// </summary>
        protected int Offset;

        /// <summary>
        /// Get or set value coordinate with the indexes
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value to be filled into the specified cell.</returns>
        public override double this[int slice, int row, int column]
        {
            get
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //return elements.Get(index(slice,row,column));
                //manually inlined:
                return Elements[Index(slice, row, column)];
            }
            set
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //int index =	index(slice,row,column);
                //manually inlined:
                int index = Index(slice, row, column);
                if (value == 0)
                    this.Elements.Remove(index);
                else
                    this.Elements.Add(index, value);
            }
        }

        /// <summary>
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="elements">the cells.</param>
        /// <param name="sliceOffsets">The slice offsets of the cells that shall be visible.</param>
        /// <param name="rowOffsets">The row offsets of the cells that shall be visible.</param>
        /// <param name="columnOffsets">The column offsets of the cells that shall be visible.</param>
        public SelectedSparseDoubleMatrix3D(IDictionary<int, Double> elements, int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets, int offset)
        {
            // be sure parameters are valid, we do not check...
            int slices = sliceOffsets.Length;
            int rows = rowOffsets.Length;
            int columns = columnOffsets.Length;
            Setup(slices, rows, columns);

            this.Elements = elements;

            this.SliceOffsets = sliceOffsets;
            this.RowOffsets = rowOffsets;
            this.ColumnOffsets = columnOffsets;

            this.Offset = offset;

            this.IsView = true;
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// </summary>
        /// <param name="rank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected new int ColumnOffset(int absRank)
        {
            return ColumnOffsets[absRank];
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// </summary>
        /// <param name="rank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected new int RowOffset(int absRank)
        {
            return RowOffsets[absRank];
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// </summary>
        /// <param name="rank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected new int SliceOffset(int absRank)
        {
            return SliceOffsets[absRank];
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
        public override Double GetQuick(int slice, int row, int column)
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
        protected new Boolean HaveSharedCellsRaw(DoubleMatrix3D other)
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
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the third-coordinate.</param>
        protected new int Index(int slice, int row, int column)
        {
            //return this.offset + base.index(slice,row,column);
            //manually inlined:
            return this.Offset + SliceOffsets[SliceZero + slice * SliceStride] + RowOffsets[RowZero + row * RowStride] + ColumnOffsets[ColumnZero + column * ColumnStride];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of slices, rows and columns.
        /// For example, if the receiver is an instance of type <see cref="DenseDoubleMatrix3D"/> the new matrix must also be of type <see cref="DenseDoubleMatrix3D"/>,
        /// if the receiver is an instance of type <see cref="SparseDoubleMatrix3D"/> the new matrix must also be of type <see cref="SparseDoubleMatrix3D"/>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="slices">the number of slices the matrix shall have.</param>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override DoubleMatrix3D Like(int Slices, int Rows, int Columns)
        {
            return new SparseDoubleMatrix3D(Slices, Rows, Columns);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <see cref="DenseDoubleMatrix3D"/> the new matrix must also be of type <see cref="DenseDoubleMatrix2D"/>,
        /// if the receiver is an instance of type <see cref="SparseDoubleMatrix3D"/> the new matrix must also be of type <see cref="SparseDoubleMatrix2D"/>, etc.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="rowStride">the number of elements between two rows, i.ed <i>index(i+1,j)-index(i,j)</i>.</param>
        /// <param name="columnStride">the number of elements between two columns, i.ed <i>index(i,j+1)-index(i,j)</i>.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        protected override DoubleMatrix2D Like2D(int Rows, int Columns, int rowZero, int columnZero, int Rowstride, int Columnstride)
        {
            // this method is never called since viewRow() and viewColumn are overridden properly.
            throw new NotImplementedException();
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
        [Obsolete("SetQuick(int slice, int row, int column, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int slice, int row, int column, double value)
        {
            this[slice, row, column] = value;
        }

        /// <summary>
        /// Sets up a matrix with a given number of slices and rows.
        /// </summary>
        /// <param name="slices">the number of slices the matrix shall have.</param>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>(double)rows*slices > int.MaxValue</i>.</exception>
        protected new void Setup(int slices, int rows, int columns)
        {
            base.Setup(slices, rows, columns);
            this.SliceStride = 1;
            this.RowStride = 1;
            this.ColumnStride = 1;
            this.Offset = 0;
        }

        /// <summary>
        /// Self modifying version of viewDice().
        /// </summary>
        /// <exception cref="ArgumentException">if some of the parameters are equal or not in range 0..2.</exception>
        protected new AbstractMatrix3D VDice(int axis0, int axis1, int axis2)
        {
            base.VDice(axis0, axis1, axis2);

            // swap offsets
            int[][] offsets = new int[3][];
            offsets[0] = this.SliceOffsets;
            offsets[1] = this.RowOffsets;
            offsets[2] = this.ColumnOffsets;

            this.SliceOffsets = offsets[axis0];
            this.RowOffsets = offsets[axis1];
            this.ColumnOffsets = offsets[axis2];

            return this;
        }

        /// <summary>
        /// Constructs and returns a new 2-dimensional <i>slice view</i> representing the slices and rows of the given column.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p>
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
        /// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
        /// To obtain 1-dimensional views on subranges, apply both steps.
        /// </summary>
        /// <param name="column">the index of the column to fix.</param>
        /// <returns>a new 2-dimensional slice view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>column &lt; 0 || column >= columns()</i>.</exception>
        /// <see cref="ViewSlice(int)"/>
        /// <see cref="ViewRow(int)"/>
        public override DoubleMatrix2D ViewColumn(int column)
        {
            CheckColumn(column);

            int viewRows = this.Slices;
            int viewColumns = this.Rows;

            int viewRowZero = SliceZero;
            int viewColumnZero = RowZero;
            int viewOffset = this.Offset + ColumnOffset(ColumnRank(column));

            int viewRowStride = this.SliceStride;
            int viewColumnStride = this.RowStride;

            int[] viewRowOffsets = this.SliceOffsets;
            int[] viewColumnOffsets = this.RowOffsets;

            return new SelectedSparseDoubleMatrix2D(viewRows, viewColumns, this.Elements, viewRowZero, viewColumnZero, viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets, viewOffset);
        }

        /// <summary>
        /// Constructs and returns a new 2-dimensional <i>slice view</i> representing the slices and columns of the given row.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p>
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
        /// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
        /// To obtain 1-dimensional views on subranges, apply both steps.
        /// </summary>
        /// <param name="row">the index of the row to fix.</param>
        /// <returns>a new 2-dimensional slice view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>row &lt; 0 || row >= row()</i>.</exception>
        /// <see cref="ViewSlice(int)"/>
        /// <see cref="ViewColumn(int)"/>
        public override DoubleMatrix2D ViewRow(int row)
        {
            CheckRow(row);

            int viewRows = this.Slices;
            int viewColumns = this.Columns;

            int viewRowZero = SliceZero;
            int viewColumnZero = ColumnZero;
            int viewOffset = this.Offset + RowOffset(RowRank(row));

            int viewRowStride = this.SliceStride;
            int viewColumnStride = this.ColumnStride;

            int[] viewRowOffsets = this.SliceOffsets;
            int[] viewColumnOffsets = this.ColumnOffsets;

            return new SelectedSparseDoubleMatrix2D(viewRows, viewColumns, this.Elements, viewRowZero, viewColumnZero, viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets, viewOffset);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="sliceOffsets">the offsets of the visible elements.</param>
        /// <param name="rowOffsets">the offsets of the visible elements.</param>
        /// <param name="columnOffsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override DoubleMatrix3D ViewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedSparseDoubleMatrix3D(this.Elements, sliceOffsets, rowOffsets, columnOffsets, this.Offset);
        }

        /// <summary>
        /// Constructs and returns a new 2-dimensional <i>slice view</i> representing the rows and columns of the given slice.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p>
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
        /// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
        /// To obtain 1-dimensional views on subranges, apply both steps.
        /// </summary>
        /// <param name="slice">the index of the slice to fix.</param>
        /// <returns>a new 2-dimensional slice view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>slice &lt; 0 || slice >= slices()</i>.</exception>
        /// <see cref="ViewRow(int)"/>
        /// <see cref="ViewColumn(int)"/>
        public override DoubleMatrix2D ViewSlice(int slice)
        {
            CheckSlice(slice);

            int viewRows = this.Rows;
            int viewColumns = this.Columns;

            int viewRowZero = RowZero;
            int viewColumnZero = ColumnZero;
            int viewOffset = this.Offset + SliceOffset(SliceRank(slice));

            int viewRowStride = this.RowStride;
            int viewColumnStride = this.ColumnStride;

            int[] viewRowOffsets = this.RowOffsets;
            int[] viewColumnOffsets = this.ColumnOffsets;

            return new SelectedSparseDoubleMatrix2D(viewRows, viewColumns, this.Elements, viewRowZero, viewColumnZero, viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets, viewOffset);
        }

        public override string ToString(int slice, int row, int column)
        {
            return this[slice, row, column].ToString();
        }
    }
}
