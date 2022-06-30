using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt;

namespace System.Collections.Generic
{
    public static class DictionaryExtension
    {

        public static HashSet<TKey> KeysToHashSet<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary)
        {
            return new HashSet<TKey>(originalDictionary.Keys.ToList());
        }

        public static HashSet<TValue> ValuesToHashSet<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary)
        {
            return new HashSet<TValue>(originalDictionary.Values.ToList());
        }

        #region Extension for IDictionary<TKey, TValue>
        public static Boolean IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary)
        {
            return originalDictionary.Count == 0 ? true : false;
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, TKey key, TValue value)
        {
            if (key != null)
            {
                if (originalDictionary.Count == 0)
                {
                    originalDictionary.Add(key, value);
                }
                else
                {
                    if (originalDictionary.ContainsKey(key))
                        originalDictionary[key] = value;
                    else
                        originalDictionary.Add(key, value);
                }
            }
        }


        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
            {
                originalDictionary.Add(item);
            }
        }

        public static void AddOrUpdateAll<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    AddOrUpdate(originalDictionary, item.Key, item.Value);
                }
            }
        }

        public static TValue GetValueAtKey<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, TKey key)
        {
            if (key != null)
            {
                if (originalDictionary.ContainsKey(key))
                {
                    return originalDictionary.FirstOrDefault(x => x.Key.Equals(key)).Value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public static IDictionary<TKey, TValue> Head<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, TKey key, Boolean inclusive = false)
        {
            var keys = originalDictionary.Keys.ToArray<TKey>();
            var tmp = new List<TKey>();

            for (int i = 0; i < keys.Count(); i++)
            {
                if (!keys[i].Equals(key))
                {
                    tmp.Add(keys[i]);
                }
                else
                {
                    if (inclusive)
                    {
                        tmp.Add(keys[i]);
                    }
                    break;
                }
            }

            return (IDictionary<TKey, TValue>)originalDictionary.Where(x => tmp.Contains(x.Key));
        }

        public static IDictionary<TKey, TValue> Tail<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, TKey key, Boolean inclusive = false)
        {
            var keys = originalDictionary.Keys.ToArray<TKey>();
            var tmp = new List<TKey>();

            for (int i = keys.Count() - 1; i >= 0; i++)
            {
                if (!keys[i].Equals(key))
                {
                    tmp.Add(keys[i]);
                }
                else
                {
                    if (inclusive)
                    {
                        tmp.Add(keys[i]);
                    }
                    break;
                }
            }

            return (IDictionary<TKey, TValue>)originalDictionary.Where(x => tmp.Contains(x.Key));
        }

        public static void Remove<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, TKey[] keysRemove)
        {
            foreach (var key in keysRemove)
            {
                originalDictionary.Remove(key);
            }
        }

        public static IDictionary<TKey, TValue> SubDictionary<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, TKey startKey, Boolean inclusiveStartKey, TKey endKey, Boolean inclusiveEndKey)
        {
            var head = originalDictionary.Head(startKey, inclusiveStartKey);
            var tail = originalDictionary.Tail(endKey, inclusiveEndKey);
            var tmp = originalDictionary.Clone();

            tmp.Remove(head.Keys.ToArray<TKey>());
            tmp.Remove(tail.Keys.ToArray<TKey>());

            return tmp;
        }

        public static IDictionary<TKey, TValue> SubDictionary<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary, TKey startKeyInclusive, TKey endKeyInclusive)
        {
            var head = originalDictionary.Head(startKeyInclusive, false);
            var tail = originalDictionary.Tail(endKeyInclusive, false);
            var tmp = originalDictionary.Clone();

            tmp.Remove(head.Keys.ToArray<TKey>());
            tmp.Remove(tail.Keys.ToArray<TKey>());

            return tmp;
        }

        public static IDictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary)
        {
            return originalDictionary.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        public static IDictionary<TKey, TValue> DeepClone<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary) where TValue : ICloneable
        {
            return originalDictionary.ToDictionary(entry => entry.Key, entry => (TValue)entry.Value.Clone());
        }

        public static List<TKey> ToKeysList<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary)
        {
            return new List<TKey>(originalDictionary.Keys);
        }

        public static List<TValue> ToValuesList<TKey, TValue>(this IDictionary<TKey, TValue> originalDictionary)
        {
            return new List<TValue>(originalDictionary.Values);
        }
        #endregion

        /// <summary>
        /// Ensures that the receiver can hold at least the specified number of elements without needing to allocate new internal memory.
        /// If necessary, allocates new internal memory and increases the capacity of the receiver.
        /// <p>
        /// This method never need be called; it is for performance tuning only.
        /// Calling this method before<tt>put()</tt>ing a large number of associations boosts performance,
        /// because the receiver will grow only once instead of potentially many times.
        /// <p>
        /// <b>This default implementation does nothing.</b> Override this method if necessary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic">Target <see cref="IDictionary{TKey, TValue}"/> object</param>
        /// <param name="minCapacity">the desired minimum capacity.</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/> object that allocated size</returns>
        public static IDictionary<TKey, TValue> EnsureCapacity<TKey, TValue>(this IDictionary<TKey, TValue> dic, int minCapacity)
        {
            if (dic.Count >= minCapacity)
                return dic;
            else
            {
                Dictionary<TKey, TValue> newDic = new Dictionary<TKey, TValue>(minCapacity);
                foreach (KeyValuePair<TKey, TValue> pair in dic)
                {
                    newDic.Add(pair.Key, pair.Value);
                }

                return (IDictionary<TKey, TValue>)newDic;
            }
        }

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

        /// <summary>
        /// Trim the excess items from the Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> TrimExcess<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            var kv = new KeyValuePair<TKey, TValue>[dic.Count];
            dic.CopyTo(kv, 0);
            List<KeyValuePair<TKey, TValue>> l = kv.ToList();
            l.TrimExcess();
            var newDic = new Dictionary<TKey, TValue>(l.Count);
            AutoParallel.AutoParallelForEach(l, (p) =>
            {
                newDic.Add(p.Key, p.Value);
            });
            return newDic;
        }
    }
}
