/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public class EnumerationDetails : IEquatable<EnumerationDetails>
    {
        public EnumerationDetails() {
        }

        public EnumerationDetails(string name, string description) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
        }

        public string Description { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj) => Equals(obj as EnumerationDetails);

        public bool Equals(EnumerationDetails other) => other != null && Description == other.Description && Name == other.Name;

        public override int GetHashCode() {
            var hashCode = -1174144137;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public override string ToString() => $"{Name}:{Description}";

        internal FullEnumerationDetails ToFull(ulong index) => new FullEnumerationDetails { Index = index, Description = Description, Name = Name };
    }

    public class EnumerationItems : IJsonCustom<EnumerationItems>
    {
        public EnumerationItems() { }

        public EnumerationItems(Dictionary<ulong, EnumerationDetails> values) => _details.AddRange(values?.Select(p => p.Value.ToFull(p.Key)));

        [JsonIgnore]
        public bool IsEmpty => _details.None();

        [JsonIgnore]
        public string TextualRepresentation => $"{_detailSeparator}" + _details.JoinedBy($"{_detailSeparator}");

        public EnumerationItems ResolveFrom(string textualRepresentation) {
            _details.Clear();
            _details.AddRange(textualRepresentation.Split(_detailSeparator, StringSplitOptions.RemoveEmptyEntries).Select(t => new FullEnumerationDetails().FromTextualRepresentation(t)));
            return this;
        }

        internal const char _detailSeparator = '#';

        internal Dictionary<ulong, EnumerationDetails> UpdateFrom() => IsEmpty ? null : ToDictionary();

        private readonly List<FullEnumerationDetails> _details = new List<FullEnumerationDetails>();

        private Dictionary<ulong, EnumerationDetails> ToDictionary() {
            var values = new Dictionary<ulong, EnumerationDetails>();
            foreach (var d in _details)
                values.Add(d.Index, d.Shorter);
            return values;
        }
    }

    internal class FullEnumerationDetails : EnumerationDetails
    {
        public FullEnumerationDetails() { }

        public ulong Index { get; set; }
        public EnumerationDetails Shorter => new EnumerationDetails { Name = Name, Description = Description };

        public override string ToString() => $"{Index}{_fieldSeparator}{Normalize(Name)}{_fieldSeparator}{Normalize(Description)}{_fieldSeparator}";

        internal FullEnumerationDetails FromTextualRepresentation(string parsing) {
            if (parsing is null)
                throw new ArgumentNullException(nameof(parsing));
            var parts = parsing.Split(_fieldSeparator, StringSplitOptions.RemoveEmptyEntries);
            Index = Convert.ToUInt64(parts[0]);
            Name = parts[1];
            Description = parts.Length > 2 ? parts[2] : null;
            return this;
        }

        private const char _fieldSeparator = '|';

        private static string Normalize(string text) => text?.Replace(_fieldSeparator, '_').Replace(EnumerationItems._detailSeparator, '?');
    }
}