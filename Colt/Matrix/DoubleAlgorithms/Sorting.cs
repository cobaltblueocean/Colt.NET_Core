// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sorting.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Matrix quicksorts and mergesorts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Colt.Matrix.DoubleAlgorithms
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
        /// A new sorted vector (matrix) view. 
        /// </returns>
        public DoubleMatrix1D Sort(DoubleMatrix1D vector)
        {
            var indexes = new int[vector.Size()]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;)
                indexes[i] = i;

            runSort(
                indexes,
                0,
                indexes.Length,
                (a, b) =>
                {
                    double av = vector[a];
                    double bv = vector[b];
                    if (av != av || bv != bv) return compareNaN(av, bv); // swap NaNs to the end
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
        public DoubleMatrix1D Sort(DoubleMatrix1D vector, DoubleComparator c)
        {
            var indexes = new int[vector.Size()]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;) indexes[i] = i;

            runSort(indexes, 0, indexes.Length, (a, b) => c(vector[a], vector[b]));

            return vector.ViewSelection(indexes);
        }

        /// <summary>
        /// Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the virtual column <tt>aggregates</tt>;
        /// Particularly efficient when comparing expensive aggregates, because aggregates need not be recomputed time and again, as is the case for comparator based sorts.
        /// Essentially, this algorithm makes expensive comparisons cheap.
        /// Normally each element of <tt>aggregates</tt> is a summary measure of a row.
        /// Speedup over comparator based sorting = <tt>2*log(rows)</tt>, on average.
        /// For this operation, quicksort is usually faster.
        /// </summary>
        /// <param name="matrix">
        /// The matrix to be sorted.
        /// </param>
        /// <param name="aggregates">
        /// The values to sort on. (As a side effect, this array will also get sorted).
        /// </param>
        /// <returns>
        /// A new matrix view having rows sorted.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <tt>aggregates.length != matrix.rows()</tt>.
        /// </exception>
        public DoubleMatrix2D Sort(DoubleMatrix2D matrix, double[] aggregates)
        {
            int rows = matrix.Rows;
            if (aggregates.Length != rows) throw new ArgumentOutOfRangeException("aggregates", "aggregates.length != matrix.rows()");

            // set up index reordering
            var indexes = new int[rows];
            for (int i = rows; --i >= 0;) indexes[i] = i;

            // sort indexes and aggregates
            runSort(
                0,
                rows,
                (x, y) =>
                {
                    double a = aggregates[x];
                    double b = aggregates[y];
                    if (a != a || b != b) return compareNaN(a, b); // swap NaNs to the end
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
        /// If <tt>column &lt; 0 || column &gt;= matrix.columns()</tt>.
        /// </exception>
        public DoubleMatrix2D Sort(DoubleMatrix2D matrix, int column)
        {
            if (column < 0 || column >= matrix.Columns) throw new IndexOutOfRangeException("column=" + column + ", matrix=" + AbstractFormatter.Shape(matrix));

            var rowIndexes = new int[matrix.Rows]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;)
            {
                rowIndexes[i] = i;
            }

            DoubleMatrix1D col = matrix.ViewColumn(column);
            runSort(
                rowIndexes,
                0,
                rowIndexes.Length,
                (a, b) =>
                {
                    double av = col[a];
                    double bv = col[b];
                    if (av != av || bv != bv) return compareNaN(av, bv); // swap NaNs to the end
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
        public DoubleMatrix2D Sort(DoubleMatrix2D matrix, DoubleMatrix1DComparator c)
        {
            var rowIndexes = new int[matrix.Rows]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;) rowIndexes[i] = i;

            var views = new DoubleMatrix1D[matrix.Rows]; // precompute views for speed
            for (int i = views.Length; --i >= 0;) views[i] = matrix.ViewRow(i);

            runSort(rowIndexes, 0, rowIndexes.Length, (a, b) => c(views[a], views[b]));

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.ViewSelection(rowIndexes, null);
        }

        protected void runSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            Colt.Sorting.QuickSort(a, fromIndex, toIndex, c);
        }

        protected void runSort(int fromIndex, int toIndex, IntComparator c, Swapper swapper)
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
        private static int compareNaN(double a, double b)
        {
            if (a != a)
            {
                if (b != b) return 0; // NaN equals NaN
                return 1; // e.g. NaN > 5
            }

            return -1; // e.g. 5 < NaN
        }
    }
}
