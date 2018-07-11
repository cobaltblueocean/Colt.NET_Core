using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.Implementation
{
    public class SelectedSparseObjectMatrix3D : ObjectMatrix3D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal IDictionary<int, Object> Elements { get; private set; }

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

        /// <summary>
        /// The offset.
        /// </summary>
        protected int offset;

        /// <summary>
        /// Constructs a matrix view with the given parameters.
        /// @param elements the cells.
        /// @param  sliceOffsets   The slice offsets of the cells that shall be visible.
        /// @param  rowOffsets   The row offsets of the cells that shall be visible.
        /// @param  columnOffsets   The column offsets of the cells that shall be visible.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="sliceOffsets"></param>
        /// <param name="rowOffsets"></param>
        /// <param name="columnOffsets"></param>
        /// <param name="offset"></param>
        public SelectedSparseObjectMatrix3D(IDictionary<int, Object> elements, int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets, int offset)
        {
            // be sure parameters are valid, we do not check...
            int slices = sliceOffsets.Length;
            int rows = rowOffsets.Length;
            int columns = columnOffsets.Length;
            Setup(slices, rows, columns);

            this.Elements = elements;

            this.sliceOffsets = sliceOffsets;
            this.rowOffsets = rowOffsets;
            this.columnOffsets = columnOffsets;

            this.offset = offset;

            this.IsView = true;
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
        protected new int ColumnOffset(int absRank)
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
        protected new int RowOffset(int absRank)
        {
            return rowOffsets[absRank];
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
        protected new int SliceOffset(int absRank)
        {
            return sliceOffsets[absRank];
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
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        [Obsolete("GetQuick(int slice, int row, int column) is deprecated, please use indexer instead.")]
        public Object GetQuick(int slice, int row, int column)
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
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected Boolean haveSharedCellsRaw(ObjectMatrix3D other)
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
        /// 
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the third-coordinate.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected int index(int slice, int row, int column)
        {
            //return this.offset + base.index(slice,row,column);
            //manually inlined:
            return this.offset + sliceOffsets[SliceZero + slice * SliceStride] + rowOffsets[RowZero + row * RowStride] + columnOffsets[ColumnZero + column * ColumnStride];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of slices, rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix3D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix3D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// 
        /// @param slices the number of slices the matrix shall have.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @return  a new empty matrix of the same dynamic type.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public override ObjectMatrix3D Like(int slices, int rows, int columns)
        {
            return new SparseObjectMatrix3D(slices, rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix2D</i>, etc.
        /// 
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @param RowZero the position of the first element.
        /// @param ColumnZero the position of the first element.
        /// @param RowStride the number of elements between two rows, i.ed <i>index(i+1,j)-index(i,j)</i>.
        /// @param ColumnStride the number of elements between two columns, i.ed <i>index(i,j+1)-index(i,j)</i>.
        /// @return  a new matrix of the corresponding dynamic type.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override ObjectMatrix2D Like2D(int rows, int columns, int RowZero, int ColumnZero, int RowStride, int ColumnStride)
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
        /// 
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @param    value the value to be filled into the specified cell.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        [Obsolete("SetQuick(int slice, int row, int column, double value) is deprecated, please use indexer instead.")]
        public void SetQuick(int slice, int row, int column, Object value)
        {
        }

        /// <summary>
        /// Sets up a matrix with a given number of slices and rows.
        /// @param slices the number of slices the matrix shall have.
        /// @param rows the number of rows the matrix shall have.
        /// @param columns the number of columns the matrix shall have.
        /// @throws	ArgumentException if <i>(Object)rows*slices > int.MaxValue</i>.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected new void Setup(int slices, int rows, int columns)
        {
            base.Setup(slices, rows, columns);
            this.SliceStride = 1;
            this.RowStride = 1;
            this.ColumnStride = 1;
            this.offset = 0;
        }

        /// <summary>
        /// Self modifying version of viewDice().
        /// @throws ArgumentException if some of the parameters are equal or not in range 0..2.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected new AbstractMatrix3D VDice(int axis0, int axis1, int axis2)
        {
            base.VDice(axis0, axis1, axis2);

            // swap offsets
            int[][] offsets = new int[3][];
            offsets[0] = this.sliceOffsets;
            offsets[1] = this.rowOffsets;
            offsets[2] = this.columnOffsets;

            this.sliceOffsets = offsets[axis0];
            this.rowOffsets = offsets[axis1];
            this.columnOffsets = offsets[axis2];

            return this;
        }
        /// <summary>
        /// Constructs and returns a new 2-dimensional <i>slice view</i> representing the slices and rows of the given column.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p>
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
        /// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
        /// To obtain 1-dimensional views on subranges, apply both steps.

        /// @param column the index of the column to fix.
        /// @return a new 2-dimensional slice view.
        /// @throws IndexOutOfRangeException if <i>column < 0 || column >= columns()</i>.
        /// @see #viewSlice(int)
        /// @see #viewRow(int)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public new ObjectMatrix2D ViewColumn(int column)
        {
            CheckColumn(column);

            int viewRows = this.Slices;
            int viewColumns = this.Rows;

            int viewRowZero = SliceZero;
            int viewColumnZero = RowZero;
            int viewOffset = this.offset + ColumnOffset(ColumnRank(column));

            int viewRowStride = this.SliceStride;
            int viewColumnStride = this.RowStride;

            int[] viewRowOffsets = this.sliceOffsets;
            int[] viewColumnOffsets = this.rowOffsets;

            return new SelectedSparseObjectMatrix2D(viewRows, viewColumns, this.Elements, viewRowZero, viewColumnZero, viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets, viewOffset);
        }

        /// <summary>
        /// Constructs and returns a new 2-dimensional <i>slice view</i> representing the slices and columns of the given row.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p>
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
        /// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
        /// To obtain 1-dimensional views on subranges, apply both steps.

        /// @param row the index of the row to fix.
        /// @return a new 2-dimensional slice view.
        /// @throws IndexOutOfRangeException if <i>row < 0 || row >= row()</i>.
        /// @see #viewSlice(int)
        /// @see #viewColumn(int)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public new ObjectMatrix2D ViewRow(int row)
        {
            CheckRow(row);

            int viewRows = this.Slices;
            int viewColumns = this.Columns;

            int viewRowZero = SliceZero;
            int viewColumnZero = ColumnZero;
            int viewOffset = this.offset + RowOffset(RowRank(row));

            int viewRowStride = this.SliceStride;
            int viewColumnStride = this.ColumnStride;

            int[] viewRowOffsets = this.sliceOffsets;
            int[] viewColumnOffsets = this.columnOffsets;

            return new SelectedSparseObjectMatrix2D(viewRows, viewColumns, this.Elements, viewRowZero, viewColumnZero, viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets, viewOffset);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// 
        /// @param sliceOffsets the offsets of the visible elements.
        /// @param rowOffsets the offsets of the visible elements.
        /// @param columnOffsets the offsets of the visible elements.
        /// @return  a new view.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        protected override ObjectMatrix3D ViewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedSparseObjectMatrix3D(this.Elements, sliceOffsets, rowOffsets, columnOffsets, this.offset);
        }

        /// <summary>
        /// Constructs and returns a new 2-dimensional <i>slice view</i> representing the rows and columns of the given slice.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p>
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
        /// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
        /// To obtain 1-dimensional views on subranges, apply both steps.

        /// @param slice the index of the slice to fix.
        /// @return a new 2-dimensional slice view.
        /// @throws IndexOutOfRangeException if <i>slice < 0 || slice >= slices()</i>.
        /// @see #viewRow(int)
        /// @see #viewColumn(int)
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public new ObjectMatrix2D ViewSlice(int slice)
        {
            CheckSlice(slice);

            int viewRows = this.Rows;
            int viewColumns = this.Columns;

            int viewRowZero = RowZero;
            int viewColumnZero = ColumnZero;
            int viewOffset = this.offset + SliceOffset(SliceRank(slice));

            int viewRowStride = this.RowStride;
            int viewColumnStride = this.ColumnStride;

            int[] viewRowOffsets = this.rowOffsets;
            int[] viewColumnOffsets = this.columnOffsets;

            return new SelectedSparseObjectMatrix2D(viewRows, viewColumns, this.Elements, viewRowZero, viewColumnZero, viewRowStride, viewColumnStride, viewRowOffsets, viewColumnOffsets, viewOffset);
        }
    }
}
