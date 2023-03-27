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

/// <summary>
/// This is the interface of the key pair codecs. Instances of this
/// interface must be thread safe and immutable.
/// </summary>
public interface IKeyPairCodec
{
    public ushort Algorithm { get; }

    /// <summary>
    /// Serializes the public key.
    /// </summary>
    /// <param name="publicKey">The public key to be serialized.</param>
    /// <returns>The serialized key in bytes.</returns>
    public byte[] SerializePublic(object publicKey);

    /// <summary>
    /// Serializes the private key.
    /// </summary>
    /// <param name="privateKey">The private key to be serialized.</param>
    /// <returns>The serialized key in bytes. The contents of this array will
    /// be properly destroyed by the caller.</returns>
    public byte[] SerializePrivate(object privateKey);

    /// <summary>
    /// Deserializes the public key.
    /// </summary>
    /// <param name="serialized">The serialized public key.</param>
    /// <returns>The deserialized public key.</returns>
    /// <exception cref="KeyStoreException">If the key could not be deserialized.</exception>
    public object DeserializePublic(byte[] serialized);

    /// <summary>
    /// Deserializes the private key.
    /// </summary>
    /// <param name="serialized">The serialized private key. This clone 
    /// this value if required because the contents of this array will be
    /// properly destroyed by the caller.</param>
    /// <returns>The deserialized private key.</returns>
    /// <exception cref="KeyStoreException">If the key could not be deserialized.</exception>
    public object DeserializePrivate(byte[] serialized);
}
