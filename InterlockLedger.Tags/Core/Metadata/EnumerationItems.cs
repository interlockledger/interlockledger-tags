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

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<EnumerationItems>))]
[JsonConverter(typeof(JsonCustomConverter<EnumerationItems>))]
public partial class EnumerationItems : ITextual<EnumerationItems>
{
    public bool IsEmpty => _details.None();
    public string TextualRepresentation { get; }
    public string? InvalidityCause { get; private init; }
    public static EnumerationItems Empty { get; } = new EnumerationItems();
    public static Regex Mask { get; } = AnythingRegex();
    public static EnumerationItems Build(string textualRepresentation) =>
        new(textualRepresentation.Safe()
                                 .Split(_detailSeparator, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(t => new FullEnumerationDetails(t)));
    public static EnumerationItems InvalidBy(string cause) => new() { InvalidityCause = cause };

    public bool Equals(EnumerationItems? other) => other is not null && _details.EquivalentTo(other._details);
    public ITextual<EnumerationItems> Textual => this;
    public override string ToString() => Textual.FullRepresentation;
    public override bool Equals(object? obj) => Equals(obj as EnumerationItems);
    public override int GetHashCode() => HashCode.Combine(Textual.FullRepresentation);

    internal const string _detailSeparator = "#";
    internal EnumerationDictionary? ToDefinition() =>
        IsEmpty || Textual.IsInvalid ? null : new(_details!.ToDictionary(d => d.Index, dd => dd.Shorter));
    internal EnumerationItems(EnumerationDictionary values) : this(values.Safe().Select(p => p.Value.ToFull(p.Key))) { }
    private EnumerationItems() => TextualRepresentation = null!;
    private EnumerationItems(IEnumerable<FullEnumerationDetails> values) {
        _details.AddRange(values);
        TextualRepresentation = IsEmpty || Textual.IsInvalid
            ? null!
            : $"{_detailSeparator}{_details.JoinedBy(_detailSeparator)}";
    }

    private readonly List<FullEnumerationDetails> _details = new();

    [GeneratedRegex("""^(#\d+\|[^\|#]+(\|[^\|#]*)?\|)*$""")]
    private static partial Regex AnythingRegex();

}