using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LiuYueSoft.Utilities
{
    /// <summary>
    /// Utilities for the <see cref="IEnumerable"/> and interfaces which extends from it.
    /// </summary>
    public static class EnumerableUtility
    {
        /// <summary>
        /// Returns true if the enumerable is null or does not iterate any element.
        /// This overload version will potentially re-enumerate the enumerable,
        /// which might have negative impact on the performance.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <typeparam name="T">The type of the elements that the enumerable iterates.</typeparam>
        /// <returns>Returns true if the enumerable is null or does not iterate any element.</returns>
        public static bool IsNullOrNotAny<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Returns true if the collection is null or does not have any element.
        /// This overload version explicitly says it does not iterate on the enumerable,
        /// thus will prevent some compiler warning about re-enumeration.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <returns>True if the collection is null or does not have any element.</returns>
        public static bool IsNullOrNotAny<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNotNullAndSizeAtLeast<T>(this ICollection<T> collection, int sizeAtLeast)
        {
            return collection != null && collection.Count >= sizeAtLeast;
        }
    }
}
