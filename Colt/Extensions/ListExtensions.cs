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
    }
}
