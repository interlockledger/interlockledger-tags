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

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<EnumerationItems>))]
[JsonConverter(typeof(JsonCustomConverter<EnumerationItems>))]
public partial class EnumerationItems : ITextual<EnumerationItems>
{
    public EnumerationItems() { }

    public EnumerationItems(string textualRepresentation) {
        var items = textualRepresentation.Safe()
                                         .Split(_detailSeparator, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(t => new FullEnumerationDetails(t));
        _details.AddRange(items);
    }

    public EnumerationItems(Dictionary<ulong, EnumerationDetails> values) => _ = Resolve(values, ConvertFromDictionary);

    [JsonIgnore]
    public bool IsEmpty => _details.None();

    public bool IsInvalid => false;

    [JsonIgnore]
    public string TextualRepresentation => IsEmpty ? null : $"{_detailSeparator}" + _details.JoinedBy($"{_detailSeparator}");

    public static EnumerationItems Empty { get; } = new EnumerationItems();
    public static EnumerationItems Invalid { get; } = new EnumerationItems();
    public static Regex Mask { get; } = AnythingRegex();
    public static string MessageForMissing { get; } = "No Enumeration Details";

    public EnumerationItems ResolveFrom(string textualRepresentation) => Resolve(textualRepresentation, ConvertFromString);

    internal const char _detailSeparator = '#';

    internal EnumerationDictionary UpdateFrom() => IsEmpty ? null : ToDictionary();

    private readonly List<FullEnumerationDetails> _details = new();

    private static IEnumerable<FullEnumerationDetails> ConvertFromDictionary(Dictionary<ulong, EnumerationDetails> values)
        => values.Select(p => p.Value.ToFull(p.Key));

    private static IEnumerable<FullEnumerationDetails> ConvertFromString(string s)
        => s.Split(_detailSeparator, StringSplitOptions.RemoveEmptyEntries).Select(t => new FullEnumerationDetails(t));

    private EnumerationItems Resolve<T>(T originalData, Func<T, IEnumerable<FullEnumerationDetails>> converter) {
        _details.Clear();
        if (originalData is not null)
            _details.AddRange(converter(originalData));
        return this;
    }

    private EnumerationDictionary ToDictionary() => new(_details.ToDictionary(d => d.Index, dd => dd.Shorter));
    public static EnumerationItems Parse(string s, IFormatProvider provider) => FromString(s);
    public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out EnumerationItems result)
        => ITextual<EnumerationItems>.TryParse(s, out result);
    public static EnumerationItems FromString(string textualRepresentation) => new(textualRepresentation);
    public static string MessageForInvalid(string textualRepresentation) => $"Invalid Enumeration Details '{textualRepresentation}'";
    [GeneratedRegex(".+")]
    private static partial Regex AnythingRegex();
}