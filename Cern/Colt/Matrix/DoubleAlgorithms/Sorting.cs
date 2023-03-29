// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sorting.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Matrix quicksorts and mergesorts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Matrix.DoubleAlgorithms
{
    using System;

    using Function;

    using Implementation;

    /// <summary>
    /// Matrix quicksorts and mergesorts.
    /// </summary>
    public class Sorting : PersistentObject
    {
        /// <summary>
        /// A prefabricated quicksort.
        /// </summary>
        public static readonly Sorting QuickSort = new Sorting();

        /// <summary>
        /// A prefabricated mergesort.
        /// </summary>
        public static readonly Sorting MergeSort = new MergeSort();

        /// <summary>
        /// Sorts the vector into ascending order, according to the <i>natural ordering</i>.
        /// </summary>
        /// <param name="vector">
        /// The  vector to be sorted.
        /// </param>
        /// <returns>
        /// A new sorted vector (matrix) viewd 
        /// </returns>
        public IDoubleMatrix1D Sort(IDoubleMatrix1D vector)
        {
            var indexes = new int[vector.Size]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;)
                indexes[i] = i;

            RunSort(
                indexes,
                0,
                indexes.Length,
                (a, b) =>
                {
                    double av = vector[a];
                    double bv = vector[b];
                    if (Double.IsNaN(av) || Double.IsNaN(bv)) return CompareNaN(av, bv); // swap NaNs to the end
                    return av < bv ? -1 : (av == bv ? 0 : 1);
                });

            return vector.ViewSelection(indexes);
        }

        /// <summary>
        /// Sorts the vector into ascending order, according to the order induced by the specified comparator.
        /// </summary>
        /// <param name="vector">
        /// The vector to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order.
        /// </param>
        /// <returns>
        /// A new matrix view sorted as specified.
        /// </returns>
        public IDoubleMatrix1D Sort(IDoubleMatrix1D vector, DoubleComparatorDelegate c)
        {
            var indexes = new int[vector.Size]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;) indexes[i] = i;

            RunSort(indexes, 0, indexes.Length, (a, b) => c(vector[a], vector[b]));

            return vector.ViewSelection(indexes);
        }

        /// <summary>
        /// Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the virtual column <i>aggregates</i>;
        /// Particularly efficient when comparing expensive aggregates, because aggregates need not be recomputed time and again, as is the case for comparator based sorts.
        /// Essentially, this algorithm makes expensive comparisons cheap.
        /// Normally each element of <i>aggregates</i> is a summary measure of a row.
        /// Speedup over comparator based sorting = <i>2*log(rows)</i>, on average.
        /// For this operation, quicksort is usually faster.
        /// </summary>
        /// <param name="matrix">
        /// The matrix to be sorted.
        /// </param>
        /// <param name="aggregates">
        /// The values to sort ond (As a side effect, this array will also get sorted).
        /// </param>
        /// <returns>
        /// A new matrix view having rows sorted.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <i>aggregates.Length != matrix.rows()</i>.
        /// </exception>
        public IDoubleMatrix2D Sort(IDoubleMatrix2D matrix, double[] aggregates)
        {
            int rows = matrix.Rows;
            if (aggregates.Length != rows) throw new ArgumentOutOfRangeException("aggregates", "aggregates.Length != matrix.rows()");

            // set up index reordering
            var indexes = new int[rows];
            for (int i = rows; --i >= 0;) indexes[i] = i;

            // sort indexes and aggregates
            RunSort(
                0,
                rows,
                (x, y) =>
                {
                    double a = aggregates[x];
                    double b = aggregates[y];
                    if (Double.IsNaN(a) || Double.IsNaN(b)) return CompareNaN(a, b); // swap NaNs to the end
                    return a < b ? -1 : (a == b) ? 0 : 1;
                },
                (x, y) =>
                {
                    int t1 = indexes[x]; indexes[x] = indexes[y]; indexes[y] = t1;
                    double t2 = aggregates[x]; aggregates[x] = aggregates[y]; aggregates[y] = t2;
                });

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.ViewSelection(indexes, null);
        }

        /// <summary>
        /// Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the given column.
        /// </summary>
        /// <param name="matrix">
        /// The matrix to be sorted.
        /// </param>
        /// <param name="column">
        /// The index of the column inducing the order.
        /// </param>
        /// <returns>
        /// A new matrix view having rows sorted by the given column.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>column &lt; 0 || column &gt;= matrix.columns()</i>.
        /// </exception>
        public IDoubleMatrix2D Sort(IDoubleMatrix2D matrix, int column)
        {
            if (column < 0 || column >= matrix.Columns) throw new IndexOutOfRangeException("column=" + column + ", matrix=" + AbstractFormatter.Shape(matrix));

            var rowIndexes = new int[matrix.Rows]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowIndexes[i] = i;
            }

            IDoubleMatrix1D col = matrix.ViewColumn(column);
            RunSort(
                rowIndexes,
                0,
                rowIndexes.Length,
                (a, b) =>
                {
                    double av = col[a];
                    double bv = col[b];
                    if (Double.IsNaN(av) || Double.IsNaN(bv)) return CompareNaN(av, bv); // swap NaNs to the end
                    return av < bv ? -1 : (av == bv ? 0 : 1);
                });

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.ViewSelection(rowIndexes, null);
        }

        /// <summary>
        /// Sorts the matrix rows according to the order induced by the specified comparator.
        /// </summary>
        /// <param name="matrix">
        /// The matrix to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order.
        /// </param>
        /// <returns>
        /// A new matrix view having rows sorted as specified.
        /// </returns>
        public IDoubleMatrix2D Sort(IDoubleMatrix2D matrix, DoubleMatrix1DComparator c)
        {
            var rowIndexes = new int[matrix.Rows]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;) rowIndexes[i] = i;

            var views = new IDoubleMatrix1D[matrix.Rows]; // precompute views for speed
            for (int i = views.Length; --i >= 0;) views[i] = matrix.ViewRow(i);

            RunSort(rowIndexes, 0, rowIndexes.Length, (a, b) => c(views[a], views[b]));

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.ViewSelection(rowIndexes, null);
        }

        public IDoubleMatrix3D Sort(DoubleMatrix3D matrix, int row, int column)
        {
            if (row < 0 || row >= matrix.Rows) throw new IndexOutOfRangeException("row=" + row + ", matrix=" + Formatter.Shape(matrix));
            if (column < 0 || column >= matrix.Columns) throw new IndexOutOfRangeException("column=" + column + ", matrix=" + Formatter.Shape(matrix));

            int[] sliceIndexes = new int[matrix.Slices]; // indexes to reorder instead of matrix itself
            for (int i = sliceIndexes.Length; --i >= 0;) sliceIndexes[i] = i;

            IDoubleMatrix1D sliceView = matrix.ViewRow(row).ViewColumn(column);
            IntComparatorDelegate comp = new IntComparatorDelegate((a, b) =>
            {
                double av = sliceView[a];
                double bv = sliceView[b];
                if (Double.IsNaN(av) || Double.IsNaN(bv)) return CompareNaN(av, bv); // swap NaNs to the end
                return av < bv ? -1 : (av == bv ? 0 : 1);
            }
                );


            RunSort(sliceIndexes, 0, sliceIndexes.Length, comp);

            // view the matrix according to the reordered slice indexes
            // take all rows and columns in the original order
            return matrix.ViewSelection(sliceIndexes, null, null);
        }

        public IDoubleMatrix3D Sort(IDoubleMatrix3D matrix, Cern.Colt.Matrix.DoubleAlgorithms.DoubleMatrix2DComparator c)
        {
            int[] sliceIndexes = new int[matrix.Slices]; // indexes to reorder instead of matrix itself
            for (int i = sliceIndexes.Length; --i >= 0;) sliceIndexes[i] = i;

            IDoubleMatrix2D[] views = new IDoubleMatrix2D[matrix.Slices]; // precompute views for speed
            for (int i = views.Length; --i >= 0;) views[i] = matrix.ViewSlice(i);

            IntComparatorDelegate comp = new IntComparatorDelegate((a, b) => { return c(views[a], views[b]); });


            RunSort(sliceIndexes, 0, sliceIndexes.Length, comp);

            // view the matrix according to the reordered slice indexes
            // take all rows and columns in the original order
            return matrix.ViewSelection(sliceIndexes, null, null);
        }

        protected void RunSort(int[] a, int fromIndex, int toIndex, IntComparatorDelegate c)
        {
            Cern.Colt.Sorting.QuickSort(a, fromIndex, toIndex, c);
        }

        protected void RunSort(int fromIndex, int toIndex, IntComparatorDelegate c, Swapper swapper)
        {
            GenericSorting.QuickSort(fromIndex, toIndex, c, swapper);
        }

        /// <summary>
        /// Compare two values, one of which is assumed to be Double.NaN
        /// </summary>
        /// <param name="a">
        /// The first value.
        /// </param>
        /// <param name="b">
        /// The second value.
        /// </param>
        /// <returns>
        /// The comparison.
        /// </returns>
        private static int CompareNaN(double a, double b)
        {
            if (Double.IsNaN(a))
            {
                if (Double.IsNaN(b)) return 0; // NaN equals NaN
                return 1; // e.gd NaN > 5
            }

            return -1; // e.gd 5 < NaN
        }
    }
}
