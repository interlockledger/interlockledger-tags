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

#nullable enable

namespace InterlockLedger.Tags;

[SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "Implemented sealed in base class")]
public partial class InterlockId
{
    public sealed class Parts : IEquatable<Parts>
    {
        public HashAlgorithm Algorithm;
        public byte[]? Data;
        public byte Type;

        public Parts() { }

        public override string ToString() => ToShortString();
        public override int GetHashCode() {
            var hash = new HashCode();
            hash.Add(Algorithm);
            hash.AddBytes(Data ?? _noBytes);
            hash.Add(Type);
            return hash.ToHashCode();
        }
        private static readonly byte[] _noBytes  = Array.Empty<byte>();
        public override bool Equals(object? obj) => Equals(obj as Parts);
        public bool Equals(Parts? other) => other is not null && Algorithm == other.Algorithm && Type == other.Type && Data.EqualTo(other.Data);
        internal static byte DefaultType;

        internal Parts(Stream s) {
            if (s.ILIntDecode() != ILTagId.InterlockId)
                throw new InvalidDataException($"This is not an {nameof(InterlockId)}");
            FromStream(s, (int)s.ILIntDecode());
        }

        internal Parts(byte type, TagHash hash) : this(type, hash.Required().Algorithm, hash.Data.Required()) { }

        internal Parts(byte type, HashAlgorithm algorithm, byte[]? data) {
            Algorithm = algorithm;
            Data = data.Required();
            Type = type;
        }

        internal Parts(string textualRepresentation) {
            static HashAlgorithm ToHashAlgorithm(string suffix) => (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), suffix, ignoreCase: true);

            static (string strippedOfSuffix, HashAlgorithm algorithm) ParseSuffix(string textualRepresentation) {
                var parts = textualRepresentation.Split(_suffixSeparator);
                return (parts[0], algorithm: parts.Length < 2 ? _defaultAlgorithm : ToHashAlgorithm(parts[1]));
            }
            (string strippedOfSuffix, var algorithm) = ParseSuffix(textualRepresentation);
            var parts = strippedOfSuffix.Split(_prefixSeparator);
            Algorithm = algorithm;
            if (parts.Length == 1) {
                Data = parts[0].FromSafeBase64();
                Type = DefaultType;
            } else {
                Data = parts[1].FromSafeBase64();
                Type = ToType(parts[0]);
            }
        }

        internal Parts(Stream s, int length) => FromStream(s, length);

        internal static IEnumerable<string> AllTypes => _knownTypes.OrderBy(p => p.Key).Select(p => $"#{p.Key} - {p.Value.typeName} {(p.Key == DefaultType ? "[default] (if prefix is ommited)" : string.Empty)}");

        internal static HashAlgorithm DefaultAlgorithm => _defaultAlgorithm;

        internal static void RegisterResolver(byte type, string typeName, Func<Parts, InterlockId> resolver)
            => _knownTypes[type] = (typeName, resolver.Required());

        internal InterlockId Resolve()
            => _knownTypes.TryGetValue(Type, out var value)
                ? value.resolver(this)
                : new InterlockId(this);

        internal string ToFullString() => $"{_typePrefix}{_dataInfix}{_algorithmSuffix}";

        internal string ToShortString() => $"{_conditionalTypePrefix}{_dataInfix}{_conditionalAlgorithmSuffix}";

        internal void ToStream(Stream s) => s.WriteSingleByte(Type).BigEndianWriteUShort((ushort)Algorithm).WriteBytes(Data.OrEmpty());

        private const HashAlgorithm _defaultAlgorithm = HashAlgorithm.SHA256;
        private const char _prefixSeparator = '!';
        private const char _suffixSeparator = '#';

        private static readonly Dictionary<byte, (string typeName, Func<Parts, InterlockId> resolver)> _knownTypes =
            new();

        private string _algorithmSuffix => $"{_suffixSeparator}{Algorithm}";
        private string _conditionalAlgorithmSuffix => Algorithm == _defaultAlgorithm ? string.Empty : _algorithmSuffix;
        private string _conditionalTypePrefix => Type == DefaultType ? string.Empty : _typePrefix;
        private string _dataInfix => Data?.ToSafeBase64() ?? string.Empty;
        private string _typePrefix => BuildTypePrefix(Type);

        private static string BuildTypePrefix(byte type) => $"{ToTypeName(type)}{_prefixSeparator}";

        private static byte ToType(string prefix) => _knownTypes.NonEmpty().First(t => Matches(t.Value.typeName, prefix)).Key;
        private static bool Matches(string typeName, string prefix) => typeName.Equals(prefix.Trim(), StringComparison.OrdinalIgnoreCase);
        private static string ToTypeName(byte type) => _knownTypes.TryGetValue(type, out var value) ? value.typeName : "?";

        private void FromStream(Stream s, int length) {
            Type = s.ReadSingleByte();
            Algorithm = (HashAlgorithm)s.BigEndianReadUShort();
            Data = s.ReadBytes(length - sizeof(ushort) - 1);
        }

    }
}
