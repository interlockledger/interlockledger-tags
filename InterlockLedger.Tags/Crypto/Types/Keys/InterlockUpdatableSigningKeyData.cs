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

public sealed class InterlockUpdatableSigningKeyData : ILTagOfExplicit<InterlockUpdatableSigningKeyData.UpdatableParts>, IInterlockKeySecretData, IBaseKey
{
    public InterlockUpdatableSigningKeyData(KeyPurpose[] purposes,
                                            string name,
                                            byte[] encrypted,
                                            TagPubKey pubKey,
                                            KeyStrength strength,
                                            DateTimeOffset creationTime,
                                            string? description = null,
                                            BaseKeyId? keyId = null)
        : this(new UpdatableParts(purposes, name, encrypted, pubKey, description, strength, keyId)) => LastSignatureTimeStamp = creationTime;

    public InterlockUpdatableSigningKeyData(UpdatableParts parts) : base(ILTagId.InterlockUpdatableSigningKey, parts) {
    }

    public InterlockKey AsInterlockKey => new(Purposes, Name, PublicKey, Id, Permissions, Strength, Description);
    public string? Description => Value.Required().Description;

    byte[] IInterlockKeySecretData.Encrypted => Value.Required().Encrypted;
    EncryptedContentType IInterlockKeySecretData.EncryptedContentType => EncryptedContentType.EncryptedKey;
    public BaseKeyId Id => Value.Required().Id;
    public BaseKeyId Identity => Value.Required().Identity;
    public DateTimeOffset LastSignatureTimeStamp { get => Value.Required().LastSignatureTimeStamp; internal set => Value.Required().LastSignatureTimeStamp = value; }
    public string Name => Value.Required().Name;
    public TagPubKey PublicKey => Value.Required().PublicKey;
    public KeyPurpose[] Purposes => Value.Required().Purposes;
    public ulong SignaturesWithCurrentKey { get => Value.Required().SignaturesWithCurrentKey; internal set => Value.Required().SignaturesWithCurrentKey = value; }
    public KeyStrength Strength => Value.Required().Strength;
    public ushort Version => Value.Required().Version;

    public IEnumerable<AppPermissions> Permissions => Value.Required().Permissions;

    public static InterlockUpdatableSigningKeyData DecodeFromBytes(byte[] bytes) {
        using var stream = new MemoryStream(bytes);
        return stream.Decode<InterlockUpdatableSigningKeyData>().Required();
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

        public UpdatableParts() => Encrypted = [];

        public UpdatableParts(KeyPurpose[] purposes, string name, byte[] encrypted, TagPubKey pubKey, string? description, KeyStrength strength, BaseKeyId? keyId)
            : base(keyId, name, description, pubKey, purposes, null) {
            Version = InterlockUpdatableSigningKeyVersion;
            Encrypted = encrypted.Required();
            LastSignatureTimeStamp = DateTimeOffsetExtensions.TimeZero;
            SignaturesWithCurrentKey = 0;
        }
    }

    internal InterlockUpdatableSigningKeyData(Stream s) : base(ILTagId.InterlockUpdatableSigningKey, s) { }

    protected override ulong CalcValueLength() => (ulong)(ToBytes()?.Length ?? 0);

    protected override Task<UpdatableParts?> ValueFromStreamAsync(WrappedReadonlyStream s) {
        var version = s.DecodeUShort();
        var result = new UpdatableParts {
            Version = version,                                      // Field index 0 //
            Name = s.DecodeString().Safe(),                         // Field index 1 //
            PurposesAsUlongs = s.DecodeILIntArray(),                // Field index 2 //
            Id = s.Decode<KeyId>().Required(),                      // Field index 3 //
            Identity = s.Decode<BaseKeyId>().Required(),            // Field index 4 //
            Description = s.DecodeString().Safe(),                  // Field index 5 //
            PublicKey = s.Decode<TagPubKey>().Required(),           // Field index 6 //
            Encrypted = s.DecodeByteArray().Required(),             // Field index 7 //
            LastSignatureTimeStamp = s.DecodeOldDateTimeOffset(),   // Field index 8 //
            SignaturesWithCurrentKey = s.DecodeILInt(),             // Field index 9 //
            Strength = version > 0 ? (KeyStrength)s.DecodeILInt() : KeyStrength.Normal, // Field index 10 //
        };
        return Task.FromResult<UpdatableParts?>(result);
    }

    protected override Task<Stream> ValueToStreamAsync(Stream s) {
        s.EncodeUShort(Value.Required().Version);               // Field index 0 //
        s.EncodeString(Value.Name);                             // Field index 1 //
        s.EncodeILIntArray(Value.PurposesAsUlongs);             // Field index 2 //
        s.EncodeInterlockId(Value.Id);                          // Field index 3 //
        s.EncodeInterlockId(Value.Identity);                    // Field index 4 //
        s.EncodeString(Value.Description);                      // Field index 5 //
        s.EncodeTag(Value.PublicKey);                           // Field index 6 //
        s.EncodeByteArray(Value.Encrypted);                     // Field index 7 //
        s.EncodeDateTimeOffset(Value.LastSignatureTimeStamp);   // Field index 8 //
        s.EncodeILInt(Value.SignaturesWithCurrentKey);          // Field index 9 //
        s.EncodeILInt((ulong)Value.Strength);                   // Field index 10 //
        return Task.FromResult(s);
    }

    private byte[] ToBytes() => TagHelpers.ToBytesHelper(s => ValueToStreamAsync(s).WaitResult());
}