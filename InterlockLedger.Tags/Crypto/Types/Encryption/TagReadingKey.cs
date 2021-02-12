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

using System;
using System.Collections.Generic;
using System.IO;

namespace InterlockLedger.Tags
{
    public class TagReadingKey : ILTagExplicit<TagReadingKey.Parts>
    {
        public TagReadingKey(string id, TagHash publicKeyHash, byte[] encryptedKey, byte[] encryptedIV)
            : base(ILTagId.ReadingKey, new Parts(id, publicKeyHash, encryptedKey, encryptedIV)) {
        }

        public byte[] EncryptedIV => Value.EncryptedIV;
        public byte[] EncryptedKey => Value.EncryptedKey;
        public TagHash PublicKeyHash => Value.PublicKeyHash;
        public string ReaderId => Value.ReaderId;

        public struct Parts : IEquatable<Parts>
        {
            public readonly byte[] EncryptedIV;
            public readonly byte[] EncryptedKey;

            public readonly TagHash PublicKeyHash;

            public readonly string ReaderId;

            public Parts(string id, TagHash publicKeyHash, byte[] encryptedKey, byte[] encryptedIV) {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("Must provide a non-empty id for this reading key", nameof(id));
                ReaderId = id;
                PublicKeyHash = publicKeyHash;
                EncryptedKey = encryptedKey ?? throw new System.ArgumentNullException(nameof(encryptedKey));
                EncryptedIV = encryptedIV.Required(nameof(encryptedIV));
            }

            public static bool operator !=(Parts left, Parts right) => !(left == right);

            public static bool operator ==(Parts left, Parts right) => left.Equals(right);

            public override bool Equals(object obj) => obj is Parts parts && Equals(parts);

            public bool Equals(Parts other) => EqualityComparer<byte[]>.Default.Equals(EncryptedIV, other.EncryptedIV) && EqualityComparer<byte[]>.Default.Equals(EncryptedKey, other.EncryptedKey) && EqualityComparer<TagHash>.Default.Equals(PublicKeyHash, other.PublicKeyHash) && ReaderId == other.ReaderId;

            public override int GetHashCode() => HashCode.Combine(EncryptedIV, EncryptedKey, PublicKeyHash, ReaderId);

            public override string ToString() => $"Reading key for reader {ReaderId}";
        }

        internal TagReadingKey(Stream s) : base(ILTagId.ReadingKey, s) {
        }

        protected override Parts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new Parts(s.DecodeString(), s.Decode<TagHash>(), s.DecodeByteArray(), s.DecodeByteArray()));

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.EncodeString(ReaderId).EncodeTag(PublicKeyHash).EncodeByteArray(EncryptedKey).EncodeByteArray(EncryptedIV));
    }
}