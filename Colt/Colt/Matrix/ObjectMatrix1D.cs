// <copyright file="ObjectMatrix1D.cs" company="CERN">
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
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
    /// <summary>
    /// Delegate that represents a condition or procedure object: takes
    /// a single argument and returns a boolean value.
    /// 
    /// Optionally can return a boolean flag to inform the object calling the procedure.
    /// </summary>
    /// <param name="element">element passed to the procedure.</param>
    /// <returns>a flag to inform the object calling the procedure.</returns>
    public delegate Boolean ObjectMatrix1DProcedure(ObjectMatrix1D element);

    public abstract class ObjectMatrix1D : AbstractMatrix1D
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor
        protected ObjectMatrix1D() { }
        #endregion

        #region Abstract Methods

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>index&lt;0 || index&gt;=Size</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        /// <exception cref=""></exception>
        public abstract Object this[int index] { get; set; }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified size.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="size">the number of cell the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public abstract ObjectMatrix1D Like(int size);

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must be of type <i>SparseObjectMatrix2D</i>, etc.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public abstract ObjectMatrix2D Like2D(int rows, int columns);

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="offsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected abstract ObjectMatrix1D ViewSelectionLike(int[] offsets);
        #endregion

        #region Local Public Methods
        /// <summary>
        /// Applies a function to each cell and aggregates the results.
        ///         Returns a value<tt> v</tt> such that <tt>v==a(size())</tt> where<tt> a(i) == aggr(a(i-1), f(get(i)) )</tt> and terminators are<tt> a(1) == f(get(0)), a(0)==null</tt>.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// cern.jet.math.Functions F = cern.jet.math.Functions.functions;
        ///         matrix = 0 1 2 3 
        ///
        /// Sum( x[i]*x[i] ) 
        /// matrix.aggregate(F.plus, F.square);
        /// --> 14
        /// </pre>
        /// For further examples, see the <see cref="Function.ObjectFunction{C}"/> doc</a>.
        /// </summary>
        /// <param name="aggr">an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.</param>
        /// <param name="f">a function transforming the current cell value.</param>
        /// <returns>the aggregated measure.</returns>
        public Object Aggregate(Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectFunction<Object> f)
        {
            if (Size == 0) return null;
            Object a = f(this[Size - 1]);
            for (int i = Size - 1; --i >= 0;)
            {
                a = aggr(a, f(this[i]));
            }
            return a;
        }

        /// <summary>
        /// Applies a function to each corresponding cell of two matrices and aggregates the results.
        /// Returns a value <i>v</i> such that<i> v==a(Size)</i> where<i> a(i) == aggr(a(i-1), f(get(i), other.Get(i)) )</i> and terminators are<i> a(1) == f(get(0),other.Get(0)), a(0)==null</i>.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// Cern.jet.math.Functions F = Cern.jet.math.Functions.Functions;
        /// x = 0 1 2 3 
        /// y = 0 1 2 3 
        ///
        /// // Sum( x[i]*y[i] )
        /// x.aggregate(y, F.plus, F.mult);
        /// --> 14
        ///
        /// // Sum( (x[i]+y[i])^2 )
        /// x.aggregate(y, F.plus, F.chain(F.square, F.plus));
        /// --> 56
        /// </pre>
        /// For further examples, see the <see cref="Function.ObjectFunction{C}"/> doc</a>.
        /// </summary>
        /// <param name="aggr">an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.</param>
        /// <param name="f">a function transforming the current cell value.</param>
        /// <returns>the aggregated measure.</returns>
        /// <exception cref="ArgumentException">if <i>Size != other.Count</i>.</exception>
        public Object Aggregate(ObjectMatrix1D other, Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectObjectFunction<Object> f)
        {
            CheckSize(other);
            if (Size == 0) return null;
            Object a = f(this[Size - 1], other[Size - 1]);
            for (int i = Size - 1; --i >= 0;)
            {
                a = aggr(a, f(this[i], other[i]));
            }
            return a;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>values</i>.
        /// <i>values</i> is required to have the same number of cells as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != Size</i>.</exception>
        public ObjectMatrix1D Assign(Object[] values)
        {
            if (values.Length != Size) throw new ArgumentException("Must have same number of cells: Length=" + values.Length + ", Size=" + Size);
            for (int i = Size; --i >= 0;)
            {
                this[i] = values[i];
            }
            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <i>x[i] = function(x[i])</i>.
        /// (Iterates downwards from<i>[Size-1]</i> to<i>[0]</i>).
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// // change each cell to its sine
        /// matrix =   0.5      1.5      2.5       3.5 
        /// matrix.assign(Cern.jet.math.Functions.sin);
        /// -->
        /// matrix ==  0.479426 0.997495 0.598472 -0.350783
        /// </pre>
        /// For further examples, see the <see cref="Function.ObjectFunction{C}"/> doc</a>.
        /// </summary>
        /// <param name="function">a function object taking as argument the current cell's value.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <see cref="Cern.Jet.Math.Functions"/>
        /// <exception cref=""></exception>
        public ObjectMatrix1D Assign(Cern.Colt.Function.ObjectFunction<Object> function)
        {
            for (int i = Size; --i >= 0;)
            {
                this[i] = function(this[i]);
            }
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same size.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        /// </summary>
        /// <param name="other">the source matrix to copy from (may be identical to the receiver).</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>Size != other.Count</i>.</exception>
        public ObjectMatrix1D Assign(ObjectMatrix1D other)
        {
            if (other == this) return this;
            CheckSize(other);
            if (HaveSharedCells(other)) other = other.Copy();

            for (int i = Size; --i >= 0;)
            {
                this[i] = other[i];
            }
            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <i>x[i] = function(x[i], y[i])</i>.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// // assign x[i] = x[i]<sup>y[i]</sup>
        /// m1 = 0 1 2 3;
        /// m2 = 0 2 4 6;
        /// m1.assign(m2, Cern.jet.math.Functions.pow);
        /// -->
        /// m1 == 1 1 16 729
        /// </pre>
        /// For further examples, see the <see cref="Function.ObjectFunction{C}"/> doc</a>.
        /// </summary>
        /// <param name="y">the secondary matrix to operate on.</param>
        /// <param name="function">a function object taking as first argument the current cell's value of <i>this</i>,and as second argument the current cell's value of <i>y</i>,</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>Size != y.Count</i>.</exception>
        /// <see cref="Cern.Jet.Math.Functions"/>
        public ObjectMatrix1D Assign(ObjectMatrix1D y, Cern.Colt.Function.ObjectObjectFunction<Object> function)
        {
            CheckSize(y);
            for (int i = Size; --i >= 0;)
            {
               this[i]  =function(this[i], y[i]);
            }
            return this;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="value">the value to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        public ObjectMatrix1D Assign(Object value)
        {
            for (int i = Size; --i >= 0;)
            {
                this[i] = value;
            }
            return this;
        }

        /// <summary>
        /// Returns the number of cells having non-zero values; ignores tolerance.
        /// </summary>
        public int Cardinality()
        {
            int cardinality = 0;
            for (int i = Size; --i >= 0;)
            {
                if (this[i] != null) cardinality++;
            }
            return cardinality;
        }

        /// <summary>
        /// Constructs and returns a deep copy of the receiver.
        /// <p>
        /// <b>Note that the returned matrix is an independent deep copy.</b>
        /// The returned matrix is not backed by this matrix, so changes in the returned matrix are not reflected in this matrix, and vice-versad 
        /// </summary>
        /// <returns>a deep copy of the receiver.</returns>
        public ObjectMatrix1D Copy()
        {
            ObjectMatrix1D copy = Like();
            copy.Assign(this);
            return copy;
        }

        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Equivalent to<i> Equals(otherObj,true)</i>d  
        /// </summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public override Boolean Equals(Object otherObj)
        { //delta
            return Equals(otherObj, true);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Returns true if and only if the specified Object is also at least an ObjectMatrix1D, both matrices have the
        /// same size, and all corresponding pairs of cells in the two matrices are the same.
        /// In other words, two matrices are defined to be equal if they contain the
        /// same cell values in the same order.
        /// Tests elements for equality or identity as specified by <i>testForEquality</i>.
        /// When testing for equality, two elements <i>e1</i> and
        /// <i>e2</i> are <i>equal</i> if <i>(e1==null ? e2==null :
        /// e1.Equals(e2))</i>d)  
        /// </summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <param name="testForEquality">if true -> tests for equality, otherwise for identity.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public Boolean Equals(Object otherObj, Boolean testForEquality)
        { //delta
            if (!(otherObj is ObjectMatrix1D)) { return false; }
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            ObjectMatrix1D other = (ObjectMatrix1D)otherObj;
            if (Size != other.Size) return false;

            if (!testForEquality)
            {
                for (int i = Size; --i >= 0;)
                {
                    if (this[i] != other[i]) return false;
                }
            }
            else
            {
                for (int i = Size; --i >= 0;)
                {
                    if (!(this[i] == null ? other[i] == null : this[i].Equals(other[i]))) return false;
                }
            }

            return true;

        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>index</i>.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <returns>the value of the specified cell.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>index&lt;0 || index&gt;=Size</i>.</exception>
        [Obsolete("Get(int index) is deprecated, please use indexer instead.")]
        public Object Get(int index)
        {
            if (index < 0 || index >= Size) CheckIndex(index);
            return this[index];
        }

        /// <summary>
        /// Fills the coordinates and values of cells having non-zero values into the specified lists.
        /// Fills into the lists, starting at index 0.
        /// After this call returns the specified lists all have a new size, the number of non-zero values.
        /// <p>
        /// In general, fill order is <i>unspecified</i>.
        /// This implementation fills like: <i>for (index = 0..Count - 1)  do ..d </i>.
        /// However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
        /// (Of course, result lists indexes are guaranteed to correspond to the same cell).
        /// <p>
        /// <b>Example:</b>
        /// <br>
        /// <pre>
        /// 0, 0, 8, 0, 7
        /// -->
        /// indexList  = (2,4)
        /// valueList  = (8,7)
        /// </pre>
        /// In other words, <i>get(2)==8, get(4)==7</i>.
        /// </summary>
        /// <param name="indexList">the list to be filled with indexes, can have any size.</param>
        /// <param name="valueList">the list to be filled with values, can have any size.</param>
        public void GetNonZeros(ref List<int> indexList, ref List<Object> valueList)
        {
            Boolean fillIndexList = indexList != null;
            Boolean fillValueList = valueList != null;
            if (fillIndexList) indexList.Clear();
            if (fillValueList) valueList.Clear();
            int s = Size;
            for (int i = 0; i < s; i++)
            {
                Object value = this[i];
                if (value != null)
                {
                    if (fillIndexList) indexList.Add(i);
                    if (fillValueList) valueList.Add(value);
                }
            }
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same size.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix1D</i> the new matrix must also be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix1D</i> the new matrix must also be of type <i>SparseObjectMatrix1D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public ObjectMatrix1D Like()
        {
            return Like(Size);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>index</i> to the specified value.
        /// </summary>
        /// <param name="index">the index of the cell.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || index &gt;= Size</i>.</exception>
        [Obsolete("Set(int index, Object value) is deprecated, please use indexer instead.")]
        public void Set(int index, Object value)
        {
            if (index < 0 || index >= Size) CheckIndex(index);
            this[index] = value;
        }

        /// <summary>
        /// Swaps each element<i> this[i]</i> with <i>other [i]</i>.
        /// </summary>
        /// <exception cref="ArgumentException">if <i>Size != other.Count</i>.</exception>
        public void Swap(ObjectMatrix1D other)
        {
            CheckSize(other);
            for (int i = Size; --i >= 0;)
            {
                Object tmp = this[i];
                this[i] = other[i];
                other[i] = tmp;
            }
            return;
        }

        /// <summary>
        /// Constructs and returns a 1-dimensional array containing the cell values.
        /// The values are copied.So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// The returned array<i> values</i> has the form
        /// <br>
        /// <i>for (int i= 0; i<Size; i++) values[i] = get(i);</i>
        /// </summary>
        /// <returns>an array filled with the values of the cells.</returns>
        public Object[] ToArray()
        {
            Object[] values = new Object[Size];
            ToArray(ref values);
            return values;
        }

        /// <summary>
        /// Fills the cell values into the specified 1-dimensional array.
        /// The values are copied.So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// After this call returns the array <i>values</i> has the form 
        /// <br>
        /// <i>for (int i= 0; i &lt; Size; i++) values[i] = get(i);</i>
        /// </summary>
        /// <exception cref="ArgumentException">if <i>values.Length<Size</i>.</exception>
        public void ToArray(ref Object[] values)
        {
            if (values.Length < Size) throw new ArgumentException("values too small");
            for (int i = Size; --i >= 0;)
            {
                values[i] = this[i];
            }
        }

        /// <summary>
        /// Returns a string representation using default formatting.
        /// </summary>
        /// <see cref="Cern.Colt.Matrix.ObjectAlgorithms.Formatter"/>
        public override String ToString()
        {
            return new Cern.Colt.Matrix.ObjectAlgorithms.Formatter().ToString(this);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i>.
        /// What used to be index<i>0</i> is now index <i>Size-1</i>, ..d, what used to be index<i> Size-1</i> is now index <i>0</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>a new flip view.</returns>
        public ObjectMatrix1D ViewFlip()
        {
            return (ObjectMatrix1D)(View().VFlip());
        }

        /// <summary>
        /// Constructs and returns a new <i>sub-range view</i> that is a<i> width</i> sub matrix starting at <i>index</i>.
        /// 
        /// Operations on the returned view can only be applied to the restricted range.
        /// Any attempt to access coordinates not contained in the view will throw an<i> IndexOutOfRangeException</i>.
        /// <p>
        /// <b>Note that the view is really just a range restriction:</b> 
        /// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versa.
        /// <p>
        /// The view contains the cells from <i>index..index+width-1</i>.
        /// and has <i>view.Count == width</i>.
        /// A view's legal coordinates are again zero based, as usual.
        /// In other words, legal coordinates of the view are <i>0 .d view.Count-1==width-1</i>.
        /// As usual, any attempt to access a cell at other coordinates will throw an<i> IndexOutOfRangeException</i>.
        /// </summary>
        /// <param name="index">The index of the first cell.</param>
        /// <param name="width">The width of the range.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>index &lt; 0 || width &lt; 0 || index + width > Size</i>.</exception>
        public ObjectMatrix1D ViewPart(int index, int width)
        {
            return (ObjectMatrix1D)(View().VPart(index, width));
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        /// There holds <i>view.Count == indexes.Length</i> and <i>view.Get(i) == this.Get(indexes[i])</i>.
        /// Indexes can occur multiple times and can be in arbitrary order.
        /// <p>
        /// <b>Example:</b>
        /// <br>
        /// <pre>
        /// this     = (0, 0, 8, 0, 7)
        /// indexes  = (0, 2, 4, 2)
        /// -- >
        /// view = (0, 8, 7, 8)
        /// </ pre >
        /// Note that modifying <i>indexes</i> after this call has returned has no effect on the view.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <param name="indexes">The indexes of the cells that shall be visible in the new view.To indicate that<i> all</i> cells shall be visible, simply set this parameter to <i>null</i>.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>!(0 &lt;= indexes[i] < Size)</i> for any<i> i = 0..indexes.Length() - 1 </ tt >.</exception>
        public ObjectMatrix1D ViewSelection(int[] indexes)
        {
            // check for "all"
            if (indexes == null)
            {
                indexes = new int[Size];
                for (int i = Size; --i >= 0;) indexes[i] = i;
            }

            CheckIndexes(indexes);
            int[] offsets = new int[indexes.Length];
            for (int i = indexes.Length; --i >= 0;)
            {
                offsets[i] = Index(indexes[i]);
            }
            return ViewSelectionLike(offsets);
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding the cells matching the given condition.
        /// Applies the condition to each cell and takes only those cells where <i>condition.apply(get(i))</i> yields<i>true</i>.
        /// <p>
        /// <b>Example:</b>
        /// <br>
        /// <pre>
        /// // extract and view all cells with even value
        /// matrix = 0 1 2 3 
        /// matrix.viewSelection(
        /// &nbsp;&nbsp;&nbsp;new ObjectProcedure()
        /// {
        /// &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;public Boolean apply(Object a) { return a % 2 == 0; }
        /// &nbsp;&nbsp;&nbsp;}
        /// );
        /// -->
        /// matrix ==  0 2
        /// </pre>
        /// For further examples, see the<a href="package-summary.html#FunctionObjects">package doc</a>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <param name="condition">The condition to be matched.</param>
        /// <returns>the new view.</returns>
        public ObjectMatrix1D ViewSelection(Cern.Colt.Function.ObjectProcedure<Object> condition)
        {
            List<int> matches = new List<int>();
            for (int i = 0; i < Size; i++)
            {
                if (condition(this[i])) matches.Add(i);
            }
            matches.TrimExcess();
            return ViewSelection(matches.ToArray());
        }

        /// <summary>
        /// Sorts the vector into ascending order, according to the<i>natural ordering</i>.
        /// This sort is guaranteed to be<i> stable</i>.
        /// For further information, see <see cref="Cern.Colt.Matrix.ObjectAlgorithms.Sorting.sort(ObjectMatrix1D)"/>.
        /// For more advanced sorting functionality, see <see cref="Cern.Colt.Matrix.ObjectAlgorithms.Sorting"/>.
        /// </summary>
        /// <returns>a new sorted vector(matrix) view.</returns>
        public ObjectMatrix1D ViewSorted()
        {
            return Cern.Colt.Matrix.ObjectAlgorithms.Sorting.mergeSort.sort(this);
        }

        /// <summary>
        /// Constructs and returns a new < i > stride view </ i > which is a sub matrix consisting of every i - th cell.
        /// More specifically, the view has size < tt > this.Count / stride </ tt > holding cells < tt > this.Get(i * stride) </ tt > for all<i> i = 0..Count / stride - 1 </ tt >.
        /// </summary>
        /// <param name="stride">the step factor.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if < tt > stride <= 0 </ tt >.</exception>
        public ObjectMatrix1D ViewStrides(int stride)
        {
            return (ObjectMatrix1D)(View().VStrides(stride));
        }
        #endregion

        #region Local Protected Methods
        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        protected ObjectMatrix1D GetContent()
        {
            return this;
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean HaveSharedCells(ObjectMatrix1D other)
        {
            if (other == null) return false;
            if (this == other) return true;
            return GetContent().HaveSharedCellsRaw(other.GetContent());
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean HaveSharedCellsRaw(ObjectMatrix1D other)
        {
            return false;
        }

        /// <summary>
        /// Constructs and returns a new view equal to the receiver.
        /// The view is a shallow cloned Calls <code>clone()</code> and casts the result.
        /// <p>
        /// <b>Note that the view is not a deep copy.</b>
        /// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versad 
        /// <p>
        /// Use <see cref="Copy()"/> to construct an independent deep copy rather than a new view.
        /// </summary>
        /// <returns>a new view of the receiver.</returns>
        protected ObjectMatrix1D View()
        {
            return (ObjectMatrix1D)Clone();
        }
        #endregion

        #region Local Private Methods

        /// <summary>
        /// Applies a procedure to each cell's value.
        /// Iterates downwards from <i>[Size-1]</i> to <i>[0]</i>,
        /// as demonstrated by this snippet:
        /// <pre>
        /// for (int i=Size; --i >=0;) {
        /// if (!procedure.apply(this[i])) return false;
        /// }
        /// return true;
        /// </pre>
        /// Note that an implementation may use more efficient techniques, but must not use any other order.
        /// </summary>
        /// <param name="procedure">a procedure object taking as argument the current cell's valued Stops iteration if the procedure returns <i>false</i>, otherwise continuesd</param>
        /// <returns><i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised</returns>
        private Boolean XforEach(Cern.Colt.Function.ObjectProcedure<Object> procedure)
        {
            for (int i = Size; --i >= 0;)
            {
                if (!procedure(this[i])) return false;
            }
            return true;
        }

        #endregion

    }
}
