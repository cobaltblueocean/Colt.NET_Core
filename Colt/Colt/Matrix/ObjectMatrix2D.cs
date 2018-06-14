// <copyright file="ObjectMatrix2D.cs" company="CERN">
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
    public delegate Boolean ObjectMatrix2DProcedure(ObjectMatrix2D element);

    /// <summary>
    /// Abstract base class for 2-d matrices holding <i>Object</i> elements.
    ///    First see the<a href="package-summary.html"> package summary</a> and javadoc<a href="package-tree.html"> tree view</a> to get the broad picture.
    ///    <p>
    ///    A matrix has a number of Rows and Columns, which are assigned upon instance construction - The matrix's size is then <i>Rows*Columns</i>.
    /// Elements are accessed via <i>[row, column]</i> coordinates.
    ///    Legal coordinates range from <i>[0,0]</i> to<i>[Rows - 1, Columns - 1]</i>.
    /// Any attempt to access an element at a coordinate<i> column&lt;0 || column&gt;=Columns || row&lt;0 || row&gt;=Rows</i> will throw an<i> IndexOutOfRangeException</i>.
    /// <p>
    /// <b>Note</b> that this implementation is not synchronized.
    /// </summary>
    public abstract class ObjectMatrix2D : AbstractMatrix2D
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Get or set the matrix cell value at coordinate <i>[row,column]</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; Columns && 0 &lt;= row &lt; Rows</i>.
        /// </summary>
        /// <param name="rowindex">the index of the row-coordinate.</param>
        /// <param name="colindex">the index of the column-coordinate.</param>
        /// <returns>the value of the specified cell.</returns>
        public abstract Object this[int rowindex, int colindex] { get; set; }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of Rows and Columns.
        /// For example, if the receiver is an instance of type <see cref="DenseObjectMatrix2D"/> the new matrix must also be of type <see cref="DenseObjectMatrix2D"/>,
        /// if the receiver is an instance of type <see cref="SparseObjectMatrix2D" /> the new matrix must also be of type <see cref="SparseObjectMatrix2D" />, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public abstract ObjectMatrix2D Like(int Rows, int Columns);

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <see cref="DenseObjectMatrix2D"/> the new matrix must be of type <see cref="DenseObjectMatrix2D"/>,
        /// if the receiver is an instance of type <see cref="SparseObjectMatrix2D" /> the new matrix must be of type <see cref="SparseObjectMatrix2D" />, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public abstract ObjectMatrix1D Like1D(int size);

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <see cref="DenseObjectMatrix2D"/> the new matrix must be of type <see cref="DenseObjectMatrix2D"/>,
        /// if the receiver is an instance of type <see cref="SparseObjectMatrix2D" /> the new matrix must be of type <see cref="SparseObjectMatrix2D" />, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <param name="zero">the index of the first element.</param>
        /// <param name="stride">the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        protected abstract ObjectMatrix1D Like1D(int size, int zero, int stride);

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="rowOffsets">the offsets of the visible elements.</param>
        /// <param name="columnOffsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected abstract ObjectMatrix2D ViewSelectionLike(int[] rowOffsets, int[] columnOffsets);

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Applies a function to each cell and aggregates the results.
        /// Returns a value <i>v</i> such that <i>v==a(Size)</i> where <i>a(i) == aggr( a(i-1), f(get(row,column)) )</i> and terminators are <i>a(1) == f(get(0,0)), a(0)==null</i>.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// cern.jet.math.Functions F = cern.jet.math.Functions.Functions;
        /// 2 x 2 matrix
        /// 0 1
        /// 2 3
        ///
        /// // Sum( x[row,col]*x[row,col] ) 
        /// matrix.aggregate(F.plus,F.square);
        /// --> 14
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// </summary>
        /// <param name="aggr">an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.</param>
        /// <param name="f">a function transforming the current cell value.</param>
        /// <returns>the aggregated measure.</returns>
        /// <see cref="Cern.Jet.Math.Functions"/>
        public Object Aggregate(Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectFunction<Object> f)
        {
            if (Size == 0) return null;
            Object a = f(this[Rows - 1, Columns - 1]);
            int d = 1; // last cell already done
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns - d; --column >= 0;)
                {
                    a = aggr(a, f(this[row, column]));
                }
                d = 0;
            }
            return a;
        }

        /// <summary>
        /// Applies a function to each corresponding cell of two matrices and aggregates the results.
        /// Returns a value <i>v</i> such that <i>v==a(Size)</i> where <i>a(i) == aggr( a(i-1), f(get(row,column),other.Get(row,column)) )</i> and terminators are <i>a(1) == f(get(0,0),other.Get(0,0)), a(0)==null</i>.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// cern.jet.math.Functions F = cern.jet.math.Functions.Functions;
        /// x == 2 x 2 matrix
        /// 0 1
        /// 2 3
        ///
        /// y == 2 x 2 matrix
        /// 0 1
        /// 2 3
        ///
        /// // Sum( x[row,col] * y[row,col] ) 
        /// x.aggregate(y, F.plus, F.mult);
        /// --> 14
        ///
        /// // Sum( (x[row,col] + y[row,col])^2 )
        /// x.aggregate(y, F.plus, F.chain(F.square,F.plus));
        /// --> 56
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// </summary>
        /// <param name="aggr">an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.</param>
        /// <param name="f">a function transforming the current cell value.</param>
        /// <returns>the aggregated measure.</returns>
        /// <exception cref="ArgumentException">if <i>Columns != other.Columns || Rows != other.Rows</i></exception>
        /// <see cref="Cern.Jet.Math.Functions"/>
        public Object Aggregate(ObjectMatrix2D other, Cern.Colt.Function.ObjectObjectFunction<Object> aggr, Cern.Colt.Function.ObjectObjectFunction<Object> f)
        {
            CheckShape(other);
            if (Size == 0) return null;
            Object a = f(this[Rows - 1, Columns - 1], other[Rows - 1, Columns - 1]);
            int d = 1; // last cell already done
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns - d; --column >= 0;)
                {
                    a = aggr(a, f(this[row, column], other[row, column]));
                }
                d = 0;
            }
            return a;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>values</i>.
        /// <i>values</i> is required to have the form <i>values[row][column]</i>
        /// and have exactly the same number of Rows and Columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != Rows || for any 0 &lt;= row &lt; Rows: values[row].Length != Columns</i>.</exception>
        public ObjectMatrix2D Assign(Object[][] values)
        {
            if (values.Length != Rows) throw new ArgumentException("Must have same number of Rows: Rows=" + values.Length + "Rows=" + Rows);
            for (int row = Rows; --row >= 0;)
            {
                Object[] currentRow = values[row];
                if (currentRow.Length != Columns) throw new ArgumentException("Must have same number of Columns in every row: Columns=" + currentRow.Length + "Columns=" + Columns);
                for (int column = Columns; --column >= 0;)
                {
                    this[row, column] = currentRow[column];
                }
            }
            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <i>x[row,col] = function(x[row,col])</i>.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// matrix = 2 x 2 matrix 
        /// 0.5 1.5      
        /// 2.5 3.5
        ///
        /// // change each cell to its sine
        /// matrix.assign(cern.jet.math.Functions.sin);
        /// -->
        /// 2 x 2 matrix
        /// 0.479426  0.997495 
        /// 0.598472 -0.350783
        /// </pre>
        /// For further examples, see the <see cref="Function.ObjectFunction{C}"/> doc</a>.
        /// </summary>
        /// <param name="function">a function object taking as argument the current cell's value.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <see cref="Cern.Jet.Math.Functions"></see>
        public ObjectMatrix2D Assign(Cern.Colt.Function.ObjectFunction<Object> function)
        {
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns; --column >= 0;)
                {
                    this[row, column] = function(this[row, column]);
                }
            }
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same number of Rows and Columns.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        /// </summary>
        /// <param name="other">the source matrix to copy from (may be identical to the receiver).</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>Columns != other.Columns || Rows != other.Rows</i></exception>
        public ObjectMatrix2D Assign(ObjectMatrix2D other)
        {
            if (other == this) return this;
            CheckShape(other);
            if (HaveSharedCells(other)) other = other.Copy();

            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns; --column >= 0;)
                {
                    this[row, column] = other[row, column];
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
        /// m1 = 2 x 2 matrix 
        /// 0 1 
        /// 2 3
        ///
        /// m2 = 2 x 2 matrix 
        /// 0 2 
        /// 4 6
        ///
        /// m1.assign(m2, cern.jet.math.Functions.pow);
        /// -->
        /// m1 == 2 x 2 matrix
        ///         1   1 
        /// 16 729
        /// </pre>
        /// For further examples, see the <see cref="Function.ObjectFunction{C}"/> doc</a>.
        /// </summary>
        /// <param name="y">the secondary matrix to operate on.</param>
        /// <param name="function">a function object taking as first argument the current cell's value of <i>this</i>, and as second argument the current cell's value of <i>y</i>,</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>Columns != other.Columns || Rows != other.Rows</i></exception>
        /// <see cref="Cern.Jet.Math.Functions"></see>
        public ObjectMatrix2D Assign(ObjectMatrix2D y, Cern.Colt.Function.ObjectObjectFunction<Object> function)
        {
            CheckShape(y);
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns; --column >= 0;)
                {
                    this[row, column] = function(this[row, column], y[row, column]);
                }
            }
            return this;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="value">the value to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        public ObjectMatrix2D Assign(Object value)
        {
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns; --column >= 0;)
                {
                    this[row, column] = value;
                }
            }
            return this;
        }

        /// <summary>
        /// Returns the number of cells having non-zero values; ignores tolerance.
        /// </summary>
        public int Cardinality()
        {
            int cardinality = 0;
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns; --column >= 0;)
                {
                    if (this[row, column] != null) cardinality++;
                }
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
        public ObjectMatrix2D Copy()
        {
            return Like().Assign(this);
        }

        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Equivalent to <i>Equals(otherObj,true)</i>d  
        /// </summary>
        /// <param name="otherObj">the Object to be compared for equality with the receiver.</param>
        /// <returns>true if the specified Object is equal to the receiver.</returns>
        public override Boolean Equals(Object otherObj)
        { //delta
            return Equals(otherObj, true);
        }

        /// <summary>
        /// Compares the specified Object with the receiver for equality.
        /// Returns true if and only if the specified Object is also at least an ObjectMatrix2D, both matrices have the
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
            if (!(otherObj is ObjectMatrix2D)) { return false; }
            if (this == otherObj) return true;
            if (otherObj == null) return false;
            ObjectMatrix2D other = (ObjectMatrix2D)otherObj;
            if (Rows != other.Rows) return false;
            if (Columns != other.Columns) return false;

            if (!testForEquality)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        if (this[row, column] != other[row, column]) return false;
                    }
                }
            }
            else
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        if (!(this[row, column] == null ? other[row, column] == null : this[row, column].Equals(other[row, column]))) return false;
                    }
                }
            }

            return true;

        }

        /// <summary>
        /// Get Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>[row,column]</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value of the specified cell.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>column&lt;0 || column&gt;=Columns || row&lt;0 || row&gt;=Rows</i></exception>
        [Obsolete("Get(int index, int column) is deprecated, please use indexer instead.")]
        public Object Get(int row, int column)
        {
            if (column < 0 || column >= Columns || row < 0 || row >= Rows) throw new IndexOutOfRangeException("row:" + row + ", column:" + column);
            return this[row, column];
        }

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        protected ObjectMatrix2D GetContent()
        {
            return this;
        }

        /// <summary>
        /// Fills the coordinates and values of cells having non-zero values into the specified lists.
        /// Fills into the lists, starting at index 0.
        /// After this call returns the specified lists all have a new size, the number of non-zero values.
        /// <p>
        /// In general, fill order is <i>unspecified</i>.
        /// This implementation fills like <i>for (row = 0..Rows-1) for (column = 0..Columns-1) do ..d </i>.
        /// However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
        /// (Of course, result lists indexes are guaranteed to correspond to the same cell).
        /// <p>
        /// <b>Example:</b>
        /// <br>
        /// <pre>
        /// 2 x 3 matrix:
        /// 0, 0, 8
        /// 0, 7, 0
        /// -->
        /// rowList    = (0,1)
        /// columnList = (2,1)
        /// valueList  = (8,7)
        /// </pre>
        /// In other words, <i>get(0,2)==8, get(1,1)==7</i>.
        /// </summary>
        /// <param name="rowList">the list to be filled with row indexes, can have any size.</param>
        /// <param name="columnList">the list to be filled with column indexes, can have any size.</param>
        /// <param name="valueList">the list to be filled with values, can have any size.</param>
        /// <returns></returns>
        /// <exception cref=""></exception>
        public void GetNonZeros(ref List<int> rowList, ref List<int> columnList, ref List<Object> valueList)
        {
            rowList.Clear();
            columnList.Clear();
            valueList.Clear();
            int r = Rows;
            int c = Columns;
            for (int row = 0; row < r; row++)
            {
                for (int column = 0; column < c; column++)
                {
                    Object value = this[row, column];
                    if (value != null)
                    {
                        rowList.Add(row);
                        columnList.Add(column);
                        valueList.Add(value);
                    }
                }
            }
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same number of Rows and Columns.
        /// For example, if the receiver is an instance of type <see cref="DenseObjectMatrix2D"/> the new matrix must also be of type <see cref="DenseObjectMatrix2D"/>,
        /// if the receiver is an instance of type ,<see cref="SparseObjectMatrix2D"/> the new matrix must also be of type <see cref="SparseObjectMatrix2D"/>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public ObjectMatrix2D Like()
        {
            return Like(Rows, Columns);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[row,column]</i> to the specified value.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        /// <exception cref="IndexOutOfRangeException">if <i>column&lt;0 || column&gt;=Columns || row&lt;0 || row&gt;=Rows</i></exception>
        [Obsolete("Set(int index, int column, Object value) is deprecated, please use indexer instead.")]
        public void Set(int row, int column, Object value)
        {
            if (column < 0 || column >= Columns || row < 0 || row >= Rows) throw new IndexOutOfRangeException("row:" + row + ", column:" + column);
            this[row, column] = value;
        }

        /// <summary>
        /// Constructs and returns a 2-dimensional array containing the cell values.
        /// The returned array <i>values</i> has the form <i>values[row][column]</i>
        /// and has the same number of Rows and Columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <returns>an array filled with the values of the cells.</returns>
        public Object[][] ToArray()
        {
            Object[][] values = (new Object[Rows, Columns]).ToJagged();
            for (int row = Rows; --row >= 0;)
            {
                Object[] currentRow = values[row];
                for (int column = Columns; --column >= 0;)
                {
                    currentRow[column] = this[row, column];
                }
            }
            return values;
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
        /// Constructs and returns a new <i>slice view</i> representing the Rows of the given column.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>viewPart(..d)</i>), then apply this method to the sub-range view.
        /// <p> 
        /// <b>Example:</b> 
        /// <table border="0">
        ///          <tr nowrap> 
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///            <td>viewColumn(0) ==></td>
        ///            <td valign="top">Matrix1D of size 2:<br>
        ///              1, 4</td>
        ///           </tr>
        /// </table>
        /// </summary>
        /// <param name="column">the column to fix.</param>
        /// <returns>a new slice view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>column &lt; 0 || column >= Columns</i>.</exception>
        /// <see cref="ViewRow(int)"/>
        public ObjectMatrix1D ViewColumn(int column)
        {
            CheckColumn(column);
            int viewSize = this.Rows;
            int viewZero = Index(0, column);
            int viewStride = this.RowStride;
            return Like1D(viewSize, viewZero, viewStride);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i> along the column axis.
        /// What used to be column <i>0</i> is now column <i>Columns-1</i>, ..d, what used to be column <i>Columns-1</i> is now column <i>0</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p> 
        /// <b>Example:</b> 
        /// <table border="0">
        ///          <tr nowrap> 
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///            <td>columnFlip ==></td>
        ///            <td valign="top">2 x 3 matrix:<br>
        ///              3, 2, 1 <br>
        ///              6, 5, 4</td>
        ///            <td>columnFlip ==></td>
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///          </tr>
        /// </table>
        /// </summary>
        /// <returns>a new flip view.</returns>
        /// <see cref="ViewRowFlip()"/>
        public ObjectMatrix2D ViewColumnFlip()
        {
            return (ObjectMatrix2D)(View().VColumnFlip());
        }

        /// <summary>
        /// Constructs and returns a new <i>dice (transposition) view</i>; Swaps axes; example: 3 x 4 matrix --> 4 x 3 matrix.
        /// The view has both dimensions exchanged; what used to be Columns become Rows, what used to be Rows become Columns.
        /// In other words: <i>view.Get(row,column)==this.Get(column,row)</i>.
        /// This is a zero-copy transposition, taking O(1), i.ed constant time.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 
        /// Use idioms like <i>result = viewDice(A).copy()</i> to generate an independent transposed matrix.
        /// <p> 
        /// <b>Example:</b> 
        /// <table border="0">
        ///          <tr nowrap> 
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///            <td>transpose ==></td>
        ///            <td valign="top">3 x 2 matrix:<br>
        ///              1, 4 <br>
        ///              2, 5 <br>
        ///              3, 6</td>
        ///            <td>transpose ==></td>
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///          </tr>
        /// </table>
        /// </summary>
        /// <returns>a new dice view.</returns>
        public ObjectMatrix2D ViewDice()
        {
            return (ObjectMatrix2D)(View().VDice());
        }

        /// <summary>
        /// Constructs and returns a new <i>sub-range view</i> that is a <i>height x width</i> sub matrix starting at <i>[row,column]</i>.
        ///
        /// Operations on the returned view can only be applied to the restricted range.
        /// Any attempt to access coordinates not contained in the view will throw an <i>IndexOutOfRangeException</i>.
        /// <p>
        /// <b>Note that the view is really just a range restriction:</b> 
        /// The returned matrix is backed by this matrix, so changes in the returned matrix are reflected in this matrix, and vice-versad 
        /// <p>
        /// The view contains the cells from <i>[row,column]</i> to <i>[row+height-1,column+width-1]</i>, all inclusive.
        /// and has <i>view.Rows == height; view.Columns == width;</i>.
        /// A view's legal coordinates are again zero based, as usual.
        /// In other words, legal coordinates of the view range from <i>[0,0]</i> to <i>[view.Rows-1==height-1,view.Columns-1==width-1]</i>.
        /// As usual, any attempt to access a cell at a coordinate <i>column&lt;0 || column&gt;=view.Columns || row&lt;0 || row&gt;=view.Rows</i> will throw an <i>IndexOutOfRangeException</i>.
        /// </summary>
        /// <param name="row">The index of the row-coordinate.</param>
        /// <param name="column">The index of the column-coordinate.</param>
        /// <param name="height">The height of the box.</param>
        /// <param name="width">The width of the box.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>column &lt; 0 || width &lt; 0 || column + width > Columns || row &lt; 0 || height &lt; 0 || row + height > Rows</i></exception>
        public ObjectMatrix2D ViewPart(int row, int column, int height, int width)
        {
            return (ObjectMatrix2D)(View().VPart(row, column, height, width));
        }

        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the Columns of the given row.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To obtain a slice view on subranges, construct a sub-ranging view (<i>viewPart(..d)</i>), then apply this method to the sub-range view.
        /// <p> 
        /// <b>Example:</b> 
        /// <table border="0">
        ///          <tr nowrap> 
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///            <td>viewRow(0) ==></td>
        ///            <td valign="top">Matrix1D of size 3:<br>
        ///              1, 2, 3</td>
        ///           </tr>
        /// </table>
        /// </summary>
        /// <param name="row">the row to fix.</param>
        /// <returns>a new slice view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>row &lt; 0 || row >= Rows</i>.</exception>
        /// <see cref="ViewColumn(int)"/>
        public ObjectMatrix1D ViewRow(int row)
        {
            CheckRow(row);
            int viewSize = this.Columns;
            int viewZero = Index(row, 0);
            int viewStride = this.ColumnStride;
            return Like1D(viewSize, viewZero, viewStride);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i> along the row axis.
        /// What used to be row <i>0</i> is now row <i>Rows-1</i>, ..d, what used to be row <i>Rows-1</i> is now row <i>0</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// <p> 
        /// <b>Example:</b> 
        /// <table border="0">
        ///          <tr nowrap> 
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///            <td>rowFlip ==></td>
        ///            <td valign="top">2 x 3 matrix:<br>
        ///              4, 5, 6 <br>
        ///              1, 2, 3</td>
        ///            <td>rowFlip ==></td>
        ///            <td valign="top">2 x 3 matrix: <br>
        ///              1, 2, 3<br>
        ///              4, 5, 6 </td>
        ///          </tr>
        /// </table>
        /// </summary>
        /// <returns>a new flip view.</returns>
        /// <see cref="ViewColumnFlip()"/>
        public ObjectMatrix2D ViewRowFlip()
        {
            return (ObjectMatrix2D)(View().VRowFlip());
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        /// There holds <i>view.Rows == rowIndexes.Length, view.Columns == columnIndexes.Length</i> and <i>view.Get(i,j) == this.Get(rowIndexes[i],columnIndexes[j])</i>.
        /// Indexes can occur multiple times and can be in arbitrary order.
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// this = 2 x 3 matrix:
        /// 1, 2, 3
        /// 4, 5, 6
        /// rowIndexes     = (0,1)
        /// columnIndexes  = (1,0,1,0)
        /// -->
        /// view = 2 x 4 matrix:
        /// 2, 1, 2, 1
        /// 5, 4, 5, 4
        /// </pre>
        /// Note that modifying the index arguments after this call has returned has no effect on the view.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 
        /// <p>
        /// To indicate "all" Rows or "all Columns", simply set the respective parameter
        /// @param     
        /// @param     
        /// @return 
        /// @thRows  
        /// @thRows  
        /// </summary>
        /// <param name="rowIndexes">The Rows of the cells that shall be visible in the new viewd To indicate that <i>all</i> Rows shall be visible, simply set this parameter to <i>null</i>.</param>
        /// <param name="columnIndexes">The Columns of the cells that shall be visible in the new viewd To indicate that <i>all</i> Columns shall be visible, simply set this parameter to <i>null</i>.</param>
        /// <returns>the new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>!(0 &lt;= rowIndexes[i] &lt; Rows)</i> for any <i>i=0..rowIndexes.Length()-1</i>.</exception>
        /// <exception cref="IndexOutOfRangeException">if <i>!(0 &lt;= columnIndexes[i] &lt; Columns)</i> for any <i>i=0..columnIndexes.Length()-1</i>.</exception>
        public ObjectMatrix2D ViewSelection(int[] rowIndexes, int[] columnIndexes)
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
            int[] rowOffsets = new int[rowIndexes.Length];
            int[] columnOffsets = new int[columnIndexes.Length];
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowOffsets[i] = RowOffset(RowRank(rowIndexes[i]));
            }
            for (int i = columnIndexes.Length; --i >= 0;)
            {
                columnOffsets[i] = ColumnOffset(ColumnRank(columnIndexes[i]));
            }
            return ViewSelectionLike(rowOffsets, columnOffsets);
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding all <b>Rows</b> matching the given condition.
        /// Applies the condition to each row and takes only those row where <i>condition(viewRow(i))</i> yields <i>true</i>.
        /// To match Columns, use a dice view.
        /// <p>
        /// <b>Example:</b>
        /// <br>
        /// <pre>
        /// // extract and view all Rows which have a value &lt; threshold in the first column (representing "age")
        /// Object threshold = 16;
        /// matrix.viewSelection( 
        /// &nbsp;&nbsp;&nbsp;new ObjectMatrix1DProcedure() {
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public Boolean apply(ObjectMatrix1D m) { return m.Get(0) &lt; threshold; }
        /// &nbsp;&nbsp;&nbsp;}
        /// );
        ///
        /// // extract and view all Rows with RMS &lt; threshold
        /// // The RMS (Root-Mean-Square) is a measure of the average "size" of the elements of a data sequence.
        /// matrix = 0 1 2 3
        /// Object threshold = 0.5;
        /// matrix.viewSelection( 
        /// &nbsp;&nbsp;&nbsp;new ObjectMatrix1DProcedure() {
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;public Boolean apply(ObjectMatrix1D m) { return System.Math.Sqrt(m.aggregate(F.plus,F.square) / m.Count) &lt; threshold; }
        /// &nbsp;&nbsp;&nbsp;}
        /// );
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 
        /// </summary>
        /// <param name="condition">The condition to be matched.</param>
        /// <returns>the new view.</returns>
        public ObjectMatrix2D ViewSelection(ObjectMatrix1DProcedure condition)
        {
            List<int> matches = new List<int>();
            for (int i = 0; i < Rows; i++)
            {
                if (condition(ViewRow(i))) matches.Add(i);
            }

            matches.TrimExcess();
            return ViewSelection(matches.ToArray(), null); // take all Columns
        }

        /// <summary>
        /// Sorts the matrix Rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the given column.
        /// This sort is guaranteed to be <i>stable</i>.
        /// For further information, see {@link Cern.Colt.Matrix.ObjectAlgorithms.Sorting#sort(ObjectMatrix2D,int)}.
        /// For more advanced sorting functionality, see {@link Cern.Colt.Matrix.ObjectAlgorithms.Sorting}.
        /// </summary>
        /// <param name=""></param>
        /// <returns>a new sorted vector (matrix) view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>column &lt; 0 || column >= Columns</i>.</exception>
        public ObjectMatrix2D ViewSorted(int column)
        {
            return Cern.Colt.Matrix.ObjectAlgorithms.Sorting.mergeSort.sort(this, column);
        }

        /// <summary>
        /// Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
        /// More specifically, the view has <i>this.Rows/Rowstride</i> Rows and <i>this.Columns/Columnstride</i> Columns holding cells <i>this.Get(i*Rowstride,j*Columnstride)</i> for all <i>i = 0..Rows/Rowstride - 1, j = 0..Columns/Columnstride - 1</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        ///
        /// @param  
        /// @param  
        /// @return 
        /// @thRows	 
        /// </summary>
        /// <param name="Rowstride">the row step factor.</param>
        /// <param name="Columnstride">the column step factor.</param>
        /// <returns>a new view.</returns>
        /// <exception cref="IndexOutOfRangeException">if <i>Rowstride &lt;= 0 || Columnstride &lt;= 0</i>.</exception>
        public ObjectMatrix2D ViewStrides(int Rowstride, int Columnstride)
        {
            return (ObjectMatrix2D)(View().VStrides(Rowstride, Columnstride));
        }

        /// <summary>
        /// Applies a procedure to each cell's value.
        /// Iterates downwards from <i>[Rows-1,Columns-1]</i> to <i>[0,0]</i>,
        /// as demonstrated by this snippet:
        /// <pre>
        /// for (int row=Rows; --row >=0;) {
        ///    for (int column=Columns; --column >= 0;) {
        ///        if (!procedure(getQuick(row,column))) return false;
        ///    }
        /// }
        /// return true;
        /// </pre>
        /// Note that an implementation may use more efficient techniques, but must not use any other order.
        /// </summary>
        /// <param name="procedure">a procedure object taking as argument the current cell's valued Stops iteration if the procedure returns <i>false</i>, otherwise continuesd </param>
        /// <returns><i>false</i> if the procedure stopped before all elements where iterated over, <i>true</i> otherwised </returns>
        private Boolean XforEach(Cern.Colt.Function.ObjectProcedure<Object> procedure)
        {
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns; --column >= 0;)
                {
                    if (!procedure(this[row, column])) return false;
                }
            }
            return true;
        }
        #endregion

        #region Local Protected Methods
        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean HaveSharedCells(ObjectMatrix2D other)
        {
            if (other == null) return false;
            if (this == other) return true;
            return GetContent().HaveSharedCellsRaw(other.GetContent());
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean HaveSharedCellsRaw(ObjectMatrix2D other)
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
        protected ObjectMatrix2D View()
        {
            return (ObjectMatrix2D)Clone();
        }

        #endregion

        #region Local Private Methods

        #endregion


    }
}
