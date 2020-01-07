        // <copyright file="DenseObjectMatrix2D.cs" company="CERN">
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
    /// Dense 2-d matrix holding <i>Object</i> elements.
    ///
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Internally holds one single contigous one-dimensional array, addressed in row majord 
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory [bytes] = 8*rows()*columns()</i>.
    /// Thus, a 1000*1000 matrix uses 8 MB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// <i>O(1)</i> (i.ed constant time) for the basic operations
    /// <i>get</i>, <i>getQuick</i>, <i>HashSet</i>, <i>setQuick</i> and <i>size</i>,
    /// <p>
    /// Cells are internally addressed in row-major.
    /// Applications demanding utmost speed can exploit this fact.
    /// Setting/getting values in a loop row-by-row is quicker than column-by-column.
    /// Thus
    /// <pre>
    ///    for (int row=0; row &lt; rows; row++) {
    /// 	  for (int column=0; column &lt; columns; column++) {
    /// 		 matrix.setQuick(row,column,someValue);
    /// 	  }
    ///    }
    /// </pre>
    /// is quicker than
    /// <pre>
    ///    for (int column=0; column &lt; columns; column++) {
    /// 	  for (int row=0; row &lt; rows; row++) {
    /// 		 matrix.setQuick(row,column,someValue);
    /// 	  }
    ///    }
    /// </pre>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class DenseObjectMatrix2D : ObjectMatrix2D
    {
        /// <summary>
        /// Gets the elements of this matrix.
        /// </summary>
        internal Object[] Elements { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <i>[row,column]</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        public override object this[int row, int column]
        {
            get
            {
                //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new IndexOutOfRangeException("row:"+row+", column:"+column);
                //return elements[index(row,column)];
                //manually inlined:
                return Elements[Index(row, column)];
            }
            set
            {
                //if (debug) if (column<0 || column>=columns || row<0 || row>=rows) throw new IndexOutOfRangeException("row:"+row+", column:"+column);
                //elements[index(row,column)] = value;
                //manually inlined:
                Elements[Index(row, column)] = value;
            }
        }

        /// <summary>
        /// Constructs a matrix with a copy of the given values.
        /// <i>values</i> is required to have the form <i>values[row][column]</i>
        /// and have exactly the same number of columns in every row.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">The values to be filled into the new matrix.</param>
        /// <exception cref="ArgumentException">if <i>for any 1 &lt;= row &lt; values.Length: values[row].Length != values[row-1].Length</i>.</exception>
        public DenseObjectMatrix2D(Object[][] values) : this(values.Length, values.Length == 0 ? 0 : values[0].Length)
        {
            Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of rows and columns.
        /// All entries are initially <i>0</i>.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">if <i>rows &lt; 0 || columns &lt; 0 || (Object)columns * rows > int.MaxValue</i>.</exception>
        public DenseObjectMatrix2D(int rows, int columns)
        {
            Setup(rows, columns);
            this.Elements = new Object[rows * columns];
        }

        /// <summary>
        /// Constructs a view with the given parameters.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <param name="elements">the cells.</param>
        /// <param name="rowZero">the position of the first element.</param>
        /// <param name="columnZero">the position of the first element.</param>
        /// <param name="rowStride">the number of elements between two rows, i.ed <i>index(i+1,j)-index(i,j)</i>.</param>
        /// <param name="columnStride">the number of elements between two columns, i.ed <i>index(i,j+1)-index(i,j)</i>.</param>
        /// <exception cref="ArgumentException">if <i>rows &lt; 0 || columns &lt; 0 || (Object)columns * rows > int.MaxValue</i> or flip's are illegal.</exception>
        public DenseObjectMatrix2D(int rows, int columns, Object[] elements, int rowZero, int columnZero, int rowStride, int columnStride)
        {
            Setup(rows, columns, rowZero, columnZero, rowStride, columnStride);
            this.Elements = elements;
            this.IsView = true;
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>values</i>.
        /// <i>values</i> is required to have the form <i>values[row][column]</i>
        /// and have exactly the same number of rows and columns as the receiver.
        /// <p>
        /// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
        /// </summary>
        /// <param name="values">the values to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>values.Length != rows() || for any 0 &lt;= row &lt; rows(): values[row].Length != columns()</i>.</exception>
        public new ObjectMatrix2D Assign(Object[][] values)
        {
            if (!this.IsView)
            {
                if (values.Length != Rows) throw new ArgumentException("Must have same number of rows: Rows = " + values.Length + ", Rows = " + Rows);
                int i = Columns * (Rows - 1);
                for (int row = Rows; --row >= 0;)
                {
                    Object[] currentRow = values[row];
                    if (currentRow.Length != Columns) throw new ArgumentException("Must have same number of columns in every row: Columns = " + currentRow.Length + ", Columns = " + Columns);
                    Array.Copy(currentRow, 0, this.Elements, i, Columns);
                    i -= Columns;
                }
            }
            else
            {
                base.Assign(values);
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
        /// matrix.assign(Cern.Jet.Math.Functions.sin);
        /// -->
        /// 2 x 2 matrix
        /// 0.479426  0.997495 
        /// 0.598472 -0.350783
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// </summary>
        /// <param name="function">a function object taking as argument the current cell's value.</param>
        /// <param name=""><i>this</i> (for convenience only).</param>
        /// <returns></returns>
        /// <see cref="Cern.Jet.Math.Functions"></see>
        public new ObjectMatrix2D Assign(Cern.Colt.Function.ObjectFunction<Object> function)
        {
            Object[] elems = this.Elements;
            if (elems == null) throw new NullReferenceException();
            int index = base.Index(0, 0);
            int cs = this.ColumnStride;
            int rs = this.RowStride;

            // the general case x[i] = f(x[i])
            for (int row = Rows; --row >= 0;)
            {
                for (int i = index, column = Columns; --column >= 0;)
                {
                    elems[i] = function(elems[i]);
                    i += cs;
                }
                index += rs;
            }
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same number of rows and columns.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <i>other</i>.
        /// </summary>
        /// <param name="source">the source matrix to copy from (may be identical to the receiver).</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>columns() != source.columns() || rows() != source.rows()</i></exception>
        public new ObjectMatrix2D Assign(ObjectMatrix2D source)
        {
            // overriden for performance only
            if (!(source is DenseObjectMatrix2D))
            {
                return base.Assign(source);
            }
            DenseObjectMatrix2D other = (DenseObjectMatrix2D)source;
            if (other == this) return this; // nothing to do
            CheckShape(other);

            if (!this.IsView && !other.IsView)
            { // quickest
                Array.Copy(other.Elements, 0, this.Elements, 0, this.Elements.Length);
                return this;
            }

            if (HaveSharedCells(other))
            {
                ObjectMatrix2D c = other.Copy();
                if (!(c is DenseObjectMatrix2D))
                { // should not happen
                    return base.Assign(other);
                }
                other = (DenseObjectMatrix2D)c;
            }

            Object[] elems = this.Elements;
            Object[] otherElems = other.Elements;
            if (Elements == null || otherElems == null) throw new NullReferenceException();
            int cs = this.ColumnStride;
            int ocs = other.ColumnStride;
            int rs = this.RowStride;
            int ors = other.RowStride;

            int otherIndex = other.Index(0, 0);
            int index = base.Index(0, 0);
            for (int row = Rows; --row >= 0;)
            {
                for (int i = index, j = otherIndex, column = Columns; --column >= 0;)
                {
                    elems[i] = otherElems[j];
                    i += cs;
                    j += ocs;
                }
                index += rs;
                otherIndex += ors;
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
        /// m1.assign(m2, Cern.Jet.Math.Functions.pow);
        /// -->
        /// m1 == 2 x 2 matrix
        ///          1   1 
        /// 16 729
        /// </pre>
        /// For further examples, see the <a href="package-summary.html#FunctionObjects">package doc</a>.
        /// </summary>
        /// <param name="y">the secondary matrix to operate on.</param>
        /// <param name="function">a function object taking as first argument the current cell's value of <i>this</i>, and as second argument the current cell's value of <i>y</i>,</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        /// <exception cref="ArgumentException">if <i>columns() != other.columns() || rows() != other.rows()</i></exception>
        /// <see cref="Cern.Jet.Math.Functions"/>
        public new ObjectMatrix2D Assign(ObjectMatrix2D y, Cern.Colt.Function.ObjectObjectFunction<Object> function)
        {
            // overriden for performance only
            if (!(y is DenseObjectMatrix2D))
            {
                return base.Assign(y, function);
            }
            DenseObjectMatrix2D other = (DenseObjectMatrix2D)y;
            CheckShape(y);

            Object[] elems = this.Elements;
            Object[] otherElems = other.Elements;
            if (elems == null || otherElems == null) throw new NullReferenceException();
            int cs = this.ColumnStride;
            int ocs = other.ColumnStride;
            int rs = this.RowStride;
            int ors = other.RowStride;

            int otherIndex = other.Index(0, 0);
            int index = base.Index(0, 0);

            // the general case x[i] = f(x[i],y[i])
            for (int row = Rows; --row >= 0;)
            {
                for (int i = index, j = otherIndex, column = Columns; --column >= 0;)
                {
                    elems[i] = function(elems[i], otherElems[j]);
                    i += cs;
                    j += ocs;
                }
                index += rs;
                otherIndex += ors;
            }
            return this;
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <i>[row,column]</i>.
        ///
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        [Obsolete("GetQuick(int index, int column) is deprecated, please use indexer instead.")]
        public override Object GetQuick(int row, int column)
        {
            return this[row, column];
        }

        /// <summary>
        /// Returns <i>true</i> if both matrices share common cells.
        /// More formally, returns <i>true</i> if <i>other != null</i> and at least one of the following conditions is met
        /// <ul>
        /// <li>the receiver is a view of the other matrix
        /// <li>the other matrix is a view of the receiver
        /// <li><i>this == other</i>
        /// </ul>
        /// </summary>
        protected new Boolean HaveSharedCellsRaw(ObjectMatrix2D other)
        {
            if (other is SelectedDenseObjectMatrix2D)
            {
                SelectedDenseObjectMatrix2D otherMatrix = (SelectedDenseObjectMatrix2D)other;
                return this.Elements == otherMatrix.Elements;
            }
            else if (other is DenseObjectMatrix2D)
            {
                DenseObjectMatrix2D otherMatrix = (DenseObjectMatrix2D)other;
                return this.Elements == otherMatrix.Elements;
            }
            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional arrayd 
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        protected new int Index(int row, int column)
        {
            // return base.index(row,column);
            // manually inlined for speed:
            return RowZero + row * RowStride + ColumnZero + column * ColumnStride;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must also be of type <i>DenseObjectMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must also be of type <i>SparseObjectMatrix2D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override ObjectMatrix2D Like(int rows, int columns)
        {
            return new DenseObjectMatrix2D(rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must be of type <i>SparseObjectMatrix1D</i>, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override ObjectMatrix1D Like1D(int size)
        {
            return new DenseObjectMatrix1D(size);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// For example, if the receiver is an instance of type <i>DenseObjectMatrix2D</i> the new matrix must be of type <i>DenseObjectMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseObjectMatrix2D</i> the new matrix must be of type <i>SparseObjectMatrix1D</i>, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <param name="zero">the index of the first element.</param>
        /// <param name="stride">the number of indexes between any two elements, i.ed <i>index(i+1)-index(i)</i>.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        protected override ObjectMatrix1D Like1D(int size, int zero, int stride)
        {
            return new DenseObjectMatrix1D(size, this.Elements, zero, stride);
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <i>[row,column]</i> to the specified value.
        ///
        /// <p>Provided with invalid parameters this method may access illegal indexes without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <param name="value">the value to be filled into the specified cell.</param>
        [Obsolete("SetQuick(int index, int column, Object value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int row, int column, Object value)
        {
            this[row, column] = value;
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="rowOffsets">the offsets of the visible elements.</param>
        /// <param name="columnOffsets">the offsets of the visible elements.</param>
        /// <returns>a new view.</returns>
        protected override ObjectMatrix2D ViewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedDenseObjectMatrix2D(this.Elements, rowOffsets, columnOffsets, 0);
        }

        public override string ToString(int row, int column)
        {
            return this[row, column].ToString();
        }
    }
}
