/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public class DataIndex : IEquatable<DataIndex>
    {
        public const string PrimaryIndexName = "~Primary~";

        public IEnumerable<DataIndexElement> Elements { get; set; }

        [JsonIgnore]
        public string ElementsAsString {
            get => Elements?.JoinedBy("|");
            set => Elements = DataIndexElement.ParseList(value.ValidateNonEmpty(nameof(ElementsAsString)));
        }

        public bool IsUnique { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj) => Equals(obj as DataIndex);

        public bool Equals(DataIndex other) =>
            other != null &&
            ElementsAsString == other.ElementsAsString &&
            IsUnique == other.IsUnique &&
            Name == other.Name;

        public override int GetHashCode() {
            var hashCode = 2121638569;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ElementsAsString);
            hashCode = hashCode * -1521134295 + IsUnique.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public override string ToString() => ElementsAsString;
    }

    public class DataIndexElement
    {
        public bool DescendingOrder { get; set; }

        public string FieldPath { get; set; } // in the form 'FieldName.ChildrenFieldName'

        public string Function { get; set; }

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

        private static DataIndexElement FromParts(string[] parts) => new DataIndexElement {
            FieldPath = parts[0],
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
                Name = s.DecodeString(),
                IsUnique = s.DecodeBool(),
                ElementsAsString = s.DecodeString(),
            });

        protected override byte[] ToBytes()
             => ToBytesHelper(s => {
                 s.EncodeString(Value.Name);
                 s.EncodeBool(Value.IsUnique);
                 s.EncodeString(Value.ElementsAsString);
             });
    }
}