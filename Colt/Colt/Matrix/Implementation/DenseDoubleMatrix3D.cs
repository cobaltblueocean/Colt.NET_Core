using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Dense 3-d matrix holding<i> double</i> elements.
    /// First see the<a href="package-summary.html"> package summary</a> and javadoc<a href="package-tree.html"> tree view</a> to get the broad picture.
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Internally holds one single contigous one-dimensional array, addressed in (in decreasing order of significance): slice major, row major, column major.
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory[bytes] = 8 * Slices() * Rows() * Columns() </ tt >.
    /// Thus, a 100*100*100 matrix uses 8 MB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// <i>O(1)</i> (i.e.constant time) for the basic operations
    /// <i> get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>,
    /// <p>
    /// Applications demanding utmost speed can exploit knowledge about the internal addressing.
    /// Setting/getting values in a loop slice-by-slice, row-by-row, column-by-column is quicker than, for example, column-by-column, row-by-row, slice-by-slice.
    /// Thus
    /// <pre>
    ///    for (int slice=0; slice<Slices; slice++) {
    /// 	  for (int row=0; row<Rows; row++) {
    /// 	     for (int column=0; column<Columns; column++) {
    /// 			matrix.setQuick(slice, row, column, someValue);
    /// 		 }		    
    /// 	  }
    ///    }
    /// </pre>
    /// is quicker than
    /// <pre>
    ///    for (int column= 0; column<Columns; column++) {
    /// 	  for (int row=0; row<Rows; row++) {
    /// 	     for (int slice=0; slice<Slices; slice++) {
    /// 			matrix.setQuick(slice, row, column, someValue);
    /// 		 }
    /// 	  }
    ///    }
    /// </pre>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class DenseDoubleMatrix3D : DoubleMatrix3D
    {
        #region Local Variables
        /// <summary>
        /// The elements of this matrix.
        /// elements are stored in slice major, then row major, then column major, in order of significance, i.e.
        /// index==slice*Slicestride+ row*Rowstride + column*Columnstride
        /// i.ed {slice0 row0..m}, {slice1 row0..m}, ..d, {sliceN row0..m}
        /// with each row storead as 
        /// {row0 column0..m}, {row1 column0..m}, ..d, {rown column0..m}
        /// </summary>
        public double[] Elements { get; set; }

        #endregion

        #region Property

        public override double this[int slice, int row, int column]
        {
            get { return Elements[SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride]; }
            set { Elements[SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride] = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of Rows in in every slice and exactly the same number of Columns in in every row.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= slice &lt; values.Length: values[slice].Length != values[slice-1].Length</i>.</exception>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= row &lt; values.GetLength(1): values[slice][row].Length != values[slice][row-1].Length</i>.</exception>
        public DenseDoubleMatrix3D(double[][][] values) : this(values.Length, (values.Length == 0 ? 0 : values.GetLength(1)), (values.Length == 0 ? 0 : values.GetLength(1) == 0 ? 0 : values[0].GetLength(1)))
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of Slices, Rows and Columns.
        /// All entries are initially <i>0</i>.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>(double)Slices*Columns*Rows > int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>Slices &ly; 0 || Rows<0 || Columns<0</i>.</exception>
        public DenseDoubleMatrix3D(int Slices, int Rows, int Columns)
        {
            Setup(Slices, Rows, Columns);
            this.Elements = new double[Slices * Rows * Columns];
        }

        /// <summary>
        /// Constructs a view with the given parameters.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="sliceZero">the position of the first element.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="Slicestride">the number of elements between two Slices, i.ed <i>index(k+1,i,j)-index(k,i,j)</i>.</param>
        /// <param name="Rowstride">the number of elements between two Rows, i.ed <i>index(k,i+1,j)-index(k,i,j)</i>.</param>
        /// <param name="columnnStride">the number of elements between two Columns, i.ed <i>index(k,i,j+1)-index(k,i,j)</i>.</param>
        /// <exception cref="ArgumentException">if <i>(double)Slices*Columns*Rows > int.MaxValue</i>.</exception>
        /// <exception cref="ArgumentException">if <i>Slices &lt; 0 || Rows<0 || Columns<0</i>.</exception>
        protected DenseDoubleMatrix3D(int Slices, int Rows, int Columns, double[] elements, int sliceZero, int rowZero, int columnZero, int Slicestride, int Rowstride, int Columnstride)
        {
            Setup(Slices, Rows, Columns, sliceZero, rowZero, columnZero, Slicestride, Rowstride, Columnstride);
            this.Elements = elements;
            this.IsView = true;
        }

        #endregion

        #region Implement Methods

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>[slice,row,column]</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices() || row&lt;0 || row&gt;=Rows() || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        [Obsolete("GetQuick(int slice, int row, int column) is deprecated, please use indexer instead.")]
        public double GetQuick(int slice, int row, int column)
        {
            //if (debug) if (slice<0 || slice>=Slices || row<0 || row>=Rows || column<0 || column>=Columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
            //return elements[index(slice,row,column)];
            //manually inlined:
            return Elements[SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of Slices, Rows and Columns.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix3D</i> the new matrix must also be of type <i>DenseDoubleMatrix3D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix3D</i> the new matrix must also be of type <i>SparseDoubleMatrix3D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="Slices">the number of Slices the matrix shall have.</param>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override DoubleMatrix3D Like(int Slices, int Rows, int Columns)
        {
            return new DenseDoubleMatrix3D(Slices, Rows, Columns);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[slice,row,column]</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>slice&lt;0 || slice&gt;=Slices() || row&lt;0 || row&gt;=Rows() || column&lt;0 || column&gt;=column()</i>.
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>value the value to be filled into the specified cell.</returns>
        [Obsolete("SetQuick(int slice, int row, int column, double value) is deprecated, please use indexer instead.")]
        public void SetQuick(int slice, int row, int column, double value)
        {
            //if (debug) if (slice<0 || slice>=Slices || row<0 || row>=Rows || column<0 || column>=Columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
            //elements[index(slice,row,column)] = value;
            //manually inlined:
            Elements[SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride] = value;
        }

        /// <summary>
        /// Construct and returns a new 2-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix3D</i> the new matrix must also be of type <i>DenseDoubleMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix3D</i> the new matrix must also be of type <i>SparseDoubleMatrix2D</i>, etc.
        /// </summary>
        /// <param name="Rows">the number of Rows the matrix shall have.</param>
        /// <param name="Columns">the number of Columns the matrix shall have.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="Rowstride">the number of elements between two Rows, i.ed <i>index(i+1,j)-index(i,j)</i>.</param>
        /// <param name="Columnstride">the number of elements between two Columns, i.ed <i>index(i,j+1)-index(i,j)</i>.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        protected override DoubleMatrix2D Like2D(int Rows, int Columns, int rowZero, int columnZero, int Rowstride, int Columnstride)
        {
            return new DenseDoubleMatrix2D(Rows, Columns, this.Elements, rowZero, columnZero, Rowstride, Columnstride);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="sliceOffsets">the offsets of the visible elements.</param>
        /// <param name="rowOffsets">the offsets of the visible elements.</param>
        /// <param name="columnOffsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override DoubleMatrix3D ViewSelectionLike(int[] sliceOffsets, int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedDenseDoubleMatrix3D(this.Elements, sliceOffsets, rowOffsets, columnOffsets, 0);
        }
        #endregion

        #region Local Public Methods

        /// <summary>
        /// Sets all cells to the state specified by <i>values</i>.
        /// <i>values</i> is required to have the form <i>values[slice][row][column]</i>
        /// and have exactly the same number of Slices, Rows and Columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != Slices() || for any 0 &lt;= slice &lt; Slices(): values[slice].Length != Rows()</i>.</exception>
        /// <exception cref="ArgumentException">if <i>for any 0 &lt;= column &lt; Columns(): values[slice][row].Length != Columns()</i>.</exception>
        public new DoubleMatrix3D Assign(double[][][] values)
        {
            if (!this.IsView)
            {
                if (values.Length != Slices) throw new ArgumentException("Must have same number of Slices: Slices=" + values.Length + "Slices=" + Slices);
                int i = Slices * Rows * Columns - Columns;
                for (int slice = Slices; --slice >= 0;)
                {
                    double[][] currentSlice = values[slice];
                    if (currentSlice.Length != Rows) throw new ArgumentException("Must have same number of Rows in every slice: Rows=" + currentSlice.Length + "Rows=" + Rows);
                    for (int row = Rows; --row >= 0;)
                    {
                        double[] currentRow = currentSlice[row];
                        if (currentRow.Length != Columns) throw new ArgumentException("Must have same number of Columns in every row: Columns=" + currentRow.Length + "Columns=" + Columns);
                        Array.Copy(currentRow, 0, this.Elements, i, Columns);
                        i -= Columns;
                    }
                }
            }
            else
            {
                base.Assign(values);
            }
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same number of Slices, Rows and Columns.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        /// </summary>
        /// <param name="source">the source matrix to copy from (may be identical to the receiver).</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>Slices() != source.Slices() || Rows() != source.Rows() || Columns() != source.Columns()</i></exception>
        public new DoubleMatrix3D Assign(DoubleMatrix3D source)
        {
            // overriden for performance only
            if (!(source is DenseDoubleMatrix3D))
            {
                return base.Assign(source);
            }
            DenseDoubleMatrix3D other = (DenseDoubleMatrix3D)source;
            if (other == this) return this;
            CheckShape(other);
            if (HaveSharedCells(other))
            {
                DoubleMatrix3D c = other.Copy();
                if (!(c is DenseDoubleMatrix3D))
                { // should not happen
                    return base.Assign(source);
                }
                other = (DenseDoubleMatrix3D)c;
            }

            if (!this.IsView && !other.IsView)
            { // quickest
                Array.Copy(other.Elements, 0, this.Elements, 0, this.Elements.Length);
                return this;
            }
            return base.Assign(other);
        }

        /// <summary>
        /// 27 neighbor stencil transformation.For efficient finite difference operations.
        /// Applies a function to a moving<i>3 x 3 x 3</i> window.
        /// Does nothing if <i>Rows() < 3 || Columns() < 3 || Slices() < 3</i>.
        /// <pre>
        /// B[k, i, j] = function.apply(
        /// &nbsp;&nbsp;&nbsp;A[k - 1, i - 1, j - 1], A[k - 1, i - 1, j], A[k - 1, i - 1, j + 1],
        /// &nbsp;&nbsp;&nbsp;A[k - 1, i, j - 1], A[k - 1, i, j], A[k - 1, i, j + 1],
        /// &nbsp;&nbsp;&nbsp;A[k - 1, i + 1, j - 1], A[k - 1, i + 1, j], A[k - 1, i + 1, j + 1],
        ///
        /// &nbsp;&nbsp;&nbsp;A[k, i - 1, j - 1], A[k, i - 1, j], A[k, i - 1, j + 1],
        /// &nbsp;&nbsp;&nbsp;A[k, i, j - 1], A[k, i, j], A[k, i, j + 1],
        /// &nbsp;&nbsp;&nbsp;A[k, i + 1, j - 1], A[k, i + 1, j], A[k, i + 1, j + 1],
        ///
        /// &nbsp;&nbsp;&nbsp;A[k + 1, i - 1, j - 1], A[k + 1, i - 1, j], A[k + 1, i - 1, j + 1],
        /// &nbsp;&nbsp;&nbsp;A[k + 1, i, j - 1], A[k + 1, i, j], A[k + 1, i, j + 1],
        /// &nbsp;&nbsp;&nbsp;A[k + 1, i + 1, j - 1], A[k + 1, i + 1, j], A[k + 1, i + 1, j + 1]
        /// &nbsp;&nbsp;&nbsp;)
        ///
        /// x x x - &nbsp;&nbsp;&nbsp; - x x x &nbsp;&nbsp;&nbsp; - - - - 
        /// x o x - &nbsp;&nbsp;&nbsp; - x o x &nbsp;&nbsp;&nbsp; - - - - 
        /// x x x - &nbsp;&nbsp;&nbsp; - x x x..d - x x x 
        /// - - - - &nbsp;&nbsp;&nbsp; - - - - &nbsp;&nbsp;&nbsp; - x o x 
        /// - - - - &nbsp;&nbsp;&nbsp; - - - - &nbsp;&nbsp;&nbsp; - x x x
        /// </pre>
        /// Make sure that cells of<i>this</i> and<i> B</i> do not overlap.
        /// In case of overlapping views, behaviour is unspecified.
        /// </pre>
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// double alpha = 0.25;
        ///         double beta = 0.75;
        ///
        ///         cern.colt.function.Double27Function f = new cern.colt.function.Double27Function() {
        /// &nbsp;&nbsp;&nbsp;public double apply(
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a000, double a001, double a002,
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a010, double a011, double a012,
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a020, double a021, double a022,
        ///
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a100, double a101, double a102,
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a110, double a111, double a112,
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a120, double a121, double a122,
        ///
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a200, double a201, double a202,
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a210, double a211, double a212,
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;double a220, double a221, double a222) {
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return beta* a111 + alpha* (a000 + ..d + a222);
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}
        /// };
        /// A.zAssign27Neighbors(B, f);
        /// </pre>
        /// </summary>
        /// <param name="B">the matrix to hold the results.</param>
        /// <param name="function">the function to be applied to the 27 cells.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">if <i>function==null</i>.</exception>
        /// <exception cref="ArgumentException">if <i>Rows() != B.Rows() || Columns() != B.Columns() || Slices() != B.Slices() </i>.</exception>
        public new void ZAssign27Neighbors(DoubleMatrix3D B, Cern.Colt.Function.Double27Function function)
        {
            // overridden for performance only
            if (!(B is DenseDoubleMatrix3D))
            {
                base.ZAssign27Neighbors(B, function);
                return;
            }
            if (function == null) throw new NullReferenceException("function must not be null.");
            CheckShape(B);
            int r = Rows - 1;
            int c = Columns - 1;
            if (Rows < 3 || Columns < 3 || Slices < 3) return; // nothing to do

            DenseDoubleMatrix3D BB = (DenseDoubleMatrix3D)B;
            int A_ss = SliceStride;
            int A_rs = RowStride;
            int B_rs = BB.RowStride;
            int A_cs = ColumnStride;
            int B_cs = BB.ColumnStride;
            double[] elems = this.Elements;
            double[] B_elems = BB.Elements;
            if (elems == null || B_elems == null) throw new NullReferenceException();

            for (int k = 1; k < Slices - 1; k++)
            {
                int A_index = Index(k, 1, 1);
                int B_index = BB.Index(k, 1, 1);

                for (int i = 1; i < r; i++)
                {
                    int A002 = A_index - A_ss - A_rs - A_cs;
                    int A012 = A002 + A_rs;
                    int A022 = A012 + A_rs;

                    int A102 = A002 + A_ss;
                    int A112 = A102 + A_rs;
                    int A122 = A112 + A_rs;

                    int A202 = A102 + A_ss;
                    int A212 = A202 + A_rs;
                    int A222 = A212 + A_rs;

                    double a000, a001, a002;
                    double a010, a011, a012;
                    double a020, a021, a022;

                    double a100, a101, a102;
                    double a110, a111, a112;
                    double a120, a121, a122;

                    double a200, a201, a202;
                    double a210, a211, a212;
                    double a220, a221, a222;

                    a000 = elems[A002]; A002 += A_cs; a001 = elems[A002];
                    a010 = elems[A012]; A012 += A_cs; a011 = elems[A012];
                    a020 = elems[A022]; A022 += A_cs; a021 = elems[A022];

                    a100 = elems[A102]; A102 += A_cs; a101 = elems[A102];
                    a110 = elems[A112]; A112 += A_cs; a111 = elems[A112];
                    a120 = elems[A122]; A122 += A_cs; a121 = elems[A122];

                    a200 = elems[A202]; A202 += A_cs; a201 = elems[A202];
                    a210 = elems[A212]; A212 += A_cs; a211 = elems[A212];
                    a220 = elems[A222]; A222 += A_cs; a221 = elems[A222];

                    int B11 = B_index;
                    for (int j = 1; j < c; j++)
                    {
                        // in each step 18 cells can be remembered in registers - they don't need to be reread from slow memory
                        // in each step 9 instead of 27 cells need to be read from memory.
                        a002 = elems[A002 += A_cs];
                        a012 = elems[A012 += A_cs];
                        a022 = elems[A022 += A_cs];

                        a102 = elems[A102 += A_cs];
                        a112 = elems[A112 += A_cs];
                        a122 = elems[A122 += A_cs];

                        a202 = elems[A202 += A_cs];
                        a212 = elems[A212 += A_cs];
                        a222 = elems[A222 += A_cs];

                        B_elems[B11] = function(
                            a000, a001, a002,
                            a010, a011, a012,
                            a020, a021, a022,

                            a100, a101, a102,
                            a110, a111, a112,
                            a120, a121, a122,

                            a200, a201, a202,
                            a210, a211, a212,
                            a220, a221, a222);
                        B11 += B_cs;

                        // move remembered cells
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
                    A_index += A_rs;
                    B_index += B_rs;
                }
            }
        }

        #endregion

        #region Local Protected Methods

        /// <summary>
        /// Returns <i>true</i> if both matrices share common cells.
        /// More formally, returns <i>true</i> if <i>other != null</i> and at least one of the following conditions is met
        /// <ul>
        /// <li>the receiver is a view of the other matrix
        /// <li>the other matrix is a view of the receiver
        /// <li><i>this == other</i>
        /// </ul>
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(DoubleMatrix3D other)
        {
            if (other is SelectedDenseDoubleMatrix3D)
            {
                SelectedDenseDoubleMatrix3D otherMatrix = (SelectedDenseDoubleMatrix3D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is DenseDoubleMatrix3D)
            {
                DenseDoubleMatrix3D otherMatrix = (DenseDoubleMatrix3D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// </summary>
        /// <param name="slice">the index of the slice-coordinate.</param>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the third-coordinate.</param>
        protected new int Index(int slice, int row, int column)
        {
            //return _sliceOffset(_sliceRank(slice)) + _rowOffset(_rowRank(row)) + _columnOffset(_columnRank(column));
            //manually inlined:
            return SliceZero + slice * SliceStride + RowZero + row * RowStride + ColumnZero + column * ColumnStride;
        }

        #endregion
    }
}
