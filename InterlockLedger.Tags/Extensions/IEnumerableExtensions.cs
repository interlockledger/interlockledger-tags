/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using IEnum = System.Collections.IEnumerable;

namespace InterlockLedger.Tags
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> AsList<T>(this IEnum items) where T : class => AsNavigableList(items).Select(o => o as T);

        public static IEnumerable<object> AsNavigableList(IEnum items) => from object item in items select item.AsNavigable();
    }
}