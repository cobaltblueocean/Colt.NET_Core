// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSorting.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Generically sorts arbitrary shaped data (for example multiple arrays, 1,2 or 3-d matrices, and so on) using a quicksort or mergesort.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt
{
    using System;

    using Function;

    /// <summary>
    /// Generically sorts arbitrary shaped data (for example multiple arrays, 1,2 or 3-d matrices, and so on) using a quicksort or mergesort.
    /// </summary>
    public static class GenericSorting
    {
        /// <summary>
        /// Arrays with less than this number of elements are considered small.
        /// </summary>
        private const int SMALL = 7;

        /// <summary>
        /// Arrays with more than this number of elements are considered big.
        /// </summary>
        private const int MEDIUM = 40;

        /// <summary>
        /// Sorts the specified range of elements according to the order induced by the specified comparator.
        /// </summary>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the generic data.
        /// </param>
        /// <param name="swapper">
        /// The delegate that knows how to swap the elements at any two indexes (a,b).
        /// </param>
        public static void MergeSort(int fromIndex, int toIndex, IntComparator c, Swapper swapper)
        {
            /*
                We retain the same method signature as quickSort.
                Given only a comparator and swapper we do not know how to copy and move elements from/to temporary arrays.
                Hence, in contrast to the JDK mergesorts this is an "in-place" mergesort, i.e. does not allocate any temporary arrays.
                A non-inplace mergesort would perhaps be faster in most cases, but would require non-intuitive delegate objects...
            */
            int length = toIndex - fromIndex;

            // Insertion sort on smallest arrays
            if (length < SMALL)
            {
                for (int i = fromIndex; i < toIndex; i++)
                    for (int j = i; j > fromIndex && (c(j - 1, j) > 0); j--)
                        swapper(j, j - 1);
                return;
            }

            // Recursively sort halves
            int mid = (fromIndex + toIndex) / 2;
            MergeSort(fromIndex, mid, c, swapper);
            MergeSort(mid, toIndex, c, swapper);

            // If list is already sorted, nothing left to do.  This is an
            // optimization that results in faster sorts for nearly ordered lists.
            if (c(mid - 1, mid) <= 0) return;

            // Merge sorted halves 
            inplace_merge(fromIndex, mid, toIndex, c, swapper);
        }

        /// <summary>
        /// Sorts the specified range of elements according to the order induced by the specified comparator.
        /// </summary>
        /// <param name="fromIndex">
        /// The index of the first element (inclusive) to be sorted.
        /// </param>
        /// <param name="toIndex">
        /// The index of the last element (exclusive) to be sorted.
        /// </param>
        /// <param name="c">
        /// The comparator to determine the order of the generic data.
        /// </param>
        /// <param name="swapper">
        /// The delegate that knows how to swap the elements at any two indexes (a,b).
        /// </param>
        public static void QuickSort(int fromIndex, int toIndex, IntComparator c, Swapper swapper)
        {
            quickSort1(fromIndex, toIndex - fromIndex, c, swapper);
        }

        /// <summary>
        /// Transforms two consecutive sorted ranges into a single sorted 
        /// range.  The initial ranges are <code>[first, middle)</code>
        /// and <code>[middle, last)</code>, and the resulting range is
        /// <code>[first, last)</code>.
        /// Elements in the first input range will precede equal elements in the 
        /// second.
        /// </summary>
        private static void inplace_merge(int first, int middle, int last, IntComparator comp, Swapper swapper)
        {
            if (first >= middle || middle >= last)
                return;
            if (last - first == 2)
            {
                if (comp(middle, first) < 0)
                    swapper(first, middle);
                return;
            }

            int firstCut;
            int secondCut;
            if (middle - first > last - middle)
            {
                firstCut = first + ((middle - first) / 2);
                secondCut = lower_bound(middle, last, firstCut, comp);
            }
            else
            {
                secondCut = middle + ((last - middle) / 2);
                firstCut = upper_bound(first, middle, secondCut, comp);
            }

            // rotate(firstCut, middle, secondCut, swapper);
            // is manually inlined for speed (jitter inlining seems to work only for small call depths, even if methods are "static private")
            // speedup = 1.7
            // begin inline
            int first2 = firstCut;
            int middle2 = middle;
            int last2 = secondCut;
            if (middle2 != first2 && middle2 != last2)
            {
                int first1 = first2;
                int last1 = middle2;
                while (first1 < --last1)
                    swapper(first1++, last1);

                first1 = middle2;
                last1 = last2;
                while (first1 < --last1) swapper(first1++, last1);
                first1 = first2;
                last1 = last2;
                while (first1 < --last1)
                    swapper(first1++, last1);
            }

            // end inline
            middle = firstCut + (secondCut - middle);
            inplace_merge(first, firstCut, middle, comp, swapper);
            inplace_merge(middle, secondCut, last, comp, swapper);
        }

        /// <summary>
        /// Performs a binary search on an already-sorted range: finds the first
        /// position where an element can be inserted without violating the ordering.
        /// </summary>
        /// <param name="first">
        /// Beginning of the range.
        /// </param>
        /// <param name="last">
        /// One past the end of the range.
        /// </param>
        /// <param name="x">
        /// Element to be searched for.
        /// </param>
        /// <param name="comp">
        /// Comparison function.
        /// </param>
        /// <returns>
        /// The largest index i such that, for every j in the range <code>[first, i)</code>, 
        /// <code>comp.apply(array[j], x)</code> is <code>true</code>.
        /// </returns>
        private static int lower_bound(int first, int last, int x, IntComparator comp)
        {
            int len = last - first;
            while (len > 0)
            {
                int half = len / 2;
                int middle = first + half;
                if (comp(middle, x) < 0)
                {
                    first = middle + 1;
                    len -= half + 1;
                }
                else
                {
                    len = half;
                }
            }

            return first;
        }

        /// <summary>
        /// Returns the index of the median of the three indexed integers.
        /// </summary>
        private static int med3(int a, int b, int c, IntComparator comp)
        {
            int ab = comp(a, b);
            int ac = comp(a, c);
            int bc = comp(b, c);
            return ab < 0 ?
                (bc < 0 ? b : ac < 0 ? c : a) :
                (bc > 0 ? b : ac > 0 ? c : a);
        }

        /// <summary>
        /// Sorts the specified sub-array into ascending order.
        /// </summary>
        private static void quickSort1(int off, int len, IntComparator comp, Swapper swapper)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                    for (int j = i; j > off && (comp(j - 1, j) > 0); j--)
                        swapper(j, j - 1);
                return;
            }

            // Choose a partition element, v
            int m = off + (len / 2);       // Small arrays, middle element
            if (len > SMALL)
            {
                int l = off;
                int n = off + len - 1;
                if (len > MEDIUM)
                {        // Big arrays, pseudomedian of 9
                    int s = len / 8;
                    l = med3(l, l + s, l + (2 * s), comp);
                    m = med3(m - s, m, m + s, comp);
                    n = med3(n - (2 * s), n - s, n, comp);
                }

                m = med3(l, m, n, comp); // Mid-size, med of 3
            }

            // Establish Invariant: v* (<v)* (>v)* v*
            int a = off, b = a, c = off + len - 1, d = c;
            while (true)
            {
                int comparison;
                while (b <= c && ((comparison = comp(b, m)) <= 0))
                {
                    if (comparison == 0)
                    {
                        if (a == m) m = b; // moving target; DELTA to JDK !!!
                        else if (b == m) m = a; // moving target; DELTA to JDK !!!
                        swapper(a++, b);
                    }

                    b++;
                }

                while (c >= b && ((comparison = comp(c, m)) >= 0))
                {
                    if (comparison == 0)
                    {
                        if (c == m) m = d; // moving target; DELTA to JDK !!!
                        else if (d == m) m = c; // moving target; DELTA to JDK !!!
                        swapper(c, d--);
                    }

                    c--;
                }

                if (b > c) break;
                if (b == m) m = d; // moving target; DELTA to JDK !!!
                else if (c == m) m = c; // moving target; DELTA to JDK !!!
                swapper(b++, c--);
            }

            // Swap partition elements back to middle
            int n1 = off + len;
            int s1 = Math.Min(a - off, b - a);
            vecswap(swapper, off, b - s1, s1);
            s1 = Math.Min(d - c, n1 - d - 1);
            vecswap(swapper, b, n1 - s1, s1);

            // Recursively sort non-partition-elements
            if ((s1 = b - a) > 1)
                quickSort1(off, s1, comp, swapper);
            if ((s1 = d - c) > 1)
                quickSort1(n1 - s1, s1, comp, swapper);
        }

        /// <summary>
        /// Performs a binary search on an already-sorted range: finds the last
        /// position where an element can be inserted without violating the ordering.
        /// </summary>
        /// <param name="first">
        /// Beginning of the range.
        /// </param>
        /// <param name="last">
        /// One past the end of the range.
        /// </param>
        /// <param name="x">
        /// Element to be searched for.
        /// </param>
        /// <param name="comp">
        /// Comparison function.
        /// </param>
        /// <returns>
        /// The largest index i such that, for every j in the range <code>[first, i)</code>, 
        /// <code>comp.apply(array[j], x)</code> is <code>false</code>.
        /// </returns>
        private static int upper_bound(int first, int last, int x, IntComparator comp)
        {
            int len = last - first;
            while (len > 0)
            {
                int half = len / 2;
                int middle = first + half;
                if (comp(x, middle) < 0)
                {
                    len = half;
                }
                else
                {
                    first = middle + 1;
                    len -= half + 1;
                }
            }

            return first;
        }

        /// <summary>
        /// Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
        /// </summary>
        private static void vecswap(Swapper swapper, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++) swapper(a, b);
        }
    }
}
