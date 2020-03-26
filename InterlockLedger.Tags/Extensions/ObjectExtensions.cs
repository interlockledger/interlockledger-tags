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
using System.Globalization;
using System.Linq;
using System.Text.Json;
using IEnum = System.Collections.IEnumerable;

namespace InterlockLedger.Tags
{
    public static class ObjectExtensions
    {
        public static object AsNavigable(this object value) {
            if (value is null)
                return null;
            if (value.GetType().IsPrimitive || value is string || value is IJsonCustomized)
                return value;
            if (value is JsonElement jo)
                return AsILTag(FromJsonElement(jo));
            if (value is IEnum items)
                return items.AsList<object>();
            return AsILTag(ToDictionary(value));
        }

        public static IEnumerable<T> AsSingle<T>(this T s) => new SingleEnumerable<T>(s);

        public static List<T> AsSingleList<T>(this T s) => s.AsSingle().ToList();

        public static string PadLeft(this object value, int totalWidth)
            => value?.ToString().PadLeft(totalWidth);

        public static string PadRight(this object value, int totalWidth)
            => value?.ToString().PadRight(totalWidth);

        [SuppressMessage("Style", "RCS1196:Call extension method as instance method.", Justification = "Better clarity about reuse of method name")]
        public static string WithDefault(this object value, string @default) => StringExtensions.WithDefault(value?.ToString(), @default);

        private static object AsILTag(object o)
            => o is Dictionary<string, object> dict && dict.Count == 2 && dict.ContainsKey("TagId") && dict.ContainsKey("Value")
                ? ILTag.DeserializeFromJson(Convert.ToUInt64(dict["TagId"], CultureInfo.InvariantCulture), dict["Value"])
                : o;

        private static object FromJsonElement(JsonElement jo) {
            switch (jo.ValueKind) {
            case JsonValueKind.False:
                return false;

            case JsonValueKind.True:
                return true;

            case JsonValueKind.Object:
                return jo.EnumerateObject().ToDictionary(p => p.Name, pp => pp.Value.AsNavigable(), StringComparer.InvariantCultureIgnoreCase);

            case JsonValueKind.Array:
                return jo.EnumerateArray().Select(js => js.AsNavigable()).ToArray();

            case JsonValueKind.String:
                return jo.GetString();

            case JsonValueKind.Number:
                if (jo.TryGetUInt64(out var value))
                    return value;
                return jo.GetInt64();

            default:
                return null;
            }
        }

        private static Dictionary<string, object> ToDictionary(object value) {
            var dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var p in value.GetType().GetProperties()) {
                object propertyValue = p.GetValue(value, null);
                object navigable = AsNavigable(propertyValue);
                dictionary[p.Name] = navigable;
            }

            return dictionary;
        }
    }
}
