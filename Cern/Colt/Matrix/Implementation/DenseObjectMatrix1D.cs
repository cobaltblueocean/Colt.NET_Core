// <copyright file="DenseObjectMatrix1D.cs" company="CERN">
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
    /// Dense 1-d matrix (aka <i>vector</i>) holding <i>Object</i> elements.
    ///
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Internally holds one single contigous one-dimensional arrayd 
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory [bytes] = 8*size()</i>.
    /// Thus, a 1000000 matrix uses 8 MB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// <i>O(1)</i> (i.ed constant time) for the basic operations
    /// <i>get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>,
    /// <p>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class DenseObjectMatrix1D : ObjectMatrix1D
    {

        /// <summary>
        /// Gets the elements of this matrix.
        /// </summary>
        protected internal Object[] Elements { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell at coordinate <i>index</i> to the specified value.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        public override object this[int index]
        {
            get
            {
                //if (debug) if (index<0 || index>=size) checkIndex(index);
                //return elements[index(index)];
                // manually inlined:
                // return Elements[Zero + index * Stride];
                return Elements[Index(index)];
            }
            set
            {
                //if (debug) if (index<0 || index>=size) checkIndex(index);
                //elements[index(index)] = value;
                // manually inlined:
                // Elements[Zero + index * Stride] = value;
                Elements[Index(index)] = value;

            }
        }

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public DenseObjectMatrix1D(Object[] values) : this(values.Length)
        {

            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of cells.
        /// All entries are initially <i>0</i>.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if size :lt; 0.</exception>
        public DenseObjectMatrix1D(int size)
        {
            Setup(size);
            this.Elements = new Object[size];
        }

        /// <summary>
        /// Constructs a matrix view with the given parameters.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="zero">the index of the first element.</param>
        /// <param name="stride">the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.</param>
        /// <exception cref="ArgumentException">if size :lt; 0.</exception>
        public DenseObjectMatrix1D(int size, Object[] elements, int zero, int stride)
        {
            Setup(size, zero, stride);
            this.Elements = elements;
            this.IsView = true;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>values</i>.
        /// <i>values</i> is required to have the same number of cells as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != size()</i>.</exception>
        public override ObjectMatrix1D Assign(Object[] values)
        {
            if (!IsView)
            {
                if (values.Length != Size) throw new ArgumentException("Must have same number of cells: Length = " + values.Length + "Size = " + Size);
                Array.Copy(values, 0, this.Elements, 0, values.Length);
            }
            else
            {
                base.Assign(values);
            }
            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <i>x[i] = function(x[i])</i>.
        /// (Iterates downwards from <i>[size()-1]</i> to <i>[0]</i>).
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// // change each cell to its sine
        /// matrix =   0.5      1.5      2.5       3.5 
        /// matrix.assign(Cern.Jet.Math.Functions.sin);
        /// -->
        /// matrix ==  0.479426 0.997495 0.598472 -0.350783
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// </summary>
        /// <param name="function">a function object taking as argument the current cell's value.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <see cref="Cern.Jet.Math.Functions"></see>
        public override ObjectMatrix1D Assign(Cern.Colt.Function.ObjectFunctionDelegate<Object> function)
        {
            int s = Stride;
            int i = Index(0);
            Object[] elems = this.Elements;
            if (Elements == null) throw new NullReferenceException();

            // the general case x[i] = f(x[i])
            for (int k = Size; --k >= 0;)
            {
                elems[i] = function(elems[i]);
                i += s;
            }
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same size.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        /// </summary>
        /// <param name="source">the source matrix to copy from (may be identical to the receiver).</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>size() != other.Count</i>.</exception>
        public override ObjectMatrix1D Assign(ObjectMatrix1D source)
        {
            // overriden for performance only
            if (!(source is DenseObjectMatrix1D))
            {
                return base.Assign(source);
            }
            DenseObjectMatrix1D other = (DenseObjectMatrix1D)source;
            if (other == this) return this;
            CheckSize(other);
            if (!IsView && !other.IsView)
            { // quickest
                Array.Copy(other.Elements, 0, this.Elements, 0, this.Elements.Length);
                return this;
            }
            if (HaveSharedCells(other))
            {
                ObjectMatrix1D c = other.Copy();
                if (!(c is DenseObjectMatrix1D))
                { // should not happen
                    return base.Assign(source);
                }
                other = (DenseObjectMatrix1D)c;
            }

            Object[] elems = this.Elements;
            Object[] otherElems = other.Elements;
            if (Elements == null || otherElems == null) throw new NullReferenceException();
            int s = this.Stride;
            int ys = other.Stride;

            int index = base.Index(0);
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
        /// Assigns the result of a function to each cell; <i>x[i] = function(x[i],y[i])</i>.
        /// (Iterates downwards from <i>[size()-1]</i> to <i>[0]</i>).
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// // assign x[i] = x[i]<sup>y[i]</sup>
        /// m1 = 0 1 2 3;
        /// m2 = 0 2 4 6;
        /// m1.assign(m2, Cern.Jet.Math.Functions.pow);
        /// -->
        /// m1 == 1 1 16 729
        ///
        /// // for non-standard functions there is no shortcut: 
        /// m1.assign(m2,
        /// &nbsp;&nbsp;&nbsp;new ObjectObjectFunction() {
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public Object apply(Object x, Object y) { return System.Math.Pow(x,y); }
        /// &nbsp;&nbsp;&nbsp;}
        /// );
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// </summary>
        /// <param name="y">the secondary matrix to operate on.</param>
        /// <param name="function">a function object taking as first argument the current cell's value of <i>this</i>, and as second argument the current cell's value of <i>y</i>,</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>size() != y.Count</i>.</exception>
        /// <see cref="Cern.Jet.Math.Functions"/>
        public override ObjectMatrix1D Assign(ObjectMatrix1D y, Cern.Colt.Function.ObjectObjectFunctionDelegate<Object> function)
        {
            // overriden for performance only
            if (!(y is DenseObjectMatrix1D))
            {
                return base.Assign(y, function);
            }
            DenseObjectMatrix1D other = (DenseObjectMatrix1D)y;
            CheckSize(y);
            Object[] elems = this.Elements;
            Object[] otherElems = other.Elements;
            if (Elements == null || otherElems == null) throw new NullReferenceException();
            int s = this.Stride;
            int ys = other.Stride;

            int index = base.Index(0);
            int otherIndex = other.Index(0);

            // the general case x[i] = f(x[i],y[i])		
            for (int k = Size; --k >= 0;)
            {
                elems[index] = function(elems[index], otherElems[otherIndex]);
                index += s;
                otherIndex += ys;
            }
            return this;
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
        public override Object GetQuick(int index)
        {
            return this[index];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix1D other)
        {
            if (other is SelectedDenseObjectMatrix1D)
            {
                SelectedDenseObjectMatrix1D otherMatrix = (SelectedDenseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is DenseObjectMatrix1D)
            {
                DenseObjectMatrix1D otherMatrix = (DenseObjectMatrix1D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the element with the given relative rank within the (virtual or non-virtual) internal 1-dimensional array.
        /// You may want to override this method for performance.
        /// </summary>
        /// <param name="rank">the rank of the element.</param>
        public override int Index(int rank)
        {
            // overriden for manual inlining only
            //return _offset(_rank(rank));
            return Zero + rank * Stride;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="size">the number of cell the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override ObjectMatrix1D Like(int size)
        {
            return new DenseObjectMatrix1D(size);
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must be of type <i>SparseObjectMatrix2D</i>, etc.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override ObjectMatrix2D Like2D(int rows, int columns)
        {
            return new DenseObjectMatrix2D(rows, columns);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>index</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=size()</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int index, Object value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int index, Object value)
        {
            this[index] = value;
        }

        /// <summary>
        /// Swaps each element <i>this[i]</i> with <i>other[i]</i>.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>size() != other.Count</i>.</exception>
        public override void Swap(ObjectMatrix1D other)
        {
            // overriden for performance only
            if (!(other is DenseObjectMatrix1D))
            {
                base.Swap(other);
            }
            DenseObjectMatrix1D y = (DenseObjectMatrix1D)other;
            if (y == this) return;
            CheckSize(y);

            Object[] elems = this.Elements;
            Object[] otherElems = y.Elements;
            if (Elements == null || otherElems == null) throw new NullReferenceException();
            int s = this.Stride;
            int ys = y.Stride;

            int index = base.Index(0);
            int otherIndex = y.Index(0);
            for (int k = Size; --k >= 0;)
            {
                Object tmp = elems[index];
                elems[index] = otherElems[otherIndex];
                otherElems[otherIndex] = tmp;
                index += s;
                otherIndex += ys;
            }
            return;
        }

        /// <summary>
        /// Fills the cell values into the specified 1-dimensional array.
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// After this call returns the array <i>values</i> has the form 
        /// <br>
        /// <i>for (int i=0; i &lt; size(); i++) values[i] = get(i);</i>
        /// </summary>
        /// <exception cref="ArgumentException">if <i>values.Length &lt; size()</i>.</exception>
        public override void ToArray(ref Object[] values)
        {
            if (values.Length < Size) throw new ArgumentException("values too small");
            if (!this.IsView) Array.Copy(this.Elements, 0, values, 0, this.Elements.Length);
            else base.ToArray(ref values);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override ObjectMatrix1D ViewSelectionLike(int[] offsets)
        {
            return new SelectedDenseObjectMatrix1D(this.Elements, offsets);
        }

        public override string ToString(int index)
        {
            return this[index].ToString();
        }
    }
}
