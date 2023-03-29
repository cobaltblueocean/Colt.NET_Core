// <copyright file="DelegateIDoubleMatrix1D.cs" company="CERN">
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
    /// 1-d matrix holding <i>double</i> elements; either a view wrapping another 2-d matrix and therefore delegating calls to it.
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class DelegateIDoubleMatrix1D: WrapperIDoubleMatrix1D
    {
        /// <summary>
        /// The elements of the matrix.
        /// </summary>
        private IDoubleMatrix2D content2D;

        /// <summary>
        /// The row this view is bound to.
        /// </summary>
        private int row;

        /// <summary>
        /// The row this view is bound to.
        /// </summary>
        public int Row
        {
            get { return row; }
            protected set { row = value; }
        }

        /// <summary>
        /// The elements of the matrix.
        /// </summary>
        public new IDoubleMatrix2D Content
        {
            get { return content2D; }
            protected set { content2D = value; }
        }

        public DelegateIDoubleMatrix1D(IDoubleMatrix2D newContent, int row):base(null)
        {
            if (row < 0 || row >= newContent.Rows) throw new ArgumentException();
            Setup(newContent.Columns);
            this.row = row;
            this.content2D = newContent;
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
            get { return content2D[row, index]; }
            set { content2D[row, index] = value; }
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        [Obsolete("GetQuick(int index) is deprecated, please use indexer instead.")]
        public override double GetQuick(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// For example, if the receiver is an instance of type <see cref="DenseDoubleMatrix1D"/> the new matrix must also be of type <see cref="DenseDoubleMatrix1D"/>,
        /// if the receiver is an instance of type <see cref="SparseDoubleMatrix1D"/> the new matrix must also be of type <see cref="SparseDoubleMatrix1D"/>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="size">the number of cell the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override IDoubleMatrix1D Like(int size)
        {
            return content2D.Like1D(size);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <see cref="DenseDoubleMatrix1D"/> the new matrix must also be of type <see cref="DenseDoubleMatrix2D"/>,
        /// if the receiver is an instance of type <see cref="SparseDoubleMatrix1D"/> the new matrix must also be of type <see cref="SparseDoubleMatrix2D"/>, etc.
        /// 
        /// 
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override IDoubleMatrix2D Like2D(int rows, int columns)
        {
            return content2D.Like(rows, columns);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>index</i> to the specified value.
        /// 
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value to be filled into the specified cell.</returns>
        [Obsolete("SetQuick(int index, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int index, double value)
        {
            this[index]= value;
        }
    }
}
