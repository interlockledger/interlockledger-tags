// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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

public abstract class InterlockSigningKey(InterlockSigningKeyData data) : ISigningKey
{
    public abstract byte[] AsSessionState { get; }
    public string? Description => _data.Description;
    public BaseKeyId Id => _data.Id;
    public string Name => _data.Name;
    public IEnumerable<AppPermissions> Permissions => _data.Permissions;
    public TagPubKey PublicKey => _data.PublicKey;
    public KeyPurpose[] Purposes => _data.Purposes;
    public KeyStrength Strength => _data.Strength;

    public static InterlockSigningKey? FromSessionState(byte[] bytes) => RISKFrom(bytes) ?? RCSKFrom(bytes);
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public abstract TagSignature Sign(byte[] data);

    public abstract TagSignature Sign<T>(T data) where T : Signable<T>, new();

    public string ToShortString() => $"SigningKey {Name} [{Purposes.ToStringAsList()}]";
    public override string ToString() => _data.ToString();

    protected readonly InterlockSigningKeyData _data = data.Required();

    protected EncryptedContentType EncryptedContentType => _data.EncryptedContentType;

    protected virtual void DisposeManagedResources() { }

    protected virtual void DisposeUnmanagedResources() { }

    private bool _disposedValue;

    ~InterlockSigningKey() { Dispose(disposing: false); }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private static InterlockSigningKey? RCSKFrom(byte[] bytes) {
        try {
            return RSACertificateSigningKey.FromSessionState(bytes);
        } catch { return null; }
    }

    private static InterlockSigningKey? RISKFrom(byte[] bytes) {
        try {
            return RSAInterlockSigningKey.FromSessionState(bytes);
        } catch { return null; }
    }
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

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

