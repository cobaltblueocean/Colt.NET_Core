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
using Cern.Colt.Function;

namespace Cern.Colt
{
    /// <summary>
    /// Given some interval boundaries, partitions arrays such that all elements falling into an interval are placed next to each other.
    /// <p>
    /// The algorithms partition arrays into two or more intervalsd 
    /// They distinguish between <i>synchronously</i> partitioning either one, two or three arrays.
    /// They further come in templated versions, either partitioning <i>int[]</i> arrays or <i>double[]</i> arrays.
    /// <p>
    /// You may want to start out reading about the simplest case: Partitioning one <i>int[]</i> array into two intervals.
    /// To do so, read {@link #partition(int[],int,int,int)}.
    ///
    /// Next, building upon that foundation comes a method partitioning <i>int[]</i> arrays into multiple intervals.
    /// See {@link #partition(int[],int,int,int[],int,int,int[])} for related documentation.
    /// <p>
    /// All other methods are no different than the one's you now already understand, except that they operate on slightly different data types.
    /// <p>
    /// <b>Performance</b>
    /// <p>
    /// Partitioning into two intervals is <i>O( N )</i>.
    /// Partitioning into k intervals is <i>O( N * log(k))</i>.
    /// Constants factors are minimized.
    /// No temporary memory is allocated; Partitioning is in-place.
    ///
    /// @see cern.colt.matrix.doublealgo.Partitioning
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 03-Jul-99
    /// </summary>
    public class Partitioning
    {
        private static int SMALL = 7;
        private static int MEDIUM = 40;

        // benchmark only
        protected static int steps = 0;
        public static int swappedElements = 0;

        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        protected Partitioning() { }

        /// <summary>
        /// Finds the given key "a" within some generic data using the binary search algorithm.
        /// </summary>
        /// <param name="a">the index of the key to search for.</param>
        /// <param name="from">the leftmost search position, inclusive.</param>
        /// <param name="to">the rightmost search position, inclusive.</param>
        /// <param name="comp">
        /// the comparator determining the order of the generic data.
        /// Takes as first argument the index <i>a</i> within the generic splitters <i>s</i>.
        /// Takes as second argument the index <i>b</i> within the generic data <i>g</i>.
        /// </param>
        /// <returns>
        /// index of the search key, if it is contained in the list;
        /// otherwise, <i>(-(<i>insertion point</i>) - 1)</i>d  The <i>insertion
        /// point</i> is defined as the the point at which the value would
        /// be inserted into the list: the index of the first
        /// element greater than the key, or <i>list.Length</i>, if all
        /// elements in the list are less than the specified keyd  Note
        /// that this guarantees that the return value will be &gt;= 0 if
        /// and only if the key is found.
        /// </returns>
        private static int BinarySearchFromTo(int a, int from, int to, IntComparator comp)
        {
            while (from <= to)
            {
                int mid = (from + to) / 2;
                int comparison = comp(mid, a);
                if (comparison < 0) from = mid + 1;
                else if (comparison > 0) to = mid - 1;
                else return mid; // key found
            }
            return -(from + 1);  // key not found.
        }

        /// <summary>
        /// Same as <see cref="DualPartition(int[], int[], int, int, int[], int, int, int[])"/>
        /// except that it <i>synchronously</i> partitions <i>double[]</i> rather than <i>int[]</i> arrays.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitFrom"></param>
        /// <param name="splitTo"></param>
        /// <param name="splitIndexes"></param>
        public static void DualPartition(double[] list, double[] secondary, int from, int to, double[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            double splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(list, l, l + s, l + 2 * s);
                        m = Med3(list, m - s, m, m + s);
                        n = Med3(list, n - 2 * s, n - s, n);
                    }
                    m = Med3(list, l, m, n); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                //medianIndex = Sorting.BinarySearchFromTo(splitters, list[m], splitFrom, splitTo);
                medianIndex = Array.BinarySearch(splitters, splitFrom, (splitTo - splitFrom), list[m]);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int splitIndex = DualPartition(list, secondary, from, to, splitter);
            splitIndexes[medianIndex] = splitIndex;

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(splitter < splitters[i]))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(splitter > splitters[i]))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                DualPartition(list, secondary, from, splitIndex, splitters, splitFrom, medianIndex - 1, splitIndexes);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                DualPartition(list, secondary, splitIndex + 1, to, splitters, medianIndex + 1, splitTo, splitIndexes);
            }
        }

        /// <summary>
        /// Same as <see cref="DualPartition(int[], int[], int, int, int)"/>
        /// except that it <i>synchronously</i> partitions <i>double[]</i> rather than <i>int[]</i> arrays.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static int DualPartition(double[] list, double[] secondary, int from, int to, double splitter)
        {
            double element;  // int, double --> template type dependent
            for (int i = from - 1; ++i <= to;)
            {
                element = list[i];
                if (element < splitter)
                {
                    // swap x[i] with x[from]
                    list[i] = list[from];
                    list[from] = element;

                    element = secondary[i];
                    secondary[i] = secondary[from];
                    secondary[from++] = element;
                }
            }
            return from - 1;
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int[], int, int, int[])"/> except that this method <i>synchronously</i> partitions two arrays at the same time;
         /// both arrays are partially sorted according to the elements of the primary array.
         /// In other words, each time an element in the primary array is moved from index A to B, the correspoding element within the secondary array is also moved from index A to B.
         /// <p>
         /// <b>Use cases:</b>
         /// <p>
         /// Image having a large list of 2-dimensional pointsd 
         /// If memory consumption and performance matter, it is a good idea to physically lay them out as two 1-dimensional arrays
         /// (using something like <i>Point2D</i> objects would be prohibitively expensive, both in terms of time and space).
         /// Now imagine wanting to histogram the points.
         /// We may want to partially sort the points by x-coordinate into intervals.
         /// This method efficiently does the job.
         /// <p>
         /// <b>Performance:</b>
         /// <p>
         /// Same as for single-partition methods.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitFrom"></param>
        /// <param name="splitTo"></param>
        /// <param name="splitIndexes"></param>
        public static void DualPartition(int[] list, int[] secondary, int from, int to, int[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            int splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(list, l, l + s, l + 2 * s);
                        m = Med3(list, m - s, m, m + s);
                        n = Med3(list, n - 2 * s, n - s, n);
                    }
                    m = Med3(list, l, m, n); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                //medianIndex = Sorting.BinarySearchFromTo(splitters, list[m], splitFrom, splitTo);
                medianIndex = Array.BinarySearch(splitters, splitFrom, (splitTo - splitFrom), list[m]);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int splitIndex = DualPartition(list, secondary, from, to, splitter);
            splitIndexes[medianIndex] = splitIndex;

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(splitter < splitters[i]))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(splitter > splitters[i]))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                DualPartition(list, secondary, from, splitIndex, splitters, splitFrom, medianIndex - 1, splitIndexes);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                DualPartition(list, secondary, splitIndex + 1, to, splitters, medianIndex + 1, splitTo, splitIndexes);
            }
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int)"/> except that this method <i>synchronously</i> partitions two arrays at the same time;
         /// both arrays are partially sorted according to the elements of the primary array.
         /// In other words, each time an element in the primary array is moved from index A to B, the correspoding element within the secondary array is also moved from index A to B.
         /// <p>
         /// <b>Performance:</b>
         /// <p>
         /// Same as for single-partition methods.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static int DualPartition(int[] list, int[] secondary, int from, int to, int splitter)
        {
            int element;  // int, double --> template type dependent
            for (int i = from - 1; ++i <= to;)
            {
                element = list[i];
                if (element < splitter)
                {
                    // swap x[i] with x[from]
                    list[i] = list[from];
                    list[from] = element;

                    element = secondary[i];
                    secondary[i] = secondary[from];
                    secondary[from++] = element;
                }
            }
            return from - 1;
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int[], int, int, int[])"/>
        /// except that it <i>generically</i> partitions arbitrary shaped data (for example matrices or multiple arrays) rather than <i>int[]</i> arrays.
        /// <p>
        /// This method operates on arbitrary shaped data and arbitrary shaped splittersd 
        /// In fact, it has no idea what kind of data by what kind of splitters it is partitioningd Comparisons and swapping 
        /// are delegated to user provided objects which know their data and can do the 
        /// jobd 
        /// <p>
        /// Lets call the generic data <i>g</i> (it may be a matrix, one array, three linked lists 
        /// or whatever)d Lets call the generic splitters <i>s</i>.
        /// This class takes a user comparison function operating on two indexes 
        /// <i>(a,b)</i>, namely an {@link IntComparator}d 
        /// The comparison function determines whether <i>s[a]</i> is equal, less or greater than <i>g[b]</i>d 
        /// This method can then decide to swap the data <i>g[b]</i> 
        /// with the data <i>g[c]</i> (yes, <i>c</i>, not <i>a</i>)d 
        /// It calls a user provided {@link cern.colt.Swapper} 
        /// object that knows how to swap the data of these two indexes.
        /// <p>
        /// Again, note the details: Comparisons compare <i>s[a]</i> with <i>g[b]</i>.
        /// Swaps swap <i>g[b]</i> with <i>g[c]</i>d 
        /// Prior to calling this method, the generic splitters <i>s</i> must be sorted ascending and must not contain multiple equal values.
        /// These preconditions are not checked; be sure that they are met.
        /// 
        /// Tip: Normally you will have <i>splitIndexes.Length == s.Length</i> as well as <i>from==0, to==g.Length-1</i> and <i>splitFrom==0, splitTo==s.Length-1</i>.
        /// </summary>
        /// <param name="from">
        /// the index of the first element within <i>g</i> to be considered.
        /// </param>
        /// <param name="to">
        /// the index of the last element within <i>g</i> to be considered.
        /// The method considers the elements<i> g[from] .d g[to]</i>.
        /// </param>
        /// <param name="splitFrom">
        /// the index of the first splitter element to be considered.
        /// </param>
        /// <param name="splitTo">
        /// the index of the last splitter element to be considered.
        /// The method considers the splitter elements <i>s[splitFrom] .d s[splitTo]</i>.
        /// </param>
        /// <param name="splitIndexes">
        /// a list into which this method fills the indexes of elements delimiting intervals.
        /// Upon return <i>splitIndexes[splitFrom..splitTo]</i> will be set accordingly.
        /// Therefore, must satisfy <i>splitIndexes.Length > splitTo</i>.
        /// </param>
        /// <param name="comp">
        /// the comparator comparing a splitter with an element of the generic data.
        /// Takes as first argument the index <i>a</i> within the generic splitters <i>s</i>.
        /// Takes as second argument the index <i>b</i> within the generic data <i>g</i>.
        /// </param>
        /// <param name="comp2">
        /// the comparator to determine the order of the generic data.
        /// Takes as first argument the index <i>a</i> within the generic data <i>g</i>.
        /// Takes as second argument the index <i>b</i> within the generic data <i>g</i>.
        /// </param>
        /// <param name="comp3">
        /// the comparator comparing a splitter with another splitter.
        /// Takes as first argument the index <i>a</i> within the generic splitters <i>s</i>.
        /// Takes as second argument the index <i>b</i> within the generic splitters <i>g</i>.
        /// </param>
        /// <param name="swapper">
        /// swapper an object that knows how to swap the elements at any two indexes (a,b).
        /// Takes as first argument the index <i>b</i> within the generic data <i>g</i>.
        /// Takes as second argument the index <i>c</i> within the generic data <i>g</i>.
        /// </param>
        /// <see cref="Sorting"/>
        /// <see cref="Sorting.BinarySearchFromTo(int[], int, int, int, IntComparator)"/>
        public static void GenericPartition(int from, int to, int splitFrom, int splitTo, int[] splitIndexes, IntComparator comp, IntComparator comp2, IntComparator comp3, Swapper swapper)
        {
            int splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(l, l + s, l + 2 * s, comp2);
                        m = Med3(m - s, m, m + s, comp2);
                        n = Med3(n - 2 * s, n - s, n, comp2);
                    }
                    m = Med3(l, m, n, comp2); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                medianIndex = BinarySearchFromTo(m, splitFrom, splitTo, comp);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = medianIndex;

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int splitIndex = GenericPartition(from, to, splitter, comp, swapper);
            splitIndexes[medianIndex] = splitIndex;


            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(comp3(splitter, i) < 0))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(comp3(splitter, i) > 0))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }


            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                GenericPartition(from, splitIndex, splitFrom, medianIndex - 1, splitIndexes, comp, comp2, comp3, swapper);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                GenericPartition(splitIndex + 1, to, medianIndex + 1, splitTo, splitIndexes, comp, comp2, comp3, swapper);
            }
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int)"/>
        /// except that it <i>generically</i> partitions arbitrary shaped data (for example matrices or multiple arrays) rather than <i>int[]</i> arrays.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <param name="comp"></param>
        /// <param name="swapper"></param>
        /// <returns></returns>
        private static int GenericPartition(int from, int to, int splitter, IntComparator comp, Swapper swapper)
        {
            for (int i = from - 1; ++i <= to;)
            {
                if (comp(splitter, i) > 0)
                {
                    // swap x[i] with x[from]
                    swapper(i, from);
                    from++;
                }
            }
            return from - 1;
        }

        /// <summary>
        /// Returns the index of the median of the three indexed elements.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int Med3(double[] x, int a, int b, int c)
        {
            return (x[a] < x[b] ?
                (x[b] < x[c] ? b : x[a] < x[c] ? c : a) :
                (x[b] > x[c] ? b : x[a] > x[c] ? c : a));
        }

        /// <summary>
        /// Returns the index of the median of the three indexed elements.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int Med3(int[] x, int a, int b, int c)
        {
            return (x[a] < x[b] ?
                (x[b] < x[c] ? b : x[a] < x[c] ? c : a) :
                (x[b] > x[c] ? b : x[a] > x[c] ? c : a));
        }

        /// <summary>
        /// Returns the index of the median of the three indexed chars.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        private static int Med3<T>(T[] x, int a, int b, int c, ObjectComparator<T> comp)
        {
            int ab = comp(x[a], x[b]);
            int ac = comp(x[a], x[c]);
            int bc = comp(x[b], x[c]);
            return (ab < 0 ?
                (bc < 0 ? b : ac < 0 ? c : a) :
                (bc > 0 ? b : ac > 0 ? c : a));
        }

        /// <summary>
        /// Returns the index of the median of the three indexed chars.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        private static int Med3(int a, int b, int c, IntComparator comp)
        {
            int ab = comp(a, b);
            int ac = comp(a, c);
            int bc = comp(b, c);
            return (ab < 0 ?
                (bc < 0 ? b : ac < 0 ? c : a) :
                (bc > 0 ? b : ac > 0 ? c : a));
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int[], int, int, int[])"/>
        /// except that it partitions <i>double[]</i> rather than <i>int[]</i> arrays.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitFrom"></param>
        /// <param name="splitTo"></param>
        /// <param name="splitIndexes"></param>
        public static void Partition(double[] list, int from, int to, double[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            double splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(list, l, l + s, l + 2 * s);
                        m = Med3(list, m - s, m, m + s);
                        n = Med3(list, n - 2 * s, n - s, n);
                    }
                    m = Med3(list, l, m, n); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                //medianIndex = Sorting.BinarySearchFromTo(splitters, list[m], splitFrom, splitTo);
                medianIndex = Array.BinarySearch(splitters, splitFrom, (splitTo - splitFrom), list[m]);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int splitIndex = Partition(list, from, to, splitter);
            splitIndexes[medianIndex] = splitIndex;

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(splitter < splitters[i]))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(splitter > splitters[i]))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                Partition(list, from, splitIndex, splitters, splitFrom, medianIndex - 1, splitIndexes);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                Partition(list, splitIndex + 1, to, splitters, medianIndex + 1, splitTo, splitIndexes);
            }
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int)"/>
        /// except that it partitions <i>double[]</i> rather than <i>int[]</i> arrays.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static int Partition(double[] list, int from, int to, double splitter)
        {
            double element;  // int, double --> template type dependent
            for (int i = from - 1; ++i <= to;)
            {
                element = list[i];
                if (element < splitter)
                {
                    // swap x[i] with x[from]
                    list[i] = list[from];
                    list[from++] = element;
                }
            }
            return from - 1;
        }

        /// <summary>
        /// Partitions (partially sorts) the given list such that all elements falling into some intervals are placed next to each other.
        /// Returns the indexes of elements delimiting intervals.
        /// <p>
        /// <b>Example:</b>
        /// <p>
        /// <i>list = (7, 4, 5, 50, 6, 4, 3, 6), splitters = (5, 10, 30)</i>
        /// defines the three intervals <i>[-infinity,5), [5,10), [10,30)</i>.
        /// Lets define to sort the entire list (<i>from=0, to=7</i>) using all splitters (<i>splitFrom==0, splitTo=2</i>).
        /// <p>
        /// The method modifies the list to be <i>list = (4, 4, 3, 6, 7, 5, 6, 50)</i>
        /// and returns the <i>splitIndexes = (2, 6, 6)</i>.
        /// In other words,
        /// <ul>
        /// <li>All values <i>list[0..2]</i> fall into <i>[-infinity,5)</i>.
        /// <li>All values <i>list[3..6]</i> fall into <i>[5,10)</i>.
        /// <li>All values <i>list[7..6]</i> fall into <i>[10,30)</i>, i.ed no elements, since <i>7>6</i>.
        /// <li>All values <i>list[7 .d 7=list.Length-1]</i> fall into <i>[30,infinity]</i>.
        /// <li>In general, all values <i>list[splitIndexes[j-1]+1 .d splitIndexes[j]]</i> fall into interval <i>j</i>.
        /// </ul>
        /// As can be seen, the list is partially sorted such that values falling into a certain interval are placed next to each other.
        /// Note that <i>within</i> an interval, elements are entirelly unsorted.
        /// They are only sorted across interval boundaries.
        /// In particular, this partitioning algorithm is not <i>stable</i>: the relative order of elements is not preserved
        /// (Producing a stable algorithm would require no more than minor modifications to method partition(int[],int,int,int)).
        /// <p>
        /// More formally, this method guarantees that upon return <i>for all j = splitFrom .d splitTo</i> there holds:
        /// <br><i>for all i = splitIndexes[j-1]+1 .d splitIndexes[j]: splitters[j-1] <= list[i] < splitters[j]</i>.
        /// <p>
        /// <b>Performance:</b>
        /// <p>
        /// Let <i>N=to-from+1</i> be the number of elements to be partitioned.
        /// Let <i>k=splitTo-splitFrom+1</i> be the number of splitter elements.
        /// Then we have the following time complexities
        /// <ul>
        /// <li>Worst case:  <i>O( N * log(k) )</i>.
        /// <li>Average case: <i>O( N * log(k) )</i>.
        /// <li>Best case: <i>O( N )</i>d 
        /// In general, the more uniform (skewed) the data is spread across intervals, the more performance approaches the worst (best) case.
        /// If no elements fall into the given intervals, running time is linear.
        /// </ul>
        /// No temporary memory is allocated; the sort is in-place.
        /// <p>
        /// <b>Implementation:</b>
        /// <p>
        /// The algorithm can be seen as a Bentley/McIlroy quicksort where swapping and insertion sort are omitted.
        /// It is designed to detect and take advantage of skew while maintaining good performance in the uniform cased
        /// 
        /// Tip: Normally you will have <i>splitIndexes.Length == splitters.Length</i> as well as <i>from==0, to==list.Length-1</i> and <i>splitFrom==0, splitTo==splitters.Length-1</i>.
        /// </summary>
        /// <param name="list">
        /// the list to be partially sorted.
        /// </param>
        /// <param name="from">
        /// the index of the first element within <i>list</i> to be considered.
        /// </param>
        /// <param name="to">
        /// the index of the last element within <i>list</i> to be considered.
        /// The method considers the elements <i>list[from] .d list[to]</i>.
        /// </param>
        /// <param name="splitters">
        /// splitters the values at which the list shall be split into intervals.
        /// Must be sorted ascending and must not contain multiple identical values.
        /// These preconditions are not checked; be sure that they are met.
        /// </param>
        /// <param name="splitFrom">
        /// the index of the first splitter element to be considered.
        /// </param>
        /// <param name="splitTo">
        /// the index of the last splitter element to be considered.
        /// The method considers the splitter elements <i>splitters[splitFrom] .d splitters[splitTo]</i>.
        /// </param>
        /// <param name="splitIndexes">
        /// a list into which this method fills the indexes of elements delimiting intervals.
        /// Upon return <i>splitIndexes[splitFrom..splitTo]</i> will be set accordingly.
        /// Therefore, must satisfy <i>splitIndexes.Length > splitTo</i>.
        /// </param>
        public static void Partition(int[] list, int from, int to, int[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            int element, splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(list, l, l + s, l + 2 * s);
                        m = Med3(list, m - s, m, m + s);
                        n = Med3(list, n - 2 * s, n - s, n);
                    }
                    m = Med3(list, l, m, n); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                //medianIndex = Sorting.BinarySearchFromTo(splitters, list[m], splitFrom, splitTo);
                medianIndex = Array.BinarySearch(splitters, splitFrom, (splitTo - splitFrom), list[m]);

                //int key = list[m];
                /*
                if (splitTo-splitFrom+1 < 5) { // on short lists linear search is quicker
                    int i=splitFrom-1;
                    while (++i <= splitTo && list[i] < key);
                    if (i > splitTo || list[i] > key) i = -i-1; // not found
                    medianIndex = i;
                }
                */
                //else {
                /*

                    int low = splitFrom;
                    int high = splitTo;
                    int comparison;

                    int mid=0;
                    while (low <= high) {
                        mid = (low + high) / 2;
                        comparison = splitters[mid]-key;
                        if (comparison < 0) low = mid + 1;
                        else if (comparison > 0) high = mid - 1;
                        else break; //return mid; // key found
                    }
                    medianIndex = mid;
                    if (low > high) medianIndex = -(medianIndex + 1);  // key not found.
                //}
                */


                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            //Console.WriteLine("medianIndex="+medianIndex);
            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            // Could simply call:
            int splitIndex = Partition(list, from, to, splitter);
            // but for speed the code is manually inlined.
            /*
            steps += to-from+1;
            int head = from;
            for (int i=from-1; ++i<=to; ) { // swap all elements < splitter to front
                element = list[i];
                if (element < splitter) {		
                    list[i] = list[head];
                    list[head++] = element;
                    //swappedElements++;
                }
            }
            int splitIndex = head-1;
            */







            //Console.WriteLine("splitIndex="+splitIndex);
            splitIndexes[medianIndex] = splitIndex;

            //if (splitFrom == splitTo) return; // done

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(splitter < splitters[i]))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(splitter > splitters[i]))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                //Console.WriteLine("1.recursive: from="+from+", to="+splitIndex+", splitFrom="+splitFrom+", splitTo="+(medianIndex-1));		
                Partition(list, from, splitIndex, splitters, splitFrom, medianIndex - 1, splitIndexes);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                //Console.WriteLine("2.recursive: from="+(splitIndex+1)+", to="+to+", splitFrom="+(medianIndex+1)+", splitTo="+splitTo);
                Partition(list, splitIndex + 1, to, splitters, medianIndex + 1, splitTo, splitIndexes);
            }
            //Console.WriteLine("BACK TRACKING\n\n");
        }

        /// <summary>
         /// Partitions (partially sorts) the given list such that all elements falling into the given interval are placed next to each other.
         /// Returns the index of the element delimiting the interval.
         /// <p>
         /// <b>Example:</b>
         /// <p>
         /// <i>list = (7, 4, 5, 50, 6, 4, 3, 6), splitter = 5</i>
         /// defines the two intervals <i>[-infinity,5), [5,+infinity]</i>.
         /// <p>
         /// The method modifies the list to be <i>list = (4, 4, 3, 50, 6, 7, 5, 6)</i>
         /// and returns the split index <i>2</i>.
         /// In other words,
         /// <ul>
         /// <li>All values <i>list[0..2]</i> fall into <i>[-infinity,5)</i>.
         /// <li>All values <i>list[3=2+1 .d 7=list.Length-1]</i> fall into <i>[5,+infinity]</i>.
         /// </ul>
         /// As can be seen, the list is partially sorted such that values falling into a certain interval are placed next to each other.
         /// Note that <i>within</i> an interval, elements are entirelly unsorted.
         /// They are only sorted across interval boundaries.
         /// In particular, this partitioning algorithm is not <i>stable</i>.
         /// <p>
         /// More formally, this method guarantees that upon return there holds:
         /// <ul>
         /// <li>for all <i>i = from .d returnValue: list[i] < splitter</i> and
         /// <li>for all <i>i = returnValue+1 .d list.Length-1: !(list[i] < splitter)</i>.
         /// </ul>
         /// <p>
         /// <b>Performance:</b>
         /// <p>
         /// Let <i>N=to-from+1</i> be the number of elements to be partially sorted.
         /// Then the time complexity is <i>O( N )</i>.
         /// No temporary memory is allocated; the sort is in-place.
        /// </summary>
        /// <param name="list">
        /// the list to be partially sorted.
        /// </param>
        /// <param name="from">
        /// the index of the first element within <i>list</i> to be considered.
        /// </param>
        /// <param name="to">
        /// the index of the last element within <i>list</i> to be considered.
        /// The method considers the elements <i>list[from] .d list[to]</i>.
        /// </param>
        /// <param name="splitter">
        /// the value at which the list shall be split.
        /// </param>
        /// <returns>the index of the largest element falling into the interval <i>[-infinity,splitter)</i>, as seen after partitioning.</returns>
        public static int Partition(int[] list, int from, int to, int splitter)
        {
            steps += to - from + 1;

            /*
            Console.WriteLine();
            if (from<=to) {
                Console.WriteLine("SORT WORKING: from="+from+", to="+to+", splitter="+splitter);
            }
            else {
                Console.WriteLine("SORT WORKING: NOTHING TO DO.");
            }
            */





            // returns index of last element < splitter


            /*
            for (int i=from-1; ++i<=to; ) {
                if (list[i] < splitter) {
                    int element = list[i];
                    list[i] = list[from];
                    list[from++] = element;
                }
            }
            */




            int element;
            for (int i = from - 1; ++i <= to;)
            {
                element = list[i];
                if (element < splitter)
                {
                    // swap x[i] with x[from]
                    list[i] = list[from];
                    list[from++] = element;
                    //swappedElements++;
                }
            }
            //if (from<=to) Console.WriteLine("Swapped "+(head-from)+" elements");


            /*	
            //JAL:
            int first = from;
            int last = to+1;
            --first;
            while (true) {
                while (++first < last && list[first] < splitter);
                while (first < --last && !(list[last] < splitter)); 
                if (first >= last) return first-1;
                int tmp = list[first];
                list[first] = list[last];
                list[last] = tmp;
            }
            */



            /*
            Console.WriteLine("splitter="+splitter);
            Console.WriteLine("before="+new List<int>(list));
            int head = from;
            int trail = to;
            int element;
            while (head<=trail) {
                head--;
                while (++head < trail && list[head] < splitter);

                trail++;
                while (--trail > head && list[trail] >= splitter);

                if (head != trail) {		
                    element = list[head];
                    list[head] = list[trail];
                    list[trail] = element;
                }
                head++;
                trail--;
                Console.WriteLine("after ="+new List<int>(list)+", head="+head);
            }
            */


            /*
            //Console.WriteLine("splitter="+splitter);
            //Console.WriteLine("before="+new List<int>(list));
            to++;
            //int head = from;
            int element;
            //int oldHead;
            while (--to >= from) {
                element = list[to];
                if (element < splitter) {
                    from--;
                    while (++from < to && list[from] < splitter);
                    //if (head != to) {
                        list[to] = list[from];
                        list[from++] = element;
                        //oldHead = list[head];
                        //list[head] = element;
                        //list[i] = oldHead;

                        //head++;
                    //}
                    //head++;
                }
                //Console.WriteLine("after ="+new List<int>(list)+", head="+head);
            }
            */

            /*
            int i=from-1;
            int head = from;
            int trail = to;
            while (++i <= trail) {
                int element = list[i];
                if (element < splitter) {
                    if (head == i) head++;
                    else {
                        // swap list[i] with list[from]
                        int oldHead = list[head];
                        int oldTrail = list[trail];
                        list[head++] = element;
                        list[i--] = oldTrail;
                        list[trail--] = oldHead;
                    }
                }
                //Console.WriteLine(new List<int>(list));

            }
            */


            return from - 1;
            //return head-1;
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int[], int, int, int[])"/>
        /// except that it partitions <i>Object[]</i> rather than <i>int[]</i> arrays.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitFrom"></param>
        /// <param name="splitTo"></param>
        /// <param name="splitIndexes"></param>
        /// <param name="comp"></param>
        public static void Partition<T>(T[] list, int from, int to, T[] splitters, int splitFrom, int splitTo, int[] splitIndexes, ObjectComparator<T> comp)
        {
            T splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(list, l, l + s, l + 2 * s, comp);
                        m = Med3(list, m - s, m, m + s, comp);
                        n = Med3(list, n - 2 * s, n - s, n, comp);
                    }
                    m = Med3(list, l, m, n, comp); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                medianIndex = Sorting.BinarySearchFromTo(splitters, list[m], splitFrom, splitTo, comp);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int splitIndex = Partition(list, from, to, splitter, comp);
            splitIndexes[medianIndex] = splitIndex;

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(comp(splitter, splitters[i]) < 0))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(comp(splitter, splitters[i]) > 0))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                Partition(list, from, splitIndex, splitters, splitFrom, medianIndex - 1, splitIndexes, comp);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                Partition(list, splitIndex + 1, to, splitters, medianIndex + 1, splitTo, splitIndexes, comp);
            }
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int)"/> 
        /// except that it <i>synchronously</i> partitions the objects of the given list by the order of the given comparator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static int Partition<T>(T[] list, int from, int to, T splitter, ObjectComparator<T> comp)
        {
            T element;  // int, double --> template type dependent
            for (int i = from - 1; ++i <= to;)
            {
                element = list[i];
                if (comp(element, splitter) < 0)
                {
                    // swap x[i] with x[from]
                    list[i] = list[from];
                    list[from] = element;
                    from++;
                }
            }
            return from - 1;
        }

        /// <summary>
        /// Equivalent to <i>partition(list.ToArray(), from, to, splitters.ToArray(), 0, splitters.Count-1, splitIndexes.ToArray())</i>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitIndexes"></param>
        public static void Partition(List<Double> list, int from, int to, List<Double> splitters, List<int> splitIndexes)
        {
            Partition(list.ToArray(), from, to, splitters.ToArray(), 0, splitters.Count - 1, splitIndexes.ToArray());
        }

        /// <summary>
        /// Equivalent to <i>partition(list.ToArray(), from, to, splitters.ToArray(), 0, splitters.Count-1, splitIndexes.ToArray())</i>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitIndexes"></param>
        public static void Partition(List<int> list, int from, int to, List<int> splitters, List<int> splitIndexes)
        {
            Partition(list.ToArray(), from, to, splitters.ToArray(), 0, splitters.Count - 1, splitIndexes.ToArray());
        }

        /// <summary>
        /// Same as <see cref="TriplePartition(int[], int[], int[], int, int, int[], int, int, int[])"/>
        /// except that it <i>synchronously</i> partitions <i>double[]</i> rather than <i>int[]</i> arrays.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="tertiary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitFrom"></param>
        /// <param name="splitTo"></param>
        /// <param name="splitIndexes"></param>
        public static void TriplePartition(double[] list, double[] secondary, double[] tertiary, int from, int to, double[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            double splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(list, l, l + s, l + 2 * s);
                        m = Med3(list, m - s, m, m + s);
                        n = Med3(list, n - 2 * s, n - s, n);
                    }
                    m = Med3(list, l, m, n); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                medianIndex = Sorting.BinarySearchFromTo(splitters, list[m], splitFrom, splitTo);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int splitIndex = TriplePartition(list, secondary, tertiary, from, to, splitter);
            splitIndexes[medianIndex] = splitIndex;

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(splitter < splitters[i]))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(splitter > splitters[i]))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                TriplePartition(list, secondary, tertiary, from, splitIndex, splitters, splitFrom, medianIndex - 1, splitIndexes);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                TriplePartition(list, secondary, tertiary, splitIndex + 1, to, splitters, medianIndex + 1, splitTo, splitIndexes);
            }
        }

        /// <summary>
        /// Same as <see cref="TriplePartition(int[], int[], int[], int, int, int)"/> 
        /// except that it <i>synchronously</i> partitions <i>double[]</i> rather than <i>int[]</i> arrays.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="tertiary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static int TriplePartition(double[] list, double[] secondary, double[] tertiary, int from, int to, double splitter)
        {
            double element;  // int, double --> template type dependent
            for (int i = from - 1; ++i <= to;)
            {
                element = list[i];
                if (element < splitter)
                {
                    // swap x[i] with x[from]
                    list[i] = list[from];
                    list[from] = element;

                    element = secondary[i];
                    secondary[i] = secondary[from];
                    secondary[from] = element;

                    element = tertiary[i];
                    tertiary[i] = tertiary[from];
                    tertiary[from++] = element;
                }
            }

            return from - 1;
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int[], int, int, int[])"/> except that this method <i>synchronously</i> partitions three arrays at the same time;
        /// all three arrays are partially sorted according to the elements of the primary array.
        /// In other words, each time an element in the primary array is moved from index A to B, the correspoding element within the secondary array as well as the corresponding element within the tertiary array are also moved from index A to B.
        /// <p>
        /// <b>Use cases:</b>
        /// <p>
        /// Image having a large list of 3-dimensional pointsd 
        /// If memory consumption and performance matter, it is a good idea to physically lay them out as three 1-dimensional arrays
        /// (using something like <i>Point3D</i> objects would be prohibitively expensive, both in terms of time and space).
        /// Now imagine wanting to histogram the points.
        /// We may want to partially sort the points by x-coordinate into intervals.
        /// This method efficiently does the job.
        /// <p>
        /// <b>Performance:</b>
        /// <p>
        /// Same as for single-partition methods.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="tertiary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitters"></param>
        /// <param name="splitFrom"></param>
        /// <param name="splitTo"></param>
        /// <param name="splitIndexes"></param>
        public static void TriplePartition(int[] list, int[] secondary, int[] tertiary, int from, int to, int[] splitters, int splitFrom, int splitTo, int[] splitIndexes)
        {
            int splitter; // int, double --> template type dependent

            if (splitFrom > splitTo) return; // nothing to do
            if (from > to)
            { // all bins are empty
                from--;
                for (int i = splitFrom; i <= splitTo;) splitIndexes[i++] = from;
                return;
            }

            // Choose a partition (pivot) index, m
            // Ideally, the pivot should be the median, because a median splits a list into two equal sized sublists.
            // However, computing the median is expensive, so we use an approximation.
            int medianIndex;
            if (splitFrom == splitTo)
            { // we don't really have a choice
                medianIndex = splitFrom;
            }
            else
            { // we do have a choice
                int m = (from + to) / 2;       // Small arrays, middle element
                int len = to - from + 1;
                if (len > SMALL)
                {
                    int l = from;
                    int n = to;
                    if (len > MEDIUM)
                    {        // Big arrays, pseudomedian of 9
                        int s = len / 8;
                        l = Med3(list, l, l + s, l + 2 * s);
                        m = Med3(list, m - s, m, m + s);
                        n = Med3(list, n - 2 * s, n - s, n);
                    }
                    m = Med3(list, l, m, n); // Mid-size, pseudomedian of 3
                }

                // Find the splitter closest to the pivot, i.ed the splitter that best splits the list into two equal sized sublists.
                medianIndex = Sorting.BinarySearchFromTo(splitters, list[m], splitFrom, splitTo);
                if (medianIndex < 0) medianIndex = -medianIndex - 1; // not found
                if (medianIndex > splitTo) medianIndex = splitTo; // not found, one past the end

            }
            splitter = splitters[medianIndex];

            // Partition the list according to the splitter, i.e.
            // Establish invariant: list[i] < splitter <= list[j] for i=from..medianIndex and j=medianIndex+1 .d to
            int splitIndex = TriplePartition(list, secondary, tertiary, from, to, splitter);
            splitIndexes[medianIndex] = splitIndex;

            // Optimization: Handle special cases to cut down recursions.
            if (splitIndex < from)
            { // no element falls into this bin
              // all bins with splitters[i] <= splitter are empty
                int i = medianIndex - 1;
                while (i >= splitFrom && (!(splitter < splitters[i]))) splitIndexes[i--] = splitIndex;
                splitFrom = medianIndex + 1;
            }
            else if (splitIndex >= to)
            { // all elements fall into this bin
              // all bins with splitters[i] >= splitter are empty
                int i = medianIndex + 1;
                while (i <= splitTo && (!(splitter > splitters[i]))) splitIndexes[i++] = splitIndex;
                splitTo = medianIndex - 1;
            }

            // recursively partition left half
            if (splitFrom <= medianIndex - 1)
            {
                TriplePartition(list, secondary, tertiary, from, splitIndex, splitters, splitFrom, medianIndex - 1, splitIndexes);
            }

            // recursively partition right half
            if (medianIndex + 1 <= splitTo)
            {
                TriplePartition(list, secondary, tertiary, splitIndex + 1, to, splitters, medianIndex + 1, splitTo, splitIndexes);
            }
        }

        /// <summary>
        /// Same as <see cref="Partition(int[], int, int, int)"/> except that this method <i>synchronously</i> partitions three arrays at the same time;
        /// all three arrays are partially sorted according to the elements of the primary array.
        /// In other words, each time an element in the primary array is moved from index A to B, the correspoding element within the secondary array as well as the corresponding element within the tertiary array are also moved from index A to B.
        /// <p>
        /// <b>Performance:</b>
        /// <p>
        /// Same as for single-partition methods.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="secondary"></param>
        /// <param name="tertiary"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static int TriplePartition(int[] list, int[] secondary, int[] tertiary, int from, int to, int splitter)
        {
            int element;  // int, double --> template type dependent
            for (int i = from - 1; ++i <= to;)
            {
                element = list[i];
                if (element < splitter)
                {
                    // swap x[i] with x[from]
                    list[i] = list[from];
                    list[from] = element;

                    element = secondary[i];
                    secondary[i] = secondary[from];
                    secondary[from] = element;

                    element = tertiary[i];
                    tertiary[i] = tertiary[from];
                    tertiary[from++] = element;
                }
            }

            return from - 1;
        }
    }
}
