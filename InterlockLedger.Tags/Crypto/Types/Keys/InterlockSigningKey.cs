/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public enum EncryptedContentType
    {
        EncryptedKey = 0,
        EmbeddedCertificate = 1
    }

    public abstract class InterlockSigningKey : ISigningKey
    {
        public abstract byte[] AsSessionState { get; }
        public TagPubKey CurrentPublicKey => _value.PublicKey;
        public Algorithm SignAlgorithm => _value.PublicKey.Algorithm;
        public ulong AppId => _value.AppId;
        public string Description => _value.Description;
        protected EncryptedContentType EncryptedContentType => _value.EncryptedContentType;
        public BaseKeyId Id => _value.Id;
        public string Name => _value.Name;
        public KeyPurpose[] Purposes => _value.Purposes;
        public IEnumerable<ulong> SpecificActions => _value.SpecificActions;
        public KeyStrength Strength => _value.Strength;

        public static InterlockSigningKey FromSessionState(byte[] bytes) => RISKFrom(bytes) ?? RCSKFrom(bytes);

        public abstract byte[] Decrypt(byte[] bytes);

        public abstract TagSignature Sign(byte[] data);

        public string ToShortString() => $"SigningKey {Name} [{Purposes.ToStringAsList()}]";

        protected readonly InterlockSigningKeyData _value;

        protected InterlockSigningKey(InterlockSigningKeyData tag) => _value = tag;

        private static InterlockSigningKey RCSKFrom(byte[] bytes) {
            try {
                return RSACertificateSigningKey.FromSessionState(bytes);
            } catch { return null; }
        }

        private static InterlockSigningKey RISKFrom(byte[] bytes) {
            try {
                return RSAInterlockSigningKey.FromSessionState(bytes);
            } catch { return null; }
        }
    }

    public class InterlockSigningKeyData : ILTagExplicit<InterlockSigningKeyParts>, IInterlockKeySecretData
    {
        public InterlockSigningKeyData(KeyPurpose[] purposes, ulong appId, IEnumerable<ulong> actionIds, string name, byte[] encrypted, TagPubKey pubKey, KeyStrength strength, string description = null, BaseKeyId keyId = null, EncryptedContentType encryptedContentType = EncryptedContentType.EncryptedKey)
            : this(new InterlockSigningKeyParts(purposes, appId, actionIds, name, encrypted, pubKey, description, strength, encryptedContentType, keyId)) { }

        public InterlockSigningKeyData(InterlockSigningKeyParts parts) : base(ILTagId.InterlockSigningKey, parts) {
        }

        public InterlockKey AsInterlockKey => new InterlockKey(Purposes, Name, PublicKey, AppId, SpecificActions, Id, Strength, Description);
        public ulong AppId => Value.AppId;
        public string Description => Value.Description;
        public byte[] Encrypted => Value.Encrypted;
        public EncryptedContentType EncryptedContentType => Value.EncryptedContentType;
        public BaseKeyId Id => Value.Id;
        public BaseKeyId Identity => Value.Identity ?? Id;
        public string Name => Value.Name;
        public TagPubKey PublicKey => Value.PublicKey;
        public KeyPurpose[] Purposes => Value.Purposes;
        public IEnumerable<ulong> SpecificActions => Value.SpecificActions;
        public KeyStrength Strength => Value.Strength;
        public ushort Version => Value.Version;

        public static InterlockSigningKeyData DecodeFromBytes(byte[] bytes) {
            using var stream = new MemoryStream(bytes);
            return stream.Decode<InterlockSigningKeyData>();
        }

        public string ToShortString() => Value.ToShortString();

        public override string ToString() => $@"## InterlockSigningKey ##
-- Version: {Version}
{Value}
";

        internal InterlockSigningKeyData(Stream s) : base(ILTagId.InterlockSigningKey, s) {
        }

        protected override InterlockSigningKeyParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => {
                var version = s.DecodeUShort();
                return new InterlockSigningKeyParts {
                    Version = version,                                // Field index 0 //
                    Name = s.DecodeString(),                          // Field index 1 //
                    PurposesAsUlongs = s.DecodeILIntArray(),          // Field index 2 //
                    Id = s.Decode<KeyId>(),                           // Field index 3 //
                    Identity = s.DecodeBaseKeyId(),                   // Field index 4 //
                    Description = s.DecodeString(),                   // Field index 5 //
                    PublicKey = s.Decode<TagPubKey>(),                // Field index 6 //
                    Encrypted = s.DecodeByteArray(),                  // Field index 7 //
                    AppId = version > 0 ? s.DecodeILInt() : 0,        // Field index 8 //
                    Strength = version > 0 ? (KeyStrength)s.DecodeILInt() : KeyStrength.Normal, // Field index 9 //
                    SpecificActions = version > 1 ? s.DecodeILIntArray() : Enumerable.Empty<ulong>(), // Field index 10 - since version 2 //
                    EncryptedContentType = version > 4 ? (EncryptedContentType)s.DecodeILInt() : EncryptedContentType.EncryptedKey, // Field index 11 - since version 5
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
                s.EncodeILInt(Value.AppId);                 // Field index 8 //
                s.EncodeILInt((ulong)Value.Strength);       // Field index 9 //
                s.EncodeILIntArray(Value.SpecificActions ?? Enumerable.Empty<ulong>());  // Field index 10 - since version 2 //
                s.EncodeILInt((ulong)Value.EncryptedContentType); // Field index 11 - since version 5
            });
    }

    public class InterlockSigningKeyParts : InterlockKeyParts
    {
        public const ushort InterlockSigningKeyVersion = 0x0005;
        public byte[] Encrypted;
        public EncryptedContentType EncryptedContentType;

        public InterlockSigningKeyParts() {
        }

        public InterlockSigningKeyParts(KeyPurpose[] purposes, ulong appId, IEnumerable<ulong> actionIds, string name, byte[] encrypted, TagPubKey pubKey, string description, KeyStrength strength, EncryptedContentType encryptedContentType, BaseKeyId keyId)
            : base(purposes, appId, actionIds, name, description, pubKey, strength, keyId) {
            Version = InterlockSigningKeyVersion;
            Encrypted = encrypted;
            EncryptedContentType = encryptedContentType;
        }
    }
}