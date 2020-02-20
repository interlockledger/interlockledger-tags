/******************************************************************************************************************************

Copyright (c) 2018-2020 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace InterlockLedger.Tags
{
    public static class IEnumerableOfTExtensions
    {
        public static bool AnyWithNoNulls<T>(this IEnumerable<T> items) => items.SafeAny() && items.NoNulls();

        public static IEnumerable<T> AppendedOf<T>(this T item, IEnumerable<T> remainingItems) => InnerAppend(item, remainingItems);

        public static IEnumerable<T> AppendedOf<T>(this T item, params T[] remainingItems) => InnerAppend(item, remainingItems);

        public static bool EqualTo<T>(this IEnumerable<T> first, IEnumerable<T> second)
            => first is null ? second is null : second is null ? false : first.SequenceEqual(second);

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "SafeAny checks")]
        public static IEnumerable<T> IfAnyDo<T>(this IEnumerable<T> values, Action action) {
            if (values.SafeAny())
                action();
            return values;
        }

        public static string JoinedBy<T>(this IEnumerable<T> list, string joiner) => list == null ? string.Empty : string.Join(joiner, list);

        public static bool None<T>(this IEnumerable<T> items) => !items.SafeAny();

        public static bool None<T>(this IEnumerable<T> items, Func<T, bool> predicate) => !items.SafeAny(predicate);

        public static bool NoNulls<T>(this IEnumerable<T> items) => items.None(item => item is null);

        public static bool SafeAny<T>(this IEnumerable<T> values) => values?.Any() ?? false;

        public static bool SafeAny<T>(this IEnumerable<T> items, Func<T, bool> predicate) => items?.Any(predicate) == true;

        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> items, IEnumerable<T> remainingItems)
            => InnerConcat(items ?? Enumerable.Empty<T>(), remainingItems);

        public static int SafeCount<T>(this IEnumerable<T> values) => values?.Count() ?? -1;

        public static bool SafeSequenceEqual<T>(this IEnumerable<T> values, IEnumerable<T> otherValues)
            => values?.SequenceEqual(EmptyIfNull(otherValues)) ?? otherValues.None();

        public static IEnumerable<TResult> SelectSkippingNulls<TSource, TResult>(this IEnumerable<TSource> values, Func<TSource, TResult> selector) where TResult : class
            => EmptyIfNull(values?.Select(selector).SkipNulls());

        public static IEnumerable<T> SkipNulls<T>(this IEnumerable<T> values) where T : class => EmptyIfNull(values?.Where(item => item != null));

        public static string WithCommas<T>(this IEnumerable<T> list, bool noSpaces = false) => JoinedBy(list, noSpaces ? "," : ", ");

        public static IEnumerable<T> WithDefault<T>(this IEnumerable<T> values, Func<IEnumerable<T>> alternativeValues)
            => values.SafeAny() ? values : EmptyIfNull(alternativeValues?.Invoke());

        public static IEnumerable<T> WithDefault<T>(this IEnumerable<T> values) => EmptyIfNull(values);

        public static IEnumerable<T> WithDefault<T>(this IEnumerable<T> values, params T[] alternativeValues)
            => WithDefault(values, (IEnumerable<T>)alternativeValues);

        public static IEnumerable<T> WithDefault<T>(this IEnumerable<T> values, IEnumerable<T> alternativeValues)
            => values.SafeAny() ? values : EmptyIfNull(alternativeValues);

        private static IEnumerable<T> EmptyIfNull<T>(IEnumerable<T> values) => values ?? Enumerable.Empty<T>();

        private static IEnumerable<T> InnerAppend<T>(T item, IEnumerable<T> remainingItems)
            => new SingleEnumerable<T>(item).SafeConcat(remainingItems);

        private static IEnumerable<T> InnerConcat<T>(IEnumerable<T> items, IEnumerable<T> remainingItems)
            => remainingItems.SafeAny() ? items.Concat(remainingItems) : items;
    }
}