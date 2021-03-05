using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class EnumeratorExtension
    {
        public static int Index<T>(this IEnumerator<T> enumerator)
        {
            var current = enumerator.Current;
            var list = enumerator.ToList();

            return list.IndexOf(current);
        }

        public static List<T> ToList<T>(this IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            enumerator.Reset();

            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            return list;
        }

        public static Boolean HasNext<T>(this IEnumerator<T> enumerator)
        {
            var list = enumerator.ToList();
            var index = enumerator.Index();

            return list.Count == 1 ? false : index < list.Count - 1 ? true : false;
        }

        public static T Next<T>(this IEnumerator<T> source)
        {
            source.MoveNext();
            return source.Current;
        }
    }
}
