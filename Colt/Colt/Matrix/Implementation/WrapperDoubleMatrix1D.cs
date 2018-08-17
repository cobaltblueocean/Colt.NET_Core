// <copyright file="WrapperDoubleMatrix1D.cs" company="CERN">
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
    /// 1-d matrix holding <i>double</i> elements; either a view wrapping another matrix or a matrix whose views are wrappers.
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class WrapperDoubleMatrix1D : DoubleMatrix1D
    {
        /// <summary>
        /// The elements of the matrix.
        /// </summary>
        private DoubleMatrix1D _content;

        public WrapperDoubleMatrix1D(DoubleMatrix1D newContent)
        {
            if (newContent != null) Setup(newContent.Count());
            this.Content = newContent;
        }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <i>index</i>.
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        public override double this[int index]
        {
            get { return Content[index]; }
            set { Content[index] = value; }
        }

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        public DoubleMatrix1D Content
        {
            get { return _content; }
            protected set { _content = value; }
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        [Obsolete("GetQuick(int index) is deprecated, please use indexer instead.")]
        public override Double GetQuick(int index)
        {
            return Content[index];
        }

        public override DoubleMatrix1D Like(int Size)
        {
            return Content.Like(Size);
        }

        public override DoubleMatrix2D Like2D(int rows, int columns)
        {
            return Content.Like2D(rows, columns);
        }

        /// <summary>
        /// Sets the matrix cell value at coordinate <i>index</i>.
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        [Obsolete("SetQuick(int index, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int index, double value)
        {
            Content[index] = value;
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i>.
        /// What used to be index <i>0</i> is now index <i>Size()-1</i>, ..d, what used to be index <i>Size()-1</i> is now index <i>0</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>a new flip view.</returns>
        public new DoubleMatrix1D ViewFlip()
        {
            DoubleMatrix1D view = new WrapperDoubleMatrix1DFlip(this);
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
        /// <param name="index">The index of the first cell.</param>
        /// <param name="width">The width of the range.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || width &lt; 0 || index + width > Size()</i>.</exception>
        public new DoubleMatrix1D ViewPart(int index, int width)
        {
            CheckRange(index, width);
            DoubleMatrix1D view = new WrapperDoubleMatrix1DPart(this, index);

            view.Size = width;
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
        public new DoubleMatrix1D ViewSelection(int[] indexes)
        {
            // check for "all"
            if (indexes == null)
            {
                indexes = new int[Size];
                for (int i = Size; --i >= 0;) indexes[i] = i;
            }

            CheckIndexes(indexes);
            int[] idx = indexes;

            DoubleMatrix1D view = new WrapperDoubleMatrix1DSelection(this, idx);


            view.Size = indexes.Length;
            return view;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override DoubleMatrix1D ViewSelectionLike(int[] offsets)
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
        public new DoubleMatrix1D ViewStrides(int _stride)
        {
            if (Stride <= 0) throw new IndexOutOfRangeException("illegal stride: " + Stride);
            DoubleMatrix1D view = new WrapperDoubleMatrix1DStrides(this, _stride);

            view.Size = Size;
            if (Size != 0) view.Size = (Size - 1) / _stride + 1;
            return view;
        }

        public override string ToString(int index)
        {
            return this[index].ToString();
        }

        private class WrapperDoubleMatrix1DFlip : WrapperDoubleMatrix1D
        {
            public WrapperDoubleMatrix1DFlip(WrapperDoubleMatrix1D newcontent) : base(newcontent.Content)
            { }

            public override double this[int index]
            {
                get { return Content[Size - 1 - index]; }
                set { Content[Size - 1 - index] = value; }
            }
        }

        private class WrapperDoubleMatrix1DPart : WrapperDoubleMatrix1D
        {
            int indexOffset;

            public WrapperDoubleMatrix1DPart(WrapperDoubleMatrix1D newcontent, int offset) : base(newcontent.Content)
            {
                indexOffset = offset;
            }

            public override double this[int index]
            {
                get { return Content[indexOffset + index]; }
                set { Content[indexOffset + index] = value; }
            }
        }

        private class WrapperDoubleMatrix1DSelection : WrapperDoubleMatrix1D
        {
            int[] idx;

            public WrapperDoubleMatrix1DSelection(WrapperDoubleMatrix1D newcontent, int[] indexes) : base(newcontent.Content)
            {
                idx = indexes;
            }

            public override double this[int index]
            {
                get { return Content[idx[index]]; }
                set { Content[idx[index]] = value; }
            }
        }

        private class WrapperDoubleMatrix1DStrides : WrapperDoubleMatrix1D
        {
            int _stride;

            public WrapperDoubleMatrix1DStrides(WrapperDoubleMatrix1D newcontent, int stride) : base(newcontent.Content)
            {
                _stride = stride;
            }

            public override double this[int index]
            {
                get { return Content[index * _stride]; }
                set { Content[index * _stride] = value; }
            }
        }
    }

}
