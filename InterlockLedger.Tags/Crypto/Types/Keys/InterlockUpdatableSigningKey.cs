/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public abstract class InterlockUpdatableSigningKey : IUpdatableSigningKey
    {
        public TagPubKey CurrentPublicKey => _value.PublicKey;
        public abstract TagPubKey NextPublicKey { get; }
        public Algorithm SignAlgorithm => _value.PublicKey.Algorithm;
        public IEnumerable<ulong> SpecificActions => Enumerable.Empty<ulong>();
        public ulong AppId => _value.AppId;
        public string Description => _value.Description;
        public byte[] EncodedBytes => _value.EncodedBytes;
        public BaseKeyId Id => _value.Id;
        public BaseKeyId Identity => _value.Identity;
        public DateTimeOffset LastSignatureTimeStamp => _value.LastSignatureTimeStamp;
        public string Name => _value.Name;
        public KeyPurpose[] Purposes => _value.Purposes;
        public ulong SignaturesWithCurrentKey => _value.SignaturesWithCurrentKey;
        public KeyStrength Strength => _value.Strength;

        public byte[] Decrypt(byte[] bytes) => throw new InvalidOperationException("Updatable keys can't decrypt");

        public abstract void DestroyKeys();

        public abstract void GenerateNextKeys();

        public TagSignature Sign(byte[] data) => throw new InvalidOperationException("Can't sign without possibly updating the key");

        public abstract TagSignature SignAndUpdate(byte[] data, Func<byte[], byte[]> encrypt = null);

        public string ToShortString() => $"UpdatableSigningKey '{Name}' [{Purposes.ToStringAsList()}]";

        protected readonly ITimeStamper _timeStamper;
        protected readonly InterlockUpdatableSigningKeyData _value;

        protected InterlockUpdatableSigningKey(InterlockUpdatableSigningKeyData tag, ITimeStamper timeStamper) {
            _value = tag ?? throw new ArgumentNullException(nameof(tag));
            _timeStamper = timeStamper ?? throw new ArgumentNullException(nameof(timeStamper));
            _value.LastSignatureTimeStamp = _timeStamper.Now;
        }
    }

    public sealed class InterlockUpdatableSigningKeyData : ILTagExplicit<InterlockUpdatableSigningKeyParts>, IInterlockKeySecretData
    {
        public InterlockUpdatableSigningKeyData(KeyPurpose[] purposes, string name, byte[] encrypted, TagPubKey pubKey, KeyStrength strength, DateTimeOffset creationTime, string description = null, BaseKeyId keyId = null)
            : this(new InterlockUpdatableSigningKeyParts(purposes, name, encrypted, pubKey, description, strength, keyId)) => LastSignatureTimeStamp = creationTime;

        public InterlockUpdatableSigningKeyData(InterlockUpdatableSigningKeyParts parts) : base(ILTagId.InterlockUpdatableSigningKey, parts) {
        }

        public InterlockKey AsInterlockKey => new InterlockKey(Purposes, Name, PublicKey, AppId, Value.SpecificActions, Id, Strength, Description);
        public DateTimeOffset LastSignatureTimeStamp { get => Value.LastSignatureTimeStamp; internal set => Value.LastSignatureTimeStamp = value; }
        public ulong SignaturesWithCurrentKey { get => Value.SignaturesWithCurrentKey; internal set => Value.SignaturesWithCurrentKey = value; }
        public ulong AppId => Value.AppId;
        public string Description => Value.Description;
        byte[] IInterlockKeySecretData.Encrypted => Value.Encrypted;
        EncryptedContentType IInterlockKeySecretData.EncryptedContentType => EncryptedContentType.EncryptedKey;
        public BaseKeyId Id => Value.Id;
        public BaseKeyId Identity => Value.Identity;
        public string Name => Value.Name;
        public TagPubKey PublicKey => Value.PublicKey;
        public KeyPurpose[] Purposes => Value.Purposes;
        public KeyStrength Strength => Value.Strength;

        public ushort Version => Value.Version;

        public static InterlockUpdatableSigningKeyData DecodeFromBytes(byte[] bytes) {
            using var stream = new MemoryStream(bytes);
            return stream.Decode<InterlockUpdatableSigningKeyData>();
        }

        public string ToShortString() => $"InterlockUpdatableSigningKey '{Name}' by {Identity}";

        public override string ToString() => $@"InterlockUpdatableSigningKey
-- Version: {Version}
{Value}";

        internal InterlockUpdatableSigningKeyData(Stream s) : base(ILTagId.InterlockUpdatableSigningKey, s) {
        }

        protected override InterlockUpdatableSigningKeyParts FromBytes(byte[] Value) =>
            FromBytesHelper(Value, s => {
                var version = s.DecodeUShort();
                return new InterlockUpdatableSigningKeyParts {
                    Version = version,                                // Field index 0 //
                    Name = s.DecodeString(),                          // Field index 1 //
                    PurposesAsUlongs = s.DecodeILIntArray(),          // Field index 2 //
                    Id = s.Decode<KeyId>(),                           // Field index 3 //
                    Identity = s.Decode<BaseKeyId>(),                 // Field index 4 //
                    Description = s.DecodeString(),                   // Field index 5 //
                    PublicKey = s.Decode<TagPubKey>(),                // Field index 6 //
                    Encrypted = s.DecodeByteArray(),                  // Field index 7 //
                    LastSignatureTimeStamp = s.DecodeDateTimeOffset(),// Field index 8 //
                    SignaturesWithCurrentKey = s.DecodeILInt(),       // Field index 9 //
                    Strength = version > 0 ? (KeyStrength)s.DecodeILInt() : KeyStrength.Normal, // Field index 10 //
                };
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => {
                s.EncodeUShort(Value.Version);              // Field index 0 //
                s.EncodeString(Value.Name);                 // Field index 1 //
                s.EncodeILIntArray(Value.PurposesAsUlongs); // Field index 2 //
                s.EncodeInterlockId(Value.Id);              // Field index 3 //
                s.EncodeInterlockId(Value.Identity);        // Field index 4 //
                s.EncodeString(Value.Description);          // Field index 5 //
                s.EncodeTag(Value.PublicKey);               // Field index 6 //
                s.EncodeByteArray(Value.Encrypted);         // Field index 7 //
                s.EncodeDateTimeOffset(Value.LastSignatureTimeStamp); // Field index 8 //
                s.EncodeILInt(Value.SignaturesWithCurrentKey); // Field index 9 //
                s.EncodeILInt((ulong)Value.Strength);       // Field index 10 //
            });
    }

    public class InterlockUpdatableSigningKeyParts : InterlockKeyParts
    {
        public const ushort InterlockUpdatableSigningKeyVersion = 0x0001;
        public byte[] Encrypted;
        public DateTimeOffset LastSignatureTimeStamp;
        public ulong SignaturesWithCurrentKey;

        public InterlockUpdatableSigningKeyParts() {
        }

        public InterlockUpdatableSigningKeyParts(KeyPurpose[] purposes, string name, byte[] encrypted, TagPubKey pubKey, string description, KeyStrength strength, BaseKeyId keyId)
            : base(purposes, 0, null, name, description, pubKey, strength, keyId) {
            Version = InterlockUpdatableSigningKeyVersion;
            Encrypted = encrypted;
            LastSignatureTimeStamp = DateTimeOffsetExtensions.TimeZero;
            SignaturesWithCurrentKey = 0;
        }
    }
}