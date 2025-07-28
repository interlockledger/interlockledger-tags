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

public enum EncryptedContentType
{
    EncryptedKey = 0,
    EmbeddedCertificate = 1
}

public abstract class InterlockSigningKeyOf<T>(T data) : AbstractDisposable, ISigningKey where T : class, IInterlockKeySecretData, IBaseKey
{
    public string? Description => KeyData.Description;
    public BaseKeyId Id => KeyData.Id;
    public string Name => KeyData.Name;
    public IEnumerable<AppPermissions> Permissions => KeyData.Permissions;
    public TagPubKey PublicKey => KeyData.PublicKey;
    public KeyPurpose[] Purposes => KeyData.Purposes;
    public KeyStrength Strength => KeyData.Strength;
    public EncryptedContentType EncryptedContentType => KeyData.EncryptedContentType;

    public TagSignature Sign<TD>(TD data) where TD : Signable<TD>, new() =>
        this is IUpdatableSigningKey
        ? throw new InvalidOperationException("Can't sign without possibly updating the key")
        : Sign(data.OpenReadingStreamAsync().WaitResult());
    public TagSignature Sign(Stream dataStream) =>
        this is IUpdatableSigningKey
        ? throw new InvalidOperationException("Can't sign without possibly updating the key")
        : new(PublicKey.Algorithm, HashAndSignStream(dataStream));
    public bool Verify<TD>(TD dataToVerify, TagSignature signature) where TD : Signable<TD>, ICacheableTag, new() {
        using var dataStream = dataToVerify.Required().OpenReadingStreamAsync().WaitResult();
        return VerifyStream(dataStream, signature);
    }
    public bool VerifyStream(Stream dataStream, TagSignature signature) =>
        signature.Required().Algorithm != KeyData.PublicKey.Algorithm
            ? throw new InvalidDataException($"Signature uses different algorithm {signature.Algorithm} from this {KeyData.PublicKey.Algorithm} key!")
            : VerifySignatureOnStream(dataStream, signature);
    protected abstract byte[] HashAndSignStream(Stream dataStream);
    protected abstract bool VerifySignatureOnStream(Stream dataStream, TagSignature signature);
    public virtual string ToShortString() => $"SigningKey {Name} {PublicKey.Algorithm} [{Purposes.ToStringAsList()}]";
    public override string? ToString() => KeyData.ToString();
    protected override void DisposeManagedResources() { }
    protected readonly T KeyData = data.Required();
}

