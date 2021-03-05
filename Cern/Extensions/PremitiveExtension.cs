using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// PremitiveTypeExtension Description
    /// </summary>
    public static class PremitiveExtension
    {
        public static int FloatToIntBits(this float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        public static Boolean AlmostEquals(this Double x, Double y, Double Esp)
        {
            return System.Math.Abs(x - y) < Esp;
        }

        public static Boolean AlmostEquals(this int x, int y, int Esp)
        {
            return System.Math.Abs(x - y) < Esp;
        }

        public static Boolean AlmostEquals(this float x, float y, float Esp)
        {
            return System.Math.Abs(x - y) < Esp;
        }

        public static Boolean AlmostEquals(this long x, long y, long Esp)
        {
            return System.Math.Abs(x - y) < Esp;
        }

        public static Boolean AlmostEquals(this decimal x, decimal y, decimal Esp)
        {
            return System.Math.Abs(x - y) < Esp;
        }


        public static T[] ToArray<T>(this T val) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return new T[] { val };
        }

        public static Nullable<T>[] ToNullableArray<T>(this T val) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return val.ToArray().Cast<Nullable<T>>().ToArray();
        }

        public static Nullable<T>[] ToArray<T>(this Nullable<T> val) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return new Nullable<T>[] { val };
        }

        public static Nullable<T>[] ToNullableArray<T>(this IEnumerable<T> val) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return val.ToList().Cast<Nullable<T>>().ToArray();
        }

        public static Nullable<T>[] ToNullableArray<T>(this IEnumerable<T> val, int length) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return val.Take(length).ToList().Cast<Nullable<T>>().ToArray();
        }

        public static T[] ToPremitiveArray<T>(this IEnumerable<Nullable<T>> val) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return val.ToList().Cast<T>().ToArray();
        }

        public static T[] ToPremitiveArray<T>(this IEnumerable<Nullable<T>> val, int length) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            return val.Take(length).ToList().Cast<T>().ToArray();
        }

        public static Boolean IsInfinite(this Double val)
        {
            return (val == Double.PositiveInfinity || val == Double.NegativeInfinity);
        }

        //public static Boolean IsInfinite(this Int16 val)
        //{
        //    return (val == Int16.PositiveInfinity || val == Int16.NegativeInfinity);
        //}

        //public static Boolean IsInfinite(this Int32 val)
        //{
        //    return (val == Int32.PositiveInfinity || val == Int32.NegativeInfinity);
        //}

        //public static Boolean IsInfinite(this Int64 val)
        //{
        //    return (val == Int64.PositiveInfinity || val == Int64.NegativeInfinity);
        //}

        public static Boolean IsInfinite(this float val)
        {
            return (val == float.PositiveInfinity || val == float.NegativeInfinity);
        }

        //public static Boolean IsInfinite(this Decimal val)
        //{
        //    return (val == Decimal.PositiveInfinity || val == Decimal.NegativeInfinity);
        //}

        public static int ToInt32(this bool b)
        {
            return b ? 1 : 0;
        }

        public static int ToSafeInt(this double value)
        {
            int result;

            if (!int.TryParse(value.ToString(), out result))
            {
                if (value <= (double)Int32.MinValue)
                    return Int32.MinValue;
                else if (value >= (double)Int32.MaxValue)
                    return Int32.MaxValue;
            }

            return result;
        }

        public static int ToInt(this double value)
        {
            return Convert.ToInt32(value);
        }
    }
}
