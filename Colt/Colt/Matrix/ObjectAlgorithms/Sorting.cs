using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix;
using Cern.Colt.Matrix.Implementation;
using Cern.Colt.Function;

namespace Cern.Colt.Matrix.ObjectAlgorithms
{
    public delegate int ObjectMatrix1DComparator(ObjectMatrix1D o1, ObjectMatrix1D o2);

    public delegate int ObjectMatrix2DComparator(ObjectMatrix2D o1, ObjectMatrix2D o2);

    public class Sorting : PersistentObject
    {

        protected delegate void RunSortExec(int[] a, int fromIndex, int toIndex, IntComparator c);

        protected delegate void RunSortSwapExec(int fromIndex, int toIndex, IntComparator c, Cern.Colt.Swapper swapper);

        protected RunSortExec SortExecuter;
        protected RunSortSwapExec SortSwapExecuter;

        /// <summary>
        /// A prefabricated quicksort.
        /// </summary>
        public static Sorting quickSort = new Sorting(); // already has quicksort implemented

        /// <summary>
        /// A prefabricated mergesort.
        /// </summary>
        public static Sorting mergeSort = new Sorting()
        {
            // override quicksort with mergesort
            SortExecuter = new RunSortExec((a, fromIndex, toIndex, c) =>
            {
                Cern.Colt.Sorting.MergeSort(a, fromIndex, toIndex, c);
            }),

            SortSwapExecuter = new RunSortSwapExec((fromIndex, toIndex, c, swapper) =>
            {
                Cern.Colt.GenericSorting.MergeSort(fromIndex, toIndex, c, swapper);
            })
        };



        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Sorting() { }
        protected void RunSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            Cern.Colt.Sorting.QuickSort(a, fromIndex, toIndex, c);
        }
        protected void RunSort(int fromIndex, int toIndex, IntComparator c, Cern.Colt.Swapper swapper)
        {
            Cern.Colt.GenericSorting.QuickSort(fromIndex, toIndex, c, swapper);
        }
        /// <summary>
        /// Sorts the vector into ascending order, according to the <i>natural ordering</i>.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To sort ranges use sub-ranging viewsd To sort descending, use flip views ...
        /// <p>
        /// <b>Example:</b> 
        /// <table border="1" cellspacing="0">
        ///           <tr nowrap> 
        ///             <td valign="top"><i> 7, 1, 3, 1<br>
        ///               </i></td>
        ///             <td valign="top"> 
        ///               <p><i> ==&gt; 1, 1, 3, 7<br>
        ///                 The vector IS NOT SORTED.<br>
        ///                 The new VIEW IS SORTED.</i></p>
        ///             </td>
        ///           </tr>
        /// </table>
        /// 
        /// @param vector the vector to be sorted.
        /// @return a new sorted vector (matrix) viewd 
        ///                 <b>Note that the original matrix is left unaffected.</b>
        /// </summary>
        public ObjectMatrix1D sort(ObjectMatrix1D vector)
        {
            int[] indexes = new int[vector.Size]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;) indexes[i] = i;

            IntComparator comp = new IntComparator((a, b) =>
            {
                IComparable av = (IComparable)(vector[a]);
                IComparable bv = (IComparable)(vector[b]);
                int r = av.CompareTo(bv);
                return r < 0 ? -1 : (r > 0 ? 1 : 0);
            });

            RunSort(indexes, 0, indexes.Length, comp);

            return vector.ViewSelection(indexes);
        }
        /// <summary>
        /// Sorts the vector into ascending order, according to the order induced by the specified comparator.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// The algorithm compares two cells at a time, determinining whether one is smaller, equal or larger than the other.
        /// To sort ranges use sub-ranging viewsd To sort descending, use flip views ...
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// // sort by sinus of cells
        /// ObjectComparator comp = new ObjectComparator() {
        /// &nbsp;&nbsp;&nbsp;public int compare(Object a, Object b) {
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Object as = System.Math.Sin(a); Object bs = System.Math.Sin(b);
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return as %lt; bs ? -1 : as == bs ? 0 : 1;
        /// &nbsp;&nbsp;&nbsp;}
        /// };
        /// sorted = quickSort(vector,comp);
        /// </pre>
        /// 
        /// @param vector the vector to be sorted.
        /// @param c the comparator to determine the order.
        /// @return a new matrix view sorted as specified.
        ///             <b>Note that the original vector (matrix) is left unaffected.</b>
        /// </summary>
        public ObjectMatrix1D sort(ObjectMatrix1D vector, IComparer<ObjectMatrix1D> c)
        {
            int[] indexes = new int[vector.Size]; // row indexes to reorder instead of matrix itself
            for (int i = indexes.Length; --i >= 0;) indexes[i] = i;

            IntComparator comp = new IntComparator((a, b) => { return c.Compare((ObjectMatrix1D)vector[a], (ObjectMatrix1D)vector[b]); });
            {
            };

            RunSort(indexes, 0, indexes.Length, comp);

            return vector.ViewSelection(indexes);
        }
        /// <summary>
        /// Sorts the matrix rows into ascending order, according to the <i>natural ordering</i> of the matrix values in the given column.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To sort ranges use sub-ranging viewsd To sort columns by rows, use dice viewsd To sort descending, use flip views ...
        /// <p>
        /// <b>Example:</b> 
        /// <table border="1" cellspacing="0">
        ///   <tr nowrap> 
        /// 	<td valign="top"><i>4 x 2 matrix: <br>
        /// 	  7, 6<br>
        /// 	  5, 4<br>
        /// 	  3, 2<br>
        /// 	  1, 0 <br>
        /// 	  </i></td>
        /// 	<td align="left" valign="top"> 
        /// 	  <p><i>column = 0;<br>
        /// 		view = quickSort(matrix,column);<br>
        /// 		Console.WriteLine(view); </i><i><br>
        /// 		==> </i></p>
        /// 	  </td>
        /// 	<td valign="top"> 
        /// 	  <p><i>4 x 2 matrix:<br>
        /// 		1, 0<br>
        /// 		3, 2<br>
        /// 		5, 4<br>
        /// 		7, 6</i><br>
        /// 		The matrix IS NOT SORTED.<br>
        /// 		The new VIEW IS SORTED.</p>
        /// 	  </td>
        ///   </tr>
        /// </table>
        ///
        /// @param matrix the matrix to be sorted.
        /// @param column the index of the column inducing the order.
        /// @return a new matrix view having rows sorted by the given column.
        /// 		<b>Note that the original matrix is left unaffected.</b>
        /// @throws IndexOutOfRangeException if <i>column %lt; 0 || column >= matrix.Columns</i>.
        /// </summary>
        public ObjectMatrix2D sort(ObjectMatrix2D matrix, int column)
        {
            if (column < 0 || column >= matrix.Columns) throw new IndexOutOfRangeException("column=" + column + ", matrix=" + Formatter.Shape(matrix));

            int[] rowIndexes = new int[matrix.Rows]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;) rowIndexes[i] = i;

            ObjectMatrix1D col = matrix.ViewColumn(column);
            IntComparator comp = new IntComparator((a, b) =>
            {
                IComparable av = (IComparable)(col[a]);
                IComparable bv = (IComparable)(col[b]);
                int r = av.CompareTo(bv);
                return r < 0 ? -1 : (r > 0 ? 1 : 0);
            });


            RunSort(rowIndexes, 0, rowIndexes.Length, comp);

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.ViewSelection(rowIndexes, null);
        }
        /// <summary>
        /// Sorts the matrix rows according to the order induced by the specified comparator.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// The algorithm compares two rows (1-d matrices) at a time, determinining whether one is smaller, equal or larger than the other.
        /// To sort ranges use sub-ranging viewsd To sort columns by rows, use dice viewsd To sort descending, use flip views ...
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// // sort by sum of values in a row
        /// ObjectMatrix1DComparator comp = new ObjectMatrix1DComparator() {
        /// &nbsp;&nbsp;&nbsp;public int compare(ObjectMatrix1D a, ObjectMatrix1D b) {
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Object as = a.zSum(); Object bs = b.zSum();
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return as %lt; bs ? -1 : as == bs ? 0 : 1;
        /// &nbsp;&nbsp;&nbsp;}
        /// };
        /// sorted = quickSort(matrix,comp);
        /// </pre>
        ///
        /// @param matrix the matrix to be sorted.
        /// @param c the comparator to determine the order.
        /// @return a new matrix view having rows sorted as specified.
        /// 		<b>Note that the original matrix is left unaffected.</b>
        /// </summary>
        public ObjectMatrix2D sort(ObjectMatrix2D matrix, ObjectMatrix1DComparator c)
        {
            int[] rowIndexes = new int[matrix.Rows]; // row indexes to reorder instead of matrix itself
            for (int i = rowIndexes.Length; --i >= 0;) rowIndexes[i] = i;

            ObjectMatrix1D[] views = new ObjectMatrix1D[matrix.Rows]; // precompute views for speed
            for (int i = views.Length; --i >= 0;) views[i] = matrix.ViewRow(i);

            IntComparator comp = new IntComparator((a, b) => { return c(views[a], views[b]); });

            RunSort(rowIndexes, 0, rowIndexes.Length, comp);

            // view the matrix according to the reordered row indexes
            // take all columns in the original order
            return matrix.ViewSelection(rowIndexes, null);
        }
        /// <summary>
        /// Sorts the matrix slices into ascending order, according to the <i>natural ordering</i> of the matrix values in the given <i>[row,column]</i> position.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// To sort ranges use sub-ranging viewsd To sort by other dimensions, use dice viewsd To sort descending, use flip views ...
        /// <p>
        /// The algorithm compares two 2-d slices at a time, determinining whether one is smaller, equal or larger than the other.
        /// Comparison is based on the cell <i>[row,column]</i> within a slice.
        /// Let <i>A</i> and <i>B</i> be two 2-d slicesd Then we have the following rules
        /// <ul>
        /// <li><i>A &lt;  B  iff A.Get(row,column) &lt;  B.Get(row,column)</i>
        /// <li><i>A == B iff A.Get(row,column) == B.Get(row,column)</i>
        /// <li><i>A &gt;  B  iff A.Get(row,column) &gt;  B.Get(row,column)</i>
        /// </ul>
        ///
        /// @param matrix the matrix to be sorted.
        /// @param row the index of the row inducing the order.
        /// @param column the index of the column inducing the order.
        /// @return a new matrix view having slices sorted by the values of the slice view <i>matrix.viewRow(row).viewColumn(column)</i>.
        /// 		<b>Note that the original matrix is left unaffected.</b>
        /// @throws IndexOutOfRangeException if <i>row %lt; 0 || row >= matrix.Rows || column %lt; 0 || column >= matrix.Columns</i>.
        /// </summary>
        public ObjectMatrix3D sort(ObjectMatrix3D matrix, int row, int column)
        {
            if (row < 0 || row >= matrix.Rows) throw new IndexOutOfRangeException("row=" + row + ", matrix=" + Formatter.Shape(matrix));
            if (column < 0 || column >= matrix.Columns) throw new IndexOutOfRangeException("column=" + column + ", matrix=" + Formatter.Shape(matrix));

            int[] sliceIndexes = new int[matrix.Slices]; // indexes to reorder instead of matrix itself
            for (int i = sliceIndexes.Length; --i >= 0;) sliceIndexes[i] = i;

            ObjectMatrix1D sliceView = matrix.ViewRow(row).ViewColumn(column);
            IntComparator comp = new IntComparator((a, b) =>
            {
                IComparable av = (IComparable)(sliceView[a]);
                IComparable bv = (IComparable)(sliceView[b]);
                int r = av.CompareTo(bv);
                return r < 0 ? -1 : (r > 0 ? 1 : 0);
            });

            RunSort(sliceIndexes, 0, sliceIndexes.Length, comp);

            // view the matrix according to the reordered slice indexes
            // take all rows and columns in the original order
            return matrix.ViewSelection(sliceIndexes, null, null);
        }
        /// <summary>
        /// Sorts the matrix slices according to the order induced by the specified comparator.
        /// The returned view is backed by this matrix, so changes in the returned view are reflected in this matrix, and vice-versa.
        /// The algorithm compares two slices (2-d matrices) at a time, determinining whether one is smaller, equal or larger than the other.
        /// To sort ranges use sub-ranging viewsd To sort by other dimensions, use dice viewsd To sort descending, use flip views ...
        /// <p>
        /// <b>Example:</b>
        /// <pre>
        /// sort by sum of values in a slice
        /// ObjectMatrix2DComparator comp = new ObjectMatrix2DComparator() {
        /// &nbsp;&nbsp;&nbsp;public int compare(ObjectMatrix2D a, ObjectMatrix2D b) {
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Object as = a.zSum(); Object bs = b.zSum();
        /// &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return as %lt; bs ? -1 : as == bs ? 0 : 1;
        /// &nbsp;&nbsp;&nbsp;}
        /// };
        /// sorted = quickSort(matrix,comp);
        /// </pre>
        ///
        /// @param matrix the matrix to be sorted.
        /// @param c the comparator to determine the order.
        /// @return a new matrix view having slices sorted as specified.
        /// 		<b>Note that the original matrix is left unaffected.</b>
        /// </summary>
        public ObjectMatrix3D sort(ObjectMatrix3D matrix, ObjectMatrix2DComparator c)
        {
            int[] sliceIndexes = new int[matrix.Slices];

            // indexes to reorder instead of matrix itself
            for (int i = sliceIndexes.Length; --i >= 0;) sliceIndexes[i] = i;

            ObjectMatrix2D[] views = new ObjectMatrix2D[matrix.Slices]; // precompute views for speed
            for (int i = views.Length; --i >= 0;) views[i] = matrix.ViewSlice(i);

            IntComparator comp = new IntComparator((a, b) => { return c(views[a], views[b]); });

            RunSort(sliceIndexes, 0, sliceIndexes.Length, comp);

            // view the matrix according to the reordered slice indexes
            // take all rows and columns in the original order
            return matrix.ViewSelection(sliceIndexes, null, null);
        }
    }
}
