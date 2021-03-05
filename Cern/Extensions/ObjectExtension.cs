using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    /// ObjectExtension Description
    /// </summary>
    /// <see href="https://www.cyotek.com/blog/comparing-the-properties-of-two-objects-via-reflection"></see>
    public static class ObjectExtension
    {
        static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public static String ValueOf<T>(this T target)
        {
            if (IsNullable(typeof(T)))
            {
                if (target == null)
                    return "null";
                else
                    return target.ToString();
            }
            else
            {
                return target.ToString();
            }
        }

        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal.
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compre.</param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        public static Boolean AreObjectsEqual<T>(this T objectA, T objectB, params string[] ignoreList)
        {
            bool result;

            try
            {
                if (objectA != null && objectB != null)
                {
                    Type objectType;

                    objectType = objectA.GetType();

                    result = true; // assume by default they are equal

                    foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name)))
                    {
                        Object valueA;
                        Object valueB;

                        valueA = propertyInfo.GetValue(objectA, null);
                        valueB = propertyInfo.GetValue(objectB, null);

                        // if it is a primative type, value type or implements IComparable, just directly try and compare the value
                        if (CanDirectlyCompare(propertyInfo.PropertyType))
                        {
                            if (!AreValuesEqual(valueA, valueB))
                            {
                                Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                                result = false;
                            }
                        }
                        // if it implements IEnumerable, then scan any items
                        else if (typeof(IEnumerable<T>).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            IEnumerable<object> collectionItems1;
                            IEnumerable<object> collectionItems2;
                            int collectionItemsCount1;
                            int collectionItemsCount2;

                            // null check
                            if (valueA == null && valueB != null || valueA != null && valueB == null)
                            {
                                Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                                result = false;
                            }
                            else if (valueA != null && valueB != null)
                            {
                                collectionItems1 = ((IEnumerable<T>)valueA).Cast<object>();
                                collectionItems2 = ((IEnumerable<T>)valueB).Cast<object>();
                                collectionItemsCount1 = collectionItems1.Count();
                                collectionItemsCount2 = collectionItems2.Count();

                                // check the counts to ensure they match
                                if (collectionItemsCount1 != collectionItemsCount2)
                                {
                                    Console.WriteLine("Collection counts for property '{0}.{1}' do not match.", objectType.FullName, propertyInfo.Name);
                                    result = false;
                                }
                                // and if they do, compare each item... this assumes both collections have the same order
                                else
                                {
                                    for (int i = 0; i < collectionItemsCount1; i++)
                                    {
                                        object collectionItem1;
                                        object collectionItem2;
                                        Type collectionItemType;

                                        collectionItem1 = collectionItems1.ElementAt(i);
                                        collectionItem2 = collectionItems2.ElementAt(i);
                                        collectionItemType = collectionItem1.GetType();

                                        if (CanDirectlyCompare(collectionItemType))
                                        {
                                            if (!AreValuesEqual(collectionItem1, collectionItem2))
                                            {
                                                Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.", i, objectType.FullName, propertyInfo.Name);
                                                result = false;
                                            }
                                        }
                                        else if (!AreObjectsEqual(collectionItem1, collectionItem2, ignoreList))
                                        {
                                            Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.", i, objectType.FullName, propertyInfo.Name);
                                            result = false;
                                        }
                                    }
                                }
                            }
                        }
                        else if (propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.IsAbstract || propertyInfo.PropertyType.IsInterface)
                        {
                            if (!AreObjectsEqual(propertyInfo.GetValue(objectA, null), propertyInfo.GetValue(objectB, null), ignoreList))
                            {
                                Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                                result = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Cannot compare property '{0}.{1}'.", objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                }
                else
                    result = object.Equals(objectA, objectB);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Determines whether value instances of the specified type can be directly compared.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this value instances of the specified type can be directly compared; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        /// <summary>
        /// Compares two values and returns if they are the same.
        /// </summary>
        /// <param name="valueA">The first value to compare.</param>
        /// <param name="valueB">The second value to compare.</param>
        /// <returns><c>true</c> if both values match, otherwise <c>false</c>.</returns>
        private static bool AreValuesEqual(Object valueA, Object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }

        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static T CastType<T>(this object input)
        {
            T result = default(T);
            try
            {
                result = (T)Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                result = (T)(object)input;
            }

            return result;
        }

        public static T[] ConvertToArray<T>(this T input)
        {
            return ConvertToList<T>(input).ToArray();
        }

        public static List<T> ConvertToList<T>(this T input)
        {
            return new List<T>() { input };
        }
    }
}
