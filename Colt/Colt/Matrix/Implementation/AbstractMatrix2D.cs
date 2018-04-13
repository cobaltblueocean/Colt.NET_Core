// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for 2-d matrices holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;

    /// <summary>
    /// Abstract base class for 2-d matrices holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
    /// </summary>
    /// <remarks>This implementation is not synchronized.</remarks>
    public abstract class AbstractMatrix2D : AbstractMatrix
    {
        /// <summary>
        /// Gets or sets the number of colums this matrix (view) has.
        /// </summary>
        public int Columns { get; protected set; }

        /// <summary>
        /// Gets or sets the number of rows this matrix (view) has.
        /// </summary>
        public int Rows { get; protected set; }

        /// <summary>
        /// Gets or sets the number of elements between two rows, i.e. <tt>index(i+1,j,k) - index(i,j,k)</tt>.
        /// </summary>
        protected int rowStride { get; set; }

        /// <summary>
        /// Gets or sets the number of elements between two columns, i.e. <tt>index(i,j+1,k) - index(i,j,k)</tt>.
        /// </summary>
        protected int columnStride { get; set; }

        /// <summary>
        /// Gets or sets the row index of the first element.
        /// </summary>
        protected int rowZero { get; set; }

        /// <summary>
        /// Gets or sets the column index of the first element.
        /// </summary>
        protected int columnZero { get; set; }

        /// <summary>
        /// Sanity check for operations requiring two matrices with the same number of columns and rows.
        /// </summary>
        /// <param name="b">
        /// The second matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>columns() != B.columns() || rows() != B.rows()</tt>.
        /// </exception>
        public void checkShape(AbstractMatrix2D b)
        {
            if (Columns != b.Columns || Rows != b.Rows) throw new ArgumentOutOfRangeException("Incompatible dimensions: " + this + " and " + b);
        }

        /// <summary>
        /// Sanity check for operations requiring matrices with the same number of columns and rows.
        /// </summary>
        /// <param name="b">
        /// The second matrix.
        /// </param>
        /// <param name="c">
        /// The third matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>columns() != B.columns() || rows() != B.rows() || columns() != C.columns() || rows() != C.rows()</tt>.
        /// </exception>
        public void checkShape(AbstractMatrix2D b, AbstractMatrix2D c)
        {
            if (Columns != b.Columns || Rows != b.Rows || Columns != c.Columns || Rows != c.Rows) throw new ArgumentOutOfRangeException("Incompatible dimensions: " + this + ", " + b + ", " + c);
        }

        /// <summary>
        /// Returns the number of cells which is <tt>rows()*columns()</tt>.
        /// </summary>
        /// <returns>
        /// The number of cells.
        /// </returns>
        public override int Size()
        {
            return Rows * Columns;
        }

        /// <summary>
        /// Teturns a string representation of the receiver's shape.
        /// </summary>
        /// <returns>
        /// A string representation of the receiver's shape.
        /// </returns>
        public override string ToString()
        {
            return AbstractFormatter.Shape(this);
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// Default implementation. Override, if necessary.
        /// </summary>
        /// <param name="absRank">the absolute rank of the element.</param>
        /// <returns>the position.</returns>
        protected virtual int columnOffset(int absRank)
        {
            return absRank;
        }

        /// <summary>
        /// Returns the absolute rank of the given relative rank.
        /// </summary>
        /// <param name="rank">
        /// The relative rank of the element.
        /// </param>
        /// <returns>
        /// The absolute rank of the element.
        /// </returns>
        protected int columnRank(int rank)
        {
            return columnZero + (rank * columnStride);
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
        protected virtual int rowOffset(int absRank)
        {
            return absRank;
        }

        /// <summary>
        /// Returns the absolute rank of the given relative rank.
        /// </summary>
        /// <param name="rank">
        /// The relative rank of the element.
        /// </param>
        /// <returns>
        /// The absolute rank of the element.
        /// </returns>
        protected virtual int rowRank(int rank)
        {
            return rowZero + (rank * rowStride);
        }

        /// <summary>
        /// Checks whether the receiver contains the given box and throws an exception, if necessary.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column &lt; 0 || width &lt; 0 || column+width &gt; columns() || row &lt; 0 || height &lt; 0 || row+height &gt; rows()</tt>
        /// </exception>
        protected void checkBox(int row, int column, int height, int width)
        {
            if (column < 0 || width < 0 || column + width > Columns || row < 0 || height < 0 || row + height > Rows) throw new ArgumentException(this + ", column:" + column + ", row:" + row + " ,width:" + width + ", height:" + height);
        }

        /// <summary>
        /// Sanity check for operations requiring a column index to be within bounds.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column &lt; 0 || column &gt;= columns()</tt>.
        /// </exception>
        protected void checkColumn(int column)
        {
            if (column < 0 || column >= Columns) throw new IndexOutOfRangeException("Attempted to access " + this + " at column=" + column);
        }

        /// <summary>
        /// Checks whether indexes are legal and throws an exception, if necessary.
        /// </summary>
        /// <param name="indexes">
        /// The indexes.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>! (0 &lt;= indexes[i] &lt; columns())</tt> for any i=0..indexes.length()-1.
        /// </exception>
        protected void checkColumnIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= Columns) checkColumn(index);
            }
        }

        /// <summary>
        /// Sanity check for operations requiring a row index to be within bounds.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>row &lt; 0 || row &gt;= rows()</tt>.
        /// </exception>
        protected void checkRow(int row)
        {
            if (row < 0 || row >= Rows) throw new IndexOutOfRangeException("Attempted to access " + this + " at row=" + row);
        }

        /// <summary>
        /// Checks whether indexes are legal and throws an exception, if necessary.
        /// </summary>
        /// <param name="indexes">
        /// The indexes.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// f <tt>! (0 &lt;= indexes[i] &lt; rows())</tt> for any i=0..indexes.length()-1.
        /// </exception>
        protected void checkRowIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= Rows) checkRow(index);
            }
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
        protected virtual int index(int row, int column)
        {
            return rowOffset(rowRank(row)) + columnOffset(columnRank(column));
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
        protected virtual void setUp(int rows, int columns)
        {
            setUp(rows, columns, 0, 0, columns, 1);
        }

        /// <summary>
        /// Sets up a matrix with a given number of rows and columns and the given strides.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <param name="rZero">
        /// The row position of the first element.
        /// </param>
        /// <param name="cZero">
        /// The column position of the first element.
        /// </param>
        /// <param name="rStride">
        /// The number of elements between two rows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
        /// </param>
        /// <param name="cStride">
        /// The number of elements between two columns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <tt>rows&lt;0 || columns&lt;0 || (double)columns*rows &gt; Integer.MAX_VALUE</tt> or flip's are illegal.
        /// </exception>
        protected void setUp(int rows, int columns, int rZero, int cZero, int rStride, int cStride)
        {
            if (rows < 0 || columns < 0) throw new ArgumentException("negative size");
            Rows = rows;
            Columns = columns;

            this.rowZero = rZero;
            this.columnZero = cZero;

            this.rowStride = rStride;
            this.columnStride = cStride;

            isView = false;
            if ((double)columns * rows > int.MaxValue) throw new ArgumentException("matrix too large");
        }

        /// <summary>
        /// Self modifying version of viewColumnFlip().
        /// </summary>
        /// <returns>
        /// A new flip view.
        /// </returns>
        protected AbstractMatrix2D vColumnFlip()
        {
            if (Columns > 0)
            {
                columnZero += (Columns - 1) * columnStride;
                columnStride = -columnStride;
                isView = true;
            }

            return this;
        }

        /// <summary>
        /// Self modifying version of viewDice().
        /// </summary>
        /// <returns>
        /// A new dice view.
        /// </returns>
        protected virtual AbstractMatrix2D vDice()
        {
            // swap;
            int tmp = Rows;
            Rows = Columns;
            Columns = tmp;
            tmp = rowStride;
            rowStride = columnStride;
            columnStride = tmp;
            tmp = rowZero;
            rowZero = columnZero;
            columnZero = tmp;

            // flips stay unaffected
            isView = true;
            return this;
        }

        /// <summary>
        /// Self modifying version of viewPart().
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// The view.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>column &lt; 0 || width &lt; 0 || column+width &gt; columns() || row &lt; 0 || height &lt; 0 || row+height &gt; rows()</tt>
        /// </exception>
        protected AbstractMatrix2D vPart(int row, int column, int height, int width)
        {
            checkBox(row, column, height, width);
            rowZero += rowStride * row;
            columnZero += columnStride * column;
            Rows = height;
            Columns = width;
            isView = true;
            return this;
        }

        /// <summary>
        /// Self modifying version of viewRowFlip().
        /// </summary>
        /// <returns>
        /// A new row flip.
        /// </returns>
        protected AbstractMatrix2D vRowFlip()
        {
            if (Rows > 0)
            {
                rowZero += (Rows - 1) * rowStride;
                rowStride = -rowStride;
                isView = true;
            }

            return this;
        }

        /// <summary>
        /// Self modifying version of viewStrides().
        /// </summary>
        /// <param name="rStride">
        /// The row stride.
        /// </param>
        /// <param name="cStride">
        /// The column stride.
        /// </param>
        /// <returns>
        /// A new stride view.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>rStride &lt;= 0 || cStride &lt;= 0</tt>.
        /// </exception>
        protected AbstractMatrix2D vStrides(int rStride, int cStride)
        {
            if (rStride <= 0 || cStride <= 0) throw new ArgumentException("illegal strides: " + rStride + ", " + cStride);
            this.rowStride *= rStride;
            this.columnStride *= cStride;
            if (Rows != 0) Rows = ((Rows - 1) / rStride) + 1;
            if (Columns != 0) Columns = ((Columns - 1) / cStride) + 1;
            isView = true;
            return this;
        }
    }
}
