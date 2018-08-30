namespace Cern.Colt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Function;

    /// <summary>
    /// Quicksorts, mergesorts and binary searches.
    /// </summary>
    public static class Sorting
    {
        #region Local Variables
        /// <summary>
        /// Arrays with less than this number of elements are considered small.
        /// </summary>
        private const int SMALL = 7;

        /// <summary>
        /// Arrays with more than this number of elements are considered big.
        /// </summary>
        private const int MEDIUM = 40;

        #endregion

        #region Property

        #endregion

        #region Constructor

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(int[] a, int value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo(int[] a, int value, int fromIndex, int toIndex, IntComparator comp)
        {
            var c = new IntComparatorWrapper();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(byte[] a, byte value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo(byte[] a, byte value, int fromIndex, int toIndex, ByteComparator comp)
        {
            var c = new ByteComparatorWrapper();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(char[] a, char value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo(char[] a, char value, int fromIndex, int toIndex, CharComparator comp)
        {
            var c = new CharComparatorWrapper();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(float[] a, float value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo(float[] a, float value, int fromIndex, int toIndex, FloatComparator comp)
        {
            var c = new FloatComparatorWrapper();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(double[] a, double value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo(double[] a, double value, int fromIndex, int toIndex, DoubleComparator comp)
        {
            var c = new DoubleComparatorWrapper();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(long[] a, long value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo(long[] a, long value, int fromIndex, int toIndex, LongComparator comp)
        {
            var c = new LongComparatorWrapper();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(short[] a, short value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo(short[] a, short value, int fromIndex, int toIndex, ShortComparator comp)
        {
            var c = new ShortComparatorWrapper();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <returns></returns>
        public static int BinarySearchFromTo(Object[] a, Object value, int fromIndex, int toIndex)
        {
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value);
        }

        /// <summary>
        /// Searches a specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="value"></param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        /// <returns></returns>
        public static int BinarySearchFromTo<T>(T[] a, T value, int fromIndex, int toIndex, ObjectComparator<T> comp)
        {
            var c = new ObjectComparatorWrapper<T>();
            c.comparator = comp;
            return Array.BinarySearch(a, fromIndex, (toIndex - fromIndex), value, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparator.
        /// </summary>
        /// <param name="a">
        /// The array to be sorted.
        /// </param>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <i>fromIndex &gt; toIndex</i>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <i>fromIndex &lt; 0</i> or <i>toIndex &gt; a.Length</i>
        /// </exception>
        public static void QuickSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            QuickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        ///
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSort(byte[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            byte[] aux = (byte[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        ///
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @param c the comparator to determine the order of the array.
        /// @throws ClassCastException if the array contains elements that are not
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// @see Comparator
        /// </summary>
        public static void MergeSort(byte[] a, int fromIndex, int toIndex, ByteComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            byte[] aux = (byte[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        ///
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSort(char[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            char[] aux = (char[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        ///
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @param c the comparator to determine the order of the array.
        /// @throws ClassCastException if the array contains elements that are not
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// @see Comparator
        /// </summary>
        public static void MergeSort(char[] a, int fromIndex, int toIndex, CharComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            char[] aux = (char[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        ///
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSort(double[] a, int fromIndex, int toIndex)
        {
            MergeSort2(a, fromIndex, toIndex);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        ///
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @param c the comparator to determine the order of the array.
        /// @throws ClassCastException if the array contains elements that are not
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// @see Comparator
        /// </summary>
        public static void MergeSort(double[] a, int fromIndex, int toIndex, DoubleComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            double[] aux = (double[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        ///
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSort(float[] a, int fromIndex, int toIndex)
        {
            MergeSort2(a, fromIndex, toIndex);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        ///
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @param c the comparator to determine the order of the array.
        /// @throws ClassCastException if the array contains elements that are not
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// @see Comparator
        /// </summary>
        public static void MergeSort(float[] a, int fromIndex, int toIndex, FloatComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            float[] aux = (float[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        ///
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSort(int[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            int[] aux = (int[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        ///
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @param c the comparator to determine the order of the array.
        /// @throws ClassCastException if the array contains elements that are not
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// @see Comparator
        /// </summary>
        public static void MergeSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            int[] aux = (int[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        ///
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSort(long[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            long[] aux = (long[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        ///
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @param c the comparator to determine the order of the array.
        /// @throws ClassCastException if the array contains elements that are not
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// @see Comparator
        /// </summary>
        public static void MergeSort(long[] a, int fromIndex, int toIndex, LongComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            long[] aux = (long[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex, c);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        ///
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSort(short[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            short[] aux = (short[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex);
        }

        /// <summary>
        /// Sorts the specified range of the specified array of elements according
        /// to the order induced by the specified comparatord  All elements in the
        /// range must be <i>mutually comparable</i> by the specified comparator
        /// (that is, <i>c(e1, e2)</i> must not throw a
        /// <i>ClassCastException</i> for any elements <i>e1</i> and
        /// <i>e2</i> in the range).<p>
        ///
        /// This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        ///
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        ///
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @param c the comparator to determine the order of the array.
        /// @throws ClassCastException if the array contains elements that are not
        ///	       <i>mutually comparable</i> using the specified comparator.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        ///	       <i>toIndex &gt; a.Length</i>
        /// @see Comparator
        /// </summary>
        public static void MergeSort(short[] a, int fromIndex, int toIndex, ShortComparator c)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            short[] aux = (short[])a.Clone();
            MergeSort1(aux, a, fromIndex, toIndex, c);
        }


        /// <summary>
        /// Sorts the specified range of the specified array of elements.
        /// 
        /// <p>This sort is guaranteed to be <i>stable</i>:  equal elements will
        /// not be reordered as a result of the sort.<p>
        /// 
        /// The sorting algorithm is a modified mergesort (in which the merge is
        /// omitted if the highest element in the low sublist is less than the
        /// lowest element in the high sublist)d  This algorithm offers guaranteed
        /// n*log(n) performance, and can approach linear performance on nearly
        /// sorted lists.
        /// 
        /// @param a the array to be sorted.
        /// @param fromIndex the index of the first element (inclusive) to be
        ///        sorted.
        /// @param toIndex the index of the last element (exclusive) to be sorted.
        /// @throws ArgumentException if <i>fromIndex &gt; toIndex</i>
        /// @throws ArrayIndexOutOfRangeException if <i>fromIndex &lt; 0</i> or
        /// 	       <i>toIndex &gt; a.Length</i>
        /// </summary>
        public static void MergeSortInPlace(int[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            int Length = toIndex - fromIndex;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = fromIndex; i < toIndex; i++)
                {
                    for (int j = i; j > fromIndex && a[j - 1] > a[j]; j--)
                    {
                        int tmp = a[j]; a[j] = a[j - 1]; a[j - 1] = tmp;
                    }
                }
                return;
            }

            // Recursively sort halves
            int mid = (fromIndex + toIndex) / 2;
            MergeSortInPlace(a, fromIndex, mid);
            MergeSortInPlace(a, mid, toIndex);

            // If list is already sorted, nothing left to dod  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (a[mid - 1] <= a[mid]) return;

            // Merge sorted halves 
            //jal.INT.Sorting.inplace_merge(a, fromIndex, mid, toIndex);
            InplaceMerge(a, fromIndex, mid, toIndex);
        }

        #endregion

        #region Local Private Methods
        /// <summary>
        /// Returns the index of the median of the three indexed ints.
        /// </summary>
        private static int Med3(int[] x, int a, int b, int c, IntComparator comp)
        {
            int ab = comp(x[a], x[b]);
            int ac = comp(x[a], x[c]);
            int bc = comp(x[b], x[c]);
            return ab < 0 ?
            (bc < 0 ? b : ac < 0 ? c : a) :
            (bc > 0 ? b : ac > 0 ? c : a);
        }

        /// <summary>
        /// Check that fromIndex and toIndex are in range, and throw an
        /// appropriate exception if they aren't.
        /// </summary>
        private static void RangeCheck(int arrayLen, int fromIndex, int toIndex)
        {
            if (fromIndex > toIndex)
                throw new ArgumentException("fromIndex(" + fromIndex +
                           ") > toIndex(" + toIndex + ")");
            if (fromIndex < 0)
                throw new IndexOutOfRangeException("fromIndex negative");
            if (toIndex > arrayLen)
                throw new IndexOutOfRangeException("toIndex greater than array lenght");
        }

        /// <summary>
        /// Sorts the specified sub-array of ints into ascending order.
        /// </summary>
        private static void QuickSort1(int[] x, int off, int len, IntComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                    for (int j = i; j > off && comp(x[j - 1], x[j]) > 0; j--)
                        Swap(x, j, j - 1);
                return;
            }

            // Choose a partition element, v
            int m = off + (len / 2);

            // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {        // Big arrays, pseudomedian of 9
                    int s = len / 8;
                    l = Med3(x, l, l + s, l + (2 * s), comp);
                    m = Med3(x, m - s, m, m + s, comp);
                    n = Med3(x, n - (2 * s), n - s, n, comp);
                }

                m = Med3(x, l, m, n, comp); // Mid-size, med of 3
            }

            var v = x[m];

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && (comparison = comp(x[b], v)) <= 0)
                {
                    if (comparison == 0)
                        Swap(x, a++, b);
                    b++;
                }

                while (c >= b && (comparison = comp(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                        Swap(x, c, d--);
                    c--;
                }

                if (b > c)
                    break;
                Swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int n1 = off + len;
            int s1 = System.Math.Min(a - off, b - a);
            Vecswap(x, off, b - s1, s1);
            s1 = System.Math.Min(d - c, n1 - d - 1);
            Vecswap(x, b, n1 - s1, s1);

            // Recursively sort non-partition-elements
            if ((s1 = b - a) > 1)
                QuickSort1(x, off, s1, comp);
            if ((s1 = d - c) > 1)
                QuickSort1(x, n1 - s1, s1, comp);
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(byte[] x, int a, int b)
        {
            byte t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(char[] x, int a, int b)
        {
            char t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(double[] x, int a, int b)
        {
            double t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(float[] x, int a, int b)
        {
            float t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(long[] x, int a, int b)
        {
            long t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(Object[] x, int a, int b)
        {
            Object t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(short[] x, int a, int b)
        {
            short t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void Swap(int[] x, int a, int b)
        {
            var t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        private static void MergeSort1(byte[] src, byte[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid);
            MergeSort1(dest, src, mid, high);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(byte[] src, byte[] dest, int low, int high, ByteComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && c(dest[j - 1], dest[j]) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid, c);
            MergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c(src[p], src[q]) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(char[] src, char[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid);
            MergeSort1(dest, src, mid, high);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(char[] src, char[] dest, int low, int high, CharComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && c(dest[j - 1], dest[j]) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid, c);
            MergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c(src[p], src[q]) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(double[] src, double[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid);
            MergeSort1(dest, src, mid, high);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(double[] src, double[] dest, int low, int high, DoubleComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && c(dest[j - 1], dest[j]) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid, c);
            MergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c(src[p], src[q]) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(float[] src, float[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid);
            MergeSort1(dest, src, mid, high);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(float[] src, float[] dest, int low, int high, FloatComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && c(dest[j - 1], dest[j]) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid, c);
            MergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c(src[p], src[q]) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(int[] src, int[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid);
            MergeSort1(dest, src, mid, high);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(int[] src, int[] dest, int low, int high, IntComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && c(dest[j - 1], dest[j]) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid, c);
            MergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c(src[p], src[q]) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(long[] src, long[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid);
            MergeSort1(dest, src, mid, high);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(long[] src, long[] dest, int low, int high, LongComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && c(dest[j - 1], dest[j]) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid, c);
            MergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c(src[p], src[q]) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(short[] src, short[] dest, int low, int high)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && dest[j - 1] > dest[j]; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid);
            MergeSort1(dest, src, mid, high);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (src[mid - 1] <= src[mid])
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && src[p] <= src[q])
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort1(short[] src, short[] dest, int low, int high, ShortComparator c)
        {
            int Length = high - low;

            // Insertion sort on smallest arrays
            if (Length < SMALL)
            {
                for (int i = low; i < high; i++)
                    for (int j = i; j > low && c(dest[j - 1], dest[j]) > 0; j--)
                        Swap(dest, j, j - 1);
                return;
            }

            // Recursively sort halves of dest into src
            int mid = (low + high) / 2;
            MergeSort1(dest, src, low, mid, c);
            MergeSort1(dest, src, mid, high, c);

            // If list is already sorted, just copy from src to destd  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(src[mid - 1], src[mid]) <= 0)
            {
                Array.Copy(src, low, dest, low, Length);
                return;
            }

            // Merge sorted halves (now in src) into dest
            for (int i = low, p = low, q = mid; i < high; i++)
            {
                if (q >= high || p < mid && c(src[p], src[q]) <= 0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        private static void MergeSort2(double[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            long NEG_ZERO_BITS = BitConverter.DoubleToInt64Bits(-0.0d);
            /*
             * The sort is done in three phases to avoid the expense of using
             * NaN and -0.0 aware comparisons during the main sort.
             */

            /*
             * Preprocessing phase:  Move any NaN's to end of array, count the
             * number of -0.0's, and turn them into 0.0'sd 
             */
            int numNegZeros = 0;
            int i = fromIndex, n = toIndex;
            while (i < n)
            {
                if (a[i] != a[i])
                {
                    a[i] = a[--n];
                    a[n] = Double.NaN;
                }
                else
                {
                    if (a[i] == 0 && BitConverter.DoubleToInt64Bits(a[i]) == NEG_ZERO_BITS)
                    {
                        a[i] = 0.0d;
                        numNegZeros++;
                    }
                    i++;
                }
            }

            // Main sort phase: mergesort everything but the NaN's
            double[] aux = (double[])a.Clone();
            MergeSort1(aux, a, fromIndex, n);

            // Postprocessing phase: change 0.0's to -0.0's as required
            if (numNegZeros != 0)
            {
                int j = new List<Double>(a).BinarySearchFromTo(0.0d, fromIndex, n - 1); // posn of ANY zero
                do
                {
                    j--;
                } while (j >= 0 && a[j] == 0.0d);

                // j is now one less than the index of the FIRST zero
                for (int k = 0; k < numNegZeros; k++)
                    a[++j] = -0.0d;
            }
        }

        private static void MergeSort2(float[] a, int fromIndex, int toIndex)
        {
            RangeCheck(a.Length, fromIndex, toIndex);
            int NEG_ZERO_BITS = -0.0f.FloatToIntBits();
            /*
             * The sort is done in three phases to avoid the expense of using
             * NaN and -0.0 aware comparisons during the main sort.
             */

            /*
             * Preprocessing phase:  Move any NaN's to end of array, count the
             * number of -0.0's, and turn them into 0.0'sd 
             */
            int numNegZeros = 0;
            int i = fromIndex, n = toIndex;
            while (i < n)
            {
                if (a[i] != a[i])
                {
                    a[i] = a[--n];
                    a[n] = float.NaN;
                }
                else
                {
                    if (a[i] == 0 && a[i].FloatToIntBits() == NEG_ZERO_BITS)   //BitConverter.ToInt32(BitConverter.GetBytes(a[i]), 0)
                    {
                        a[i] = 0.0f;
                        numNegZeros++;
                    }
                    i++;
                }
            }

            // Main sort phase: mergesort everything but the NaN's
            float[] aux = (float[])a.Clone();
            MergeSort1(aux, a, fromIndex, n);

            // Postprocessing phase: change 0.0's to -0.0's as required
            if (numNegZeros != 0)
            {
                int j = new List<float>(a).BinarySearchFromTo(0.0f, fromIndex, n - 1); // posn of ANY zero
                do
                {
                    j--;
                } while (j >= 0 && a[j] == 0.0f);

                // j is now one less than the index of the FIRST zero
                for (int k = 0; k < numNegZeros; k++)
                    a[++j] = -0.0f;
            }
        }

        /// <summary>
        /// Swaps x[a .d (a+n-1)] with x[b .d (b+n-1)].
        /// </summary>
        private static void Vecswap(int[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
                Swap(x, a, b);
        }

        private static int LowerBound(int[] array, int first, int last, int x)
        {
            int len = last - first;
            while (len > 0)
            {
                int half = len / 2;
                int middle = first + half;
                if (array[middle] < x)
                {
                    first = middle + 1;
                    len -= half + 1;
                }
                else
                    len = half;
            }
            return first;
        }

        private static int UpperBound(int[] array, int first, int last, int x)
        {
            int len = last - first;
            while (len > 0)
            {
                int half = len / 2;
                int middle = first + half;
                if (x < array[middle])
                    len = half;
                else
                {
                    first = middle + 1;
                    len -= half + 1;
                }
            }
            return first;
        }

        private static void InplaceMerge(int[] array, int first, int middle, int last)
        {
            if (first >= middle || middle >= last)
                return;
            if (last - first == 2)
            {
                if (array[middle] < array[first])
                {
                    int tmp = array[first];
                    array[first] = array[middle];
                    array[middle] = tmp;
                }
                return;
            }
            int firstCut;
            int secondCut;
            if (middle - first > last - middle)
            {
                firstCut = first + (middle - first) / 2;
                secondCut = LowerBound(array, middle, last, array[firstCut]);
            }
            else
            {
                secondCut = middle + (last - middle) / 2;
                firstCut = UpperBound(array, first, middle, array[secondCut]);
            }

            //rotate(array, firstCut, middle, secondCut);
            // is manually inlined for speed (jitter inlining seems to work only for small call depths, even if methods are "static private")
            // speedup = 1.7
            // begin inline
            int first2 = firstCut; int middle2 = middle; int last2 = secondCut;
            if (middle2 != first2 && middle2 != last2)
            {
                int first1 = first2; int last1 = middle2;
                int tmp;
                while (first1 < --last1) { tmp = array[first1]; array[last1] = array[first1]; array[first1++] = tmp; }
                first1 = middle2; last1 = last2;
                while (first1 < --last1) { tmp = array[first1]; array[last1] = array[first1]; array[first1++] = tmp; }
                first1 = first2; last1 = last2;
                while (first1 < --last1) { tmp = array[first1]; array[last1] = array[first1]; array[first1++] = tmp; }
            }
        }

        #endregion

        private class ByteComparatorWrapper : IComparer<byte>
        {
            public ByteComparator comparator;

            public int Compare(byte x, byte y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

        private class CharComparatorWrapper : IComparer<char>
        {
            public CharComparator comparator;

            public int Compare(char x, char y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

        private class DoubleComparatorWrapper : IComparer<double>
        {
            public DoubleComparator comparator;

            public int Compare(double x, double y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

        private class FloatComparatorWrapper : IComparer<float>
        {
            public FloatComparator comparator;

            public int Compare(float x, float y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

        private class IntComparatorWrapper : IComparer<int>
        {
            public IntComparator comparator;

            public int Compare(int x, int y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

        private class LongComparatorWrapper : IComparer<long>
        {
            public LongComparator comparator;

            public int Compare(long x, long y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

        private class ShortComparatorWrapper : IComparer<short>
        {
            public ShortComparator comparator;

            public int Compare(short x, short y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

        private class ObjectComparatorWrapper<T> : IComparer<T>
        {
            public ObjectComparator<T> comparator;

            public int Compare(T x, T y)
            {
                if (comparator != null)
                    return comparator(x, y);
                else
                    return -1;
            }
        }

    }
}
