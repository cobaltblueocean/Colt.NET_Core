using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ArrayExtensions
    {
        public static T[] EnsureCapacity<T>(this T[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            T[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity * 3) / 2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new T[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        //public static byte[] EnsureCapacity(this byte[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    byte[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new byte[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static char[] EnsureCapacity(this char[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    char[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new char[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static double[] EnsureCapacity(this double[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    double[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new double[newCapacity];
        //        //for (int i = oldCapacity; --i >= 0; ) newArray[i] = array[i];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static float[] EnsureCapacity(this float[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    float[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new float[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static int[] EnsureCapacity(this int[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    int[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new int[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static long[] EnsureCapacity(this long[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    long[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new long[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static Object[] EnsureCapacity(this Object[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    Object[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new Object[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static short[] EnsureCapacity(this short[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    short[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new short[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static Boolean[] EnsureCapacity(this Boolean[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    Boolean[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new Boolean[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}



    }
}
