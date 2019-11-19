/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    [TypeConverter(typeof(TagHashConverter))]
    public sealed class TagHash : ILTagExplicit<TagHashParts>, IEquatable<TagHash>
    {
        public static readonly TagHash Empty = new TagHash(HashAlgorithm.SHA256, HashSha256(Array.Empty<byte>()));

        public TagHash(HashAlgorithm algorithm, byte[] data) : base(ILTagId.Hash, new TagHashParts { Algorithm = algorithm, Data = data }) {
        }

        public TagHash(string textualRepresentation) : base(ILTagId.Hash, Split(textualRepresentation)) {
        }

        public override object AsJson => TextualRepresentation;
        public override string Formatted => TextualRepresentation;
        public string TextualRepresentation => ToString();
        public HashAlgorithm Algorithm => Value.Algorithm;
        public byte[] Data => Value.Data;

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "SafeAny takes care")]
        public static TagHash From(string textualRepresentation) => textualRepresentation.SafeAny() ? new TagHash(textualRepresentation.Trim()) : null;

        public static TagHash HashSha256Of(byte[] data) => new TagHash(HashAlgorithm.SHA256, HashSha256(data));

        public static TagHash HashSha256Of(IEnumerable<byte> data) => HashSha256Of(data.ToArray());

        public static implicit operator string(TagHash Value) => Value?.ToString();

        public static bool operator !=(TagHash a, TagHash b) => !(a == b);

        public static bool operator ==(TagHash a, TagHash b) => a?.Equals(b) ?? b is null;

        public override bool Equals(object obj) => Equals(obj as TagHash);

        public bool Equals(TagHash other) => !(other is null) && Algorithm == other.Algorithm && DataEquals(other.Data);

        public override int GetHashCode() => -1_574_110_226 + DataHashCode + Algorithm.GetHashCode();

        public override string ToString()
            => $"{Data?.ToSafeBase64() ?? ""}#{Algorithm}";

        internal TagHash(Stream s) : base(ILTagId.Hash, s) {
        }

        internal static TagHash HashFrom(X509Certificate2 certificate) {
            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));
            return new TagHash(HashAlgorithm.SHA1, certificate.GetCertHash());
        }

        protected override TagHashParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new TagHashParts {
                Algorithm = (HashAlgorithm)s.BigEndianReadUShort(),
                Data = s.ReadBytes(bytes.Length - sizeof(ushort))
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.BigEndianWriteUShort((ushort)Value.Algorithm).WriteBytes(Value.Data));

        private int DataHashCode => Data?.Aggregate(19, (sum, b) => sum + b) ?? 19;

        private static byte[] HashSha256(byte[] data) {
            using var hasher = SHA256.Create();
            hasher.Initialize();
            return hasher.ComputeHash(data);
        }

        private static bool IsNullOrEmpty(byte[] data) => data is null || data.Length == 0;

        private static TagHashParts Split(string textualRepresentation) {
            if (string.IsNullOrWhiteSpace(textualRepresentation))
                throw new ArgumentNullException(nameof(textualRepresentation));
            var parts = textualRepresentation.Split('#');
            var algorithm = parts.Length < 2 ? HashAlgorithm.SHA256 : (HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), parts[1], ignoreCase: true);
            return new TagHashParts { Algorithm = algorithm, Data = parts[0].FromSafeBase64() };
        }

        private bool DataEquals(byte[] otherData) => IsNullOrEmpty(Data) && IsNullOrEmpty(otherData) || Data.HasSameBytesAs(otherData);
    }

    public class TagHashConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor)) return true;
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object Value) {
            if (Value is string text) {
                text = text.Trim();
                return TagHash.From(text);
            }
            return base.ConvertFrom(context, culture, Value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object Value, Type destinationType) {
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));
            if (Value == null)
                throw new ArgumentNullException(nameof(Value));
            if (destinationType != typeof(string) || !(Value is TagHash))
                throw new InvalidOperationException("Can only convert TagHash to string!!!");
            return Value.ToString();
        }
    }

    public class TagHashParts
    {
        public HashAlgorithm Algorithm;
        public byte[] Data;
    }
}