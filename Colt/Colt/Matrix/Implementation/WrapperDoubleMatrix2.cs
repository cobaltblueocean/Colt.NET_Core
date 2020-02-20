// <copyright file="WrapperDoubleMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
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
    /// 2-d matrix holding <tt>double</tt> elements; either a view wrapping another matrix or a matrix whose views are wrappers.
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 04/14/2000
    /// </summary>
    public class WrapperDoubleMatrix2D : DoubleMatrix2D
    {
        /// <summary>
        /// The elements of the matrix.
        /// </summary>
        private DoubleMatrix2D _content;

        public WrapperDoubleMatrix2D(DoubleMatrix2D newContent)
        {
            if (newContent != null) Setup(newContent.Rows, newContent.Columns);
            this._content = newContent;
        }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <i>index</i>.
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size()</i>.
        /// </summary>
        /// <param name="row">The index of the row-coordinate.</param>
        /// <param name="column">The index of the column-coordinate.</param>
        /// <returns>the value of the specified cell.</returns>
        public override double this[int row, int column]
        {
            get { return _content[row, column]; }
            set { _content[row, column] = value; }
        }

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        public DoubleMatrix2D Content
        {
            get
            {
                return this._content;
            }
            protected set
            {
                this._content = value;
            }
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size()</i>.
        /// </summary>
        /// <param name="row">The index of the row-coordinate.</param>
        /// <param name="column">The index of the column-coordinate.</param>
        /// <returns>the value of the specified cell.</returns>
        [Obsolete("GetQuick(int row, int column) is deprecated, please use indexer instead.")]
        public override Double GetQuick(int row, int column)
        {
            return _content[row, column];
        }

        public override DoubleMatrix2D Like(int rows, int columns)
        {
            return _content.Like(rows, columns);
        }

        public override DoubleMatrix1D Like1D(int size)
        {
            return _content.Like1D(size);
        }

        public override DoubleMatrix1D Like1D(int rows, int columns, int stride)
        {
            return _content.Like1D(rows, columns, stride);
        }

        /// <summary>
        /// Sets the matrix cell value at coordinate <i>index</i>.
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size()</i>.
        /// </summary>
        /// <param name="row">The index of the row-coordinate.</param>
        /// <param name="column">The index of the column-coordinate.</param>
        /// <returns>the value of the specified cell.</returns>
        [Obsolete("SetQuick(int row, int column, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int row, int column, double value)
        {
            _content[row, column] = value;
        }

        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the rows of the given column.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To obtain a slice view on subranges, construct a sub-ranging view (<tt>viewPart(...)</tt>), then apply this method to the sub-range view.
        /// </summary>
        /// <param name="column">the column to fix.</param>
        /// <returns>a new slice view.</returns>
        /// <exception cref="IndexOutOfBoundsException">if <tt>column &lt; 0 || column >= columns()</tt>.</exception>
        /// <example>
        /// 2 x 3 matrix: 
        ///     |1, 2, 3|
        ///     |4, 5, 6|
        /// 
        /// viewColumn(0) ==> Matrix1D of size 2:
        ///                      |1, 4|
        /// </example>
        public override DoubleMatrix1D ViewColumn(int column)
        {
            return ViewDice().ViewRow(column);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i>.
        /// What used to be index <i>0</i> is now index <i>Size()-1</i>, ..d, what used to be index <i>Size()-1</i> is now index <i>0</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>a new flip view.</returns>
        public override DoubleMatrix2D ViewColumnFlip()
        {
            DoubleMatrix2D view = new WrapperDoubleMatrix2DColumnFlip(this);
            return view;
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i>.
        /// What used to be index <i>0</i> is now index <i>Size()-1</i>, ..d, what used to be index <i>Size()-1</i> is now index <i>0</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>a new flip view.</returns>
        public override DoubleMatrix2D ViewDice()
        {
            DoubleMatrix2D view = new WrapperDoubleMatrix2DDice(this, Columns, Rows);
            return view;
        }

        /// <summary>
        /// Constructs and returns a new <i>sub-range view</i> that is a <i>width</i> sub matrix starting at <i>index</i>.
        /// 
        /// Operations on the returned view can only be applied to the restricted range.
        /// Any attempt to access coordinates not contained in the view will throw an <i>IndexOutOfRangeException</i>.
        /// <p>
        /// <b>Note that the view is really just a range restriction:</b> 
        /// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versad 
        /// <p>
        /// The view contains the cells from <i>index..index+width-1</i>.
        /// and has <i>view.Count == width</i>.
        /// A view's legal coordinates are again zero based, as usual.
        /// In other words, legal coordinates of the view are <i>0 .d view.Count-1==width-1</i>.
        /// As usual, any attempt to access a cell at other coordinates will throw an <i>IndexOutOfRangeException</i>.
        /// </summary>
        /// <param name="row">The index of the row-coordinate.</param>
        /// <param name="column">The index of the column-coordinate.</param>
        /// <param name="height">The height of the box.</param>
        /// <param name="width">The width of the box.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || width &lt; 0 || index + width > Size()</i>.</exception>
        public override DoubleMatrix2D ViewPart(int row, int column, int height, int width)
        {
            CheckBox(row, column, height, width);
            DoubleMatrix2D view = new WrapperDoubleMatrix2DPart(this, row, column, height, width);

            view.Size = width;
            return view;
        }

        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the columns of the given row.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To obtain a slice view on subranges, construct a sub-ranging view (<tt>viewPart(...)</tt>), then apply this method to the sub-range view.
        /// </summary>
        /// <param name="row">the row to fix.</param>
        /// <returns>a new slice view.</returns>
        /// <exception cref="IndexOutOfBoundsException">if <tt>row &lt; 0 || row >= rows()</tt>.</exception>
        /// <example>
        /// 2 x 3 matrix: 
        ///    |1, 2, 3|
        ///    |4, 5, 6|
        /// 
        /// viewRow(0) ==> Matrix1D of size 3:
        ///                         |1, 2, 3|
        /// </example>
        public override DoubleMatrix1D ViewRow(int row)
        {
            CheckRow(row);
            return new DelegateDoubleMatrix1D(this, row);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i> along the row axis.
        /// What used to be row <tt>0</tt> is now row <tt>rows()-1</tt>, ..., what used to be row <tt>rows()-1</tt> is now row <tt>0</tt>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>a new flip view.</returns>
        /// <example>
        /// 2 x 3 matrix: 
        ///    |1, 2, 3|
        ///    |4, 5, 6|
        /// 
        /// rowFlip ==> 2 x 3 matrix:
        ///                   |4, 5, 6|
        ///                   |1, 2, 3|
        /// 
        /// rowFlip ==> 2 x 3 matrix:
        ///                   |1, 2, 3|
        ///                   |4, 5, 6|
        /// 
        /// 
        /// </example>
        public override DoubleMatrix2D ViewRowFlip()
        {
            if (Rows == 0) return this;
            DoubleMatrix2D view = new WrapperDoubleMatrix2DRowFlip(this);
            return view;
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        /// There holds <i>view.Count == indexes.Length</i> and <i>view.Get(i) == this.Get(indexes[i])</i>.
        /// Indexes can occur multiple times and can be in arbitrary order.
        /// 
        /// Note that modifying <i>indexes</i> after this call has returned has no effect on the view.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 
        /// </summary>
        /// <param name="indexes">The indexes of the cells that shall be visible in the new viewd To indicate that <i>all</i> cells shall be visible, simply set this parameter to <i>null</i>.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>!(0 &lt;= indexes[i] &lt; Size)</i> for any <i>i=0..indexes.Length()-1</i>.</exception>
        /// <example>
        /// this     = (0,0,8,0,7)
        /// indexes  = (0,2,4,2)
        /// -->
        /// view     = (0,8,7,8)
        /// </example>
        public override DoubleMatrix2D ViewSelection(int[] rowIndexes, int[] columnIndexes)
        {
            // check for "all"
            if (rowIndexes == null)
            {
                rowIndexes = new int[Rows];
                for (int i = Rows; --i >= 0;) rowIndexes[i] = i;
            }
            if (columnIndexes == null)
            {
                columnIndexes = new int[Columns];
                for (int i = Columns; --i >= 0;) columnIndexes[i] = i;
            }

            CheckRowIndexes(rowIndexes);
            CheckColumnIndexes(columnIndexes);
            int[] rix = rowIndexes;
            int[] cix = columnIndexes;

            DoubleMatrix2D view = new WrapperDoubleMatrix2DSelection(this, rix, cix);
            return view;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override DoubleMatrix2D ViewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
        /// More specifically, the view has Size <i>this.Count/stride</i> holding cells <i>this.Get(i*stride)</i> for all <i>i = 0..Count/stride - 1</i>.
        /// </summary>
        /// <param name="_stride">the step factor.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>stride &lt;= 0</i>.</exception>
        public override DoubleMatrix2D ViewStrides(int _rowStride, int _columnStride)
        {
            if (_rowStride <= 0 || _columnStride <= 0) throw new IndexOutOfRangeException("illegal stride");
            DoubleMatrix2D view = new WrapperDoubleMatrix2DStrides(this, _rowStride, _columnStride);

            return view;
        }

        public override string ToString(int row, int column)
        {
            return this[row, column].ToString();
        }

        private class WrapperDoubleMatrix2DColumnFlip : WrapperDoubleMatrix2D
        {
            public WrapperDoubleMatrix2DColumnFlip(WrapperDoubleMatrix2D newcontent) : base(newcontent._content)
            { }

            public override double this[int row, int column]
            {
                get { return _content[row, Columns - 1 - column]; }
                set { _content[row, Columns - 1 - column] = value; }
            }
        }

        private class WrapperDoubleMatrix2DRowFlip : WrapperDoubleMatrix2D
        {
            public WrapperDoubleMatrix2DRowFlip(WrapperDoubleMatrix2D newcontent) : base(newcontent._content)
            { }

            public override double this[int row, int column]
            {
                get { return _content[Rows - 1 - row, column]; }
                set { _content[Rows - 1 - row, column] = value; }
            }
        }

        private class WrapperDoubleMatrix2DDice : WrapperDoubleMatrix2D
        {
            public WrapperDoubleMatrix2DDice(WrapperDoubleMatrix2D newcontent, int rows, int columns) : base(newcontent._content)
            {
                Setup(columns, rows);
            }

            public override double this[int row, int column]
            {
                get { return _content[column, row]; }
                set { _content[column, row] = value; }
            }
        }

        private class WrapperDoubleMatrix2DPart : WrapperDoubleMatrix2D
        {
            int rowOffset;
            int columnOffset;

            public WrapperDoubleMatrix2DPart(WrapperDoubleMatrix2D newcontent, int rowoffset, int columnoffset, int height, int width) : base(newcontent._content)
            {
                rowOffset = rowoffset;
                columnOffset = columnoffset;
                Setup(height, width);
            }

            public override double this[int row, int column]
            {
                get { return _content[row + rowOffset, column + columnOffset]; }
                set { _content[row + rowOffset, column + columnOffset] = value; }
            }
        }

        private class WrapperDoubleMatrix2DSelection : WrapperDoubleMatrix2D
        {
            int[] rdx;
            int[] cdx;

            public WrapperDoubleMatrix2DSelection(WrapperDoubleMatrix2D newcontent, int[] rowindexes, int[] colindexes) : base(newcontent._content)
            {
                rdx = rowindexes;
                cdx = colindexes;

                Setup(rdx.Length, cdx.Length);
            }

            public override double this[int row, int column]
            {
                get { return _content[rdx[row], cdx[column]]; }
                set { _content[rdx[row], cdx[column]] = value; }
            }
        }

        private class WrapperDoubleMatrix2DStrides : WrapperDoubleMatrix2D
        {
            int _rowstride;
            int _colstride;

            public WrapperDoubleMatrix2DStrides(WrapperDoubleMatrix2D newcontent, int rowstride, int colstride) : base(newcontent._content)
            {
                _rowstride = rowstride;
                _colstride = colstride;

                int _rows, _columns;
                _rows = newcontent.Rows;
                _columns = newcontent.Columns;
                if (newcontent.Rows != 0) _rows = (_rows - 1) / _rowstride + 1;
                if (newcontent.Columns != 0) _columns = (_columns - 1) / _colstride + 1;

                Setup(_rows, _columns);
            }

            public override double this[int row, int column]
            {
                get { return _content[_rowstride * row, _colstride * column]; }
                set { _content[_rowstride * row, _colstride * column] = value; }
            }
        }
    }
}
