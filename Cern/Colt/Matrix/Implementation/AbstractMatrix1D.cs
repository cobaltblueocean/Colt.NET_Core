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

namespace Cern.Colt.Matrix.Implementation
{
    using System;

    /// <summary>
    /// Abstract base class for 1-d matrices (aka <i>vectors</i>) holding objects or primitive data types such as <code>int</code>, <code>double</code>, etc.
    /// </summary>
    public abstract class AbstractMatrix1D<T> : AbstractMatrix, IMatrix1D<T>
    {
        /// <summary>
        /// Gets or sets the number of indexes between any two elements, i.e. <tt>index(i+1) - index(i)</tt>.
        /// </summary>
        private int _stride;

        /// <summary>
        /// Gets or sets the index of the first element.
        /// </summary>
        private int _zero;

        /// <summary>
        /// Sanity check for operations requiring two matrices with the same size.
        /// </summary>
        /// <param name="b">
        /// The second matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != B.size()</tt>.
        /// </exception>
        public void CheckSize(IMatrix1D<T> b)
        {
            if (Size != b.Size) throw new ArgumentOutOfRangeException("Incompatible sizes: " + this + " and " + b);
        }

        /// <summary>
        /// Gets or sets the number of cells this matrix (view) has.
        /// </summary>
        /// <returns>
        /// The number of cells.
        /// </returns>
        public override int Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        /// <summary>
        /// the number of indexes between any two elements, i.e. <tt>index(i+1) - index(i)</tt>.
        /// </summary>
        public int Stride
        {
            get { return _stride; }
            set { _stride = value; }
        }

        public int Zero
        {
            get { return _zero; }
            set { _zero = value; }
        }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>index</tt>.
        /// </summary>
        /// <param name="index">
        /// The index of the cell.
        /// </param>
        public abstract T this[int index] { get; set; }

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
        public virtual int Index(int rank)
        {
            return GetOffset(GetRank(rank));
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
        protected void CheckIndex(int index)
        {
            if (index < 0 || index >= Size) throw new IndexOutOfRangeException("Attempted to access " + this + " at index=" + index);
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
        protected void CheckIndexes(int[] indexes)
        {
            for (int i = indexes.Length; --i >= 0;)
            {
                int index = indexes[i];
                if (index < 0 || index >= Size) CheckIndex(index);
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
        protected void CheckRange(int index, int width)
        {
            if (index < 0 || index + width > Size)
                throw new ArgumentException("index: " + index + ", width: " + width + ", size=" + Size);
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
        protected void CheckSize(double[] b)
        {
            if (Size != b.Length) throw new ArgumentOutOfRangeException("Incompatible sizes: " + this + " and " + b.Length);
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
        protected virtual int GetOffset(int absRank)
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
        protected virtual int GetRank(int rank)
        {
            return Zero + (rank * Stride);
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
        protected virtual void Setup(int s)
        {
            Setup(s, 0, 1);
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
        protected virtual void Setup(int sz, int z, int str)
        {
            if (sz < 0) throw new ArgumentOutOfRangeException("sz", "negative size");

            this.Size = sz;
            this.Zero = z;
            this.Stride = str;
            IsView = false;
        }

        /// <summary>
        /// Self modifying version of viewFlip().
        /// What used to be index <tt>0</tt> is now index <tt>size()-1</tt>, ..., what used to be index <tt>size()-1</tt> is now index <tt>0</tt>.
        /// </summary>
        /// <returns>
        /// A new flip view.
        /// </returns>
        public IMatrix1D<T> VFlip()
        {
            if (Size > 0)
            {
                this.Zero += (this.Size - 1) * this.Stride;
                this.Stride = -this.Stride;
                IsView = true;
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
        public IMatrix1D<T> VPart(int index, int width)
        {
            CheckRange(index, width);
            this.Zero += this.Stride * index;
            this.Size = width;
            IsView = true;
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
        public IMatrix1D<T> VStrides(int str)
        {
            if (Stride <= 0) throw new ArgumentOutOfRangeException("str", "illegal stride: " + Stride);
            this.Stride *= Stride;
            if (this.Size != 0) this.Size = ((this.Size - 1) / Stride) + 1;
            IsView = true;
            return this;
        }

        /// <summary>
        /// Return String type converted value of coordinated index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract String ToString(int index);

        /// <summary>
        /// Returns a string representation of the receiver's shape.
        /// </summary>
        /// <returns></returns>
        public String ToStringShort()
        {
            return AbstractFormatter.Shape(this);
        }
    }
}
