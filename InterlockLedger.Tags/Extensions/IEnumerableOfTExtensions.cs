/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace InterlockLedger.Tags
{
    public static class IEnumerableOfTExtensions
    {
        public static IEnumerable<T> AppendedOf<T>(this T item, IEnumerable<T> remainingItems) => InnerAppend(item, remainingItems);

        public static IEnumerable<T> AppendedOf<T>(this T item, params T[] remainingItems) => InnerAppend(item, remainingItems);

        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> items, IEnumerable<T> remainingItems)
            => InnerConcat(items ?? Enumerable.Empty<T>(), remainingItems);

        private static IEnumerable<T> InnerAppend<T>(T item, IEnumerable<T> remainingItems)
            => new T[] { item }.SafeConcat(remainingItems);

        private static IEnumerable<T> InnerConcat<T>(IEnumerable<T> items, IEnumerable<T> remainingItems)
            => remainingItems.SafeAny() ? items.Concat(remainingItems) : items;
    }
}