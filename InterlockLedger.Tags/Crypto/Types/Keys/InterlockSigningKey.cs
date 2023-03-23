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

namespace InterlockLedger.Tags;

public enum EncryptedContentType
{
    EncryptedKey = 0,
    EmbeddedCertificate = 1
}

public abstract class InterlockSigningKey : ISigningKey
{
    public abstract byte[] AsSessionState { get; }
    public string Description => _data.Description;
    public BaseKeyId Id => _data.Id;
    public string Name => _data.Name;
    public IEnumerable<AppPermissions> Permissions => _data.Permissions;
    public TagPubKey PublicKey => _data.PublicKey;
    public KeyPurpose[] Purposes => _data.Purposes;
    public Algorithm SignAlgorithm => _data.PublicKey.Algorithm;
    public KeyStrength Strength => _data.Strength;

    public static InterlockSigningKey FromSessionState(byte[] bytes) => RISKFrom(bytes) ?? RCSKFrom(bytes);

    public abstract byte[] Decrypt(byte[] bytes);

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public abstract TagSignature Sign(byte[] data);

    public abstract TagSignature Sign<T>(T data) where T : Signable<T>, new();

    public string ToShortString() => $"SigningKey {Name} [{Purposes.ToStringAsList()}]";

    protected readonly InterlockSigningKeyData _data;

    protected InterlockSigningKey(InterlockSigningKeyData data) => _data = data.Required();

    protected EncryptedContentType EncryptedContentType => _data.EncryptedContentType;

    protected virtual void DisposeManagedResources() { }

    protected virtual void DisposeUnmanagedResources() { }

    private bool _disposedValue;

    ~InterlockSigningKey() { Dispose(disposing: false); }

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

    private void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                DisposeManagedResources();
            }

            DisposeUnmanagedResources();
            _disposedValue = true;
        }
    }
}

public class InterlockSigningKeyData : ILTagExplicit<InterlockSigningKeyParts>, IInterlockKeySecretData, IBaseKey
{
    private bool _disposedValue;

    public InterlockSigningKeyData(KeyPurpose[] purposes, IEnumerable<AppPermissions> permissions, string name, byte[] encrypted, TagPubKey pubKey, KeyStrength strength, string? description = null, BaseKeyId keyId = null, EncryptedContentType encryptedContentType = EncryptedContentType.EncryptedKey)
        : this(new InterlockSigningKeyParts(purposes, permissions, name, encrypted, pubKey, description, strength, encryptedContentType, keyId)) { }

    public InterlockSigningKeyData(InterlockSigningKeyParts parts) : base(ILTagId.InterlockSigningKey, parts) {
    }

    public InterlockKey AsInterlockKey => new(Purposes, Name, PublicKey, Id, Permissions, Strength, Description);
    public string? Description => Value.Description;
    public byte[] Encrypted => Value.Encrypted;
    public EncryptedContentType EncryptedContentType => Value.EncryptedContentType;
    public BaseKeyId Id => Value.Id;
    public BaseKeyId Identity => Value.Identity ?? Id;
    public string Name => Value.Name;
    public IEnumerable<AppPermissions> Permissions => Value.Permissions;
    public TagPubKey PublicKey => Value.PublicKey;
    public KeyPurpose[] Purposes => Value.Purposes;
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
            var result = new InterlockSigningKeyParts {
                Version = version,                                // Field index 0 //
                Name = s.DecodeString(),                          // Field index 1 //
                PurposesAsUlongs = s.DecodeILIntArray(),          // Field index 2 //
                Id = s.Decode<KeyId>(),                           // Field index 3 //
                Identity = s.DecodeBaseKeyId(),                   // Field index 4 //
                Description = s.DecodeString(),                   // Field index 5 //
                PublicKey = s.Decode<TagPubKey>(),                // Field index 6 //
                Encrypted = s.DecodeByteArray(),                  // Field index 7 //
                FirstAppId = version > 0 ? s.DecodeILInt() : 0,        // Field index 8 //
                Strength = version > 0 ? (KeyStrength)s.DecodeILInt() : KeyStrength.Normal, // Field index 9 //
                FirstActions = version > 1 ? s.DecodeILIntArray() : Enumerable.Empty<ulong>(), // Field index 9 - since version 3 //
                EncryptedContentType = version > 4 ? (EncryptedContentType)s.DecodeILInt() : EncryptedContentType.EncryptedKey, // Field index 11 - since version 5
            };
            if (version > 5)
                result.Permissions = s.DecodeTagArray<AppPermissions.Tag>().Select(t => t.Value);
            return result;
        });

    protected override byte[] ToBytes(InterlockSigningKeyParts value)
        => TagHelpers.ToBytesHelper(s => {
            s.EncodeUShort(Value.Version);              // Field index 0 //
            s.EncodeString(Value.Name);                 // Field index 1 //
            s.EncodeILIntArray(Value.PurposesAsUlongs); // Field index 2 //
            s.EncodeInterlockId(Value.Id);              // Field index 3 //
            s.EncodeInterlockId(Value.Identity);        // Field index 4 //
            s.EncodeString(Value.Description);          // Field index 5 //
            s.EncodeTag(Value.PublicKey);               // Field index 6 //
            s.EncodeByteArray(Value.Encrypted);         // Field index 7 //
            s.EncodeILInt(Value.FirstAppId);            // Field index 8 //
            s.EncodeILInt((ulong)value.Strength);       // Field index 9 //
            s.EncodeILIntArray(Value.FirstActions ?? Enumerable.Empty<ulong>());  // Field index 10 - since version 2 //
            s.EncodeILInt((ulong)value.EncryptedContentType); // Field index 11 - since version 5
            s.EncodeTagArray(Value.Permissions.Select(p => p.AsTag)); // Field index 12 - since version 6 //
        });

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~InterlockSigningKeyData()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class InterlockSigningKeyParts : InterlockKey.Parts
{
    public const ushort InterlockSigningKeyVersion = 0x0006;
    public byte[] Encrypted;
    public EncryptedContentType EncryptedContentType;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public InterlockSigningKeyParts() {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public InterlockSigningKeyParts(KeyPurpose[] purposes, IEnumerable<AppPermissions> permissions, string name, byte[] encrypted, TagPubKey pubKey, string? description, KeyStrength strength, EncryptedContentType encryptedContentType, BaseKeyId keyId)
        : base(purposes, name, description, pubKey, strength, keyId, permissions) {
        Version = InterlockSigningKeyVersion;
        Encrypted = encrypted;
        EncryptedContentType = encryptedContentType;
    }
}