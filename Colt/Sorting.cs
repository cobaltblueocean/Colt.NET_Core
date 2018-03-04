namespace Colt
{
    using System;

    using Function;

    /// <summary>
    /// Quicksorts, mergesorts and binary searches.
    /// </summary>
    public static class Sorting
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
        /// If <tt>fromIndex &gt; toIndex</tt>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// If <tt>fromIndex &lt; 0</tt> or <tt>toIndex &gt; a.length</tt>
        /// </exception>
        public static void QuickSort(int[] a, int fromIndex, int toIndex, IntComparator c)
        {
            rangeCheck(a.Length, fromIndex, toIndex);
            quickSort1(a, fromIndex, toIndex - fromIndex, c);
        }

        /// <summary>
        /// Returns the index of the median of the three indexed integers.
        /// </summary>
        private static int med3(int[] x, int a, int b, int c, IntComparator comp)
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
        private static void rangeCheck(int arrayLen, int fromIndex, int toIndex)
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
        /// Sorts the specified sub-array of integers into ascending order.
        /// </summary>
        private static void quickSort1(int[] x, int off, int len, IntComparator comp)
        {
            // Insertion sort on smallest arrays
            if (len < SMALL)
            {
                for (int i = off; i < len + off; i++)
                    for (int j = i; j > off && comp(x[j - 1], x[j]) > 0; j--)
                        swap(x, j, j - 1);
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
                    l = med3(x, l, l + s, l + (2 * s), comp);
                    m = med3(x, m - s, m, m + s, comp);
                    n = med3(x, n - (2 * s), n - s, n, comp);
                }

                m = med3(x, l, m, n, comp); // Mid-size, med of 3
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
                        swap(x, a++, b);
                    b++;
                }

                while (c >= b && (comparison = comp(x[c], v)) >= 0)
                {
                    if (comparison == 0)
                        swap(x, c, d--);
                    c--;
                }

                if (b > c)
                    break;
                swap(x, b++, c--);
            }

            // Swap partition elements back to middle
            int n1 = off + len;
            int s1 = Math.Min(a - off, b - a);
            vecswap(x, off, b - s1, s1);
            s1 = Math.Min(d - c, n1 - d - 1);
            vecswap(x, b, n1 - s1, s1);

            // Recursively sort non-partition-elements
            if ((s1 = b - a) > 1)
                quickSort1(x, off, s1, comp);
            if ((s1 = d - c) > 1)
                quickSort1(x, n1 - s1, s1, comp);
        }

        /// <summary>
        /// Swaps x[a] with x[b].
        /// </summary>
        private static void swap(int[] x, int a, int b)
        {
            var t = x[a];
            x[a] = x[b];
            x[b] = t;
        }

        /// <summary>
        /// Swaps x[a .. (a+n-1)] with x[b .. (b+n-1)].
        /// </summary>
        private static void vecswap(int[] x, int a, int b, int n)
        {
            for (int i = 0; i < n; i++, a++, b++)
                swap(x, a, b);
        }
    }
}
