using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ListExtensions
    {
        public static void EnsureCapacity<T>(this List<T> list, int minCapacity)
        {
            list = list.ToArray().EnsureCapacity(minCapacity).ToList();
        }

        public static Boolean ForEach<T>(this List<Double> list, Cern.Colt.Function.DoubleProcedure procedure)
        {
            Double[] theElements = list.ToArray();
            int theSize = list.Count;

            for (int i = 0; i < theSize;) if (!procedure(theElements[i++])) return false;
            return true;
        }

        public static void SetSize<T>(this List<T> list, int size)
        {
            EnsureCapacity(list, size);
        }

        public static List<T> Copy<T>(this List<T> list)
        {
            T[] buf = new T[list.Count];
            list.CopyTo(buf);

            return new List<T>(buf);
        }

        public static int BinarySearchFromTo<T>(this List<T> list, T item, int from, int to)
        {
            var dc = new DefaultComparer<T>();
            if (from > to)
                throw new IndexOutOfRangeException();

            int count = to - from;
            return list.BinarySearch(from, count, item, dc);
        }

        private class DefaultComparer<T> : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                if (Nullable.GetUnderlyingType(x.GetType()) != null)
                {
                    if (x == null)
                    {
                        if (y == null)
                        {
                            // If x is null and y is null, they're
                            // equald 
                            return 0;
                        }
                        else
                        {
                            // If x is null and y is not null, y
                            // is greaterd 
                            return -1;
                        }
                    }
                    else
                    {
                        // If x is not null...
                        //
                        if (y == null)
                        // ...and y is null, x is greater.
                        {
                            return 1;
                        }
                        else
                        {
                            return Comparer<T>.Default.Compare(x, y);
                        }
                    }
                }
                else
                {
                    return Comparer<T>.Default.Compare(x, y);
                }
            }
        }

        /// <summary>
        /// Replaces a number of elements in the receiver with the same number of elements of another list.
        /// Replaces elements in the receiver, between <code>from</code> (inclusive) and <code>to</code> (inclusive),
        /// with elements of <code>other</code>, starting from <code>otherFrom</code> (inclusive).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="from">the position of the first element to be replaced in the receiver</param>
        /// <param name="to">the position of the last element to be replaced in the receiver</param>
        /// <param name="other">list holding elements to be copied into the receiver.</param>
        /// <param name="otherFrom">position of first element within other list to be copied.</param>
        public static void ReplaceFromToWithFrom<T>(this List<T> list, int from, int to, List<T> other, int otherFrom)
        {
            // overridden for performance only.
            //if (!(other is List<T>)) {
            //    // slower
            //    base.ReplaceFromToWithFrom(from, to, other, otherFrom);
            //    return;
            //}
            int Length = to - from + 1;
            if (Length > 0)
            {
                list.CheckRangeFromTo(from, to, list.Count);
                other.CheckRangeFromTo(otherFrom, otherFrom + Length - 1, other.Count);
                //Array.Copy(((List<T>)other).Elements, otherFrom, Elements, from, Length);
                int count = to - from;
                list.RemoveRange(from, count);
                list.InsertRange(from, other.GetRange(otherFrom, count));

            }
        }

        /// <summary>
        /// Checks if the given range is within the contained array's bounds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="theSize"></param>
        /// <exception cref="IndexOutOfRangeException">if <i>to!=from-1 || from&lt;0 || from&gt;to || to&gt;=size()</i>.</exception>
        public static void CheckRangeFromTo<T>(this List<T> list, int from, int to, int theSize)
        {
            if (to == from - 1) return;
            if (from < 0 || from > to || to >= theSize)
                throw new IndexOutOfRangeException("from: " + from + ", to: " + to + ", size=" + theSize);
        }
    }
}
