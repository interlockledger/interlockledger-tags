// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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
public class InterlockSigningKeyData : ILTagOfExplicit<InterlockSigningKeyData.Parts>, IInterlockKeySecretData, IBaseKey
{
    public class Parts : InterlockKey.Parts
    {
        public const ushort InterlockSigningKeyVersion = 0x0006;
        public byte[] Encrypted;
        public EncryptedContentType EncryptedContentType;

        public Parts() => Encrypted = [];

        public Parts(KeyPurpose[] purposes, IEnumerable<AppPermissions> permissions, string name, TagPubKey pubKey, byte[] encrypted, string? description, EncryptedContentType encryptedContentType, BaseKeyId? keyId)
            : base(keyId, name, description, pubKey, purposes, permissions) {
            Version = InterlockSigningKeyVersion;
            Encrypted = encrypted;
            EncryptedContentType = encryptedContentType;
        }

        public static Parts FromStream(Stream s) {
            var version = s.DecodeUShort();
            var result = new Parts {
                Version = version,                                // Field index 0 //
                Name = s.DecodeString().Required(),               // Field index 1 //
                PurposesAsUlongs = s.DecodeILIntArray(),          // Field index 2 //
                Id = s.Decode<KeyId>().Required(),                // Field index 3 //
                Identity = s.DecodeBaseKeyId().Required(),        // Field index 4 //
                Description = s.DecodeString(),                   // Field index 5 //
                PublicKey = s.Decode<TagPubKey>().Required(),     // Field index 6 //
                Encrypted = s.DecodeByteArray().Required(),       // Field index 7 //
                FirstAppId = version > 0 ? s.DecodeILInt() : 0,   // Field index 8 //
                Strength = version > 0 ? (KeyStrength)s.DecodeILInt() : KeyStrength.Normal, // Field index 9 //
                FirstActions = version > 1 ? s.DecodeILIntArray() : Enumerable.Empty<ulong>(), // Field index 10 - since version 3 //
                EncryptedContentType = version > 4 ? (EncryptedContentType)s.DecodeILInt() : EncryptedContentType.EncryptedKey, // Field index 11 - since version 5
            };
            if (version > 5) // Field index 12 - since version 6 //
                result.Permissions = s.DecodeTagArray<AppPermissions.Tag>().Safe().Select(t => t!.Value)!;
            return result;
        }

        public Stream ToStream(Stream s) {
            s.EncodeUShort(Version);              // Field index 0 //
            s.EncodeString(Name);                 // Field index 1 //
            s.EncodeILIntArray(PurposesAsUlongs); // Field index 2 //
            s.EncodeInterlockId(Id);              // Field index 3 //
            s.EncodeInterlockId(Identity);        // Field index 4 //
            s.EncodeString(Description);          // Field index 5 //
            s.EncodeTag(PublicKey);               // Field index 6 //
            s.EncodeByteArray(Encrypted);         // Field index 7 //
            s.EncodeILInt(FirstAppId);            // Field index 8 //
            s.EncodeILInt((ulong)Strength);       // Field index 9 //
            s.EncodeILIntArray(FirstActions ?? []);  // Field index 10 - since version 2 //
            s.EncodeILInt((ulong)EncryptedContentType); // Field index 11 - since version 5
            s.EncodeTagArray(Permissions.Select(p => p.AsTag)); // Field index 12 - since version 6 //
            return s;
        }

    }

    public InterlockSigningKeyData(KeyPurpose[] purposes, IEnumerable<AppPermissions> permissions, string name, byte[] encrypted, TagPubKey pubKey, string? description = null, BaseKeyId? keyId = null, EncryptedContentType encryptedContentType = EncryptedContentType.EncryptedKey)
        : this(new Parts(purposes, permissions, name, pubKey, encrypted, description, encryptedContentType, keyId)) { }

    private InterlockSigningKeyData(Parts parts) : base(ILTagId.InterlockSigningKey, parts) {
    }

    public InterlockKey AsInterlockKey => new(Purposes, Name, PublicKey, Id, Permissions, Strength, Description);
    public string? Description => Value?.Description;
    public byte[] Encrypted => Value.Required().Encrypted;
    public EncryptedContentType EncryptedContentType => Value.Required().EncryptedContentType;
    public BaseKeyId Id => Value.Required().Id;
    public BaseKeyId Identity => Value.Required().Identity ?? Id;
    public string Name => Value.Required().Name;
    public IEnumerable<AppPermissions> Permissions => Value.Required().Permissions;
    public TagPubKey PublicKey => Value.Required().PublicKey;
    public KeyPurpose[] Purposes => Value.Required().Purposes;
    public KeyStrength Strength => Value.Required().Strength;
    public ushort Version => Value.Required().Version;

    public static InterlockSigningKeyData DecodeFromBytes(byte[] bytes) {
        using var stream = new MemoryStream(bytes);
        return stream.Decode<InterlockSigningKeyData>().Required();
    }

    public string ToShortString() => Value.Required().ToShortString();

    public override string ToString() => Value.Required().ToString();

    internal InterlockSigningKeyData(Stream s) : base(ILTagId.InterlockSigningKey, s) {
    }
    protected override Task<Parts?> ValueFromStreamAsync(WrappedReadonlyStream s) => Task.FromResult<Parts?>(Parts.FromStream(s));
    protected override Task<Stream> ValueToStreamAsync(Stream s) => Task.FromResult(Value.Required().ToStream(s));
}
