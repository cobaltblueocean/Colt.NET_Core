// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for 2-d matrices holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using DoubleAlgorithms;
    using Function;
    using Implementation;
    using LinearAlgebra;
    using Cern.Colt.List;

    /// <summary>
    /// A condition or procedure : takes a single argument and returns a boolean value.
    /// </summary>
    /// <param name="element">
    /// The element passed to the procedure.
    /// </param>
    /// <returns>
    /// A flag to inform the object calling the procedure.
    /// </returns>
    public delegate bool DoubleMatrix2DProcedure(DoubleMatrix2D element);

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
    public delegate double DoubleMatrix2DDinstance(DoubleMatrix2D x, DoubleMatrix2D y);

    /// <summary>
    /// Abstract base class for 2-d matrices holding <tt>double</tt> elements.
    /// </summary>
    public abstract class DoubleMatrix2D : AbstractMatrix2D, IEnumerable<double>
    {
        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <tt>[row,column]</tt>.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        public abstract double this[int row, int column]
        {
            get;
            set;
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
        public double Aggregate(DoubleDoubleFunction aggr, DoubleFunction f)
        {
            if (Size == 0) 
                return double.NaN;
            double a = f(this[Rows - 1, Columns - 1]);
            int d = 1; // last cell already done
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns - d; --column >= 0;)
                    a = aggr(a, f(this[row, column]));
                d = 0;
            }

            return a;
        }

        /// <summary>
        /// Applies a function to each corresponding cell of two matrices and aggregates the results.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <param name="aggr">
        /// An aggregation function taking as first argument the current aggregation and as second argument the transformed current cell values.
        /// </param>
        /// <param name="f">
        /// A function transforming the current cell values.
        /// </param>
        /// <returns>
        /// The aggregated measure.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>columns() != other.columns() || rows() != other.rows()</tt>
        /// </exception>
        public double Aggregate(DoubleMatrix2D other, DoubleDoubleFunction aggr, DoubleDoubleFunction f)
        {
            CheckShape(other);
            if (Size == 0) return double.NaN;
            double a = f(this[Rows - 1, Columns - 1], other[Rows - 1, Columns - 1]);
            int d = 1; // last cell already done
            for (int row = Rows; --row >= 0;)
            {
                for (int column = Columns - d; --column >= 0;)
                    a = aggr(a, f(this[row, column], other[row, column]));
                d = 0;
            }

            return a;
        }

        /// <summary>
        /// Sets all cells to the state specified by <tt>values</tt>.
        /// <tt>values</tt> is required to have the form <tt>values[row][column]</tt>
        /// and have exactly the same number of rows and columns as the receiver.
        /// <para>
        /// The values are copiedd So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        /// </para>
        /// </summary>
        /// <param name="values">
        /// The values to be filled into the cells.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>values.Length != rows() || for any 0 &lt;= row &lt; rows(): values[row].Length != columns()</tt>.
        /// </exception>
        public virtual DoubleMatrix2D Assign(double[][] values)
        {
            if (values.Length != Rows) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Matrix_MustHaveSameNumberOfRows, values.Length, Rows));
            for (int row = Rows; --row >= 0;)
            {
                double[] currentRow = values[row];
                if (currentRow.Length != Columns) throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Matrix_MustHaveSameNumberOfColumnsInEveryRow, currentRow.Length , Columns));
                for (int column = Columns; --column >= 0;)
                    this[row, column] = currentRow[column];
            }

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
        public virtual DoubleMatrix2D Assign(double value)
        {
            int r = Rows;
            int c = Columns;
            for (int row = 0; row < r; row++)
                for (int column = 0; column < c; column++)
                    this[row, column] = value;
            return this;
        }

        /// <summary>
        /// Assigns the result of a function to each cell; <tt>x[row,col] = function(x[row,col])</tt>.
        /// </summary>
        /// <param name="function">
        /// The function taking as argument the current cell's value.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public virtual DoubleMatrix2D Assign(DoubleFunction function)
        {
            for (int row = Rows; --row >= 0;)
                for (int column = Columns; --column >= 0;)
                    this[row, column] = function(this[row, column]);
            return this;
        }

        /// <summary>
        /// Replaces all cell values of the receiver with the values of another matrix.
        /// Both matrices must have the same number of rows and columns.
        /// If both matrices share the same cells (as is the case if they are views derived from the same matrix) and intersect in an ambiguous way, then replaces <i>as if</i> using an intermediate auxiliary deep copy of <tt>other</tt>.
        /// </summary>
        /// <param name="other">
        /// The source matrix to copy from (may be identical to the receiver).
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <tt>columns() != other.columns() || rows() != other.rows()</tt>
        /// </exception>
        public virtual DoubleMatrix2D Assign(DoubleMatrix2D other)
        {
            if (other == this) return this;
            CheckShape(other);
            if (HaveSharedCells(other)) other = other.Copy();

            for (int row = Rows; --row >= 0;)
                for (int column = Columns; --column >= 0;)
                    this[row, column] = other[row, column];
            return this;
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
        public virtual DoubleMatrix2D Assign(DoubleMatrix2D y, DoubleDoubleFunction function)
        {
            CheckShape(y);
            for (int row = Rows; --row >= 0;) for (int column = Columns; --column >= 0;) this[row, column] = function(this[row, column], y[row, column]);
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
            int result = 0;
            for (int row = Rows; --row >= 0;)
                for (int column = Columns; --column >= 0;)
                    if (this[row, column] != 0) result++;
            return result;
        }

        /// <summary>
        /// Constructs and returns a deep copy of the receiver.
        /// </summary>
        /// <returns>
        /// A deep copy of the receiver.
        /// </returns>
        public DoubleMatrix2D Copy()
        {
            return Like().Assign(this);
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
        /// not <code>null</code> and is at least a <code>DoubleMatrix2D</code> object
        /// that has the same number of columns and rows as the receiver and 
        /// has exactly the same values at the same coordinates.
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
            if (!(obj is DoubleMatrix2D)) return false;

            return Property.DEFAULT.Equals(this, (DoubleMatrix2D)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Assigns the result of a function to each <i>non-zero</i> cell; <tt>x[row,col] = function(x[row,col])</tt>.
        /// Use this method for fast special-purpose iteration.
        /// If you want to modify another matrix instead of <tt>this</tt> (i.ed work in read-only mode), simply return the input value unchanged.
        /// Parameters to function are as follows: <tt>first==row</tt>, <tt>second==column</tt>, <tt>third==nonZeroValue</tt>.
        /// </summary>
        /// <param name="function">
        /// A function taking as argument the current non-zero cell's row, column and value.
        /// </param>
        /// <returns>
        /// <tt>this</tt> (for convenience only).
        /// </returns>
        public virtual DoubleMatrix2D ForEachNonZero(IntIntDoubleFunction function)
        {
            for (int row = Rows; --row >= 0;)
                for (int column = Columns; --column >= 0;)
                {
                    double value = this[row, column];
                    if (value != 0)
                    {
                        double r = function(row, column, value);
                        if (r != value) this[row, column] = r;
                    }
                }

            return this;
        }

        /// <summary>
        /// Returns the matrix cell value at coordinate <tt>[row,column]</tt>.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        /// <returns>
        /// The value of the specified cell.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column&lt;0 || column&gt;=columns() || row&lt;0 || row&gt;=rows()</tt>
        /// </exception>
        [Obsolete("GetQuick(int row, int column) is deprecated, please use indexer instead.")]
        public virtual double GetQuick(int row, int column)
        {
            if (column < 0 || column >= Columns || row < 0 || row >= Rows) throw new IndexOutOfRangeException("row:" + row + ", column:" + column);
            return this[row, column];
        }

        /// <summary>
        /// Fills the coordinates and values of cells having non-zero values into the specified lists.
        /// Fills into the lists, starting at index 0.
        /// After this call returns the specified lists all have a new size, the number of non-zero values.
        /// </summary>
        /// <param name="rowList">
        /// The list to be filled with row indexes, can have any size.
        /// </param>
        /// <param name="columnList">
        /// The list to be filled with column indexes, can have any size.
        /// </param>
        /// <param name="valueList">
        /// The ist to be filled with values, can have any size.
        /// </param>
        public void GetNonZeros(IntArrayList rowList, IntArrayList columnList, List<double> valueList)
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
                    double value = this[row, column];
                    if (value != 0)
                    {
                        rowList.Add(row);
                        columnList.Add(column);
                        valueList.Add(value);
                    }
                }
            }
        }

        public Boolean IsDiagonal
        {
            get
            {
                Boolean flag = this.IsSquare;
                int n = this.Rows;

                if (flag)
                {
                    for (int i = 0; flag && i < n - 1; i++)
                    {
                        for (int j = i + 1; flag && j < n; j++)
                        {
                            flag = (this[i, j].Equals(0) && this[j, i].Equals(0));
                        }
                    }
                }
                return flag;
            }
        }


        public Boolean IsSquare
        {
            get
            {
                return (this.Columns == this.Rows);
            }
        }


        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the same number of rows and columns.
        /// </summary>
        /// <returns>
        /// A new empty matrix of the same dynamic type.
        /// </returns>
        public DoubleMatrix2D Like()
        {
            return Like(Rows, Columns);
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
        public abstract DoubleMatrix2D Like(int rows, int columns);

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        public abstract DoubleMatrix1D Like1D(int size);

        /// <summary>
        /// Return an iterator over non-zero elements.
        /// </summary>
        /// <returns>
        /// An iterator over non zero elements.
        /// </returns>
        public virtual IEnumerable<Element> NonZeroElements()
        {
            for (int row = Rows; --row >= 0;)
                for (int column = Columns; --column >= 0;)
                {
                    var value = this[row, column];
                    if (value != 0) yield return new Element { Row = row, Column = column, Value = value };
                }
        }

        /// <summary>
        /// Sets the matrix cell at coordinate <tt>[row,column]</tt> to the specified value.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        /// <param name="value">
        /// The value to be filled into the specified cell.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column&lt;0 || column&gt;=columns() || row&lt;0 || row&gt;=rows()</tt>
        /// </exception>
        [Obsolete("SetQuick(int row, int column, double value) is deprecated, please use indexer instead.")]
        public virtual void SetQuick(int row, int column, double value)
        {
            if (column < 0 || column >= Columns || row < 0 || row >= Rows) throw new IndexOutOfRangeException("row:" + row + ", column:" + column);
            this[row, column] = value;
        }

        /// <summary>
        /// Constructs and returns a 2-dimensional array containing the cell values.
        /// The returned array <tt>values</tt> has the form <tt>values[row][column]</tt>
        /// and has the same number of rows and columns as the receiver.
        /// <para>
        /// The values are copiedd So subsequent changes in <tt>values</tt> are not reflected in the matrix, and vice-versa.
        /// </para>
        /// </summary>
        /// <returns>
        /// An array filled with the values of the cells.
        /// </returns>
        public double[][] ToArray()
        {
            var values = new double[Rows][];
            for (int row = Rows; --row >= 0;)
            {
                var currentRow = new double[Columns];
                for (int column = Columns; --column >= 0;)
                    currentRow[column] = this[row, column];
                values[row] = currentRow;
            }

            return values;
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
        /// Constructs and returns a new <i>slice view</i> representing the rows of the given column.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To obtain a slice view on subranges, construct a sub-ranging view (<tt>viewPart(..d)</tt>), then apply this method to the sub-range view.
        /// </summary>
        /// <param name="column">
        /// The column to fix.
        /// </param>
        /// <returns>
        /// A new slice view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column &lt; 0 || column &gt;= Columns</tt>.
        /// </exception>
        public virtual DoubleMatrix1D ViewColumn(int column)
        {
            CheckColumn(column);
            int viewSize = Rows;
            int viewZero = Index(0, column);
            int viewStride = RowStride;
            return Like1D(viewSize, viewZero, viewStride);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i> along the column axis.
        /// What used to be column <tt>0</tt> is now column <tt>columns()-1</tt>, ..d, what used to be column <tt>columns()-1</tt> is now column <tt>0</tt>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>
        /// A new flip view.
        /// </returns>
        public virtual DoubleMatrix2D ViewColumnFlip()
        {
            return (DoubleMatrix2D)View().VColumnFlip();
        }

        /// <summary>
        /// Constructs and returns a new <i>dice (transposition) view</i>; Swaps axes; example: 3 x 4 matrix --> 4 x 3 matrix.
        /// The view has both dimensions exchanged; what used to be columns become rows, what used to be rows become columns.
        /// In other words: <tt>view.Get(row,column)==this.Get(column,row)</tt>.
        /// This is a zero-copy transposition, taking O(1), i.ed constant time.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versad 
        /// </summary>
        /// <returns>
        /// A new dice view.
        /// </returns>
        public virtual DoubleMatrix2D ViewDice()
        {
            return (DoubleMatrix2D)View().VDice();
        }

        /// <summary>
        /// Constructs and returns a new <i>sub-range view</i> that is a <tt>height x width</tt> sub matrix starting at <tt>[row,column]</tt>.
        /// </summary>
        /// <param name="row">
        /// The index of the row-coordinate.
        /// </param>
        /// <param name="column">
        /// The index of the column-coordinate.
        /// </param>
        /// <param name="height">
        /// The height of the box.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <returns>
        /// The new view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column &lt; 0 || width &lt; 0 || column+width &gt; columns() || row &lt; 0 || height &lt; 0 || row+height &gt; rows()</tt>
        /// </exception>
        public virtual DoubleMatrix2D ViewPart(int row, int column, int height, int width)
        {
            return (DoubleMatrix2D)View().VPart(row, column, height, width);
        }

        /// <summary>
        /// Constructs and returns a new <i>slice view</i> representing the columns of the given row.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <param name="row">
        /// The row to fix.
        /// </param>
        /// <returns>
        /// A new slice view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>row &lt; 0 || row &gt;= Rows</tt>.
        /// </exception>
        public virtual DoubleMatrix1D ViewRow(int row)
        {
            CheckRow(row);
            int viewSize = Columns;
            int viewZero = Index(row, 0);
            int viewStride = ColumnStride;
            return Like1D(viewSize, viewZero, viewStride);
        }

        /// <summary>
        /// Constructs and returns a new <i>flip view</i> along the row axis.
        /// What used to be row <tt>0</tt> is now row <tt>rows()-1</tt>, ..d, what used to be row <tt>rows()-1</tt> is now row <tt>0</tt>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <returns>
        /// A new flip view.
        /// </returns>
        public virtual DoubleMatrix2D ViewRowFlip()
        {
            return (DoubleMatrix2D)View().VRowFlip();
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding the indicated cells.
        /// There holds <tt>view.rows() == rowIndexes.Length, view.columns() == columnIndexes.Length</tt> and <tt>view.Get(i,j) == this.Get(rowIndexes[i],columnIndexes[j])</tt>.
        /// Indexes can occur multiple times and can be in arbitrary order.
        /// </summary>
        /// <param name="rowIndexes">
        /// The rows of the cells that shall be visible in the new viewd To indicate that <i>all</i> rows shall be visible, simply set this parameter to <tt>null</tt>.
        /// </param>
        /// <param name="columnIndexes">
        /// The columns of the cells that shall be visible in the new viewd To indicate that <i>all</i> columns shall be visible, simply set this parameter to <tt>null</tt>.
        /// </param>
        /// <returns>
        /// The new view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>!(0 &lt;= rowIndexes[i] &lt; rows())</tt> for any <tt>i=0..rowIndexes.Length()-1</tt> or if <tt>!(0 &lt;= columnIndexes[i] &lt; columns())</tt> for any <tt>i=0..columnIndexes.Length()-1</tt>.
        /// </exception>
        public virtual DoubleMatrix2D ViewSelection(int[] rowIndexes, int[] columnIndexes)
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
            var rowOffsets = new int[rowIndexes.Length];
            var columnOffsets = new int[columnIndexes.Length];
            for (int i = rowIndexes.Length; --i >= 0;)
                rowOffsets[i] = RowOffset(RowRank(rowIndexes[i]));
            for (int i = columnIndexes.Length; --i >= 0;)
                columnOffsets[i] = ColumnOffset(ColumnRank(columnIndexes[i]));
            return ViewSelectionLike(rowOffsets, columnOffsets);
        }

        /// <summary>
        /// Constructs and returns a new <i>selection view</i> that is a matrix holding all <b>rows</b> matching the given condition.
        /// Applies the condition to each row and takes only those row where <tt>condition.apply(viewRow(i))</tt> yields <tt>true</tt>.
        /// To match columns, use a dice view.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The new view.
        /// </returns>
        public virtual DoubleMatrix2D ViewSelection(DoubleMatrix1DProcedure condition)
        {
            var matches = new IntArrayList();
            for (int i = 0; i < Rows; i++)
            {
                if (condition(ViewRow(i))) matches.Add(i);
            }

            return ViewSelection(matches.ToArray(), null); // take all columns
        }

        /// <summary>
        /// Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the given column.
        /// This sort is guaranteed to be <i>stable</i>.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// A new sorted vector (matrix) view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>column &lt; 0 || column &gt;= columns()</tt>.
        /// </exception>
        public virtual DoubleMatrix2D ViewSorted(int column)
        {
            return Sorting.MergeSort.Sort(this, column);
        }

        /// <summary>
        /// Constructs and returns a new <i>stride view</i> which is a sub matrix consisting of every i-th cell.
        /// More specifically, the view has <tt>this.rows()/rStride</tt> rows and <tt>this.columns()/cStride</tt> columns holding cells <tt>this.Get(i*rStride,j*cStride)</tt> for all <tt>i = 0..rows()/rStride - 1, j = 0..columns()/cStride - 1</tt>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// </summary>
        /// <param name="rStride">
        /// The row step factor.
        /// </param>
        /// <param name="cStride">
        /// The column step factor.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>rStride &lt;= 0 || cStride &lt;= 0</tt>.
        /// </exception>
        public virtual DoubleMatrix2D ViewStrides(int rStride, int cStride)
        {
            return (DoubleMatrix2D)View().VStrides(rStride, cStride);
        }

        /// <summary>
        /// 8 neighbor stencil transformationd For efficient finite difference operations.
        /// Applies a function to a moving <tt>3 x 3</tt> window.
        /// Does nothing if <tt>rows() &lt; 3 || columns() &lt; 3</tt>.
        /// </summary>
        /// <param name="b">
        /// The matrix to hold the results.
        /// </param>
        /// <param name="function">
        /// The function to be applied to the 9 cells.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <tt>function==null</tt>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>rows() != B.rows() || columns() != B.columns()</tt>.
        /// </exception>
        public virtual void ZAssign8Neighbors(DoubleMatrix2D b, Double9Function function)
        {
            if (function == null) throw new ArgumentNullException("function", Cern.LocalizedResources.Instance().Exception_FuncionMustNotBeNull);
            CheckShape(b);
            if (Rows < 3 || Columns < 3) return; // nothing to do
            int r = Rows - 1;
            int c = Columns - 1;
            for (int i = 1; i < r; i++)
            {
                double a00 = this[i - 1, 0];
                double a01 = this[i - 1, 1];
                double a10 = this[i, 0];
                double a11 = this[i, 1];
                double a20 = this[i + 1, 0];
                double a21 = this[i + 1, 1];

                for (int j = 1; j < c; j++)
                {
                    // in each step six cells can be remembered in registers - they don't need to be reread from slow memory
                    // in each step 3 instead of 9 cells need to be read from memory.
                    double a02 = this[i - 1, j + 1];
                    double a12 = this[i, j + 1];
                    double a22 = this[i + 1, j + 1];

                    b[i, j] = function(a00, a01, a02, a10, a11, a12, a20, a21, a22);

                    a00 = a01;
                    a10 = a11;
                    a20 = a21;

                    a01 = a02;
                    a11 = a12;
                    a21 = a22;
                }
            }
        }

        /// <summary>
        /// Linear algebraic matrix-vector multiplication; <tt>z = A * y</tt>; 
        /// Equivalent to <tt>return A.zMult(y,z,1,0);</tt>
        /// </summary>
        /// <param name="y">
        /// The matrix.
        /// </param>
        /// <param name="z">
        /// The vector.
        /// </param>
        /// <returns>
        /// The matrix-vector multiplication.
        /// </returns>
        public DoubleMatrix1D ZMult(DoubleMatrix1D y, DoubleMatrix1D z)
        {
            return ZMult(y, z, 1, (z == null ? 1 : 0), false);
        }

        /// <summary>
        /// Linear algebraic matrix-vector multiplication; <tt>z = alpha * A * y + beta*z</tt>.
        /// </summary>
        /// <param name="y">
        /// The source vector.
        /// </param>
        /// <param name="z">
        /// The vector where results are to be storedd Set this parameter to <tt>null</tt> to indicate that a new result vector shall be constructed.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="beta">
        /// The beta.
        /// </param>
        /// <param name="transposeA">
        /// Whether A must be transposed.
        /// </param>
        /// <returns>
        /// z (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>A.columns() != y.Count || A.rows() &gt; z.Count)</tt>.
        /// </exception>
        public virtual DoubleMatrix1D ZMult(DoubleMatrix1D y, DoubleMatrix1D z, double alpha, double beta, bool transposeA)
        {
            if (transposeA) return ViewDice().ZMult(y, z, alpha, beta, false);
            if (z == null) z = new DenseDoubleMatrix1D(Rows);
            if (Columns != y.Size || Rows > z.Size)
                throw new ArgumentOutOfRangeException("Incompatible args: " + this + ", " + y + ", " + z);

            for (int i = Rows; --i >= 0;)
            {
                double s = 0;
                for (int j = Columns; --j >= 0;)
                    s += this[i, j] * y[j];
                z[i] = (alpha * s) + (beta * z[i]);
            }

            return z;
        }

        /// <summary>
        /// Linear algebraic matrix-matrix multiplication; <tt>C = A x B</tt>.
        /// </summary>
        /// <param name="b">
        /// The matrix B.
        /// </param>
        /// <param name="c">
        /// The matrix C.
        /// </param>
        /// <returns>
        /// The matrix C (for convenience only).
        /// </returns>
        public DoubleMatrix2D ZMult(DoubleMatrix2D b, DoubleMatrix2D c)
        {
            return ZMult(b, c, 1, (c == null ? 1 : 0), false, false);
        }

        /// <summary>
        /// Linear algebraic matrix-matrix multiplication; <tt>C = alpha * A x B + beta*C</tt>.
        /// </summary>
        /// <param name="b">
        /// The second source matrix.
        /// </param>
        /// <param name="c">
        /// The matrix where results are to be storedd Set this parameter to <tt>null</tt> to indicate that a new result matrix shall be constructed.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="beta">
        /// The beta.
        /// </param>
        /// <param name="transposeA">
        /// Whether A must be transposed.
        /// </param>
        /// <param name="transposeB">
        /// Whether B must be transposed.
        /// </param>
        /// <returns>
        /// C (for convenience only).
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>B.rows() != A.columns()</tt>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <tt>C.rows() != A.rows() || C.columns() != B.columns()</tt>.
        /// </exception>
        /// <exception cref="ArithmeticException">
        /// If <tt>A == C || B == C</tt>.
        /// </exception>
        public virtual DoubleMatrix2D ZMult(DoubleMatrix2D b, DoubleMatrix2D c, double alpha, double beta, bool transposeA, bool transposeB)
        {
            if (transposeA) return ViewDice().ZMult(b, c, alpha, beta, false, transposeB);
            if (transposeB) return ZMult(b.ViewDice(), c, alpha, beta, false, false);

            int m = Rows;
            int n = Columns;
            int p = b.Columns;

            if (c == null) c = new DenseDoubleMatrix2D(m, p);
            if (b.Rows != n)
                throw new ArgumentOutOfRangeException("b", String.Format(Cern.LocalizedResources.Instance().Exception_Matrix2DInnerDimensionMustAgree, this, b));
            if (c.Rows != m || c.Columns != p)
                throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_IncompatibleResultMatrix, this , b , c));
            if (this == c || b == c)
                throw new ArithmeticException(Cern.LocalizedResources.Instance().Exception_MatricesMustNotBeIdentical);

            for (int j = p; --j >= 0;)
            {
                for (int i = m; --i >= 0;)
                {
                    double s = 0;
                    for (int k = n; --k >= 0;)
                        s += this[i, k] * b[k, j];
                    c[i, j] = (alpha * s) + (beta * c[i, j]);
                }
            }

            return c;
        }

        /// <summary>
        /// Returns the sum of all cells; <tt>Sum( x[i,j] )</tt>.
        /// </summary>
        /// <returns>
        /// The sum of all cells.
        /// </returns>
        public virtual double ZSum()
        {
            if (Size == 0) return 0;
            return Aggregate(BinaryFunctions.Plus, UnaryFunctions.Identity);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, sharing the same cells.
        /// </summary>
        /// <param name="size">
        /// The number of cells the matrix shall have.
        /// </param>
        /// <param name="zero">
        /// The index of the first element.
        /// </param>
        /// <param name="stride">
        /// The number of indexes between any two elements, i.ed <tt>index(i+1)-index(i)</tt>.
        /// </param>
        /// <returns>
        /// A new matrix of the corresponding dynamic type.
        /// </returns>
        public abstract DoubleMatrix1D Like1D(int size, int zero, int stride);

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        /// <returns>
        /// The content of this matrix if it is a wrapper; or <tt>this</tt> otherwise.
        /// </returns>
        protected DoubleMatrix2D GetContent()
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
        protected bool HaveSharedCells(DoubleMatrix2D other)
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
        protected virtual bool HaveSharedCellsRaw(DoubleMatrix2D other)
        {
            return false;
        }

        /// <summary>
        /// Constructs and returns a new view equal to the receiver.
        /// </summary>
        /// <returns>
        /// A new view equal to the receiver.
        /// </returns>
        protected DoubleMatrix2D View()
        {
            return (DoubleMatrix2D)Clone();
        }

        /// <summary>
        /// Construct and returns a new selection view.
        /// </summary>
        /// <param name="rowOffsets">
        /// The row offsets of the visible elements.
        /// </param>
        /// <param name="cOffsets">
        /// The column offsets of the visible elements.
        /// </param>
        /// <returns>
        /// A new view.
        /// </returns>
        protected abstract DoubleMatrix2D ViewSelectionLike(int[] rowOffsets, int[] cOffsets);

        public virtual IEnumerator<double> GetEnumerator()
        {
            for(int i = 0; i< Rows; i++)
            {
                for (int j = 0; j< Columns; j++)
                {
                    yield return this[i, j];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Element
        {
            public int Row;

            public int Column;

            public double Value;
        }
    }
}
