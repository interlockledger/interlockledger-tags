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

using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;

public abstract class BaseCertificateSigningKey : InterlockSigningKey
{
    public BaseCertificateSigningKey(InterlockSigningKeyData data, byte[] certificateBytes, string password) :
        this(data, BuildCert(certificateBytes, password)) {
    }
    public BaseCertificateSigningKey(InterlockSigningKeyData data, X509Certificate2 x509Certificate) :
        base(data) {
        _x509Certificate = x509Certificate;
        if (data.Required().EncryptedContentType != EncryptedContentType.EmbeddedCertificate)
            throw new ArgumentException($"Wrong kind of EncryptedContentType {data.EncryptedContentType}", nameof(data));
        if (!_x509Certificate.HasPrivateKey)
            throw new InvalidOperationException("The private key is missing");
        if (!_hasCorrectPrivateKey)
            throw new InvalidOperationException($"The private key is of the incorrect type - certificate key type is {_x509Certificate.GetKeyAlgorithm}");
    }
    protected readonly X509Certificate2 _x509Certificate;
    private bool _hasCorrectPrivateKey =>
        KeyData.PublicKey.Algorithm switch {
            Algorithm.RSA => _x509Certificate.GetRSAPrivateKey() is not null,
            Algorithm.EcDSA => _x509Certificate.GetECDsaPrivateKey() is not null,
            _ => throw new NotSupportedException($"Unsupported certificate key algorithm {KeyData.PublicKey.Algorithm}")
        };
    protected sealed override void DisposeManagedResources() =>
        _x509Certificate?.Dispose();
    private static X509Certificate2 BuildCert(byte[] certificateBytes, string password) =>
        new(certificateBytes.Required(),
            password.Required(),
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
}