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

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;
public class ECDsaCertificateSigningKey : InterlockSigningKey
{
    public ECDsaCertificateSigningKey(InterlockSigningKeyData data, byte[] certificateBytes, string password) : base(data) {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (data.EncryptedContentType != EncryptedContentType.EmbeddedCertificate)
            throw new ArgumentException($"Wrong kind of EncryptedContentType {data.EncryptedContentType}", nameof(data));
        _password = password.Required();
        _certificateBytes = certificateBytes.Required();
    }

    public override byte[] AsSessionState {
        get {
            using var ms = new MemoryStream();
            ms.EncodeTag(_value);
            ms.EncodeString(_password);
            ms.EncodeByteArray(_certificateBytes);
            return ms.ToArray();
        }
    }

    public static new ECDsaCertificateSigningKey FromSessionState(byte[] bytes) {
        using var ms = new MemoryStream(bytes);
        var tag = ms.Decode<InterlockSigningKeyData>();
        var password = ms.DecodeString();
        var certificateBytes = ms.DecodeByteArray();
        return new ECDsaCertificateSigningKey(tag, certificateBytes, password);
    }

    public override byte[] Decrypt(byte[] bytes) {
        using var x509Certificate = _certificateBytes.OpenCertificate(_password);
        using var ecdsa = x509Certificate.GetECDsaPrivateKey();
        throw new NotSupportedException();
    }

    public override TagSignature Sign(byte[] data) => new(Algorithm.EcDSA, HashAndSign(data));

    public override TagSignature Sign<T>(T data) => new(Algorithm.EcDSA, HashAndSignBytes(data));

    private readonly byte[] _certificateBytes;
    private readonly string _password;

    private byte[] HashAndSign(byte[] dataToSign) {
        using var x509Certificate = _certificateBytes.OpenCertificate(_password);
        using var ecdsa = x509Certificate.GetECDsaPrivateKey();
        return ecdsa.SignData(dataToSign, HashAlgorithmName.SHA256);
    }

    private byte[] HashAndSignBytes<T>(T data) where T : Signable<T>, new() {
        using var x509Certificate = _certificateBytes.OpenCertificate(_password);
        using var ecdsa = x509Certificate.GetECDsaPrivateKey();
        return ecdsa.SignData(data.OpenReadingStreamAsync().Result, HashAlgorithmName.SHA256);
    }
}