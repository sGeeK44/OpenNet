using System;
using System.Collections.Generic;
using System.Linq;
using OpenNet.Orm.Interfaces;

namespace OpenNet.Orm
{
    public static class Extensions
    {
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static void CreateOrUpdateStore(this IDataStore store)
        {
            if(store.StoreExists)
            {
                store.EnsureCompatibility();
            }
            else
            {
                store.CreateStore();
            }
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using
        /// the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="obj1">An System.Collections.Generic.IEnumerable`1 to compare to second.</param>
        /// <param name="obj2">An System.Collections.Generic.IEnumerable`1 to compare to the first sequence.</param>
        /// <returns>
        /// true if the two source sequences are of equal length and their corresponding
        /// elements are equal according to the default equality comparer for their type;
        /// otherwise, false.
        /// </returns>
        public static bool IsEquals<T>(this List<T> obj1, List<T> obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return obj1 == null && obj2 == null;
            }

            return obj1.SequenceEqual(obj2);
        }
    }
}
