// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedDenseDoubleMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Selection view on dense 2-d matrices holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;

    /// <summary>
    /// Selection view on dense 2-d matrices holding <tt>double</tt> elements.
    /// </summary>
    public class SelectedDenseDoubleMatrix2D : DoubleMatrix2D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedDenseDoubleMatrix2D"/> class.
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="rowOffsets">
        /// The row offsets of the cells that shall be visible.
        /// </param>
        /// <param name="columnOffsets">
        /// The column offsets of the cells that shall be visible.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        protected internal SelectedDenseDoubleMatrix2D(double[] elements, int[] rowOffsets, int[] columnOffsets, int offset)
        {
            Setup(rowOffsets.Length, columnOffsets.Length, 0, 0, 1, 1);

            this.Elements = elements;
            this.RowOffsets = rowOffsets;
            this.ColumnOffsets = columnOffsets;
            this.Offset = offset;

            IsView = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedDenseDoubleMatrix2D"/> class.
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="rowZero">
        /// The row of the first element.
        /// </param>
        /// <param name="columnZero">
        /// The column of the first element.
        /// </param>
        /// <param name="rowStride">
        /// The number of elements between two rows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
        /// </param>
        /// <param name="columnStride">
        /// The number of elements between two columns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
        /// </param>
        /// <param name="rowOffsets">
        /// The row offsets of the cells that shall be visible.
        /// </param>
        /// <param name="columnOffsets">
        /// The column offsets of the cells that shall be visible.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        public SelectedDenseDoubleMatrix2D(int rows, int columns, double[] elements, int rowZero, int columnZero, int rowStride, int columnStride, int[] rowOffsets, int[] columnOffsets, int offset)
        {
            // be sure parameters are valid, we do not check...
            Setup(rows, columns, rowZero, columnZero, rowStride, columnStride);

            this.Elements = elements;
            this.RowOffsets = rowOffsets;
            this.ColumnOffsets = columnOffsets;
            this.Offset = offset;

            IsView = true;
        }

        /// <summary>
        /// Gets the elements of this matrix.
        /// </summary>
        protected internal double[] Elements { get; private set; }

        /// <summary>
        /// Gets the row offsets of the visible cells of this matrix.
        /// </summary>
        protected int[] RowOffsets { get; private set; }

        /// <summary>
        /// Gets the column offsets of the visible cells of this matrix.
        /// </summary>
        protected int[] ColumnOffsets { get; private set; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        protected int Offset { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>[row,column]</tt>.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        public override double this[int row, int column]
        {
            get
            {
                return Elements[Offset + RowOffsets[RowZero + (row * RowStride)] + ColumnOffsets[ColumnZero + (column * ColumnStride)]];
            }

            set
            {
                Elements[Offset + RowOffsets[RowZero + (row * RowStride)] + ColumnOffsets[ColumnZero + (column * ColumnStride)]] = value;
            }
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public override DoubleMatrix2D Like(int rows, int columns)
        {
            return new DenseDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        public override DoubleMatrix1D Like1D(int size)
        {
            return new DenseDoubleMatrix1D(size);
        }

        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the rows of the given column.
        /// </summary>
        /// <param name="column">
        /// The column to fix.
        /// </param>
        /// <returns>
        /// A new slice view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column &lt; 0 || column &gt;= columns()</tt>.
        /// </exception>
        public override DoubleMatrix1D ViewColumn(int column)
        {
            CheckColumn(column);
            int viewSize = this.Rows;
            int viewZero = this.RowZero;
            int viewStride = this.RowStride;
            int[] viewOffsets = this.RowOffsets;
            int viewOffset = this.Offset + ColumnOffset(ColumnRank(column));
            return new SelectedDenseDoubleMatrix1D(viewSize, this.Elements, viewZero, viewStride, viewOffsets, viewOffset);
        }

        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the columns of the given row.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <param name="row">
        /// The row to fix.
        /// </param>
        /// <returns>
        /// A new slice view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>row &lt; 0 || row &gt;= Rows</tt>.
        /// </exception>
        public override DoubleMatrix1D ViewRow(int row)
        {
            CheckRow(row);
            int viewSize = this.Columns;
            int viewZero = ColumnZero;
            int viewStride = this.ColumnStride;
            int[] viewOffsets = this.ColumnOffsets;
            int viewOffset = this.Offset + RowOffset(RowRank(row));
            return new SelectedDenseDoubleMatrix1D(viewSize, this.Elements, viewZero, viewStride, viewOffsets, viewOffset);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <param name="zero">
        /// The index of the first element.
        /// </param>
        /// <param name="stride">
        /// The number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        protected internal override DoubleMatrix1D Like1D(int size, int zero, int stride)
        {
            throw new ApplicationException(); // this method is never called since viewRow() and viewColumn are overridden properly.
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array. 
        /// Default implementation. Override, if necessary.
        /// </summary>
        /// <param name="absRank">
        /// The absolute rank of the element.
        /// </param>
        /// <returns>
        /// The position.
        /// </returns>
        protected override int ColumnOffset(int absRank)
        {
            return ColumnOffsets[absRank];
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// Default implementation. Override, if necessary.
        /// </summary>
        /// <param name="absRank">
        /// The absolute rank of the element.
        /// </param>
        /// <returns>
        /// The position.
        /// </returns>
        protected override int RowOffset(int absRank)
        {
            return RowOffsets[absRank];
        }

        /// <summary>
        /// Returns <tt>true</tt> if both matrices share at least one identical cell.
        /// </summary>
        /// <param name="other">
        /// The other matrix.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if both matrices share at least one identical cell.
        /// </returns>
        protected override bool HaveSharedCellsRaw(DoubleMatrix2D other)
        {
            if (other is SelectedDenseDoubleMatrix2D)
            {
                var otherMatrix = (SelectedDenseDoubleMatrix2D)other;
                return this.Elements == otherMatrix.Elements;
            }

            if (other is DenseDoubleMatrix2D)
            {
                var otherMatrix = (DenseDoubleMatrix2D)other;
                return this.Elements == otherMatrix.elements;
            }

            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        /// <returns>
        /// The position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
        /// </returns>
        protected override int Index(int row, int column)
        {
            ////return this.offset + super.index(row,column);
            // manually inlined:
            return this.Offset + RowOffsets[RowZero + (row * RowStride)] + ColumnOffsets[ColumnZero + (column * ColumnStride)];
        }

        /// <summary>
        /// Sets up a matrix with a given number of rows and columns.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <tt>rows &lt; 0 || columns &lt; 0 || (double)columns*rows &gt; Integer.MAX_VALUE</tt>.
        /// </exception>
        protected override void Setup(int rows, int columns)
        {
            base.Setup(rows, columns);
            this.RowStride = 1;
            this.ColumnStride = 1;
            this.Offset = 0;
        }

        /// <summary>
        /// Self modifying version of viewDice().
        /// </summary>
        /// <returns>
        /// A new dice view.
        /// </returns>
        protected override AbstractMatrix2D VDice()
        {
            base.VDice();

            // swap
            int[] tmp = RowOffsets;
            RowOffsets = ColumnOffsets;
            ColumnOffsets = tmp;

            // flips stay unaffected
            IsView = true;
            return this;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="rOffsets">
        /// The row offsets of the visible elements.
        /// </param>
        /// <param name="cOffsets">
        /// The column offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        protected override DoubleMatrix2D ViewSelectionLike(int[] rOffsets, int[] cOffsets)
        {
            return new SelectedDenseDoubleMatrix2D(this.Elements, rOffsets, cOffsets, this.Offset);
        }
    }
}
