using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix
{
    using System;
    using System.Collections.Generic;
    using DoubleAlgorithms;
    using Function;
    using Implementation;
    using LinearAlgebra;

    /// <summary>
    /// Abstract base class for 3-d matrices holding <i>double</i> elements.
    /// </summary>
    public abstract class DoubleMatrix3D : AbstractMatrix3D
    {
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected DoubleMatrix3D() { }

        /// <summary>
///        Applies a function to each cell and aggregates the results.
///        Returns a value <i>v</i> such that<i> v==a(size())</i> where<i> a(i) == aggr(a(i-1), f(get(slice, row, column)) )</i> and terminators are<i> a(1) == f(get(0,0,0)), a(0)==Double.NaN</i>.
/// <p>
/// <b>Example:</b>
/// <pre>
/// cern.jet.math.Functions F = cern.jet.math.Functions.Functions;
/// 2 x 2 x 2 matrix
/// 0 1
/// 2 3
/// 4 5
/// 6 7
//// Sum( x[slice,row,col]*x[slice,row,col] ) 
/// matrix.aggregate(F.plus,F.square);
/// --> 140
/// </pre>
/// For further examples, see the<a href= "package-summary.html#FunctionObjects" > package doc</a>.
/// @param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell value.
/// @param f a function transforming the current cell value.
/// @return the aggregated measure.
/// @see cern.jet.math.Functions
/// </summary>
public double aggregate(Cern.Colt.Function.DoubleDoubleFunction aggr, Cern.Colt.Function.DoubleFunction f)
        {
            if (size() == 0) return Double.NaN;
            double a = f(getQuick(Slices - 1, Rows - 1, Columns - 1));
            int d = 1; // last cell already done
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns - d; --column >= 0;)
                    {
                        a = aggr(a, f(getQuick(slice, row, column)));
                    }
                    d = 0;
                }
            }
            return a;
        }
//        /// <summary>
//        Applies a function to each corresponding cell of two matrices and aggregates the results.
//        Returns a value <i>v</i> such that<i> v==a(size())</i> where<i> a(i) == aggr(a(i-1), f(get(slice, row, column),other.Get(slice,row,column)) )</i> and terminators are<i> a(1) == f(get(0,0,0),other.Get(0,0,0)), a(0)==Double.NaN</i>.
//<p>
//<b>Example:</b>
//<pre>
//cern.jet.math.Functions F = cern.jet.math.Functions.Functions;
//        x = 2 x 2 x 2 matrix
//0 1
//2 3

//4 5
//6 7

//y = 2 x 2 x 2 matrix
//0 1
//2 3

//4 5
//6 7
// Sum( x[slice,row,col]/// y[slice,row,col] ) 
//x.aggregate(y, F.plus, F.mult);
//--> 140

//// Sum( (x[slice,row,col] + y[slice,row,col])^2 )
//x.aggregate(y, F.plus, F.chain(F.square,F.plus));
//--> 560
//</pre>
//For further examples, see the<a href= "package-summary.html#FunctionObjects" > package doc</a>.

//@param aggr an aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
//@param f a function transforming the current cell values.
//@return the aggregated measure.
//@thRows ArgumentException if <i>Slices != other.Slices || Rows != other.Rows || Columns != other.Columns</i>
//@see cern.jet.math.Functions
/// </summary>
public double aggregate(DoubleMatrix3D other, Cern.Colt.Function.DoubleDoubleFunction aggr, Cern.Colt.Function.DoubleDoubleFunction f)
        {
            checkShape(other);
            if (size() == 0) return Double.NaN;
            double a = f(getQuick(Slices - 1, Rows - 1, Columns - 1), other.getQuick(Slices - 1, Rows - 1, Columns - 1));
            int d = 1; // last cell already done
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns - d; --column >= 0;)
                    {
                        a = aggr(a, f(getQuick(slice, row, column), other.getQuick(slice, row, column)));
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
        public DoubleMatrix3D assign(double[][][] values)
        {
            if (values.Length != Slices) throw new ArgumentException("Must have same number of Slices: Slices=" + values.Length + "Slices=" + Slices);
            for (int slice = Slices; --slice >= 0;)
            {
                double[][] currentSlice = values[slice];
                if (currentSlice.Length != Rows) throw new ArgumentException("Must have same number of Rows in every slice: Rows=" + currentSlice.Length + "Rows=" + Rows);
                for (int row = Rows; --row >= 0;)
                {
                    double[] currentRow = currentSlice[row];
                    if (currentRow.Length != Columns) throw new ArgumentException("Must have same number of Columns in every row: Columns=" + currentRow.Length + "Columns=" + Columns);
                    for (int column = Columns; --column >= 0;)
                    {
                        setQuick(slice, row, column, currentRow[column]);
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
        public DoubleMatrix3D assign(double value)
        {
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        setQuick(slice, row, column, value);
                    }
                }
            }
            return this;
        }
        /// <summary>
//        Assigns the result of a function to each cell; <i>x[slice, row, col] = function(x[slice, row, col])</i>.
//<p>
//<b>Example:</b>
//<pre>
//matrix = 1 x 2 x 2 matrix
//0.5 1.5      
//2.5 3.5

//// change each cell to its sine
//matrix.assign(cern.jet.math.Functions.sin);
//-->
//1 x 2 x 2 matrix
//0.479426  0.997495 
//0.598472 -0.350783
//</pre>
//For further examples, see the<a href= "package-summary.html#FunctionObjects" > package doc</a>.

//@param function a function object taking as argument the current cell's value.
//@return<i>this</i> (for convenience only).
//@see cern.jet.math.Functions
/// </summary>
public DoubleMatrix3D assign(Cern.Colt.Function.DoubleFunction function)
        {
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        setQuick(slice, row, column, function(getQuick(slice, row, column)));
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
        public DoubleMatrix3D assign(DoubleMatrix3D other)
        {
            if (other == this) return this;
            checkShape(other);
            if (haveSharedCells(other)) other = other.copy();

            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        setQuick(slice, row, column, other.getQuick(slice, row, column));
                    }
                }
            }
            return this;
        }
        /// <summary>
//        Assigns the result of a function to each cell; <i>x[row, col] = function(x[row, col], y[row, col])</i>.
//<p>
//<b>Example:</b>
//<pre>
//// assign x[row,col] = x[row,col]<sup>y[row,col]</sup>
//m1 = 1 x 2 x 2 matrix 
//0 1 
//2 3

//m2 = 1 x 2 x 2 matrix 
//0 2 
//4 6

//m1.assign(m2, cern.jet.math.Functions.pow);
//-->
//m1 == 1 x 2 x 2 matrix
// 1   1 
//16 729
//</pre>
//For further examples, see the<a href= "package-summary.html#FunctionObjects" > package doc</a>.

//@param y the secondary matrix to operate on.
//@param function a function object taking as first argument the current cell's value of <i>this</i>,
//and as second argument the current cell's value of <i>y</i>,
//@return<i>this</i> (for convenience only).
//@thRows ArgumentException if <i>Slices != other.Slices || Rows != other.Rows || Columns != other.Columns</i>
//@see cern.jet.math.Functions
/// </summary>
public DoubleMatrix3D assign(DoubleMatrix3D y, Cern.Colt.Function.DoubleDoubleFunction function)
        {
            checkShape(y);
            for (int slice = Slices; --slice >= 0;)
            {
                for (int row = Rows; --row >= 0;)
                {
                    for (int column = Columns; --column >= 0;)
                    {
                        setQuick(slice, row, column, function(getQuick(slice, row, column), y.getQuick(slice, row, column)));
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
                        if (getQuick(slice, row, column) != 0) cardinality++;
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
        public DoubleMatrix3D copy()
        {
            return like().assign(this);
        }
        /// <summary>
        /// Returns whether all cells are equal to the given value.
        ///
        /// @param     value the value to test against.
        /// @return    <i>true</i> if all cells are equal to the given value, <i>false</i> otherwise.
        /// </summary>
        public Boolean Equals(double value)
        {
            return Cern.Colt.Matrix.LinearAlgebra.Property.DEFAULT.Equals(this, value);
        }
        /// <summary>
        /// Compares this object against the specified object.
        /// The result is <code>true</code> if and only if the argument is 
        /// not <code>null</code> and is at least a <code>DoubleMatrix3D</code> object
        /// that has the same number of Slices, Rows and Columns as the receiver and 
        /// has exactly the same values at the same coordinates.
        /// @param   obj   the object to compare with.
        /// @return  <code>true</code> if the objects are the same;
        ///          <code>false</code> otherwise.
        /// </summary>
        public override Boolean Equals(Object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (!(obj is DoubleMatrix3D)) return false;

            return Cern.Colt.Matrix.LinearAlgebra.Property.DEFAULT.Equals(this, (DoubleMatrix3D)obj);
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
        public double get(int slice, int row, int column)
        {
            if (slice < 0 || slice >= Slices || row < 0 || row >= Rows || column < 0 || column >= Columns) throw new IndexOutOfRangeException("slice:" + slice + ", row:" + row + ", column:" + column);
            return getQuick(slice, row, column);
        }
        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        protected DoubleMatrix3D getContent()
        {
            return this;
        }
        /// <summary>
//        Fills the coordinates and values of cells having non-zero values into the specified lists.
//        Fills into the lists, starting at index 0.
//After this call returns the specified lists all have a new size, the number of non-zero values.
//<p>
//In general, fill order is <i>unspecified</i>.
//This implementation fill like: <i>for (slice = 0.Slices - 1) for (row = 0.Rows-1) for (column = 0.colums-1) do .d </i>.
//However, subclasses are free to us any other order, even an order that may change over time as cell values are changed.
//(Of course, result lists indexes are guaranteed to correspond to the same cell).
//For an example, see {@link DoubleMatrix2D#getNonZeros(List<int>,List<int>,List<Double>)}.

//@param sliceList the list to be filled with slice indexes, can have any size.
//@param rowList the list to be filled with row indexes, can have any size.
//@param columnList the list to be filled with column indexes, can have any size.
//@param valueList the list to be filled with values, can have any size.
/// </summary>
public void getNonZeros(List<int> sliceList, List<int> rowList, List<int> columnList, List<Double> valueList)
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
                        double value = getQuick(slice, row, column);
                        if (value != 0)
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
        /// Returns the matrix cell value at coordinate <i>[slice,row,column]</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices || row&lt;0 || row&gt;=Rows || column&lt;0 || column&gt;=column()</i>.
        ///
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @return    the value at the specified coordinate.
        /// </summary>
        public abstract double getQuick(int slice, int row, int column);
        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean haveSharedCells(DoubleMatrix3D other)
        {
            if (other == null) return false;
            if (this == other) return true;
            return getContent().haveSharedCellsRaw(other.getContent());
        }
        /// <summary>
        /// Returns <i>true</i> if both matrices share at least one identical cell.
        /// </summary>
        protected Boolean haveSharedCellsRaw(DoubleMatrix3D other)
        {
            return false;
        }
        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same number of Slices, Rows and Columns.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix3D</i> the new matrix must also be of type <i>DenseDoubleMatrix3D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix3D</i> the new matrix must also be of type <i>SparseDoubleMatrix3D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        ///
        /// @return  a new empty matrix of the same dynamic type.
        /// </summary>
        public DoubleMatrix3D like()
        {
            return like(Slices, Rows, Columns);
        }
        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of Slices, Rows and Columns.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix3D</i> the new matrix must also be of type <i>DenseDoubleMatrix3D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix3D</i> the new matrix must also be of type <i>SparseDoubleMatrix3D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        ///
        /// @param Slices the number of Slices the matrix shall have.
        /// @param Rows the number of Rows the matrix shall have.
        /// @param Columns the number of Columns the matrix shall have.
        /// @return  a new empty matrix of the same dynamic type.
        /// </summary>
        public abstract DoubleMatrix3D like(int Slices, int Rows, int Columns);
        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix3D</i> the new matrix must also be of type <i>DenseDoubleMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix3D</i> the new matrix must also be of type <i>SparseDoubleMatrix2D</i>, etc.
        ///
        /// @param Rows the number of Rows the matrix shall have.
        /// @param Columns the number of Columns the matrix shall have.
        /// @param rowZero the position of the first element.
        /// @param columnZero the position of the first element.
        /// @param Rowstride the number of elements between two Rows, i.ed <i>index(i+1,j)-index(i,j)</i>.
        /// @param Columnstride the number of elements between two Columns, i.ed <i>index(i,j+1)-index(i,j)</i>.
        /// @return  a new matrix of the corresponding dynamic type.
        /// </summary>
        protected abstract DoubleMatrix2D like2D(int Rows, int Columns, int rowZero, int columnZero, int Rowstride, int Columnstride);
        /// <summary>
        /// Sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        ///
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @param    value the value to be filled into the specified cell.
        /// @thRows	IndexOutOfRangeException if <i>row&lt;0 || row&gt;=Rows || slice&lt;0 || slice&gt;=Slices || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        public void set(int slice, int row, int column, double value)
        {
            if (slice < 0 || slice >= Slices || row < 0 || row >= Rows || column < 0 || column >= Columns) throw new IndexOutOfRangeException("slice:" + slice + ", row:" + row + ", column:" + column);
            setQuick(slice, row, column, value);
        }
        /// <summary>
        /// Sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices || row&lt;0 || row&gt;=Rows || column&lt;0 || column&gt;=column()</i>.
        ///
        /// @param     slice   the index of the slice-coordinate.
        /// @param     row   the index of the row-coordinate.
        /// @param     column   the index of the column-coordinate.
        /// @param    value the value to be filled into the specified cell.
        /// </summary>
        public abstract void setQuick(int slice, int row, int column, double value);
        /// <summary>
        /// Constructs and returns a 2-dimensional array containing the cell values.
        /// The returned array <i>values</i> has the form <i>values[slice][row][column]</i>
        /// and has the same number of Slices, Rows and Columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        ///
        /// @return an array filled with the values of the cells.
        /// </summary>
        public double[][][] ToArray()
        {
            double[][][] values = new double[Slices][][];
            values = values.Initialize(Slices, Rows, Columns);
            for (int slice = Slices; --slice >= 0;)
            {
                double[][] currentSlice = values[slice];
                for (int row = Rows; --row >= 0;)
                {
                    double[] currentRow = currentSlice[row];
                    for (int column = Columns; --column >= 0;)
                    {
                        currentRow[column] = getQuick(slice, row, column);
                    }
                }
            }
            return values;
        }
        /// <summary>
        /// Returns a string representation using default formatting.
        /// @see Cern.Colt.Matrix.DoubleAlgorithms.Formatter
        /// </summary>
        public override String ToString()
        {
            return new Cern.Colt.Matrix.DoubleAlgorithms.Formatter().ToString(this);
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
        protected DoubleMatrix3D view()
        {
            return (DoubleMatrix3D)Clone();
        }
        /// <summary>
//        Constructs and returns a new 2-dimensional<i> slice view</i> representing the Slices and Rows of the given column.
//        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
//        <p>
//To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(.d)</i>), then apply this method to the sub-range view.
//To obtain 1-dimensional views, apply this method, then apply another slice view (methods<i> viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
//To obtain 1-dimensional views on subranges, apply both steps.

//@param column the index of the column to fix.
//@return a new 2-dimensional slice view.
//@thRows IndexOutOfRangeException if <i>column< 0 || column >= Columns</i>.
//@see #viewSlice(int)
//@see #viewRow(int)
/// </summary>
public DoubleMatrix2D viewColumn(int column)
        {
            checkColumn(column);
            int sliceRows = this.Slices;
            int sliceColumns = this.Rows;

            //int sliceOffset = index(0,0,column);
            int sliceRowZero = sliceZero;
            int sliceColumnZero = rowZero + columnOffset(columnRank(column));

            int sliceRowstride = this.Slicestride;
            int sliceColumnstride = this.Rowstride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowstride, sliceColumnstride);
        }
        /// <summary>
//        Constructs and returns a new <i>flip view</i> along the column axis.
//What used to be column<i>0</i> is now column <i>Columns-1</i>, .d, what used to be column<i> Columns-1</i> is now column<i>0</i>.
//The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

//@return a new flip view.
//@see #viewSliceFlip()
//@see #viewRowFlip()
/// </summary>
public DoubleMatrix3D viewColumnFlip()
        {
            return (DoubleMatrix3D)(view().vColumnFlip());
        }
        /// <summary>
//        Constructs and returns a new <i>dice view</i>; Swaps dimensions(axes); Example: 3 x 4 x 5 matrix --> 4 x 3 x 5 matrix.
//       The view has dimensions exchanged; what used to be one axis is now another, in all desired permutations.
//       The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

//       @param axis0 the axis that shall become axis 0 (legal values 0.2).
//@param axis1 the axis that shall become axis 1 (legal values 0.2).
//@param axis2 the axis that shall become axis 2 (legal values 0.2).
//@return a new dice view.
//@thRows ArgumentException if some of the parameters are equal or not in range 0.2.
/// </summary>
public DoubleMatrix3D viewDice(int axis0, int axis1, int axis2)
        {
            return (DoubleMatrix3D)(view().vDice(axis0, axis1, axis2));
        }
        /// <summary>
//        Constructs and returns a new <i>sub-range view</i> that is a<i> depth x height x width</i> sub matrix starting at <i>[slice, row, column]</i>;
//        Equivalent to<i>view().part(slice, row, column, depth, height, width)</i>; Provided for convenience only.
//            The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

//            @param slice   The index of the slice-coordinate.
//            @param row   The index of the row-coordinate.
//            @param column   The index of the column-coordinate.
//            @param depth   The depth of the box.
//            @param height   The height of the box.
//            @param width   The width of the box.
//            @thRows IndexOutOfRangeException if <i>slice<0 || depth<0 || slice+depth>Slices || row<0 || height<0 || row+height>Rows || column<0 || width<0 || column+width>Columns</i>
//@return the new view.
/// </summary>
public DoubleMatrix3D viewPart(int slice, int row, int column, int depth, int height, int width)
        {
            return (DoubleMatrix3D)(view().vPart(slice, row, column, depth, height, width));
        }
        /// <summary>
//        Constructs and returns a new 2-dimensional<i> slice view</i> representing the Slices and Columns of the given row.
//        The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
//        <p>
//To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(.d)</i>), then apply this method to the sub-range view.
//To obtain 1-dimensional views, apply this method, then apply another slice view (methods<i> viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
//To obtain 1-dimensional views on subranges, apply both steps.

//@param row the index of the row to fix.
//@return a new 2-dimensional slice view.
//@thRows IndexOutOfRangeException if <i>row< 0 || row >= row()</i>.
//@see #viewSlice(int)
//@see #viewColumn(int)
/// </summary>
public DoubleMatrix2D viewRow(int row)
        {
            checkRow(row);
            int sliceRows = this.Slices;
            int sliceColumns = this.Columns;

            //int sliceOffset = index(0,row,0);
            int sliceRowZero = sliceZero;
            int sliceColumnZero = columnZero + rowOffset(rowRank(row));

            int sliceRowstride = this.Slicestride;
            int sliceColumnstride = this.Columnstride;
            return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowstride, sliceColumnstride);
        }
        /// <summary>
//        Constructs and returns a new <i>flip view</i> along the row axis.
//What used to be row<i>0</i> is now row <i>Rows-1</i>, .d, what used to be row<i> Rows-1</i> is now row<i>0</i>.
//The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

//@return a new flip view.
//@see #viewSliceFlip()
//@see #viewColumnFlip()
/// </summary>
public DoubleMatrix3D viewRowFlip()
        {
            return (DoubleMatrix3D)(view().vRowFlip());
        }
        /// <summary>
//        Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
//There holds <i>view.Slices == sliceIndexes.Length, view.Rows == rowIndexes.Length, view.Columns == columnIndexes.Length</i> and
//<i>view.Get(k, i, j) == this.Get(sliceIndexes[k], rowIndexes[i], columnIndexes[j])</i>.
//Indexes can occur multiple times and can be in arbitrary order.
//For an example see {
//            @link DoubleMatrix2D#viewSelection(int[],int[])}.
//< p >
//Note that modifying the index arguments after this call has returned has no effect on the view.
//The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

//@param  sliceIndexes The Slices of the cells that shall be visible in the new view.To indicate that < i > all </ i > Slices shall be visible, simply set this parameter to < tt > null </ tt >.
//   @param  rowIndexes The Rows of the cells that shall be visible in the new view.To indicate that < i > all </ i > Rows shall be visible, simply set this parameter to < tt > null </ tt >.
//      @param  columnIndexes The Columns of the cells that shall be visible in the new view.To indicate that < i > all </ i > Columns shall be visible, simply set this parameter to < tt > null </ tt >.
//         @return the new view.
//         @thRows IndexOutOfRangeException if < tt > !(0 <= sliceIndexes[i] < Slices) </ tt > for any<i> i = 0.sliceIndexes.Length() - 1 </ tt >.
//               @thRows IndexOutOfRangeException if < tt > !(0 <= rowIndexes[i] < Rows) </ tt > for any<i> i = 0.rowIndexes.Length() - 1 </ tt >.
//                     @thRows IndexOutOfRangeException if < tt > !(0 <= columnIndexes[i] < Columns) </ tt > for any<i> i = 0.columnIndexes.Length() - 1 </ tt >.
                           /// </summary>
public DoubleMatrix3D viewSelection(int[] sliceIndexes, int[] rowIndexes, int[] columnIndexes)
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

            checkSliceIndexes(sliceIndexes);
            checkRowIndexes(rowIndexes);
            checkColumnIndexes(columnIndexes);

            int[] sliceOffsets = new int[sliceIndexes.Length];
            int[] rowOffsets = new int[rowIndexes.Length];
            int[] columnOffsets = new int[columnIndexes.Length];

            for (int i = sliceIndexes.Length; --i >= 0;)
            {
                sliceOffsets[i] = sliceOffset(sliceRank(sliceIndexes[i]));
            }
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowOffsets[i] = rowOffset(rowRank(rowIndexes[i]));
            }
            for (int i = columnIndexes.Length; --i >= 0;)
            {
                columnOffsets[i] = columnOffset(columnRank(columnIndexes[i]));
            }

            return viewSelectionLike(sliceOffsets, rowOffsets, columnOffsets);
        }
        /// <summary>
//        Constructs and returns a new <i>selection view</i> that is a matrix holding all <b>Slices</b> matching the given condition.
//        Applies the condition to each slice and takes only those where<i> condition(viewSlice(i))</i> yields<i>true</i>.
//To match Rows or Columns, use a dice view.
//<p>
//<b>Example:</b>
//<br>
//<pre>
//// extract and view all Slices which have an aggregate sum > 1000
//matrix.viewSelection( 
//&nbsp;&nbsp;&nbsp;new DoubleMatrix2DProcedure()
//        {
//            &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;public Boolean apply(DoubleMatrix2D m) { return m.zSum > 1000; }
//&nbsp;&nbsp;&nbsp;}
//);
//</pre>
//For further examples, see the<a href="package-summary.html#FunctionObjects">package doc</a>.
//The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

//@param condition The condition to be matched.
//@return the new view.
/// </summary>
public DoubleMatrix3D viewSelection(DoubleMatrix2DProcedure condition)
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
    protected abstract DoubleMatrix3D viewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets);
    /// <summary>
//    Constructs and returns a new 2-dimensional<i> slice view</i> representing the Rows and Columns of the given slice.
//    The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
//    <p>
//To obtain a slice view on subranges, construct a sub-ranging view (<i>view().part(.d)</i>), then apply this method to the sub-range view.
//To obtain 1-dimensional views, apply this method, then apply another slice view (methods<i> viewColumn</i>, <i>viewRow</i>) on the intermediate 2-dimensional view.
//To obtain 1-dimensional views on subranges, apply both steps.

//@param slice the index of the slice to fix.
//@return a new 2-dimensional slice view.
//@thRows IndexOutOfRangeException if <i>slice< 0 || slice >= Slices</i>.
//@see #viewRow(int)
//@see #viewColumn(int)
/// </summary>
public DoubleMatrix2D viewSlice(int slice)
    {
        checkSlice(slice);
        int sliceRows = this.Rows;
        int sliceColumns = this.Columns;

        //int sliceOffset = index(slice,0,0);
        int sliceRowZero = rowZero;
        int sliceColumnZero = columnZero + sliceOffset(sliceRank(slice));

        int sliceRowstride = this.Rowstride;
        int sliceColumnstride = this.Columnstride;
        return like2D(sliceRows, sliceColumns, sliceRowZero, sliceColumnZero, sliceRowstride, sliceColumnstride);
    }
        /// <summary>
        //    Constructs and returns a new <i>flip view</i> along the slice axis.
        //What used to be slice<i>0</i> is now slice <i>Slices-1</i>, .d, what used to be slice<i> Slices-1</i> is now slice<i>0</i>.
        //The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.

        //@return a new flip view.
        //@see #viewRowFlip()
        //@see #viewColumnFlip()
        ///// </summary>
        public DoubleMatrix3D viewSliceFlip()
        {
            return (DoubleMatrix3D)(view().vSliceFlip());
        }
        //    /// <summary>
        //    Sorts the matrix Slices into ascending order, according to the<i> natural ordering</i> of the matrix values in the given<i>[row, column]</i> position.
        //    This sort is guaranteed to be<i> stable</i>.
        //For further information, see {
        //        @link Cern.Colt.Matrix.DoubleAlgorithms.Sorting#sort(DoubleMatrix3D,int,int)}.
        //For more advanced sorting functionality, see { @link Cern.Colt.Matrix.DoubleAlgorithms.Sorting}.
        //@return a new sorted vector(matrix) view.
        //@thRows IndexOutOfRangeException if < tt > row < 0 || row >= Rows || column < 0 || column >= Columns </ tt >.
        /// </summary>
        public DoubleMatrix3D viewSorted(int row, int column)
    {
        return Cern.Colt.Matrix.DoubleAlgorithms.Sorting.MergeSort.Sort(this, row, column);
    }
    /// <summary>
//    Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
//More specifically, the view has<i>this.Slices/Slicestride</i> Slices and<i> this.Rows/Rowstride</i> Rows and <i>this.Columns/Columnstride</i> Columns
//holding cells <i>this.Get(k* Slicestride, i* Rowstride, j* Columnstride)</ tt > for all<i> k = 0.Slices / Slicestride - 1, i = 0.Rows / Rowstride - 1, j = 0.Columns / Columnstride - 1 </ tt >.
//      The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice - versa.

//      @param Slicestride the slice step factor.
//      @param Rowstride the row step factor.
//      @param Columnstride the column step factor.
//      @return a new view.
//      @thRows IndexOutOfRangeException if < tt > Slicestride <= 0 || Rowstride <= 0 || Columnstride <= 0 </ tt >.
      /// </summary>
public DoubleMatrix3D viewStrides(int Slicestride, int Rowstride, int Columnstride)
    {
        return (DoubleMatrix3D)(view().vStrides(Slicestride, Rowstride, Columnstride));
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
    private Boolean xforEach(Cern.Colt.Function.DoubleProcedure procedure)
    {
        for (int slice = Slices; --slice >= 0;)
        {
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns; --column >= 0;)
                {
                    if (!procedure(getQuick(slice, row, column))) return false;
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
/// <summary>
//27 neighbor stencil transformation.For efficient finite difference operations.
//Applies a function to a moving<i>3 x 3 x 3</i> window.
//Does nothing if <i>Rows < 3 || Columns < 3 || Slices < 3</i>.
//<pre>
//B[k, i, j] = function(
//&nbsp;&nbsp;&nbsp;A[k - 1, i - 1, j - 1], A[k - 1, i - 1, j], A[k - 1, i - 1, j + 1],
//&nbsp;&nbsp;&nbsp;A[k - 1, i, j - 1], A[k - 1, i, j], A[k - 1, i, j + 1],
//&nbsp;&nbsp;&nbsp;A[k - 1, i + 1, j - 1], A[k - 1, i + 1, j], A[k - 1, i + 1, j + 1],

//&nbsp;&nbsp;&nbsp;A[k, i - 1, j - 1], A[k, i - 1, j], A[k, i - 1, j + 1],
//&nbsp;&nbsp;&nbsp;A[k, i, j - 1], A[k, i, j], A[k, i, j + 1],
//&nbsp;&nbsp;&nbsp;A[k, i + 1, j - 1], A[k, i + 1, j], A[k, i + 1, j + 1],

//&nbsp;&nbsp;&nbsp;A[k + 1, i - 1, j - 1], A[k + 1, i - 1, j], A[k + 1, i - 1, j + 1],
//&nbsp;&nbsp;&nbsp;A[k + 1, i, j - 1], A[k + 1, i, j], A[k + 1, i, j + 1],
//&nbsp;&nbsp;&nbsp;A[k + 1, i + 1, j - 1], A[k + 1, i + 1, j], A[k + 1, i + 1, j + 1]
//&nbsp;&nbsp;&nbsp;)

//x x x - &nbsp;&nbsp;&nbsp; - x x x &nbsp;&nbsp;&nbsp; - - - - 
//x o x - &nbsp;&nbsp;&nbsp; - x o x &nbsp;&nbsp;&nbsp; - - - - 
//x x x - &nbsp;&nbsp;&nbsp; - x x x.d - x x x 
//- - - - &nbsp;&nbsp;&nbsp; - - - - &nbsp;&nbsp;&nbsp; - x o x 
//- - - - &nbsp;&nbsp;&nbsp; - - - - &nbsp;&nbsp;&nbsp; - x x x
//</pre>
//Make sure that cells of<i>this</i> and<i> B</i> do not overlap.
//In case of overlapping views, behaviour is unspecified.
//</pre>
//<p>
//<b>Example:</b>
//<pre>
//double alpha = 0.25;
//    double beta = 0.75;

//    Cern.Colt.Function.Double27Function f = new Cern.Colt.Function.Double27Function() {
//&nbsp;&nbsp;&nbsp;public double apply(
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a000, double a001, double a002,
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a010, double a011, double a012,
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a020, double a021, double a022,

//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a100, double a101, double a102,
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a110, double a111, double a112,
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a120, double a121, double a122,

//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a200, double a201, double a202,
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a210, double a211, double a212,
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a220, double a221, double a222) {
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return beta* a111 + alpha*(a000 + .d + a222);
//&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
//};
//A.zAssign27Neighbors(B,f);
//</pre>
	
//@param B the matrix to hold the results.
//@param function the function to be applied to the 27 cells.
//@thRows NullReferenceException if <i>function==null</i>.
//@thRows ArgumentException if <i>Rows != B.Rows || Columns != B.Columns || Slices != B.Slices </i>.
/// </summary>
public void zAssign27Neighbors(DoubleMatrix3D B, Cern.Colt.Function.Double27Function function)
{
    if (function == null) throw new NullReferenceException("function must not be null.");
    checkShape(B);
    if (Rows < 3 || Columns < 3 || Slices < 3) return; // nothing to do
    int r = Rows - 1;
    int c = Columns - 1;
    double a000, a001, a002;
    double a010, a011, a012;
    double a020, a021, a022;

    double a100, a101, a102;
    double a110, a111, a112;
    double a120, a121, a122;

    double a200, a201, a202;
    double a210, a211, a212;
    double a220, a221, a222;

    for (int k = 1; k < Slices - 1; k++)
    {
        for (int i = 1; i < r; i++)
        {
            a000 = getQuick(k - 1, i - 1, 0); a001 = getQuick(k - 1, i - 1, 1);
            a010 = getQuick(k - 1, i, 0); a011 = getQuick(k - 1, i, 1);
            a020 = getQuick(k - 1, i + 1, 0); a021 = getQuick(k - 1, i + 1, 1);

            a100 = getQuick(k - 1, i - 1, 0); a101 = getQuick(k, i - 1, 1);
            a110 = getQuick(k, i, 0); a111 = getQuick(k, i, 1);
            a120 = getQuick(k, i + 1, 0); a121 = getQuick(k, i + 1, 1);

            a200 = getQuick(k + 1, i - 1, 0); a201 = getQuick(k + 1, i - 1, 1);
            a210 = getQuick(k + 1, i, 0); a211 = getQuick(k + 1, i, 1);
            a220 = getQuick(k + 1, i + 1, 0); a221 = getQuick(k + 1, i + 1, 1);

            for (int j = 1; j < c; j++)
            {
                // in each step 18 cells can be remembered in registers - they don't need to be reread from slow memory
                // in each step 9 instead of 27 cells need to be read from memory.
                a002 = getQuick(k - 1, i - 1, j + 1);
                a012 = getQuick(k - 1, i, j + 1);
                a022 = getQuick(k - 1, i + 1, j + 1);

                a102 = getQuick(k, i - 1, j + 1);
                a112 = getQuick(k, i, j + 1);
                a122 = getQuick(k, i + 1, j + 1);

                a202 = getQuick(k + 1, i - 1, j + 1);
                a212 = getQuick(k + 1, i, j + 1);
                a222 = getQuick(k + 1, i + 1, j + 1);

                B.setQuick(k, i, j, function(
                    a000, a001, a002,
                    a010, a011, a012,
                    a020, a021, a022,

                    a100, a101, a102,
                    a110, a111, a112,
                    a120, a121, a122,

                    a200, a201, a202,
                    a210, a211, a212,
                    a220, a221, a222));

                a000 = a001; a001 = a002;
                a010 = a011; a011 = a012;
                a020 = a021; a021 = a022;

                a100 = a101; a101 = a102;
                a110 = a111; a111 = a112;
                a120 = a121; a121 = a122;

                a200 = a201; a201 = a202;
                a210 = a211; a211 = a212;
                a220 = a221; a221 = a222;
            }
        }
    }
}
/// <summary>
/// Returns the sum of all cells; <i>Sum( x[i,j,k] )</i>.
/// @return the sum.
/// </summary>
public double zSum()
{
    if (size() == 0) return 0;
    return aggregate(Cern.Jet.Math.Functions.DoubleDoubleFunctions.Plus, Cern.Jet.Math.Functions.DoubleFunctions.Identity);
}

    }
}
