// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
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

namespace InterlockLedger.Tags;

public class DataIndex : IEquatable<DataIndex>
{
    public const string PrimaryIndexName = "~Primary~";

    public IEnumerable<DataIndexElement> Elements { get; set; } = Enumerable.Empty<DataIndexElement>();

    [JsonIgnore]
    public string ElementsAsString {
        get => Elements.JoinedBy("|");
        init => Elements = DataIndexElement.ParseList(value.Required());
    }

    public bool IsUnique { get; set; }

    public string Name { get; set; } = "?";

    public override bool Equals(object? obj) => Equals(obj as DataIndex);

    public bool Equals(DataIndex? other) =>
        other is not null &&
        ElementsAsString == other.ElementsAsString &&
        IsUnique == other.IsUnique &&
        Name == other.Name;

    public override int GetHashCode() => HashCode.Combine(ElementsAsString, IsUnique, Name);

    public override string ToString() => $"{Name}^{ElementsAsString}";
}

public class DataIndexElement
{
    public bool DescendingOrder { get; set; }

    public string FieldPath { get; set; } // in the form 'FieldName.ChildrenFieldName'

    public DataIndexElement(string fieldPath) => FieldPath = fieldPath;

    public string? Function { get; set; }

    [JsonIgnore]
    public bool HasFunction => !string.IsNullOrWhiteSpace(Function);

    public override string ToString() {
        var sb = new StringBuilder();
        sb.Append(FieldPath);
        var hasFunction = HasFunction;
        if (DescendingOrder || hasFunction) {
            sb.Append(':').Append(DescendingOrder ? "-" : "+");
            if (hasFunction)
                sb.Append(':').Append(Function);
        }
        return sb.ToString();
    }

    internal static IEnumerable<DataIndexElement> ParseList(string Value) {
        var elements = new List<DataIndexElement>();
        foreach (var element in Value.Split('|'))
            elements.Add(FromParts(element.Split(':')));
        return elements;
    }

    private static DataIndexElement FromParts(string[] parts) => new(parts[0]) {
        DescendingOrder = parts.Length > 1 && parts[1] == "-",
        Function = parts.Length > 2 ? parts[2].TrimToNull() : null
    };
}

public class ILTagDataIndex : ILTagExplicit<DataIndex>
{
    public ILTagDataIndex(DataIndex index) : base(ILTagId.DataIndex, index) {
    }

    public ILTagDataIndex(Stream s) : base(ILTagId.DataIndex, s) {
    }

    protected override DataIndex FromBytes(byte[] bytes)
        => FromBytesHelper(bytes, s => new DataIndex {
            Name = s.DecodeString().WithDefault("?"),
            IsUnique = s.DecodeBool(),
            ElementsAsString = s.DecodeString().Safe(),
        });

    protected override byte[] ToBytes(DataIndex value)
         => TagHelpers.ToBytesHelper(s => {
             s.EncodeString(Value.Required().Name);
             s.EncodeBool(Value.IsUnique);
             s.EncodeString(Value.ElementsAsString);
         });
}