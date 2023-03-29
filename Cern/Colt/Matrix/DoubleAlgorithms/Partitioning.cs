// <copyright file="Partitioning.cs" company="CERN">
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
using Cern.Colt.Matrix;
using Cern.Colt.Function;

namespace Cern.Colt.Matrix.DoubleAlgorithms
{
    /// <summary>
    /// Given some interval boundaries, partitions matrices such that cell values falling into an interval are placed next to each other.
    /// <p>
    /// <b>Performance</b>
    /// <p>
    /// Partitioning into two intervals is <i>O( N )</i>.
    /// Partitioning into k intervals is <i>O( N * log(k))</i>.
    /// Constants factors are minimizedd 
    /// 
    /// @see cern.colt.Partitioning "Partitioning arrays (provides more documentation)"
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class Partitioning
    {
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Partitioning() { }

        /// <summary>
        /// Same as <see cref="Cern.Colt.Partitioning.Partition(int[], int, int, int[], int, int, int[])"/>
        /// except that it <i>synchronously</i> partitions the rows of the given matrix by the values of the given matrix column;
        /// This is essentially the same as partitioning a list of composite objects by some instance variable;
        /// In other words, two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Let's say, a "row" is an "object" (tuple, d-dimensional point).
        /// A "column" is the list of "object" values of a given variable (field, dimension).
        /// A "matrix" is a list of "objects" (tuples, points).
        /// <p>
        /// Now, rows (objects, tuples) are partially sorted according to their values in one given variable (dimension).
        /// Two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Note that arguments are not checked for validity.
        /// 
        /// </summary>
        /// <param name="matrix">
        /// the matrix to be partitioned.
        /// </param>
        /// <param name="rowIndexes">
        /// the index of the i-th row; is modified by this method to reflect partitioned indexes.
        /// </param>
        /// <param name="rowFrom">
        /// the index of the first row (inclusive).
        /// </param>
        /// <param name="rowTo">
        /// the index of the last row (inclusive).
        /// </param>
        /// <param name="column">
        /// the index of the column to partition on.
        /// </param>
        /// <param name="splitters">
        /// the values at which the rows shall be split into intervals.
        /// Must be sorted ascending and must not contain multiple identical values.
        /// These preconditions are not checked; be sure that they are met.
        /// </param>
        /// <param name="splitFrom">
        /// the index of the first splitter element to be considered.
        /// </param>
        /// <param name="splitTo">
        /// the index of the last splitter element to be considered.
        /// The method considers the splitter elements<i>splitters[splitFrom] .d splitters[splitTo]</i>.
        /// </param>
        /// <param name="splitIndexes">
        /// a list into which this method fills the indexes of rows delimiting intervals.
        /// Upon return <i>splitIndexes[splitFrom..splitTo]</i> will be set accordingly.
        /// Therefore, must satisfy <i>splitIndexes.Length >= splitters.Length</i>.
        /// </param>
        /// <example>
        /// <table border="1" cellspacing="0">
        ///           <tr nowrap> 
        ///             <td valign="top"><i>8 x 3 matrix:<br>
        ///               23, 22, 21<br>
        ///               20, 19, 18<br>
        ///               17, 16, 15<br>
        ///               14, 13, 12<br>
        ///               11, 10, 9<br>
        ///               8,  7,  6<br>
        ///               5,  4,  3<br>
        ///               2,  1,  0 </i></td>
        ///             <td align="left" valign="top"> 
        ///               <p><i>column = 0;<br>
        ///                 rowIndexes = {0,1,2,.d,matrix.Rows-1};
        ///                 rowFrom = 0;<br>
        ///                 rowTo = matrix.Rows-1;<br>
        ///                 splitters = {5,10,12}<br>
        ///                 c = 0; <br>
        ///                 d = splitters.Length-1;<br>
        ///                 partition(matrix,rowIndexes,rowFrom,rowTo,column,splitters,c,d,splitIndexes);<br>
        ///                 ==><br>
        ///                 splitIndexes == {0, 2, 3}<br>
        ///                 rowIndexes == {7, 6, 5, 4, 0, 1, 2, 3}</i></p>
        ///               </td>
        ///             <td valign="top">
        ///               The matrix IS NOT REORDERED.<br>
        ///               Here is how it would look<br>
        ///               like, if it would be reordered<br>
        ///               accoring to <i>rowIndexes</i>.<br>
        ///               <i>8 x 3 matrix:<br>
        ///               2,  1,  0<br>
        ///               5,  4,  3<br>
        ///               8,  7,  6<br>
        ///               11, 10, 9<br>
        ///               23, 22, 21<br>
        ///               20, 19, 18<br>
        ///               17, 16, 15<br>
        ///               14, 13, 12 </i></td>
        ///           </tr>
        /// </table>
        /// </example>
        public static void Partition(IDoubleMatrix2D matrix, int[] rowIndexes, int rowFrom, int rowTo, int column, double[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            if (rowFrom < 0 || rowTo >= matrix.Rows || rowTo >= rowIndexes.Length) throw new ArgumentException();
            if (column < 0 || column >= matrix.Columns) throw new ArgumentException();
            if (splitFrom < 0 || splitTo >= splitters.Length) throw new ArgumentException();
            if (splitIndexes.Length < splitters.Length) throw new ArgumentException();

            // this one knows how to swap two row indexes (a,b)
            int[] g = rowIndexes;
            Swapper swapper = new Swapper((b, c) =>
            {

                int tmp = g[b]; g[b] = g[c]; g[c] = tmp;
            });

            // compare splitter[a] with columnView[rowIndexes[b]]
            IDoubleMatrix1D columnView = matrix.ViewColumn(column);
            IntComparatorDelegate comp = new IntComparatorDelegate((a, b) =>
            {
                double av = splitters[a];
                double bv = columnView[g[b]];
                return av < bv ? -1 : (av == bv ? 0 : 1);
            });

            // compare columnView[rowIndexes[a]] with columnView[rowIndexes[b]]
            IntComparatorDelegate comp2 = new IntComparatorDelegate((a, b) =>
            {

                double av = columnView[g[a]];
                double bv = columnView[g[b]];
                return av < bv ? -1 : (av == bv ? 0 : 1);
            });

            // compare splitter[a] with splitter[b]
            IntComparatorDelegate comp3 = new IntComparatorDelegate((a, b) =>
            {

                double av = splitters[a];
                double bv = splitters[b];
                return av < bv ? -1 : (av == bv ? 0 : 1);
            });

            // generic partitioning does the main work of reordering row indexes
            Cern.Colt.Partitioning.GenericPartition(rowFrom, rowTo, splitFrom, splitTo, splitIndexes, comp, comp2, comp3, swapper);
        }

        /// <summary>
        /// Same as <see cref="Cern.Colt.Partitioning.Partition(int[], int, int, int[], int, int, int[])"/>
        /// except that it <i>synchronously</i> partitions the rows of the given matrix by the values of the given matrix column;
        /// This is essentially the same as partitioning a list of composite objects by some instance variable;
        /// In other words, two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Let's say, a "row" is an "object" (tuple, d-dimensional point).
        /// A "column" is the list of "object" values of a given variable (field, dimension).
        /// A "matrix" is a list of "objects" (tuples, points).
        /// <p>
        /// Now, rows (objects, tuples) are partially sorted according to their values in one given variable (dimension).
        /// Two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Note that arguments are not checked for validity.
        /// </summary>
        /// <param name="matrix">
        /// the matrix to be partitioned.
        /// </param>
        /// <param name="column">
        /// the index of the column to partition on.
        /// </param>
        /// <param name="splitters">
        /// the values at which the rows shall be split into intervals.
        ///             Must be sorted ascending and must not contain multiple identical values.
        ///             These preconditions are not checked; be sure that they are met.
        /// </param>
        /// <param name="splitIndexes">
        /// a list into which this method fills the indexes of rows delimiting intervals.
        /// Therefore, must satisfy <i>splitIndexes.Length >= splitters.Length</i>.
        /// </param>
        /// <returns>a new matrix view having rows partitioned by the given column and splitters.</returns>
        /// <example>
        /// <table border="1" cellspacing="0">
        ///           <tr nowrap> 
        ///             <td valign="top"><i>8 x 3 matrix:<br>
        ///               23, 22, 21<br>
        ///               20, 19, 18<br>
        ///               17, 16, 15<br>
        ///               14, 13, 12<br>
        ///               11, 10, 9<br>
        ///               8,  7,  6<br>
        ///               5,  4,  3<br>
        ///               2,  1,  0 </i></td>
        ///             <td align="left" valign="top"> 
        ///                 <i>column = 0;<br>
        ///                 splitters = {5,10,12}<br>
        ///                 partition(matrix,column,splitters,splitIndexes);<br>
        ///                 ==><br>
        ///                 splitIndexes == {0, 2, 3}</i></p>
        ///               </td>
        ///             <td valign="top">
        ///               The matrix IS NOT REORDERED.<br>
        ///               The new VIEW IS REORDERED:<br>
        ///               <i>8 x 3 matrix:<br>
        ///               2,  1,  0<br>
        ///               5,  4,  3<br>
        ///               8,  7,  6<br>
        ///               11, 10, 9<br>
        ///               23, 22, 21<br>
        ///               20, 19, 18<br>
        ///               17, 16, 15<br>
        ///               14, 13, 12 </i></td>
        ///           </tr>
        /// </table>
        /// </example>
        public static IDoubleMatrix2D Partition(IDoubleMatrix2D matrix, int column, double[] splitters, int[] splitIndexes)
        {
            int rowFrom = 0;
            int rowTo = matrix.Rows - 1;
            int splitFrom = 0;
            int splitTo = splitters.Length - 1;
            int[] rowIndexes = new int[matrix.Rows]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;) rowIndexes[i] = i;

            Partition(matrix, rowIndexes, rowFrom, rowTo, column, splitters, splitFrom, splitTo, splitIndexes);

            // take all columns in the original order
            int[] columnIndexes = new int[matrix.Columns];
            for (int i = columnIndexes.Length; --i >= 0;) columnIndexes[i] = i;

            // view the matrix according to the reordered row indexes
            return matrix.ViewSelection(rowIndexes, columnIndexes);
        }

        /// <summary>
        /// Same as <see cref="partition(int[], int, int, int[], int, int, int[])"/>
        /// except that it <i>synchronously</i> partitions the rows of the given matrix by the values of the given matrix column;
        /// This is essentially the same as partitioning a list of composite objects by some instance variable;
        /// In other words, two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Let's say, a "row" is an "object" (tuple, d-dimensional point).
        /// A "column" is the list of "object" values of a given variable (field, dimension).
        /// A "matrix" is a list of "objects" (tuples, points).
        /// <p>
        /// Now, rows (objects, tuples) are partially sorted according to their values in one given variable (dimension).
        /// Two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Of course, the column must not be a column of a different matrix.
        /// More formally, there must hold: <br>
        /// There exists an <i>i</i> such that <i>matrix.ViewColumn(i)==column</i>.
        /// <p>
        /// Note that arguments are not checked for validity.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="column"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitFrom"></param>
        /// <param name="splitTo"></param>
        /// <param name="splitIndexes"></param>
        /// <example>
        /// <table border="1" cellspacing="0">
        ///           <tr nowrap> 
        ///             <td valign="top"><i>8 x 3 matrix:<br>
        ///               23, 22, 21<br>
        ///               20, 19, 18<br>
        ///               17, 16, 15<br>
        ///               14, 13, 12<br>
        ///               11, 10, 9<br>
        ///               8,  7,  6<br>
        ///               5,  4,  3<br>
        ///               2,  1,  0 </i></td>
        ///             <td align="left"> 
        ///               <p><i>column = matrix.ViewColumn(0);<br>
        ///                 a = 0;<br>
        ///                 b = column.Count-1;</i><i><br>
        ///                 splitters={5,10,12}<br>
        ///                 c=0; <br>
        ///                 d=splitters.Length-1;</i><i><br>
        ///                 partition(matrix,column,a,b,splitters,c,d,splitIndexes);<br>
        ///                 ==><br>
        ///                 splitIndexes == {0, 2, 3}</i></p>
        ///               </td>
        ///             <td valign="top"><i>8 x 3 matrix:<br>
        ///               2,  1,  0<br>
        ///               5,  4,  3<br>
        ///               8,  7,  6<br>
        ///               11, 10, 9<br>
        ///               23, 22, 21<br>
        ///               20, 19, 18<br>
        ///               17, 16, 15<br>
        ///               14, 13, 12 </i></td>
        ///           </tr>
        /// </table>
        /// </example>
        private static void xPartitionOld(IDoubleMatrix2D matrix, IDoubleMatrix1D column, int from, int to, double[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            /*
            double splitter; // int, double --> template type dependent

            if (splitFrom>splitTo) return; // nothing to do
            if (from>to) { // all bins are empty
                from--;
                for (int i = splitFrom; i<=splitTo; ) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom==splitTo) { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else { // we do have a choice
                int m = (from+to) / 2;       // Small arrays, middle element
                int len = to-from+1;
                if (len > SMALL) {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM) {        // Big arrays, pseudomedian of 9
                        int s = len/8;
                        l = med3(column, l,     l+s, l+2*s);
                        m = med3(column, m-s,   m,   m+s);
                        n = med3(column, n-2*s, n-s, n);
                    }
                    m = med3(column, l, m, n); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                medianIndex = cern.colt.Sorting.BinarySearchFromTo(splitters,column.getQuick(m),splitFrom,splitTo);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int	splitIndex = xPartitionOld(matrix,column,from,to,splitter);
            splitIndexes[medianIndex] = splitIndex;

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from) { // no element falls into this bin
                // all bins with splitters[i] <= splitter are empty
                int i = medianIndex-1;
                while (i>=splitFrom && (!(splitter < splitters[i]))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex+1;
            }
            else if (splitIndex >= to) { // all elements fall into this bin
                // all bins with splitters[i] >= splitter are empty
                int i = medianIndex+1;
                while (i<=splitTo && (!(splitter > splitters[i]))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex-1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex-1) {
                xPartitionOld(matrix, column, from,         splitIndex, splitters, splitFrom, medianIndex-1,  splitIndexes);
            }

            // recursively partition right half
            if (medianIndex+1 <= splitTo) {
                xPartitionOld(matrix, column, splitIndex+1, to,         splitters, medianIndex+1,  splitTo,   splitIndexes);
            }
            */
        }

        /// <summary>
        /// Same as <see cref="partition(int[], int, int)"/> 
        /// except that it <i>synchronously</i> partitions the rows of the given matrix by the values of the given matrix column;
        /// This is essentially the same as partitioning a list of composite objects by some instance variable;
        /// In other words, two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Let's say, a "row" is an "object" (tuple, d-dimensional point).
        /// A "column" is the list of "object" values of a given variable (field, dimension).
        /// A "matrix" is a list of "objects" (tuples, points).
        /// <p>
        /// Now, rows (objects, tuples) are partially sorted according to their values in one given variable (dimension).
        /// Two entire rows of the matrix are swapped, whenever two column values indicate so.
        /// <p>
        /// Of course, the column must not be a column of a different matrix.
        /// More formally, there must hold: <br>
        /// There exists an <i>i</i> such that <i>matrix.ViewColumn(i)==column</i>.
        ///
        /// Note that arguments are not checked for validity.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="column"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        private static int xPartitionOld(IDoubleMatrix2D matrix, IDoubleMatrix1D column, int from, int to, double splitter)
        {
            /*
            double element;  // int, double --> template type dependent
            for (int i=from-1; ++i<=to; ) {
                element = column.getQuick(i);
                if (element < splitter) {
                    // swap x[i] with x[from]
                    matrix.swapRows(i,from);
                    from++;
                }
            }
            return from-1;
            */
            return 0;
        }
    }
}
