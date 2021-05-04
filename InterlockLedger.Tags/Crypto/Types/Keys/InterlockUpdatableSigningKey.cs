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

using System;
using System.Collections.Generic;
using System.IO;

namespace InterlockLedger.Tags
{
    public abstract class InterlockUpdatableSigningKey : IUpdatableSigningKey
    {
        public string Description => _value.Description;
        public byte[] EncodedBytes => _value.EncodedBytes;
        public BaseKeyId Id => _value.Id;
        public BaseKeyId Identity => _value.Identity;
        public DateTimeOffset LastSignatureTimeStamp => _value.LastSignatureTimeStamp;
        public string Name => _value.Name;
        public abstract TagPubKey NextPublicKey { get; }
        public IEnumerable<AppPermissions> Permissions { get; } = InterlockKey.Parts.NoPermissions;
        public TagPubKey PublicKey => _value.PublicKey;
        public KeyPurpose[] Purposes => _value.Purposes;
        public Algorithm SignAlgorithm => _value.PublicKey.Algorithm;
        public ulong SignaturesWithCurrentKey => _value.SignaturesWithCurrentKey;
        public KeyStrength Strength => _value.Strength;

        public byte[] Decrypt(byte[] bytes) => throw new InvalidOperationException("Updatable keys can't decrypt");

        public abstract void DestroyKeys();

        public abstract void GenerateNextKeys();

        public TagSignature Sign(byte[] data) => throw new InvalidOperationException("Can't sign without possibly updating the key");

        public TagSignature Sign<T>(T data) where T : Signable<T>, new() => throw new InvalidOperationException("Can't sign without possibly updating the key");

        public abstract TagSignature SignAndUpdate<T>(T data, Func<byte[], byte[]> encrypt = null) where T : Signable<T>, new();

        public abstract TagSignature SignAndUpdate(byte[] data, Func<byte[], byte[]> encrypt = null);

        public IdentifiedSignature SignWithId(byte[] data) => throw new InvalidOperationException("Can't sign without possibly updating the key");

        public string ToShortString() => $"UpdatableSigningKey '{Name}' [{Purposes.ToStringAsList()}]";

        protected readonly ITimeStamper _timeStamper;
        protected readonly InterlockUpdatableSigningKeyData _value;

        protected InterlockUpdatableSigningKey(InterlockUpdatableSigningKeyData tag, ITimeStamper timeStamper) {
            _value = tag.Required(nameof(tag));
            _timeStamper = timeStamper.Required(nameof(timeStamper));
            _value.LastSignatureTimeStamp = _timeStamper.Now;
        }
    }

    public sealed class InterlockUpdatableSigningKeyData : ILTagExplicit<InterlockUpdatableSigningKeyData.UpdatableParts>, IInterlockKeySecretData
    {
        public InterlockUpdatableSigningKeyData(KeyPurpose[] purposes, string name, byte[] encrypted, TagPubKey pubKey, KeyStrength strength, DateTimeOffset creationTime, string description = null, BaseKeyId keyId = null)
            : this(new UpdatableParts(purposes, name, encrypted, pubKey, description, strength, keyId)) => LastSignatureTimeStamp = creationTime;

        public InterlockUpdatableSigningKeyData(UpdatableParts parts) : base(ILTagId.InterlockUpdatableSigningKey, parts) {
        }

        public InterlockKey AsInterlockKey => new(Purposes, Name, PublicKey, Id, Value.Permissions, Strength, Description);
        public string Description => Value.Description;
        byte[] IInterlockKeySecretData.Encrypted => Value.Encrypted;
        EncryptedContentType IInterlockKeySecretData.EncryptedContentType => EncryptedContentType.EncryptedKey;
        public BaseKeyId Id => Value.Id;
        public BaseKeyId Identity => Value.Identity;
        public DateTimeOffset LastSignatureTimeStamp { get => Value.LastSignatureTimeStamp; internal set => Value.LastSignatureTimeStamp = value; }
        public string Name => Value.Name;
        public TagPubKey PublicKey => Value.PublicKey;
        public KeyPurpose[] Purposes => Value.Purposes;
        public ulong SignaturesWithCurrentKey { get => Value.SignaturesWithCurrentKey; internal set => Value.SignaturesWithCurrentKey = value; }
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

        public class UpdatableParts : InterlockKey.Parts
        {
            public const ushort InterlockUpdatableSigningKeyVersion = 0x0001;
            public byte[] Encrypted;
            public DateTimeOffset LastSignatureTimeStamp;
            public ulong SignaturesWithCurrentKey;

            public UpdatableParts() {
            }

            public UpdatableParts(KeyPurpose[] purposes, string name, byte[] encrypted, TagPubKey pubKey, string description, KeyStrength strength, BaseKeyId keyId)
                : base(purposes, name, description, pubKey, strength, keyId, null) {
                Version = InterlockUpdatableSigningKeyVersion;
                Encrypted = encrypted;
                LastSignatureTimeStamp = DateTimeOffsetExtensions.TimeZero;
                SignaturesWithCurrentKey = 0;
            }
        }

        internal InterlockUpdatableSigningKeyData(Stream s) : base(ILTagId.InterlockUpdatableSigningKey, s) {
        }

        protected override UpdatableParts FromBytes(byte[] Value) =>
            FromBytesHelper(Value, s => {
                var version = s.DecodeUShort();
                return new UpdatableParts {
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

        protected override byte[] ToBytes(UpdatableParts value)
            => TagHelpers.ToBytesHelper(s => {
                s.EncodeUShort(value.Version);              // Field index 0 //
                s.EncodeString(value.Name);                 // Field index 1 //
                s.EncodeILIntArray(value.PurposesAsUlongs); // Field index 2 //
                s.EncodeInterlockId(value.Id);              // Field index 3 //
                s.EncodeInterlockId(value.Identity);        // Field index 4 //
                s.EncodeString(value.Description);          // Field index 5 //
                s.EncodeTag(value.PublicKey);               // Field index 6 //
                s.EncodeByteArray(value.Encrypted);         // Field index 7 //
                s.EncodeDateTimeOffset(value.LastSignatureTimeStamp); // Field index 8 //
                s.EncodeILInt(value.SignaturesWithCurrentKey); // Field index 9 //
                s.EncodeILInt((ulong)value.Strength);       // Field index 10 //
            });
    }
}