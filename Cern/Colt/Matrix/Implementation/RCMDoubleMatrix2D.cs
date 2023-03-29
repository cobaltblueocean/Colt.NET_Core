// <copyright file="RCMDoubleMatrix2D.cs" company="CERN">
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
using Cern.Colt.List;

namespace Cern.Colt.Matrix.Implementation
{
    /// <summary>
    /// Sparse row-compressed-modified 2-d matrix holding <i>double</i> elements.
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 0.9, 04/14/2000
    /// </summary>
    public class RCMDoubleMatrix2D : WrapperDoubleMatrix2D
    {

        /// <summary>
        /// The elements of the matrix.
        /// </summary>
        private IntArrayList[] indexes;
        private List<Double>[] values;

        /// <summary>
        /// Gets or sets the matrix cell value at coordinate <i>[row,column]</i>.
        /// 
        /// <p>Provided with invalid parameters this method may return invalid objects without throwing any exception.
        /// <b>You should only use this method when you are absolutely sure that the coordinate is within bounds.</b>
        /// Precondition (unchecked): <i>0 &lt;= column &lt; columns() && 0 &lt;= row &lt; rows()</i>.
        /// </summary>
        /// <param name="row">the index of the row-coordinate.</param>
        /// <param name="column">the index of the column-coordinate.</param>
        /// <returns>the value at the specified coordinate.</returns>
        public override double this[int row, int column]
        {
            get
            {
                int k = -1;
                if (indexes[row] != null) k = indexes[row].BinarySearch(column);
                if (k < 0) return 0;
                return values[row][k];
            }
            set
            {
                int i = row;
                int j = column;

                int k = -1;
                IntArrayList indexList = indexes[i];
                if (indexList != null) k = indexList.BinarySearch(j);

                if (k >= 0)
                { // found
                    if (value == 0)
                    {
                        List<Double> valueList = values[i];
                        indexList.Remove(k);
                        valueList.Remove(k);
                        int s = indexList.Count;
                        if (s > 2 && s * 3 < indexList.ToArray().Length)
                        {
                            indexList.SetSize(s * 3 / 2);
                            indexList.TrimToSize();
                            indexList.SetSize(s);

                            valueList.SetSize(s * 3 / 2);
                            valueList.TrimExcess();
                            valueList.SetSize(s);
                        }
                    }
                    else
                    {
                        values[i][k] = value;
                    }
                }
                else
                { // not found
                    if (value == 0) return;

                    k = -k - 1;

                    if (indexList == null)
                    {
                        indexes[i] = new IntArrayList(3);
                        values[i] = new List<Double>(3);
                    }
                    indexes[i].Insert(k, j);
                    values[i].Insert(k, value);
                }
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
        public RCMDoubleMatrix2D(double[][] values) : this(values.Length, values.Length == 0 ? 0 : values.GetLength(1))
        {
            base.Assign(values);
        }

        /// <summary>
        /// Constructs a matrix with a given number of rows and columns.
        /// All entries are initially <i>0</i>.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <exception cref="ArgumentException">if <i>rows &lt; 0 || columns &lt; 0 || (double)columns * rows > int.MaxValue</i>.</exception>
        public RCMDoubleMatrix2D(int rows, int columns) : base(null)
        {

            Setup(rows, columns);
            indexes = new IntArrayList[rows];
            values = new List<Double>[rows];
        }

        /// <summary>
        /// Sets all cells to the state specified by <i>value</i>.
        /// </summary>
        /// <param name="value">the value to be filled into the cells.</param>
        /// <returns><i>this</i> (for convenience only).</returns>
        public override IDoubleMatrix2D Assign(double value)
        {
            // overriden for performance only
            if (value == 0)
            {
                for (int row = Rows; --row >= 0;)
                {
                    indexes[row] = null;
                    values[row] = null;
                }
            }
            else base.Assign(value);
            return this;
        }

        /// <summary>
        /// Returns the content of this matrix if it is a wrapper; or <i>this</i> otherwise.
        /// Override this method in wrappers.
        /// </summary>
        /// <returns></returns>
        protected new IDoubleMatrix2D GetContent()
        {
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
        [Obsolete("GetQuick(int row, int column) is deprecated, please use indexer instead.")]
        public override double GetQuick(int row, int column)
        {
            int k = -1;
            if (indexes[row] != null) k = indexes[row].BinarySearch(column);
            if (k < 0) return 0;
            return values[row][k];
        }

        /// <summary>
        /// Construct and returns a new empty matrix <i>of the same dynamic type</i> as the receiver, having the specified number of rows and columns.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix2D</i> the new matrix must also be of type <i>DenseDoubleMatrix2D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix2D</i> the new matrix must also be of type <i>SparseDoubleMatrix2D</i>, etc.
        /// In general, the new matrix should have internal parametrization as similar as possible.
        /// </summary>
        /// <param name="rows">the number of rows the matrix shall have.</param>
        /// <param name="columns">the number of columns the matrix shall have.</param>
        /// <returns>a new empty matrix of the same dynamic type.</returns>
        public override IDoubleMatrix2D Like(int rows, int columns)
        {
            return new RCMDoubleMatrix2D(rows, columns);
        }

        /// <summary>
        /// Construct and returns a new 1-d matrix <i>of the corresponding dynamic type</i>, entirelly independent of the receiver.
        /// For example, if the receiver is an instance of type <i>DenseDoubleMatrix2D</i> the new matrix must be of type <i>DenseIDoubleMatrix1D</i>,
        /// if the receiver is an instance of type <i>SparseDoubleMatrix2D</i> the new matrix must be of type <i>SparseIDoubleMatrix1D</i>, etc.
        /// </summary>
        /// <param name="size">the number of cells the matrix shall have.</param>
        /// <returns>a new matrix of the corresponding dynamic type.</returns>
        public override IDoubleMatrix1D Like1D(int size)
        {
            return new SparseDoubleMatrix1D(size);
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
        [Obsolete("SetQuick(int row, int column, double value) is deprecated, please use indexer instead.")]
        public override void SetQuick(int row, int column, double value)
        {
            int i = row;
            int j = column;

            int k = -1;
            IntArrayList indexList = indexes[i];
            if (indexList != null) k = indexList.BinarySearch(j);

            if (k >= 0)
            { // found
                if (value == 0)
                {
                    List<Double> valueList = values[i];
                    indexList.Remove(k);
                    valueList.Remove(k);
                    int s = indexList.Count;
                    if (s > 2 && s * 3 < indexList.ToArray().Length)
                    {
                        indexList.SetSize(s * 3 / 2);
                        indexList.TrimToSize();
                        indexList.SetSize(s);

                        valueList.SetSize(s * 3 / 2);
                        valueList.TrimExcess();
                        valueList.SetSize(s);
                    }
                }
                else
                {
                    values[i][k] = value;
                }
            }
            else
            { // not found
                if (value == 0) return;

                k = -k - 1;

                if (indexList == null)
                {
                    indexes[i] = new IntArrayList(3);
                    values[i] = new List<Double>(3);
                }
                indexes[i].Insert(k, j);
                values[i].Insert(k, value);
            }
        }

        /// <summary>
        /// Linear algebraic matrix-vector multiplication; <i>z = A * y</i>.
        /// <i>z[i] = alpha*Sum(A[i,j] * y[j]) + beta*z[i], i=0..A.rows()-1, j=0..y.Count-1</i>.
        /// Where <i>A == this</i>.
        /// </summary>
        /// <param name="y">the source vector.</param>
        /// <param name="z">the vector where results are to be stored.</param>
        /// <param name="nonZeroIndexes"></param>
        /// <param name="allRows"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <exception cref="ArgumentException">if <i>A.columns() != y.Count || A.rows() > z.Count)</i>.</exception>
        protected void ZMult(IDoubleMatrix1D y, IDoubleMatrix1D z, IntArrayList nonZeroIndexes, IDoubleMatrix1D[] allRows, double alpha, double beta)
        {
            if (Columns != y.Size || Rows > z.Size)
                throw new ArgumentException(String.Format(Cern.LocalizedResources.Instance().Exception_IncompatibleArgs, ToStringShort(), y.ToStringShort(), z.ToStringShort()));

            z.Assign(Cern.Jet.Math.Functions.DoubleFunctions.Mult(beta / alpha));
            for (int i = indexes.Length; --i >= 0;)
            {
                if (indexes[i] != null)
                {
                    for (int k = indexes[i].Count; --k >= 0;)
                    {
                        int j = indexes[i][k];
                        double value = values[i][k];
                        z[i] = z[i] + value * y[j];
                    }
                }
            }

            z.Assign(Cern.Jet.Math.Functions.DoubleFunctions.Mult(alpha));
        }
    }
}
