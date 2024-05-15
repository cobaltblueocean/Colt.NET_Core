using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt;

namespace System.Collections.Generic
{
    public static class ColtIDictionaryExtension
    {
        /// <summary>
        /// Chooses a new prime table capacity optimized for growing that (approximately) satisfies the invariant
        /// c * minLoadFactor &lt;= size &lt;= c * maxLoadFactor
        /// and has at least one FREE slot for the given size.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="size"></param>
        /// <param name="minLoad"></param>
        /// <param name="maxLoad"></param>
        /// <returns></returns>
        public static int ChooseGrowCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int size, double minLoad, double maxLoad)
        {
            return PrimeFinder.NextPrime(System.Math.Max(size + 1, (int)((4 * size / (3 * minLoad + maxLoad)))));
        }

        /// <summary>
        /// Returns new high water mark threshold based on current capacity and maxLoadFactor.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="capacity"></param>
        /// <param name="maxLoad"></param>
        /// <returns>the new threshold.</returns>
        public static int ChooseHighWaterMark<TKey, TValue>(this IDictionary<TKey, TValue> dic, int capacity, double maxLoad)
        {
            return System.Math.Min(capacity - 2, (int)(capacity * maxLoad)); //makes sure there is always at least one FREE slot
        }

        /// <summary>
        /// Returns new low water mark threshold based on current capacity and minLoadFactor.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="capacity"></param>
        /// <param name="minLoad"></param>
        /// <returns>the new threshold.</returns>
        public static int ChooseLowWaterMark<TKey, TValue>(this IDictionary<TKey, TValue> dic, int capacity, double minLoad)
        {
            return (int)(capacity * minLoad);
        }

        /// <summary>
        /// Chooses a new prime table capacity neither favoring shrinking nor growing,
        /// that (approximately) satisfies the invariant
        /// c * minLoadFactor &lt;= size &lt;= c * maxLoadFactor
        /// and has at least one FREE slot for the given size.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="size"></param>
        /// <param name="minLoad"></param>
        /// <param name="maxLoad"></param>
        /// <returns></returns>
        public static int ChooseMeanCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int size, double minLoad, double maxLoad)
        {
            return PrimeFinder.NextPrime(System.Math.Max(size + 1, (int)((2 * size / (minLoad + maxLoad)))));
        }

        /// <summary>
        /// Chooses a new prime table capacity optimized for shrinking that (approximately) satisfies the invariant
        /// c * minLoadFactor &lt;= size &lt;= c * maxLoadFactor
        /// and has at least one FREE slot for the given size.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="size"></param>
        /// <param name="minLoad"></param>
        /// <param name="maxLoad"></param>
        /// <returns></returns>
        public static int ChooseShrinkCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int size, double minLoad, double maxLoad)
        {
            return PrimeFinder.NextPrime(System.Math.Max(size + 1, (int)((4 * size / (minLoad + 3 * maxLoad)))));
        }
    }
}
