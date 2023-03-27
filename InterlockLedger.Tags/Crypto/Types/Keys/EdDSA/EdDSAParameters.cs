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
    }

    public byte[] AsBytes { get; internal set; }
    public byte[] AsPublicBytes { get; internal set; }
    public bool HasPrivatePart { get; internal set; }
}
/// <summary>
/// This class implements a <see cref="Ed25519KeyPairCodec"/> that can serialize
/// Ed25519 (EdDSA) public and private key.
/// </summary>
public class Ed25519KeyPairCodec : IKeyPairCodec
{

    /// <summary>
    /// Returns a singleton instance of this codec.
    /// </summary>
    /// <value>The singleton instance of this codec.</value>
    public static Ed25519KeyPairCodec Instance { get; } = new();

    /// <summary>
    /// The algorithm ID as specified by in
    /// <a href="https://github.com/interlockledger/specification/blob/master/crypto/dsign.md">IL2 Cryptographic digital signature algorithms</a>.
    /// </summary>
    public const ushort ALGORITHM_ID = 5;

    public ushort Algorithm => ALGORITHM_ID;

    public byte[] SerializePublic(object publicKey) {
        if (publicKey is not Ed25519PublicKeyParameters) {
            throw new ArgumentException($"Bad public key. Expecting {typeof(Ed25519PublicKeyParameters)} but got {typeof(object)}.");
        }
        byte[] serialized = new byte[32];
        ((Ed25519PublicKeyParameters)publicKey).Encode(serialized, 0);
        return serialized;
    }

    public byte[] SerializePrivate(object privateKey) {
        if (privateKey is not Ed25519PrivateKeyParameters) {
            throw new ArgumentException($"Bad private key. Expecting {typeof(Ed25519PrivateKeyParameters)} but got {typeof(object)}.");
        }
        byte[] serialized = new byte[32];
        ((Ed25519PrivateKeyParameters)privateKey).Encode(serialized, 0);
        return serialized;
    }

    public object DeserializePublic(byte[] serialized) {
        try {
            return new Ed25519PublicKeyParameters(serialized, 0);
        } catch (Exception e) {
            throw new KeyStoreException("Unable to deserialize the public key.", e);
        }
    }

    public object DeserializePrivate(byte[] serialized) {
        try {
            return new Ed25519PrivateKeyParameters(serialized, 0);
        } catch (Exception e) {
            throw new KeyStoreException("Unable to deserialize the private key.", e);
        }
    }
}
