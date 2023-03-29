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

using Org.BouncyCastle.Crypto.Parameters;

namespace InterlockLedger.Tags;

public class EdDSAParameters
{
    public EdDSAParameters(byte[] bytes) {
        int pubKeySize = Ed25519PublicKeyParameters.KeySize;
        AsBytes = bytes.MinLength(pubKeySize);
        AsPublicBytes = bytes[0..(pubKeySize - 1)];
        PublicKeyParameters = new Ed25519PublicKeyParameters(bytes, 0);
        PrivateKeyParameters = bytes.Length > pubKeySize
            ? new Ed25519PrivateKeyParameters(bytes.ExactLength(pubKeySize + Ed25519PrivateKeyParameters.KeySize), pubKeySize)
            : null;
    }

    public EdDSAParameters(Ed25519PublicKeyParameters pubKey, Ed25519PrivateKeyParameters privKey) {
        PublicKeyParameters = pubKey.Required();
        PrivateKeyParameters = privKey.Required();
        var buf = new byte[Ed25519PublicKeyParameters.KeySize];
        pubKey.Encode(buf);
        AsPublicBytes = buf;
        var privBuf = new byte[Ed25519PrivateKeyParameters.KeySize];
        privKey.Encode(privBuf);
        AsBytes = buf.Concat(privBuf).ToArray();
    }

    public byte[] AsBytes { get; }
    public byte[] AsPublicBytes { get; }
    public bool HasPrivatePart => PrivateKeyParameters is not null;

    internal Ed25519PublicKeyParameters PublicKeyParameters { get; }

    internal Ed25519PrivateKeyParameters? PrivateKeyParameters { get; }

     
}
