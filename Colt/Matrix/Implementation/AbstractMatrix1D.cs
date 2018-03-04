// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for 1-d matrices (aka <i>vectors</i>) holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt.Matrix.Implementation
{
    using System;

    /// <summary>
    /// Abstract base class for 1-d matrices (aka <i>vectors</i>) holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
    /// </summary>
    public abstract class AbstractMatrix1D : AbstractMatrix
    {
        /// <summary>
        /// Gets or sets the number of cells this matrix (view) has.
        /// </summary>
        protected internal int size { get; set; }

        /// <summary>
        /// Gets or sets the number of indexes between any two elements, i.e. <tt>index(i+1) - index(i)</tt>.
        /// </summary>
        protected internal int stride { get; set; }

        /// <summary>
        /// Gets or sets the index of the first element.
        /// </summary>
        protected int zero { get; set; }

        /// <summary>
        /// Sanity check for operations requiring two matrices with the same size.
        /// </summary>
        /// <param name="b">
        /// The second matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != B.size()</tt>.
        /// </exception>
        public void CheckSize(AbstractMatrix1D b)
        {
            if (size != b.size) throw new ArgumentOutOfRangeException("Incompatible sizes: " + this + " and " + b);
        }

        /// <summary>
        /// Returns the number of cells.
        /// </summary>
        /// <returns>
        /// The number of cells.
        /// </returns>
        public override int Size()
        {
            return size;
        }

        /// <summary>
        /// Returns a string representation of the receiver's shape.
        /// </summary>
        /// <returns>
        /// A string representation of the receiver's shape.
        /// </returns>
        public override string ToString()
        {
            return AbstractFormatter.Shape(this);
        }

        /// <summary>
        /// Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// You may want to override this method for performance.
        /// </summary>
        /// <param name="rank">
        /// The rank of the element.
        /// </param>
        /// <returns>
        /// Returns the position of the element with the given relative rank.
        /// </returns>
        protected internal virtual int index(int rank)
        {
            return _offset(_rank(rank));
        }

        /// <summary>
        /// Sanity check for operations requiring an index to be within bounds.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>index &lt; 0 || index &gt;= size()</tt>.
        /// </exception>
        protected void checkIndex(int index)
        {
            if (index < 0 || index >= size) throw new IndexOutOfRangeException("Attempted to access " + this + " at index=" + index);
        }

        /// <summary>
        /// Checks whether indexes are legal and throws an exception, if necessary.
        /// </summary>
        /// <param name="indexes">
        /// The indexes.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>! (0 &lt;= indexes[i] &lt; size())</tt> for any i=0..indexes.length()-1.
        /// </exception>
        protected void checkIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= size) checkIndex(index);
            }
        }

        /// <summary>
        /// Checks whether the receiver contains the given range and throws an exception, if necessary.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <tt>index &lt; 0 || index+width &gt; size()</tt>.
        /// </exception>
        protected void checkRange(int index, int width)
        {
            if (index < 0 || index + width > size)
                throw new ArgumentException("index: " + index + ", width: " + width + ", size=" + size);
        }

        /// <summary>
        /// Sanity check for operations requiring two matrices with the same size.
        /// </summary>
        /// <param name="b">
        /// The second matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != B.size()</tt>.
        /// </exception>
        protected void checkSize(double[] b)
        {
            if (size != b.Length) throw new ArgumentOutOfRangeException("Incompatible sizes: " + this + " and " + b.Length);
        }

        /// <summary>
        /// Returns the position of the given absolute rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// Default implementation. Override, if necessary.
        /// </summary>
        /// <param name="absRank">
        /// The absolute rank of the element.
        /// </param>
        /// <returns>
        /// The position
        /// </returns>
        protected virtual int _offset(int absRank)
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
        protected virtual int _rank(int rank)
        {
            return zero + (rank * stride);
        }

        /// <summary>
        /// Sets up a matrix with a given number of cells.
        /// </summary>
        /// <param name="s">
        /// The the number of cells the matrix shall have.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size &lt; 0</tt>.
        /// </exception>
        protected virtual void setUp(int s)
        {
            setUp(s, 0, 1);
        }

        /// <summary>
        /// Sets up a matrix with the given parameters.
        /// </summary>
        /// <param name="sz">
        /// The number of elements the matrix shall have.
        /// </param>
        /// <param name="z">
        /// The index of the first element.
        /// </param>
        /// <param name="str">
        /// The number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size &lt; 0</tt>.
        /// </exception>
        protected virtual void setUp(int sz, int z, int str)
        {
            if (sz < 0) throw new ArgumentOutOfRangeException("sz", "negative size");

            this.size = sz;
            this.zero = z;
            this.stride = str;
            isView = false;
        }

        /// <summary>
        /// Self modifying version of viewFlip().
        /// What used to be index <tt>0</tt> is now index <tt>size()-1</tt>, ..., what used to be index <tt>size()-1</tt> is now index <tt>0</tt>.
        /// </summary>
        /// <returns>
        /// A new flip view.
        /// </returns>
        protected AbstractMatrix1D vFlip()
        {
            if (size > 0)
            {
                this.zero += (this.size - 1) * this.stride;
                this.stride = -this.stride;
                isView = true;
            }

            return this;
        }

        /// <summary>
        /// Self modifying version of viewStrides().
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// A new sub-range view.
        /// </returns>
        /// /// <exception cref="ArgumentException">
        /// If <tt>index &lt; 0 || index+width &gt; size()</tt>.
        /// </exception>
        protected AbstractMatrix1D vPart(int index, int width)
        {
            checkRange(index, width);
            this.zero += this.stride * index;
            this.size = width;
            isView = true;
            return this;
        }

        /// <summary>
        /// Self modifying version of viewStrides().
        /// </summary>
        /// <param name="str">
        /// The stride.
        /// </param>
        /// <returns>
        /// The new stride view.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>stride &lt;= 0</tt>.
        /// </exception>
        protected AbstractMatrix1D vStrides(int str)
        {
            if (stride <= 0) throw new ArgumentOutOfRangeException("str", "illegal stride: " + stride);
            this.stride *= stride;
            if (this.size != 0) this.size = ((this.size - 1) / stride) + 1;
            isView = true;
            return this;
        }
    }
}
