// <copyright file="ObjectMatrix3D.cs" company="CERN">
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
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
    /// <summary>
    /// Delegate that represents a condition or procedure object: takes
    /// a single argument and returns a Boolean value.
    /// 
    /// Optionally can return a Boolean flag to inform the object calling the procedure.
    /// </summary>
    /// <param name="element">element passed to the procedure.</param>
    /// <returns>a flag to inform the object calling the procedure.</returns>
    public delegate Boolean ObjectMatrix3DProcedure(ObjectMatrix3D element);

    public abstract class ObjectMatrix3D : AbstractMatrix3D
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Get or set the matrix cell value at coordinate <i>[slice,row,column]</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices || row&lt;0 || row&gt;=Rows || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="colum">the index of the column-coordinate.</param>
        /// <returns>the value of the specified coordinate.</returns>
        public abstract Object this[int slice, int row, int colum] { get; set; }

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods

        #endregion

        #region Local Private Methods

        #endregion

        /// <summary>
/// Applies a function to each cell and aggregates the results.
/// Returns a value <i>v</i> such that <i>v==a(Size)</i> where <i>a(i) == aggr( a(i-1), f(get(slice,row,column)) )</i> and terminators are <i>a(1) == f(get(0,0,0)), a(0)==null</i>.
/// <p>
/// <b>Example:</b>
/// <pre>
/// Cern.jet.math.Functions F = Cern.jet.math.Functions.Functions;
/// 2 x 2 x 2 matrix
/// 0 1
/// 2 3
///
/// 4 5
/// 6 7
///
/// // Sum( x[slice,row,col]*x[slice,row,col] ) 
/// matrix.aggregate(F.plus,F.square);
/// --> 140
/// </pre>
/// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
///
/// @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.
/// @param f a function transforming the current cell value.
/// @return the aggregated measure.
/// @see Cern.jet.math.Functions
/// </summary>
        public Object aggregate(Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectFunction<Object> f)
        {
            if (Size == 0) return null;
            Object a = f(this[Slices - 1, Rows - 1, Columns - 1]);
            int d = 1; // last cell already done
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns - d; --column >= 0;)
                    {
                        a = aggr(a, f(this[slice, row, column]));
                    }
                    d = 0;
                }
            }
            return a;
        }
        /// <summary>
/// Applies a function to each corresponding cell of two matrices and aggregates the results.
/// Returns a value <i>v</i> such that <i>v==a(Size)</i> where <i>a(i) == aggr( a(i-1), f(get(slice,row,column),other.Get(slice,row,column)) )</i> and terminators are <i>a(1) == f(get(0,0,0),other.Get(0,0,0)), a(0)==null</i>.
/// <p>
/// <b>Example:</b>
/// <pre>
/// Cern.jet.math.Functions F = Cern.jet.math.Functions.Functions;
/// x = 2 x 2 x 2 matrix
/// 0 1
/// 2 3
///
/// 4 5
/// 6 7
///
/// y = 2 x 2 x 2 matrix
/// 0 1
/// 2 3
///
/// 4 5
/// 6 7
///
/// // Sum( x[slice,row,col] * y[slice,row,col] ) 
/// x.aggregate(y, F.plus, F.mult);
/// --> 140
///
/// // Sum( (x[slice,row,col] + y[slice,row,col])^2 )
/// x.aggregate(y, F.plus, F.chain(F.square,F.plus));
/// --> 560
/// </pre>
/// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
///
/// @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
/// @param f a function transforming the current cell values.
/// @return the aggregated measure.
/// @thRows	ArgumentException if <i>Slices != other.Slices || Rows != other.Rows || Columns != other.Columns</i>
/// @see Cern.jet.math.Functions
/// </summary>
        public Object aggregate(ObjectMatrix3D other, Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectObjectFunction<Object> f)
        {
            CheckShape(other);
            if (Size == 0) return null;
            Object a = f(this[Slices - 1, Rows - 1, Columns - 1], other[Slices - 1, Rows - 1, Columns - 1]);
            int d = 1; // last cell already done
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns - d; --column >= 0;)
                    {
                        a = aggr(a, f(this[slice, row, column], other[slice, row, column]));
                    }
                    d = 0;
                }
            }
            return a;
        }
        /// <summary>
         /// Sets all cells to the state specified by <i>values</i>.
         /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
         /// and have exactly the same number of Slices, Rows and Columns as the receiver.
         /// <p>
         /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
         /// 
         /// @param    values the values to be filled into the cells.
         /// @return <i>this</i> (for convenience only).
         /// @thRows ArgumentException if <i>values.Length != Slices || for any 0 &lt;= slice &lt; Slices: values[slice].Length != Rows</i>.
         /// @thRows ArgumentException if <i>for any 0 &lt;= column &lt; Columns: values[slice][row].Length != Columns</i>.
         /// </summary>
        public ObjectMatrix3D assign(Object[][][] values)
        {
            if (values.Length != Slices) throw new ArgumentException("Must have same number of Slices: Slices=" + values.Length + "Slices=" + Slices);
            for (int slice = Slices; --slice >= 0;)
            {
                Object[][] currentSlice = values[slice];
                if (currentSlice.Length != Rows) throw new ArgumentException("Must have same number of Rows in every slice: Rows=" + currentSlice.Length + "Rows=" + Rows);
                for (int row = Rows; --row >= 0;)
                {
                    Object[] currentRow = currentSlice[row];
                    if (currentRow.Length != Columns) throw new ArgumentException("Must have same number of Columns in every row: Columns=" + currentRow.Length + "Columns=" + Columns);
                    for (int column = Columns; --column >= 0;)
                    {
                        this[slice, row, column] = currentRow[column];
                    }
                }
            }
            return this;
        }
        /// <summary>
/// Assigns the result of a function to each cell; <i>x[slice,row,col] = function(x[slice,row,col])</i>.
/// <p>
/// <b>Example:</b>
/// <pre>
/// matrix = 1 x 2 x 2 matrix
/// 0.5 1.5      
/// 2.5 3.5
///
/// // change each cell to its sine
/// matrix.assign(Cern.jet.math.Functions.sin);
/// -->
/// 1 x 2 x 2 matrix
/// 0.479426  0.997495 
/// 0.598472 -0.350783
/// </pre>
/// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
///
/// @param function a function object taking as argument the current cell's value.
/// @return <i>this</i> (for convenience only).
/// @see Cern.jet.math.Functions
/// </summary>
        public ObjectMatrix3D assign(Cern.Colt.Function.ObjectFunction<Object> function)
        {
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        this[slice, row, column] = function(this[slice, row, column]);
                    }
                }
            }
            return this;
        }
        /// <summary>
         /// Replaces all cell values of the receiver with the values of another matrix.
         /// Both matrices must have the same number of Slices, Rows and Columns.
         /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
         /// 
         /// @param     other   the source matrix to copy from (may be identical to the receiver).
         /// @return <i>this</i> (for convenience only).
         /// @thRows	ArgumentException if <i>Slices != other.Slices || Rows != other.Rows || Columns != other.Columns</i>
         /// </summary>
        public ObjectMatrix3D assign(ObjectMatrix3D other)
        {
            if (other == this) return this;
            CheckShape(other);
            if (haveSharedCells(other)) other = other.copy();

            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        this[slice, row, column] = other[slice, row, column];
                    }
                }
            }
            return this;
        }
        /// <summary>
/// Assigns the result of a function to each cell; <i>x[row,col] = function(x[row,col],y[row,col])</i>.
/// <p>
/// <b>Example:</b>
/// <pre>
/// // assign x[row,col] = x[row,col]<sup>y[row,col]</sup>
/// m1 = 1 x 2 x 2 matrix 
/// 0 1 
/// 2 3
///
/// m2 = 1 x 2 x 2 matrix 
/// 0 2 
/// 4 6
///
/// m1.assign(m2, Cern.jet.math.Functions.pow);
/// -->
/// m1 == 1 x 2 x 2 matrix
///         1   1 
/// 16 729
/// </pre>
/// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
///
/// @param y the secondary matrix to operate on.
/// @param function a function object taking as first argument the current cell's value of <i>this</i>,
/// and as second argument the current cell's value of <i>y</i>,
/// @return <i>this</i> (for convenience only).
/// @thRows	ArgumentException if <i>Slices != other.Slices || Rows != other.Rows || Columns != other.Columns</i>
/// @see Cern.jet.math.Functions
/// </summary>
        public ObjectMatrix3D assign(ObjectMatrix3D y, Cern.Colt.Function.ObjectObjectFunction<Object> function)
        {
            CheckShape(y);
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        this[slice, row, column] = function(this[slice, row, column], y[slice, row, column]);
                    }
                }
            }
            return this;
        }

        /// <summary>
         /// Sets all cells to the state specified by <i>value</i>.
         /// @param    value the value to be filled into the cells.
         /// @return <i>this</i> (for convenience only).
         /// </summary>
        public ObjectMatrix3D assign(Object value)
        {
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        this[slice, row, column] = value;
                    }
                }
            }
            return this;
        }

        /// <summary>
         /// Returns the number of cells having non-zero values; ignores tolerance.
         /// </summary>
        public int cardinality()
        {
            int cardinality = 0;
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        if (this[slice, row, column] != null) cardinality++;
                    }
                }
            }
            return cardinality;
        }

        /// <summary>
         /// Constructs and returns a deep copy of the receiver.
         /// <p>
         /// <b>Note that the returned matrix is an independent deep copy.</b>
         /// The returned matrix is not backed by this matrix, so changes in the returned matrix are not reflected in this matrix, and vice-versad 
         /// 
         /// @return  a deep copy of the receiver.
         /// </summary>
        public ObjectMatrix3D copy()
        {
            return like().assign(this);
        }

        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Equivalent to <i>Equals(otherObj,true)</i>d  
        /// 
        /// @param otherObj the Object to be compared for equality with the receiver.
        /// @return true if the specified Object is equal to the receiver.
/// </summary>
        public override Boolean Equals(Object otherObj)
        { //delta
            return Equals(otherObj, true);
        }

        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Returns true if and only if the specified Object is also at least an ObjectMatrix3D, both matrices have the
        /// same size, and all corresponding pairs of cells in the two matrices are the same.
        /// In other words, two matrices are defined to be equal if they contain the
        /// same cell values in the same order.
        /// Tests elements for equality or identity as specified by <i>testForEquality</i>.
        /// When testing for equality, two elements <i>e1</i> and
        /// <i>e2</i> are <i>equal</i> if <i>(e1==null ? e2==null :
        /// e1.Equals(e2))</i>d)  
        /// 
        /// @param otherObj the Object to be compared for equality with the receiver.
        /// @param testForEquality if true -> tests for equality, otherwise for identity.
        /// @return true if the specified Object is equal to the receiver.
/// </summary>
        public Boolean Equals(Object otherObj, Boolean testForEquality)
        { //delta
            if (!(otherObj is ObjectMatrix3D)) { return false; }
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            ObjectMatrix3D other = (ObjectMatrix3D)otherObj;
            if (Rows != other.Rows) return false;
            if (Columns != other.Columns) return false;

            if (!testForEquality)
            {
                for (int slice = Slices; --slice >= 0;)
                {
                    for (int row = Rows; --row >= 0;)
                    {
                        for (int column = Columns; --column >= 0;)
                        {
                            if (this[slice, row, column] != other[slice, row, column]) return false;
                        }
                    }
                }
            }
            else
            {
                for (int slice = Slices; --slice >= 0;)
                {
                    for (int row = Rows; --row >= 0;)
                    {
                        for (int column = Columns; --column >= 0;)
                        {
                            if (!(this[slice, row, column] == null ? other[slice, row, column] == null : this[slice, row, column].Equals(other[slice, row, column]))) return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Get a hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
         /// Returns the matrix cell value at coordinate <i>[slice,row,column]</i>.
         /// 
         /// @param     slice   the index of the slice-coordinate.
         /// @param     row   the index of the row-coordinate.
         /// @param     column   the index of the column-coordinate.
         /// @return    the value of the specified cell.
         /// @thRows	IndexOutOfRangeException if <i>slice&lt;0 || slice&gt;=Slices || row&lt;0 || row&gt;=Rows || column&lt;0 || column&gt;=column()</i>.
         /// </summary>
        [Obsolete("Get(int slice, int index, int column) is deprecated, please use indexer instead.")]
        public Object get(int slice, int row, int column)
        {
            if (slice < 0 || slice >= Slices || row < 0 || row >= Rows || column < 0 || column >= Columns) throw new IndexOutOfRangeException("slice:" + slice + ", row:" + row + ", column:" + column);
            return this[slice, row, column];
        }

        /// <summary>
         /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
         /// Override this method in wrappers.
         /// </summary>
        protected ObjectMatrix3D getContent()
        {
            return this;
        }

        /// <summary>
/// Fills the coordinates and values of cells having non-zero values into the specified lists.
/// Fills into the lists, starting at index 0.
/// After this call returns the specified lists all have a new size, the number of non-zero values.
/// <p>
/// In general, fill order is <i>unspecified</i>.
/// This implementation fill like: <i>for (slice = 0..Slices-1) for (row = 0..Rows-1) for (column = 0..colums-1) do ..d </i>.
/// However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
/// (Of course, result lists indexes are guaranteed to correspond to the same cell).
/// For an example, see {@link ObjectMatrix2D#getNonZeros(List<int>,List<int>,List<Object>)}.
///
/// @param sliceList the list to be filled with slice indexes, can have any size.
/// @param rowList the list to be filled with row indexes, can have any size.
/// @param columnList the list to be filled with column indexes, can have any size.
/// @param valueList the list to be filled with values, can have any size.
/// </summary>
        public void getNonZeros(List<int> sliceList, List<int> rowList, List<int> columnList, List<Object> valueList)
        {
            sliceList.Clear();
            rowList.Clear();
            columnList.Clear();
            valueList.Clear();
            int s = Slices;
            int r = Rows;
            int c = Columns;
            for (int slice = 0; slice < s; slice++)
            {
                for (int row = 0; row < r; row++)
                {
                    for (int column = 0; column < c; column++)
                    {
                        Object value = this[slice, row, column];
                        if (value != null)
                        {
                            sliceList.Add(slice);
                            rowList.Add(row);
                            columnList.Add(column);
                            valueList.Add(value);
                        }
                    }
                }
            }
        }

        /// <summary>
         /// Returns <i>true</i> if both matrices share at least one identical cell.
         /// </summary>
        protected Boolean haveSharedCells(ObjectMatrix3D other)
        {
            if (other == null) return false;
            if (this == other) return true;
            return getContent().haveSharedCellsRaw(other.getContent());
        }

        /// <summary>
         /// Returns <i>true</i> if both matrices share at least one identical cell.
         /// </summary>
        protected Boolean haveSharedCellsRaw(ObjectMatrix3D other)
        {
            return false;
        }

        /// <summary>
         /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same number of Slices, Rows and Columns.
         /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix3D</i>,
         /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix3D</i>, etc.
         /// In general, the new matrix should have internal parametrization as similar as possible.
         /// 
         /// @return  a new empty matrix of the same dynamic type.
         /// </summary>
        public ObjectMatrix3D like()
        {
            return like(Slices, Rows, Columns);
        }

        /// <summary>
         /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of Slices, Rows and Columns.
         /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix3D</i>,
         /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix3D</i>, etc.
         /// In general, the new matrix should have internal parametrization as similar as possible.
         /// 
         /// @param Slices the number of Slices the matrix shall have.
         /// @param Rows the number of Rows the matrix shall have.
         /// @param Columns the number of Columns the matrix shall have.
         /// @return  a new empty matrix of the same dynamic type.
         /// </summary>
        public abstract ObjectMatrix3D like(int Slices, int Rows, int Columns);

        /// <summary>
         /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
         /// For example, if the receiver is an instance of type <i>DenseObjectMatrix3D</i> the new matrix must also be of type <i>DenseObjectMatrix2D</i>,
         /// if the receiver is an instance of type <i>SparseObjectMatrix3D</i> the new matrix must also be of type <i>SparseObjectMatrix2D</i>, etc.
         /// 
         /// @param Rows the number of Rows the matrix shall have.
         /// @param Columns the number of Columns the matrix shall have.
         /// @param RowZero the position of the first element.
         /// @param columnZero the position of the first element.
         /// @param Rowstride the number of elements between two Rows, i.ed <i>index(i+1,j)-index(i,j)</i>.
         /// @param Columnstride the number of elements between two Columns, i.ed <i>index(i,j+1)-index(i,j)</i>.
         /// @return  a new matrix of the corresponding dynamic type.
         /// </summary>
        protected abstract ObjectMatrix2D like2D(int Rows, int Columns, int RowZero, int columnZero, int Rowstride, int Columnstride);

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        /// 
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @param    value the value to be filled into the specified cell.
        /// @thRows	IndexOutOfRangeException if <i>row&lt;0 || row&gt;=Rows || slice&lt;0 || slice&gt;=Slices || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        [Obsolete("Get(int slice, int index, int column) is deprecated, please use indexer instead.")]
        public void set(int slice, int row, int column, Object value)
        {
            if (slice < 0 || slice >= Slices || row < 0 || row >= Rows || column < 0 || column >= Columns) throw new IndexOutOfRangeException("slice:" + slice + ", row:" + row + ", column:" + column);
            this[slice, row, column] = value;
        }

        /// <summary>
         /// Constructs and returns a 2-dimensional array containing the cell values.
         /// The returned array <i>values</i> has the form <i>values[slice][row][column]</i>
         /// and has the same number of Slices, Rows and Columns as the receiver.
         /// <p>
         /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
         /// 
         /// @return an array filled with the values of the cells.
         /// </summary>
        public Object[][][] ToArray()
        {
            Object[][][] values = (new Object[Slices, Rows, Columns]).ToJagged();
            for (int slice = Slices; --slice >= 0;)
            {
                Object[][] currentSlice = values[slice];
                for (int row = Rows; --row >= 0;)
                {
                    Object[] currentRow = currentSlice[row];
                    for (int column = Columns; --column >= 0;)
                    {
                        currentRow[column] = this[slice, row, column];
                    }
                }
            }
            return values;
        }
        /// <summary>
         /// Returns a string representation using default formatting.
         /// @see Cern.Colt.matrix.objectalgo.Formatter
         /// </summary>
        public override String ToString()
        {
            return new Cern.Colt.Matrix.ObjectAlgorithms.Formatter().ToString(this);
        }
        /// <summary>
         /// Constructs and returns a new view equal to the receiver.
         /// The view is a shallow cloned Calls <code>clone()</code> and casts the result.
         /// <p>
         /// <b>Note that the view is not a deep copy.</b>
         /// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versad 
         /// <p>
         /// Use {@link #copy()} if you want to construct an independent deep copy rather than a new view.
         /// 
         /// @return  a new view of the receiver.
         /// </summary>
        protected ObjectMatrix3D view()
        {
            return (ObjectMatrix3D)Clone();
        }
        /// <summary>
/// Constructs and returns a new 2-dimensional <i>slice view</i> representing the Slices and Rows of the given column.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
/// <p>
/// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
/// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
/// To obtain 1-dimensional views on subranges, apply both steps.

/// @param column the index of the column to fix.
/// @return a new 2-dimensional slice view.
/// @thRows IndexOutOfRangeException if <i>column < 0 || column >= Columns</i>.
/// @see #viewSlice(int)
/// @see #viewRow(int)
/// </summary>
        public ObjectMatrix2D viewColumn(int column)
        {
            CheckColumn(column);
            int sliceRows = this.Slices;
            int sliceColumns = this.Rows;

            //int sliceOffset = index(0,0,column);
            int sliceRowZero = SliceZero;
            int sliceColumnZero = RowZero + ColumnOffset(ColumnRank(column));

            int sliceRowstride = this.SliceStride;
            int sliceColumnstride = this.RowStride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowstride, sliceColumnstride);
        }
        /// <summary>
/// Constructs and returns a new <i>flip view</i> along the column axis.
/// What used to be column <i>0</i> is now column <i>Columns-1</i>, ..d, what used to be column <i>Columns-1</i> is now column <i>0</i>.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @return a new flip view.
/// @see #viewSliceFlip()
/// @see #viewRowFlip()
/// </summary>
        public ObjectMatrix3D viewColumnFlip()
        {
            return (ObjectMatrix3D)(view().VColumnFlip());
        }
        /// <summary>
/// Constructs and returns a new <i>dice view</i>; Swaps dimensions (axes); Example: 3 x 4 x 5 matrix --> 4 x 3 x 5 matrix.
/// The view has dimensions exchanged; what used to be one axis is now another, in all desired permutations.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @param axis0 the axis that shall become axis 0 (legal values 0..2).
/// @param axis1 the axis that shall become axis 1 (legal values 0..2).
/// @param axis2 the axis that shall become axis 2 (legal values 0..2).
/// @return a new dice view.
/// @thRows ArgumentException if some of the parameters are equal or not in range 0..2.
/// </summary>
        public ObjectMatrix3D viewDice(int axis0, int axis1, int axis2)
        {
            return (ObjectMatrix3D)(view().VDice(axis0, axis1, axis2));
        }
        /// <summary>
/// Constructs and returns a new <i>sub-range view</i> that is a <i>depth x height x width</i> sub matrix starting at <i>[slice,row,column]</i>;
/// Equivalent to <i>view().part(slice,row,column,depth,height,width)</i>; Provided for convenience only.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 

/// @param     slice   The index of the slice-coordinate.
/// @param     row   The index of the row-coordinate.
/// @param     column   The index of the column-coordinate.
/// @param     depth   The depth of the box.
/// @param     height   The height of the box.
/// @param     width   The width of the box.
/// @thRows	IndexOutOfRangeException if <i>slice<0 || depth<0 || slice+depth>Slices || row<0 || height<0 || row+height>Rows || column<0 || width<0 || column+width>Columns</i>
/// @return the new view.
                
/// </summary>
        public ObjectMatrix3D viewPart(int slice, int row, int column, int depth, int height, int width)
        {
            return (ObjectMatrix3D)(view().VPart(slice, row, column, depth, height, width));
        }
        /// <summary>
/// Constructs and returns a new 2-dimensional <i>slice view</i> representing the Slices and Columns of the given row.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
/// <p>
/// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
/// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
/// To obtain 1-dimensional views on subranges, apply both steps.

/// @param row the index of the row to fix.
/// @return a new 2-dimensional slice view.
/// @thRows IndexOutOfRangeException if <i>row < 0 || row >= row()</i>.
/// @see #viewSlice(int)
/// @see #viewColumn(int)
/// </summary>
        public ObjectMatrix2D viewRow(int row)
        {
            CheckRow(row);
            int sliceRows = this.Slices;
            int sliceColumns = this.Columns;

            //int sliceOffset = index(0,row,0);
            int sliceRowZero = SliceZero;
            int sliceColumnZero = ColumnZero + RowOffset(RowRank(row));

            int sliceRowstride = this.SliceStride;
            int sliceColumnstride = this.ColumnStride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowstride, sliceColumnstride);
        }
        /// <summary>
/// Constructs and returns a new <i>flip view</i> along the row axis.
/// What used to be row <i>0</i> is now row <i>Rows-1</i>, ..d, what used to be row <i>Rows-1</i> is now row <i>0</i>.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @return a new flip view.
/// @see #viewSliceFlip()
/// @see #viewColumnFlip()
/// </summary>
        public ObjectMatrix3D viewRowFlip()
        {
            return (ObjectMatrix3D)(view().VRowFlip());
        }
        /// <summary>
/// Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
/// There holds <i>view.Slices == slicees.Length, view.Rows == rowes.Length, view.Columns == columnIndexes.Length</i> and 
/// <i>view.Get(k,i,j) == this.Get(slicees[k],rowes[i],columnIndexes[j])</i>.
/// Indexes can occur multiple times and can be in arbitrary order.
/// For an example see {@link ObjectMatrix2D#viewSelection(int[],int[])}.
/// <p>
/// Note that modifying the index arguments after this call has returned has no effect on the view.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 

/// @param  slicees   The Slices of the cells that shall be visible in the new viewd To indicate that <i>all</i> Slices shall be visible, simply set this parameter to <i>null</i>.
/// @param  rowes   The Rows of the cells that shall be visible in the new viewd To indicate that <i>all</i> Rows shall be visible, simply set this parameter to <i>null</i>.
/// @param  columnIndexes   The Columns of the cells that shall be visible in the new viewd To indicate that <i>all</i> Columns shall be visible, simply set this parameter to <i>null</i>.
/// @return the new view.
/// @thRows IndexOutOfRangeException if <i>!(0 <= slicees[i] < Slices)</i> for any <i>i=0..slicees.Length()-1</i>.
/// @thRows IndexOutOfRangeException if <i>!(0 <= rowes[i] < Rows)</i> for any <i>i=0..rowes.Length()-1</i>.
/// @thRows IndexOutOfRangeException if <i>!(0 <= columnIndexes[i] < Columns)</i> for any <i>i=0..columnIndexes.Length()-1</i>.
/// </summary>
        public ObjectMatrix3D viewSelection(int[] sliceIndexes, int[] rowIndexes, int[] columnIndexes)
        {
            // check for "all"
            if (sliceIndexes == null)
            {
                sliceIndexes = new int[Slices];
                for (int i = Slices; --i >= 0;) sliceIndexes[i] = i;
            }
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

            CheckSliceIndexes(sliceIndexes);
            CheckRowIndexes(rowIndexes);
            CheckColumnIndexes(columnIndexes);

            int[] sliceOffsets = new int[sliceIndexes.Length];
            int[] rowOffsets = new int[rowIndexes.Length];
            int[] columnOffsets = new int[columnIndexes.Length];

            for (int i = sliceIndexes.Length; --i >= 0;)
            {
                sliceOffsets[i] = SliceOffset(SliceRank(sliceIndexes[i]));
            }
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowOffsets[i] = RowOffset(RowRank(rowIndexes[i]));
            }
            for (int i = columnIndexes.Length; --i >= 0;)
            {
                columnOffsets[i] = ColumnOffset(ColumnRank(columnIndexes[i]));
            }

            return viewSelectionLike(sliceOffsets, rowOffsets, columnOffsets);
        }
        /// <summary>
/// Constructs and returns a new <i>selection view</i> that is a matrix holding all <b>Slices</b> matching the given condition.
/// Applies the condition to each slice and takes only those where <i>condition(viewSlice(i))</i> yields <i>true</i>.
/// To match Rows or Columns, use a dice view.
/// <p>
/// <b>Example:</b>
/// <br>
/// <pre>
/// // extract and view all Slices which have an aggregate sum > 1000
/// matrix.ViewSelection( 
/// &nbsp;&nbsp;&nbsp;new ObjectMatrix2DProcedure() {
/// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public Boolean apply(ObjectMatrix2D m) { return m.zSum > 1000; }
/// &nbsp;&nbsp;&nbsp;}
/// );
/// </pre>
/// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 

/// @param  condition The condition to be matched.
/// @return the new view.
/// </summary>
        public ObjectMatrix3D viewSelection(ObjectMatrix2DProcedure condition)
        {
            List<int> matches = new List<int>();
            for (int i = 0; i < Slices; i++)
            {
                if (condition(viewSlice(i))) matches.Add(i);
            }

            matches.TrimExcess();
            return viewSelection(matches.ToArray(), null, null); // take all Rows and Columns
        }
        /// <summary>
         /// Construct and returns a new selection view.
         /// 
         /// @param sliceOffsets the offsets of the visible elements.
         /// @param rowOffsets the offsets of the visible elements.
         /// @param columnOffsets the offsets of the visible elements.
         /// @return  a new view.
         /// </summary>
        protected abstract ObjectMatrix3D viewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets);
        /// <summary>
/// Constructs and returns a new 2-dimensional <i>slice view</i> representing the Rows and Columns of the given slice.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
/// <p>
/// To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(..d)</i>), then apply this method to the sub-range view.
/// To obtain 1-dimensional views, apply this method, then apply another slice view (methods <i>viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
/// To obtain 1-dimensional views on subranges, apply both steps.

/// @param slice the index of the slice to fix.
/// @return a new 2-dimensional slice view.
/// @thRows IndexOutOfRangeException if <i>slice < 0 || slice >= Slices</i>.
/// @see #viewRow(int)
/// @see #viewColumn(int)
/// </summary>
        public ObjectMatrix2D viewSlice(int slice)
        {
            CheckSlice(slice);
            int sliceRows = this.Rows;
            int sliceColumns = this.Columns;

            //int sliceOffset = index(slice,0,0);
            int sliceRowZero = RowZero;
            int sliceColumnZero = ColumnZero + SliceOffset(SliceRank(slice));

            int sliceRowstride = this.RowStride;
            int sliceColumnstride = this.ColumnStride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowstride, sliceColumnstride);
        }
        /// <summary>
/// Constructs and returns a new <i>flip view</i> along the slice axis.
/// What used to be slice <i>0</i> is now slice <i>Slices-1</i>, ..d, what used to be slice <i>Slices-1</i> is now slice <i>0</i>.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @return a new flip view.
/// @see #viewRowFlip()
/// @see #viewColumnFlip()
/// </summary>
        public ObjectMatrix3D viewSliceFlip()
        {
            return (ObjectMatrix3D)(view().VSliceFlip());
        }
        /// <summary>
/// Sorts the matrix Slices into ascending order, according to the <i>natural ordering</i> of the matrix values in the given <i>[row,column]</i> position.
/// This sort is guaranteed to be <i>stable</i>.
/// For further information, see {@link Cern.Colt.matrix.objectalgo.Sorting#sort(ObjectMatrix3D,int,int)}.
/// For more advanced sorting functionality, see {@link Cern.Colt.matrix.objectalgo.Sorting}.
/// @return a new sorted vector (matrix) view.
/// @thRows IndexOutOfRangeException if <i>row < 0 || row >= Rows || column < 0 || column >= Columns</i>.
/// </summary>
        public ObjectMatrix3D viewSorted(int row, int column)
        {
            return Cern.Colt.Matrix.ObjectAlgorithms.Sorting.mergeSort.sort(this, row, column);
        }
        /// <summary>
/// Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
/// More specifically, the view has <i>this.Slices/Slicestride</i> Slices and <i>this.Rows/Rowstride</i> Rows and <i>this.Columns/Columnstride</i> Columns 
/// holding cells <i>this.Get(k*Slicestride,i*Rowstride,j*Columnstride)</i> for all <i>k = 0..Slices/Slicestride - 1, i = 0..Rows/Rowstride - 1, j = 0..Columns/Columnstride - 1</i>.
/// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

/// @param Slicestride the slice step factor.
/// @param Rowstride the row step factor.
/// @param Columnstride the column step factor.
/// @return a new view.
/// @thRows	IndexOutOfRangeException if <i>Slicestride<=0 || Rowstride<=0 || Columnstride<=0</i>.
/// </summary>
        public ObjectMatrix3D viewStrides(int Slicestride, int Rowstride, int Columnstride)
        {
            return (ObjectMatrix3D)(view().VStrides(Slicestride, Rowstride, Columnstride));
        }
        /// <summary>
         /// Applies a procedure to each cell's value.
         /// Iterates downwards from <i>[Slices-1,Rows-1,Columns-1]</i> to <i>[0,0,0]</i>,
         /// as demonstrated by this snippet:
         /// <pre>
         /// for (int slice=Slices; --slice >=0;) {
         ///    for (int row=Rows; --row >= 0;) {
         ///       for (int column=Columns; --column >= 0;) {
         ///           if (!procedure(get(slice,row,column))) return false;
         ///       }
         ///    }
         /// }
         /// return true;
         /// </pre>
         /// Note that an implementation may use more efficient techniques, but must not use any other order.
         /// 
         /// @param procedure a procedure object taking as argument the current cell's valued Stops iteration if the procedure returns <i>false</i>, otherwise continuesd 
         /// @return <i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised 
         /// </summary>
        private Boolean xforEach(Cern.Colt.Function.ObjectProcedure<Object> procedure)
        {
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        if (!procedure(this[slice, row, column])) return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
         /// Applies a procedure to each cell's coordinate.
         /// Iterates downwards from <i>[Slices-1,Rows-1,Columns-1]</i> to <i>[0,0,0]</i>,
         /// as demonstrated by this snippet:
         /// <pre>
         /// for (int slice=Slices; --slice >=0;) {
         ///    for (int row=Rows; --row >= 0;) {
         ///       for (int column=Columns; --column >= 0;) {
         ///           if (!procedure(slice,row,column)) return false;
         ///       }
         ///    }
         /// }
         /// return true;
         /// </pre>
         /// Note that an implementation may use more efficient techniques, but must not use any other order.
         /// 
         /// @param procedure a procedure object taking as first argument the current slice, as second argument the current row, and as third argument the current columnd Stops iteration if the procedure returns <i>false</i>, otherwise continuesd 
         /// @return <i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised 
         /// </summary>
        private Boolean xforEachCoordinate(Cern.Colt.Function.IntIntIntProcedure procedure)
        {
            for (int column = Columns; --column >= 0;)
            {
                for (int slice = Slices; --slice >= 0;)
                {
                    for (int row = Rows; --row >= 0;)
                    {
                        if (!procedure(slice, row, column)) return false;
                    }
                }
            }
            return true;
        }
    }
}
