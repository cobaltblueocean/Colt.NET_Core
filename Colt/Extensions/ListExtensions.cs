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
                            // equal. 
                            return 0;
                        }
                        else
                        {
                            // If x is null and y is not null, y
                            // is greater. 
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
    }
}
