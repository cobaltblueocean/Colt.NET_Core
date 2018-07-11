// <copyright file="SelectedSparseObjectMatrix2D.cs" company="CERN">
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
    /// Selection view on sparse 2-d matrices holding <i>Object</i> elements.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Objects of this class are typically constructed via <i>viewIndexes</i> methods on some source matrix.
    /// The interface introduced in abstract base classes defines everything a user can do.
    /// From a user point of view there is nothing special about this class; it presents the same functionality with the same signatures and semantics as its abstract baseclass(es) while introducing no additional functionality.
    /// Thus, this class need not be visible to users.
    /// By the way, the same principle applies to concrete DenseXXX and SparseXXX classes: they presents the same functionality with the same signatures and semantics as abstract baseclass(es) while introducing no additional functionality.
    /// Thus, they need not be visible to users, eitherd 
    /// Factory methods could hide all these concrete types.
    /// <p>
    /// This class uses no delegationd 
    /// Its instances point directly to the datad 
    /// Cell addressing overhead is 1 additional int addition and 2 additional array index accesses per get/set.
    /// <p>
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory [bytes] = 4*(rowIndexes.Length+columnIndexes.Length)</i>.
    /// Thus, an index view with 1000 x 1000 indexes additionally uses 8 KB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// Depends on the parent view holding cells.
    /// <p>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class SelectedSparseObjectMatrix2D : ObjectMatrix2D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal IDictionary<int, Object> Elements { get; private set; }

        /// <summary>
        /// The offsets of the visible cells of this matrix.
        /// </summary>
        protected int[] rowOffsets;
        protected int[] columnOffsets;

        /// <summary>
        /// The offset.
        /// </summary>
        protected int offset;

        public override object this[int rowindex, int colindex]
        {
            get
            {
                //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new IndexOutOfRangeException("row:"+row+", column:"+column);
                //return elements.Get(index(row,column));
                //manually inlined:
                return Elements[offset + rowOffsets[RowZero + rowindex * RowStride] + columnOffsets[ColumnZero + colindex * ColumnStride]];
            }
            set
            {
                //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new IndexOutOfRangeException("row:"+row+", column:"+column);
                //int index =	index(row,column);
                //manually inlined:
                int index = offset + rowOffsets[RowZero + rowindex * RowStride] + columnOffsets[ColumnZero + colindex * ColumnStride];

                if (value == null)
                    this.Elements.Remove(index);
                else
                    this.Elements.Add(index, value);
            }
        }

        /// <summary>
        /// Constructs a matrix view with the given parameters.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @param elements the cells.
        /// @param rowZero the position of the first element.
        /// @param columnZero the position of the first element.
        /// @param rowStride the number of elements between two rows, i.ed <i>index(i+1,j)-index(i,j)</i>.
        /// @param columnStride the number of elements between two columns, i.ed <i>index(i,j+1)-index(i,j)</i>.
        /// @param  rowOffsets   The row offsets of the cells that shall be visible.
        /// @param  columnOffsets   The column offsets of the cells that shall be visible.
        /// @param  offset   
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected SelectedSparseObjectMatrix2D(int rows, int columns, IDictionary<int, Object> elements, int rowZero, int columnZero, int rowStride, int columnStride, int[] rowOffsets, int[] columnOffsets, int offset)
        {
            // be sure parameters are valid, we do not check...
            Setup(rows, columns, rowZero, columnZero, rowStride, columnStride);

            this.Elements = elements;
            this.rowOffsets = rowOffsets;
            this.columnOffsets = columnOffsets;
            this.offset = offset;

            this.IsView = true;
        }

        /// <summary>
        /// Constructs a matrix view with the given parameters.
        /// @param elements the cells.
        /// @param  rowOffsets   The row offsets of the cells that shall be visible.
        /// @param  columnOffsets   The column offsets of the cells that shall be visible.
        /// @param  offset   
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected SelectedSparseObjectMatrix2D(IDictionary<int, Object> elements, int[] rowOffsets, int[] columnOffsets, int offset) : this(rowOffsets.Length, columnOffsets.Length, elements, 0, 0, 1, 1, rowOffsets, columnOffsets, offset)
        {

        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// 
        /// @param  rank   the absolute rank of the element.
        /// @return the position.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override int ColumnOffset(int absRank)
        {
            return columnOffsets[absRank];
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// Default implementationd Override, if necessary.
        /// 
        /// @param  rank   the absolute rank of the element.
        /// @return the position.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override int RowOffset(int absRank)
        {
            return rowOffsets[absRank];
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>[row,column]</i>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// 
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @return    the value at the specified coordinate.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        [Obsolete("GetQuick(int row, int column) is deprecated, please use indexer instead.")]
        public Object GetQuick(int row, int column)
        {
            return this[row, column];
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
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
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
        /// 
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override int Index(int row, int column)
        {
            //return this.offset + base.index(row,column);
            //manually inlined:
            return this.offset + rowOffsets[RowZero + row * RowStride] + columnOffsets[ColumnZero + column * ColumnStride];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must also be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must also be of type <i>SparseObjectMatrix2D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// 
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @return  a new empty matrix of the same dynamic type.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public override ObjectMatrix2D Like(int Rows, int Columns)
        {
            return new SparseObjectMatrix2D(Rows, Columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must be of type <i>SparseObjectMatrix1D</i>, etc.
        /// 
        /// @param size the number of cells the matrix shall have.
        /// @return  a new matrix of the corresponding dynamic type.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public override ObjectMatrix1D Like1D(int size)
        {
            return new SparseObjectMatrix1D(size);
        }


        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must be of type <i>SparseObjectMatrix1D</i>, etc.
        /// 
        /// @param size the number of cells the matrix shall have.
        /// @param zero the index of the first element.
        /// @param stride the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.
        /// @return  a new matrix of the corresponding dynamic type.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override ObjectMatrix1D Like1D(int size, int zero, int stride)
        {
            // this method is never called since viewRow() and viewColumn are overridden properly.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[row,column]</i> to the specified value.
        /// 
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// 
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @param    value the value to be filled into the specified cell.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        [Obsolete("SetQuick(int row, int column, Object value) is deprecated, please use indexer instead.")]
        public void SetQuick(int row, int column, Object value)
        {
            this[row, column] = value;
        }

        /// <summary>
        /// Sets up a matrix with a given number of rows and columns.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @throws	ArgumentException if <i>(Object)columns*rows > int.MaxValue</i>.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override void Setup(int rows, int columns)
        {
            base.Setup(rows, columns);
            this.RowStride = 1;
            this.ColumnStride = 1;
            this.offset = 0;
        }

        /// <summary>
        /// Self modifying version of viewDice().
        /// </summary>
        protected override AbstractMatrix2D VDice()
        {
            base.VDice();
            // swap
            int[] tmp = rowOffsets; rowOffsets = columnOffsets; columnOffsets = tmp;

            // flips stay unaffected

            this.IsView = true;
            return this;
        }
        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the rows of the given column.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>viewPart(..d)</i>), then apply this method to the sub-range view.
        /// <p> 
        /// <b>Example:</b> 
        /// <table border="0">
        ///          <tr nowrap> 
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///            <td>viewColumn(0) ==></td>
        ///            <td valign="top">Matrix1D of size 2:<br>
        ///              1, 4</td>
        ///           </tr>
        /// </table>
        /// @param the column to fix.
        /// @return a new slice view.
        /// @throws ArgumentException if <i>column < 0 || column >= columns()</i>.
        /// @see #viewRow(int)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public new ObjectMatrix1D ViewColumn(int column)
        {
            CheckColumn(column);
            int viewSize = this.Rows;
            int viewZero = this.RowZero;
            int viewStride = this.RowStride;
            int[] viewOffsets = this.rowOffsets;
            int viewOffset = this.offset + ColumnOffset(ColumnRank(column));
            return new SelectedSparseObjectMatrix1D(viewSize, this.Elements, viewZero, viewStride, viewOffsets, viewOffset);
        }

        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the columns of the given row.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>viewPart(..d)</i>), then apply this method to the sub-range view.
        /// <p> 
        /// <b>Example:</b> 
        /// <table border="0">
        ///          <tr nowrap> 
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///            <td>viewRow(0) ==></td>
        ///            <td valign="top">Matrix1D of size 3:<br>
        ///              1, 2, 3</td>
        ///           </tr>
        /// </table>
        /// @param the row to fix.
        /// @return a new slice view.
        /// @throws IndexOutOfRangeException if <i>row < 0 || row >= rows()</i>.
        /// @see #viewColumn(int)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public new ObjectMatrix1D ViewRow(int row)
        {
            CheckRow(row);
            int viewSize = this.Columns;
            int viewZero = ColumnZero;
            int viewStride = this.ColumnStride;
            int[] viewOffsets = this.columnOffsets;
            int viewOffset = this.offset + RowOffset(RowRank(row));
            return new SelectedSparseObjectMatrix1D(viewSize, this.Elements, viewZero, viewStride, viewOffsets, viewOffset);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// 
        /// @param rowOffsets the offsets of the visible elements.
        /// @param columnOffsets the offsets of the visible elements.
        /// @return  a new view.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override ObjectMatrix2D ViewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedSparseObjectMatrix2D(this.Elements, rowOffsets, columnOffsets, this.offset);
        }
    }
}
