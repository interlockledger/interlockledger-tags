// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

#nullable enable

using System.Globalization;
using System.Text.Json;

using IEnum = System.Collections.IEnumerable;

namespace InterlockLedger.Tags;
public static class ObjectExtensions
{
    public static string AsJson<T>(this T json) => JsonSerializer.Serialize<T>(json, StringExtensions._jsonOptions);

    public static object? AsNavigable(this object? value)
        => value switch {
            null => null,
            string s => s,
            ITextual o => o,
            JsonElement jo => AsILTag(FromJsonElement(jo)),
            IEnum items => items.AsList<object>(),
            _ => IsPrimitive(value) ? value : AsILTag(ToDictionary(value))
        };

    private static object? AsILTag(object? o)
        => o is Dictionary<string, object> dict && dict.Count == 2 && dict.TryGetValue("TagId", out object? tagId) && dict.TryGetValue("Value", out object? value)
            ? TagProvider.DeserializeFromJson(Convert.ToUInt64(tagId, CultureInfo.InvariantCulture), value)
            : o;

    private static object? FromJsonElement(JsonElement jo) {
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

    private static bool IsPrimitive(object? value) => value?.GetType().IsPrimitive ?? false;

    private static Dictionary<string, object?> ToDictionary(object value) {
        var dictionary = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var p in value.GetType().GetProperties()) {
            object? propertyValue = p.GetValue(value, null);
            dictionary[p.Name] = AsNavigable(propertyValue);
        }
        return dictionary;
    }
}