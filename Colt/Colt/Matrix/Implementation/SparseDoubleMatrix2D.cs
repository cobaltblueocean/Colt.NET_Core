// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SparseDoubleMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Sparse hashed 2-d matrix holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.Implementation
{
    using System;
    using System.Collections.Generic;

    using Function;

    /// <summary>
    /// Sparse hashed 2-d matrix holding <tt>double</tt> elements.
    /// </summary>
    public sealed class SparseDoubleMatrix2D : DoubleMatrix2D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparseDoubleMatrix2D"/> class with a copy of the given values.
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the new matrix.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>for any 1 &lt;= row &lt; values.length: values[row].length != values[row-1].length</tt>.
        /// </exception>
        public SparseDoubleMatrix2D(double[][] values)
        {
            var rows = values.Length;
            var columns = values.Length == 0 ? 0 : values[0].Length;
            Setup(rows, columns);
            this.elements = new Dictionary<int, double>(rows * (columns / 1000));
            Assign(values);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseDoubleMatrix2D"/> class.
        /// All entries are initially <tt>0</tt>.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <exception cref="ArithmeticException">
        /// If <tt>rows &lt; 0 || columns &lt; 0 || (double)columns*rows &gt; Integer.MAX_VALUE</tt>.
        /// </exception>
        public SparseDoubleMatrix2D(int rows, int columns)
        {
            Setup(rows, columns);
            this.elements = new Dictionary<int, double>(rows * (columns / 1000));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseDoubleMatrix2D"/> class.
        /// Constructs a view with the given parameters.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <param name="elements">
        /// The cells.
        /// </param>
        /// <param name="rowZero">
        /// The row of the first element.
        /// </param>
        /// <param name="columnZero">
        /// The column of the first element.
        /// </param>
        /// <param name="rowStride">
        /// The number of elements between two rows, i.e. <tt>index(i+1,j)-index(i,j)</tt>.
        /// </param>
        /// <param name="columnStride">
        /// The number of elements between two columns, i.e. <tt>index(i,j+1)-index(i,j)</tt>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <tt>rows &lt; 0 || columns &lt; 0 || (double)columns*rows &gt; Integer.MAX_VALUE</tt> or flip's are illegal.
        /// </exception>
        internal SparseDoubleMatrix2D(int rows, int columns, IDictionary<int, double> elements, int rowZero, int columnZero, int rowStride, int columnStride)
        {
            Setup(rows, columns, rowZero, columnZero, rowStride, columnStride);
            this.elements = elements;
            this.IsView = true;
        }

        /// <summary>
        /// Gets he elements of the matrix.
        /// </summary>
        internal IDictionary<int, double> elements { get; private set; }

        /// <summary>
        /// Gets or sets the matrix cell at coordinate <tt>[row,column]</tt> to the specified value.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        public override double this[int row, int column]
        {
            get
            {
                int index = RowZero + (row * RowStride) + ColumnZero + (column * ColumnStride);
                return elements.ContainsKey(index) ? elements[index] : 0;
            }

            set
            {
                int index = RowZero + (row * RowStride) + ColumnZero + (column * ColumnStride);

                if (value == 0)
                    this.elements.Remove(index);
                else
                    this.elements[index] = value;
            }
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
        public override DoubleMatrix2D Assign(double value)
        {
            // overriden for performance only
            if (!IsView && value == 0) this.elements.Clear();
            else base.Assign(value);
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same number of rows and columns.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
        /// </summary>
        /// <param name="source">
        /// The source matrix.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>columns() != other.columns() || rows() != other.rows()</tt>
        /// </exception>
        public override DoubleMatrix2D Assign(DoubleMatrix2D source)
        {
            // overriden for performance only
            if (!(source is SparseDoubleMatrix2D))
                return base.Assign(source);

            var other = (SparseDoubleMatrix2D)source;
            if (other == this) return this; // nothing to do
            CheckShape(other);

            if (!this.IsView && !other.IsView)
            { // quickest
                this.elements = new Dictionary<int, double>(other.elements);
                return this;
            }

            return base.Assign(source);
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col],y[row,col])</tt>.
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
        /// <exception cref="ArgumentException">
        /// If <tt>columns() != other.columns() || rows() != other.rows()</tt>
        /// </exception>
        public override DoubleMatrix2D Assign(DoubleMatrix2D y, DoubleDoubleFunction function)
        {
            if (!IsView)
                CheckShape(y);
            return base.Assign(y, function);
        }

        /// <summary>
        /// Returns the number of cells having non-zero values.
        /// </summary>
        /// <returns>
        /// The number of cells having non-zero values.
        /// </returns>
        public override int Cardinality()
        {
            return IsView ? base.Cardinality() : this.elements.Count;
        }

        /// <summary>
        /// Assigns the result of a function to each <i>non-zero</i> cell; <tt>x[row,col] = function(x[row,col])</tt>.
        /// </summary>
        /// <param name="function">
        /// A function taking as argument the current non-zero cell's row, column and value.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public override DoubleMatrix2D ForEachNonZero(IntIntDoubleFunction function)
        {
            if (IsView) base.ForEachNonZero(function);
            else
            {
                foreach (var e in elements)
                {
                    int i = e.Key / Columns;
                    int j = e.Key & Columns;
                    double r = function(i, j, e.Value);
                    if (r != e.Value) elements[e.Key] = e.Value;
                }
            }

            return this;
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// </summary>
        /// <param name="rows">
        /// The number of rows the matrix shall have.
        /// </param>
        /// <param name="columns">
        /// The number of columns the matrix shall have.
        /// </param>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public override DoubleMatrix2D Like(int rows, int columns)
        {
            return new SparseDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        public override DoubleMatrix1D Like1D(int size)
        {
            return new SparseDoubleMatrix1D(size);
        }

        /// <summary>
        /// Return an iterator over non-zero elements.
        /// </summary>
        /// <returns>
        /// An iterator over non zero elements.
        /// </returns>
        public override IEnumerable<Element> NonZeroElements()
        {
            foreach (var e in elements) yield return new Element { Row = e.Key / Columns, Column = e.Key & Columns, Value = e.Value };
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="stride">
        /// The number of indexes between any two elements, i.e. <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        protected internal override DoubleMatrix1D Like1D(int size, int offset, int stride)
        {
            return new SparseDoubleMatrix1D(size, this.elements, offset, stride);
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
        protected override bool HaveSharedCellsRaw(DoubleMatrix2D other)
        {
            if (other is SelectedSparseDoubleMatrix2D)
            {
                var otherMatrix = (SelectedSparseDoubleMatrix2D)other;
                return this.elements == otherMatrix.elements;
            }

            if (other is SparseDoubleMatrix2D)
            {
                var otherMatrix = (SparseDoubleMatrix2D)other;
                return this.elements == otherMatrix.elements;
            }

            return false;
        }

        /// <summary>
        /// Returns the position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        /// <returns>
        /// The position of the given coordinate within the (virtual or non-virtual) internal 1-dimensional array. 
        /// </returns>
        protected override int Index(int row, int column)
        {
            // return super.index(row,column);
            // manually inlined for speed:
            return RowZero + (row * RowStride) + ColumnZero + (column * ColumnStride);
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="rowOffsets">
        /// The row offsets of the visible elements.
        /// </param>
        /// <param name="columnOffsets">
        /// The column offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        protected override DoubleMatrix2D ViewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            return new SelectedSparseDoubleMatrix2D(this.elements, rowOffsets, columnOffsets, 0);
        }
    }
}
