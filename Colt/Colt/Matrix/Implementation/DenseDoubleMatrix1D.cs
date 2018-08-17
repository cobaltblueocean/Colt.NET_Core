// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DenseDoubleMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Dense 1-d matrix (aka <i>vector</i>) holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;

    using Function;

    /// <summary>
    /// Dense 1-d matrix (aka <i>vector</i>) holding <tt>double</tt> elements.
    /// </summary>
    public class DenseDoubleMatrix1D : DoubleMatrix1D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDoubleMatrix1D"/> class with a copy of the given values.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the new matrix.
        /// </param>
        public DenseDoubleMatrix1D(double[] values)
        {
            Setup(values.Length);
            Elements = new double[values.Length];
            Assign(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDoubleMatrix1D"/> class with a given number of cells.
        /// All entries are initially <tt>0</tt>.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size &lt; 0</tt>.
        /// </exception>
        public DenseDoubleMatrix1D(int size)
        {
            Setup(size);
            Elements = new double[size];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDoubleMatrix1D"/> class.
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="zero">
        /// The index of the first element.
        /// </param>
        /// <param name="stride">
        /// The number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size &lt; 0</tt>.
        /// </exception>
        public DenseDoubleMatrix1D(int size, double[] elements, int zero, int stride)
        {
            Setup(size, zero, stride);
            this.Elements = elements;
            IsView = true;
        }

        /// <summary>
        /// Gets the elements of this matrix.
        /// </summary>
        internal double[] Elements { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>index</tt>.
        /// </summary>
        /// <param name="index">
        /// The index of the cell.
        /// </param>
        public override double this[int index]
        {
            get
            {
                return Elements[Index(index)];
            }

            set
            {
                Elements[Index(index)] = value;
            }
        }

        /// <summary>
        /// Sets all cells to the state specified by <tt>values</tt>.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the cells.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>values.length != size()</tt>.
        /// </exception>
        public override DoubleMatrix1D Assign(double[] values)
        {
            if (IsView)
            {
                base.Assign(values);
            }
            else
            {
                if (values.Length != Size)
                    throw new ArgumentOutOfRangeException("Must have same number of cells: length = " + values.Length + ", Size = " + Size);
                Array.Copy(values, 0, Elements, 0, values.Length);
            }

            return this;
        }

        /// <summary>
        /// ets all cells to the state specified by <tt>value</tt>.
        /// </summary>
        /// <param name="value">
        /// The value to be filled into the cells.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public override DoubleMatrix1D Assign(double value)
        {
            int index = this.Index(0);
            int s = Stride;
            double[] elems = Elements;
            for (int i = Size; --i >= 0;)
            {
                elems[index] = value;
                index += s;
            }

            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[i] = function(x[i])</tt>.
        /// (Iterates downwards from <tt>[size()-1]</tt> to <tt>[0]</tt>).
        /// </summary>
        /// <param name="function">
        /// A function taking as argument the current cell's value.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public override DoubleMatrix1D Assign(DoubleFunction function)
        {
            int s = Stride;
            int i = Index(0);
            double[] elems = Elements;
            if (elems == null) throw new ApplicationException();

            for (int k = Size; --k >= 0;)
            {
                elems[i] = function(elems[i]);
                i += s;
            }

            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// </summary>
        /// <param name="source">
        /// The source matrix to copy from (may be identical to the receiver).
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>size() != other.size()</tt>.
        /// </exception>
        public override DoubleMatrix1D Assign(DoubleMatrix1D source)
        {
            // overriden for performance only
            if (!(source is DenseDoubleMatrix1D))
                return base.Assign(source);

            var other = (DenseDoubleMatrix1D)source;
            if (other == this) return this;
            CheckSize(other);
            if (!IsView && !other.IsView)
            {
                // quickest
                Array.Copy(other.Elements, 0, Elements, 0, Elements.Length);
                return this;
            }

            if (HaveSharedCells(other))
            {
                DoubleMatrix1D c = other.Copy();
                if (!(c is DenseDoubleMatrix1D))
                {
                    // should not happen
                    return Assign(source);
                }

                other = (DenseDoubleMatrix1D)c;
            }

            double[] elems = Elements;
            double[] otherElems = other.Elements;
            if (Elements == null || otherElems == null) throw new ArgumentException();
            int s = Stride;
            int ys = other.Stride;

            int index = this.Index(0);
            int otherIndex = other.Index(0);
            for (int k = Size; --k >= 0;)
            {
                elems[index] = otherElems[otherIndex];
                index += s;
                otherIndex += ys;
            }

            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[i] = function(x[i],y[i])</tt>.
        /// (Iterates downwards from <tt>[size()-1]</tt> to <tt>[0]</tt>).
        /// </summary>
        /// <param name="y">
        /// The secondary matrix to operate on.
        /// </param>
        /// <param name="function">
        /// The function taking as first argument the current cell's value of <tt>this</tt>,
        /// and as second argument the current cell's value of <tt>y</tt>.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != y.size()</tt>.
        /// </exception>
        public override DoubleMatrix1D Assign(DoubleMatrix1D y, DoubleDoubleFunction function)
        {
            // overriden for performance only
            if (!(y is DenseDoubleMatrix1D))
                return base.Assign(y, function);

            var other = (DenseDoubleMatrix1D)y;
            CheckSize(y);
            double[] elems = Elements;
            double[] otherElems = other.Elements;
            if (elems == null || otherElems == null) throw new ApplicationException();
            int s = Stride;
            int ys = other.Stride;

            int index = this.Index(0);
            int otherIndex = other.Index(0);

            // specialized for speed
            for (int k = Size; --k >= 0;)
            {
                elems[index] = function(elems[index], otherElems[otherIndex]);
                index += s;
                otherIndex += ys;
            }

            return this;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// </summary>
        /// <param name="n">
        /// The number of cell the matrix shall have.
        /// </param>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public override DoubleMatrix1D Like(int n)
        {
            return new DenseDoubleMatrix1D(n);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        public override DoubleMatrix2D Like2D(int rows, int columns)
        {
            return new DenseDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        ///  Returns the matrix cell value at coordinate <tt>index</tt>.
        ///  
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <tt>index&lt;0 || index&gt;=size()</tt>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <param name="value">the value of the specified cell.</param>
        [Obsolete("SetQuick(int index, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int index, double value)
        {
            this[index] = value;
        }

        /// <summary>
        /// Swaps each element <tt>this[i]</tt> with <tt>other[i]</tt>.
        /// </summary>
        /// <param name="other">
        /// The other matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != other.size()</tt>.
        /// </exception>
        public override void Swap(DoubleMatrix1D other)
        {
            // overriden for performance only
            if (!(other is DenseDoubleMatrix1D))
                base.Swap(other);

            var y = (DenseDoubleMatrix1D)other;
            if (y == this) return;
            CheckSize(y);

            double[] elems = Elements;
            double[] otherElems = y.Elements;
            if (Elements == null || otherElems == null) throw new ApplicationException();
            int s = Stride;
            int ys = y.Stride;

            int index = this.Index(0);
            int otherIndex = y.Index(0);
            for (int k = Size; --k >= 0;)
            {
                double tmp = elems[index];
                elems[index] = otherElems[otherIndex];
                otherElems[otherIndex] = tmp;
                index += s;
                otherIndex += ys;
            }

            return;
        }

        /// <summary>
        /// Fills the cell values into the specified 1-dimensional array.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>values.length &lt; size()</tt>.
        /// </exception>
        public override void ToArray(ref double[] values)
        {
            if (values.Length < Size) throw new ArgumentOutOfRangeException("values", "values too small");
            if (!IsView) Array.Copy(Elements, 0, values, 0, Elements.Length);
            else base.ToArray(ref values);
        }

        /// <summary>
        /// Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>.
        /// Where <tt>x == this</tt>.
        /// Operates on cells at indexes <tt>from .. Min(size(),y.size(),from+length)-1</tt>. 
        /// </summary>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <param name="from">
        /// The first index to be considered.
        /// </param>
        /// <param name="length">
        /// The number of cells to be considered.
        /// </param>
        /// <returns>
        /// The sum of products; zero if <tt>from &lt; 0 || length &lt; 0</tt>.
        /// </returns>
        public override double ZDotProduct(DoubleMatrix1D y, int from, int length)
        {
            if (!(y is DenseDoubleMatrix1D))
                return base.ZDotProduct(y, from, length);

            var yy = (DenseDoubleMatrix1D)y;

            int tail = from + length;
            if (from < 0 || length < 0) return 0;
            if (Size < tail) tail = Size;
            if (y.Size < tail) tail = y.Size;
            int min = tail - from;

            int i = Index(from);
            int j = yy.Index(from);
            int s = Stride;
            int ys = yy.Stride;
            double[] elems = Elements;
            double[] yElems = yy.Elements;
            if (elems == null || yElems == null) throw new ApplicationException();

            double sum = 0;

            // optimized
            // loop unrolling
            i -= s;
            j -= ys;
            for (int k = min / 4; --k >= 0;)
            {
                sum += (elems[i += s] * yElems[j += ys]) +
                    (elems[i += s] * yElems[j += ys]) +
                    (elems[i += s] * yElems[j += ys]) +
                    (elems[i += s] * yElems[j += ys]);
            }

            for (int k = min % 4; --k >= 0;)
            {
                sum += elems[i += s] * yElems[j += ys];
            }

            return sum;
        }

        /// <summary>
        /// Returns the sum of all cells; <tt>Sum( x[i] )</tt>.
        /// </summary>
        /// <returns>
        /// The sum.
        /// </returns>
        public override double ZSum()
        {
            double sum = 0;
            int s = Stride;
            int i = Index(0);
            double[] elems = Elements;
            if (elems == null) throw new ApplicationException();
            for (int k = Size; --k >= 0;)
            {
                sum += elems[i];
                i += s;
            }

            return sum;
        }

        /// <summary>
        /// Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// You may want to override this method for performance.
        /// </summary>
        /// <param name="rank">
        /// The rank.
        /// </param>
        /// <returns>
        /// The position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// </returns>
        protected internal override int Index(int rank)
        {
            // overriden for manual inlining only
            return Zero + (rank * Stride);
        }

        /// <summary>
        /// Returns the number of cells having non-zero values, but at most maxCardinality; ignores tolerance.
        /// </summary>
        /// <param name="maxCardinality">
        /// The max cardinality.
        /// </param>
        /// <returns>
        /// The number of cells having non-zero values.
        /// </returns>
        protected override int Cardinality(int maxCardinality)
        {
            int cardinality = 0;
            int index = this.Index(0);
            int s = Stride;
            double[] elems = Elements;
            int i = Size;
            while (--i >= 0 && cardinality < maxCardinality)
            {
                if (elems[index] != 0) cardinality++;
                index += s;
            }

            return cardinality;
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <tt>index</tt>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <tt>index&lt;0 || index&gt;=size()</tt>.
        /// </summary>
        /// <param name="index">
        /// the index of the cell.
        /// </param>
        /// <returns>
        /// the value of the specified cell.
        /// </returns>
        [Obsolete("GetQuick(int index) is deprecated, please use indexer instead.")]
        public override Double GetQuick(int index)
        {
            //if (debug) if (index<0 || index>=size) checkIndex(index);
            //return elements[index(index)];
            // manually inlined:
            return this[index];
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
        protected override bool HaveSharedCellsRaw(DoubleMatrix1D other)
        {
            if (other is SelectedDenseDoubleMatrix1D)
            {
                var otherMatrix = (SelectedDenseDoubleMatrix1D)other;
                return Elements == otherMatrix.Elements;
            }

            if (other is DenseDoubleMatrix1D)
            {
                var otherMatrix = (DenseDoubleMatrix1D)other;
                return Elements == otherMatrix.Elements;
            }

            return false;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">
        /// The offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        protected override DoubleMatrix1D ViewSelectionLike(int[] offsets)
        {
            return new SelectedDenseDoubleMatrix1D(Elements, offsets);
        }

        public override string ToString(int index)
        {
            return this[index].ToString();
        }
    }
}
