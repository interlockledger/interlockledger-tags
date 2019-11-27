/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;

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

        public FullEnumerationDetails ToFull(ulong index) => new FullEnumerationDetails { Index = index, Description = Description, Name = Name };

        public override string ToString() => $"{Name}: {Description}";
    }

    public class FullEnumerationDetails : EnumerationDetails
    {
        public FullEnumerationDetails() { }

        public FullEnumerationDetails(string parsing) {
            if (parsing is null)
                throw new ArgumentNullException(nameof(parsing));
            var parts = parsing.Split('|', StringSplitOptions.RemoveEmptyEntries);
            Index = Convert.ToUInt64(parts[0].Replace("#",""));
            Name = parts[1];
            Description = parts.Length > 2 ? parts[2] : null;
        }

        public ulong Index { get; set; }

        public EnumerationDetails Shorter => new EnumerationDetails { Name = Name, Description = Description };

        public override string ToString() => $"|#{Index}|{Normalize(Name)}|{Normalize(Description)}|";

        private static string Normalize(string text) => text?.Replace('|', '_').Replace(':', '?');
    }
}