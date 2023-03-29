// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDoubleMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   A condition or procedure : takes a single argument and returns a boolean value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Cern.Colt.List;
    using Cern.Jet.Math;
    using DoubleAlgorithms;

    using Function;
    using Implementation;
    using LinearAlgebra;

    /// <summary>
    /// A condition or procedure : takes a single argument and returns a boolean value.
    /// </summary>
    /// <param name="element">
    /// The element passed to the procedure.
    /// </param>
    /// <returns>
    /// A flag to inform the object calling the procedure.
    /// </returns>
    public delegate bool IDoubleMatrix1DProcedure(IDoubleMatrix1D element);

    /// <summary>
    /// A binary function of two 1-d matrices returning a single value.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// The dinstance.
    /// </returns>
    public delegate double IDoubleMatrix1DDinstance(IDoubleMatrix1D x, IDoubleMatrix1D y);

    /// <summary>
    /// Abstract base class for 1-d matrices (aka <i>vectors</i>) holding <tt>double</tt> elements.
    /// </summary>
    public abstract class DoubleMatrix1D : AbstractMatrix1D<Double>, IDoubleMatrix1D
    {
        /// <summary>
        /// Implicit conversione from array.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// A new matrix.
        /// </returns>
        public static implicit operator DoubleMatrix1D(double[] values)
        {
            return new DenseDoubleMatrix1D(values);
        }

        /// <summary>
        /// Applies a function to each cell and aggregates the results.
        /// </summary>
        /// <param name="aggr">
        /// An aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.
        /// </param>
        /// <param name="f">
        /// A function transforming the current cell value.
        /// </param>
        /// <returns>
        /// The aggregated measure.
        /// </returns>
        public virtual double Aggregate(IDoubleDoubleFunction aggr, IDoubleFunction f)
        {
            if (Size == 0) return double.NaN;
            double a = f.Apply(this[Size - 1]);
            for (int i = Size - 1; --i >= 0;)
                a = aggr.Apply(a, f.Apply(this[i]));
            return a;
        }

        /// <summary>
        /// Applies a function to each corresponding cell of two matrices and aggregates the results.
        /// </summary>
        /// <param name="other">
        /// The other matrix.
        /// </param>
        /// <param name="aggr">
        /// An aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
        /// </param>
        /// <param name="f">
        /// A  function transforming the current cell values.
        /// </param>
        /// <returns>
        /// The aggregated measure.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != other.size()</tt>.
        /// </exception>
        public double Aggregate(IMatrix1D<double> other, IDoubleDoubleFunction aggr, IDoubleDoubleFunction f)
        {
            CheckSize(other);
            if (Size == 0) return double.NaN;
            double a = f.Apply(this[Size - 1], other[Size - 1]);
            for (int i = Size - 1; --i >= 0;)
                a = aggr.Apply(a, f.Apply(this[i], other[i]));
            return a;
        }


        /// <summary>
        /// Sets all cells to the state specified by <tt>values</tt>.
        /// <tt>values</tt> is required to have the same number of cells as the receiver.
        /// <para>
        /// The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        /// </para>
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
        public virtual IDoubleMatrix1D Assign(double[] values)
        {
            if (values.Length != Size) throw new ArgumentOutOfRangeException(String.Format(Cern.LocalizedResources.Instance().Matrix_MustHaveSameNumberOfCell, values.Length, Size));
            for (int i = Size; --i >= 0;)
                this[i] = values[i];
            return this;
        }

        /// <summary>
        /// Sets all cells to the state specified by <tt>value</tt>.
        /// </summary>
        /// <param name="value">
        /// The value to be filled into the cells.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public virtual IDoubleMatrix1D Assign(double value)
        {
            for (int i = Size; --i >= 0;)
                this[i] = value;
            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[i] = function(x[i])</tt>.
        /// (Iterates downwards from <tt>[size()-1]</tt> to <tt>[0]</tt>).
        /// </summary>
        /// <param name="function">
        /// A  function taking as argument the current cell's value.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public virtual IDoubleMatrix1D Assign(IDoubleFunction function)
        {
            for (int i = Size; --i >= 0;)
                this[i] = function.Apply(this[i]);
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same size.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
        /// </summary>
        /// <param name="other">
        /// The source matrix to copy from (may be identical to the receiver).
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != other.size()</tt>.
        /// </exception>
        public virtual IDoubleMatrix1D Assign(IDoubleMatrix1D other)
        {
            if (other == this) return this;
            CheckSize(other);
            if (HaveSharedCells(other)) other = other.Copy();

            for (int i = Size; --i >= 0;)
                this[i] = other[i];
            return this;
        }


        public virtual IDoubleMatrix1D Assign(Cern.Jet.Math.Mult mult)
        {
            for (int i = Size; --i >= 0;)
                this[i] = mult.Apply(this[i]);
            return this;
        }


        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[i] = function(x[i],y[i])</tt>.
        /// </summary>
        /// <param name="y">
        /// The secondary matrix to operate on.
        /// </param>
        /// <param name="function">
        /// A function taking as first argument the current cell's value of <tt>this</tt>,
        /// and as second argument the current cell's value of <tt>y</tt>.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>size() != y.size()</tt>.
        /// </exception>
        public virtual IDoubleMatrix1D Assign(IDoubleMatrix1D y, IDoubleDoubleFunction function)
        {
            CheckSize(y);
            for (int i = Size; --i >= 0;)
                this[i] = function.Apply(this[i], y[i]);
            return this;
        }

        //public IDoubleMatrix1D Assign(IDoubleMatrix1D y, Cern.Jet.Math.PlusMult function)
        //{
        //    return Assign(y, function);
        //}

        public IDoubleMatrix1D Assign(IDoubleMatrix1D y, Cern.Jet.Math.PlusMult function, IntArrayList nonZeroIndexes)
        {
            CheckSize(y);
            int[] nonZeroElements = nonZeroIndexes.ToArray();

            double multiplicator = function.Multiplicator;
            if (multiplicator == 0)
            { // x[i] = x[i] + 0*y[i]
                return this;
            }
            else if (multiplicator == 1)
            { // x[i] = x[i] + y[i]
                for (int index = nonZeroIndexes.Count; --index >= 0;)
                {
                    int i = nonZeroElements[index];
                    this[i] = this[i] + y[i];
                }
            }
            else if (multiplicator == -1)
            { // x[i] = x[i] - y[i]
                for (int index = nonZeroIndexes.Count; --index >= 0;)
                {
                    int i = nonZeroElements[index];
                    this[i] = this[i] - y[i];
                }
            }
            else
            { // the general case x[i] = x[i] + mult*y[i]
                for (int index = nonZeroIndexes.Count; --index >= 0;)
                {
                    int i = nonZeroElements[index];
                    this[i] = this[i] + multiplicator * y[i];
                }
            }

            return this;
        }

        public IDoubleMatrix1D Assign(IDoubleMatrix1D y, IDoubleDoubleFunction function, IntArrayList nonZeroIndexes)
        {
            CheckSize(y);
            int[] nonZeroElements = nonZeroIndexes.ToArray();

            // specialized for speed
            if (function is Cern.Jet.Math.Mult)
            {  // x[i] = x[i] * y[i]
                int j = 0;
                for (int index = nonZeroIndexes.Count; --index >= 0;)
                {
                    int i = nonZeroElements[index];
                    for (; j < i; j++) this[j] = 0; // x[i] = 0 for all zeros
                    this[i] = this[i] * y[i];  // x[i] * y[i] for all nonZeros
                    j++;
                }
            }
            //else if (function is Cern.Jet.Math.PlusMult.Apply)
            //{

            //}
            else
            { // the general case x[i] = f(x[i],y[i])
                return Assign(y, function);
            }
            return this;
        }


        /// <summary>
        /// Returns the number of cells having non-zero values; ignores tolerance.
        /// </summary>
        /// <returns>
        /// The number of cells having non-zero values.
        /// </returns>
        public virtual int Cardinality()
        {
            int cardinality = 0;
            for (int i = Size; --i >= 0;)
                if (this[i] != 0) cardinality++;
            return cardinality;
        }

        /// <summary>
        /// Constructs and returns a deep copy of the receiver.
        /// </summary>
        /// <returns>
        /// A deep copy of the receiver.
        /// </returns>
        public IDoubleMatrix1D Copy()
        {
            IDoubleMatrix1D copy = Like();
            copy.Assign(this);
            return copy;
        }

        /// <summary>
        /// Returns whether all cells are equal to the given value.
        /// </summary>
        /// <param name="value">
        /// The value to test against.
        /// </param>
        /// <returns>
        /// <tt>true</tt> if all cells are equal to the given value, <tt>false</tt> otherwise.
        /// </returns>
        public bool Equals(double value)
        {
            return Property.DEFAULT.Equals(this, value);
        }

        /// <summary>
        /// Compares this object against the specified object.
        /// The result is <code>true</code> if and only if the argument is 
        /// not <code>null</code> and is at least a <code>IDoubleMatrix1D</code> object
        /// that has the same sizes as the receiver and 
        /// has exactly the same values at the same indexes.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with.
        /// </param>
        /// <returns>
        /// <code>true</code> if the objects are the same;
        /// <code>false</code> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (!(obj is IDoubleMatrix1D)) return false;

            return Property.DEFAULT.Equals(this, (IDoubleMatrix1D)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Enumerate vector elements.
        /// </summary>
        /// <returns>
        /// Each vector elements
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <tt>index</tt>.
        /// </summary>
        /// <param name="index">
        /// The index of the cell.
        /// </param>
        /// <returns>
        /// The value of the specified cell.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>index&lt;0 || index&gt;=size()</tt>.
        /// </exception>
        [Obsolete("GetQuick(int index) is deprecated, please use indexer instead.")]
        public virtual double GetQuick(int index)
        {
            if (index < 0 || index >= Size) CheckIndex(index);
            return this[index];
        }

        /// <summary>
        /// Fills the coordinates and values of cells having non-zero values into the specified lists.
        /// Fills into the lists, starting at index 0.
        /// After this call returns the specified lists all have a new size, the number of non-zero values.
        /// </summary>
        /// <param name="indexList">
        /// The list to be filled with indexes, can have any size.
        /// </param>
        /// <param name="valueList">
        /// The list to be filled with values, can have any size.
        /// </param>
        public virtual void GetNonZeros(IntArrayList indexList, List<double> valueList)
        {
            bool fillIndexList = indexList != null;
            bool fillValueList = valueList != null;
            if (fillIndexList) indexList.Clear();
            if (fillValueList) valueList.Clear();
            int s = Size;
            for (int i = 0; i < s; i++)
            {
                double value = this[i];
                if (value != 0)
                {
                    if (fillIndexList) indexList.Add(i);
                    if (fillValueList) valueList.Add(value);
                }
            }
        }

        /// <summary>
        /// Fills the coordinates and values of cells having non-zero values into the specified lists.
        /// Fills into the lists, starting at index 0.
        /// After this call returns the specified lists all have a new size, the number of non-zero values.
        /// </summary>
        /// <param name="indexList">
        /// The list to be filled with indexes, can have any size.
        /// </param>
        /// <param name="valueList">
        /// The list to be filled with values, can have any size.
        /// </param>
        /// <param name="maxCardinality">
        /// The max cardinality.
        /// </param>
        public void GetNonZeros(IntArrayList indexList, List<double> valueList, int maxCardinality)
        {
            bool fillIndexList = indexList != null;
            bool fillValueList = valueList != null;
            int card = Cardinality(maxCardinality);
            if (fillIndexList) indexList.Size = card;
            if (fillValueList) valueList.Capacity = card;
            if (!(card < maxCardinality)) return;

            if (fillIndexList) indexList.Clear();
            if (fillValueList) valueList.Clear();
            int s = Size;
            for (int i = 0; i < s; i++)
            {
                double value = this[i];
                if (value != 0)
                {
                    if (fillIndexList) indexList.Add(i);
                    if (fillValueList) valueList.Add(value);
                }
            }
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same size.
        /// </summary>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public IDoubleMatrix1D Like()
        {
            return Like(Size);
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// </summary>
        /// <param name="size">
        /// The number of cell the matrix shall have.
        /// </param>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public abstract IDoubleMatrix1D Like(int size);

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
        public abstract IDoubleMatrix2D Like2D(int rows, int columns);

        /// <summary>
        /// Sets the matrix cell at coordinate <tt>index</tt> to the specified value.
        /// </summary>
        /// <param name="index">
        /// The index of the cell.
        /// </param>
        /// <param name="value">
        /// The value to be filled into the specified cell.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>index&lt;0 || index&gt;=size()</tt>.
        /// </exception>
        [Obsolete("SetQuick(int index, double value) is deprecated, please use indexer instead.")]
        public virtual void SetQuick(int index, double value)
        {
            if (index < 0 || index >= Size) CheckIndex(index);
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
        public virtual void Swap(IDoubleMatrix1D other)
        {
            CheckSize(other);
            for (int i = Size; --i >= 0;)
            {
                double tmp = this[i];
                this[i] = other[i];
                other[i] = tmp;
            }
        }

        /// <summary>
        /// Constructs and returns a 1-dimensional array containing the cell values.
        /// The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <returns>
        /// An array filled with the values of the cells.
        /// </returns>
        public virtual double[] ToArray()
        {
            var values = new double[Size];
            ToArray(ref values);
            return values;
        }

        /// <summary>
        /// Fills the cell values into the specified 1-dimensional array.
        /// The values are copied. So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>values.length &lt; size()</tt>.
        /// </exception>
        public virtual void ToArray(ref double[] values)
        {
            if (values.Length < Size) throw new ArgumentOutOfRangeException("values", Cern.LocalizedResources.Instance().Exception_ValuesTooSmall);
            for (int i = Size; --i >= 0;)
            {
                values[i] = this[i];
            }
        }

        /// <summary>
        /// Enumerates vector elements.
        /// </summary>
        /// <returns>
        /// Vector elements, in index order.
        /// </returns>
        public virtual IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < Size; i++) yield return this[i];
        }

        /// <summary>
        /// Returns a string representation using default formatting.
        /// </summary>
        /// <returns>
        /// A string representation using default formatting.
        /// </returns>
        public override string ToString()
        {
            return new Formatter().ToString(this);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i>.
        /// What used to be index <tt>0</tt> is now index <tt>size()-1</tt>, ..., what used to be index <tt>size()-1</tt> is now index <tt>0</tt>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>
        /// A new flip view.
        /// </returns>
        public virtual IDoubleMatrix1D ViewFlip()
        {
            return (IDoubleMatrix1D)View().VFlip();
        }

        /// <summary>
        /// Constructs and returns a new <i>sub-range view</i> that is a <tt>width</tt> sub matrix starting at <tt>index</tt>.
        /// </summary>
        /// <param name="index">
        /// The index of the first cell.
        /// </param>
        /// <param name="width">
        /// The width of the range.
        /// </param>
        /// <returns>
        /// A new sub-range view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>index &lt; 0 || width &lt; 0 || index+width &gt; size()</tt>.
        /// </exception>
        public virtual IDoubleMatrix1D ViewPart(int index, int width)
        {
            return (IDoubleMatrix1D)View().VPart(index, width);
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        /// There holds <tt>view.size() == indexes.length</tt> and <tt>view.get(i) == this.get(indexes[i])</tt>.
        /// Indexes can occur multiple times and can be in arbitrary order.
        /// </summary>
        /// <param name="indexes">
        /// The indexes of the cells that shall be visible in the new view. To indicate that <i>all</i> cells shall be visible, simply set this parameter to <tt>null</tt>.
        /// </param>
        /// <returns>
        /// The new view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>!(0 &lt;= indexes[i] &lt; size())</tt> for any <tt>i=0..indexes.length()-1</tt>.
        /// </exception>
        public virtual IDoubleMatrix1D ViewSelection(int[] indexes)
        {
            // check for "all"
            if (indexes == null)
            {
                indexes = new int[Size];
                for (int i = Size; --i >= 0;) indexes[i] = i;
            }

            CheckIndexes(indexes);
            var offsets = new int[indexes.Length];
            for (int i = indexes.Length; --i >= 0;)
                offsets[i] = base.Index(indexes[i]); //Index(indexes[i]);
            return ViewSelectionLike(offsets);
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding the cells matching the given condition.
        /// Applies the condition to each cell and takes only those cells where <tt>condition.apply(get(i))</tt> yields <tt>true</tt>.
        /// </summary>
        /// <param name="condition">
        /// The condition to be matched.
        /// </param>
        /// <returns>
        /// The new view.
        /// </returns>
        public virtual IDoubleMatrix1D ViewSelection(IDoubleProcedure condition)
        {
            var matches = new IntArrayList();
            for (int i = 0; i < Size; i++)
                if (condition.Apply(this[i])) matches.Add(i);
            return ViewSelection(matches.ToArray());
        }

        /// <summary>
        /// Sorts the vector into ascending order, according to the <i>natural ordering</i>.
        /// This sort is guaranteed to be <i>stable</i>.
        /// </summary>
        /// <returns>
        /// A new sorted vector (matrix) view.
        /// </returns>
        public virtual IDoubleMatrix1D ViewSorted()
        {
            return Sorting.MergeSort.Sort(this);
        }

        /// <summary>
        /// Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
        /// More specifically, the view has size <tt>this.size()/stride</tt> holding cells <tt>this.get(i*stride)</tt> for all <tt>i = 0..size()/stride - 1</tt>.
        /// </summary>
        /// <param name="s">
        /// The step factor.
        /// </param>
        /// <returns>
        /// The new stride view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>stride &lt;= 0</tt>.
        /// </exception>
        public virtual IDoubleMatrix1D ViewStrides(int s)
        {
            return (IDoubleMatrix1D)View().VStrides(s);
        }

        /// <summary>
        /// Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>,
        /// where <tt>x == this</tt>.
        /// Operates on cells at indexes <tt>0 .. Math.min(size(),y.size())</tt>.
        /// </summary>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The sum of products.
        /// </returns>
        public virtual double ZDotProduct(IDoubleMatrix1D y)
        {
            return ZDotProduct(y, 0, Size);
        }

        /// <summary>
        /// Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>, 
        /// where <tt>x == this</tt>.
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
        public virtual double ZDotProduct(IDoubleMatrix1D y, int from, int length)
        {
            if (from < 0 || length <= 0) return 0;

            int tail = from + length;
            if (Size < tail) tail = Size;
            if (y.Size < tail) tail = y.Size;
            length = tail - from;

            double sum = 0;
            int i = tail - 1;
            for (int k = length; --k >= 0; i--)
                sum += this[i] * y[i];
            return sum;
        }

        /// <summary>
        /// Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>, 
        /// where <tt>x == this</tt>.
        /// </summary>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="nonZeroIndexes">
        /// The indexes of cells in <tt>y</tt>having a non-zero value.
        /// </param>
        /// <returns>
        /// The sum of products.
        /// </returns>
        public virtual double ZDotProduct(IDoubleMatrix1D y, int from, int length, IntArrayList nonZeroIndexes)
        {
            // determine minimum length
            if (from < 0 || length <= 0) return 0;

            int tail = from + length;
            if (Size < tail) tail = Size;
            if (y.Size < tail) tail = y.Size;
            length = tail - from;
            if (length <= 0) return 0;

            // setup
            int[] nonZeroIndexElements = nonZeroIndexes.ToArray();
            int index = 0;
            int s = nonZeroIndexes.Count;

            // skip to start
            while ((index < s) && nonZeroIndexElements[index] < from) index++;

            // now the sparse dot product
            int i;
            double sum = 0;
            while ((--length >= 0) && (index < s) && ((i = nonZeroIndexElements[index]) < tail))
            {
                sum += this[i] * y[i];
                index++;
            }

            return sum;
        }

        /// <summary>
        /// Returns the sum of all cells; <tt>Sum( x[i] )</tt>.
        /// </summary>
        /// <returns>
        /// The sum.
        /// </returns>
        public virtual double ZSum()
        {
            if (Size == 0) return 0;
            return Aggregate(BinaryFunctions.Plus, UnaryFunctions.Identity);
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
        protected virtual int Cardinality(int maxCardinality)
        {
            int cardinality = 0;
            int i = Size;
            while (--i >= 0 && cardinality < maxCardinality)
                if (this[i] != 0) cardinality++;
            return cardinality;
        }

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        /// <returns>
        /// The content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
        /// </returns>
        public IDoubleMatrix1D GetContent()
        {
            return this;
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
        public virtual bool HaveSharedCells(IDoubleMatrix1D other)
        {
            if (other == null) return false;
            if (this == other) return true;
            return GetContent().HaveSharedCellsRaw(other.GetContent());
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
        public virtual bool HaveSharedCellsRaw(IDoubleMatrix1D other)
        {
            return false;
        }

        /// <summary>
        /// Constructs and returns a new view equal to the receiver.
        /// The view is a shallow clone. Calls <code>clone()</code> and casts the result.
        /// </summary>
        /// <returns>
        /// A new view of the receiver.
        /// </returns>
        public IDoubleMatrix1D View()
        {
            return (IDoubleMatrix1D)Clone();
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
        public abstract IDoubleMatrix1D ViewSelectionLike(int[] offsets);

        /// <summary>
        /// Returns the dot product of two vectors x and y, which is <tt>Sum(x[i]*y[i])</tt>,
        /// where <tt>x == this</tt>.
        /// </summary>
        /// <param name="y">
        /// The second vector.
        /// </param>
        /// <param name="nonZeroIndexes">
        /// The indexes of cells in <tt>y</tt>having a non-zero value.
        /// </param>
        /// <returns>
        /// The sum of products.
        /// </returns>
        public double ZDotProduct(IDoubleMatrix1D y, IntArrayList nonZeroIndexes)
        {
            return ZDotProduct(y, 0, Size, nonZeroIndexes);
        }
    }
}
