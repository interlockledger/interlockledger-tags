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
    public struct TagEncryptedParts : IEquatable<TagEncryptedParts>
    {
        public CipherAlgorithm Algorithm;
        public byte[] CipherData;

        public static bool operator !=(TagEncryptedParts left, TagEncryptedParts right) => !(left == right);

        public static bool operator ==(TagEncryptedParts left, TagEncryptedParts right) => left.Equals(right);

        public override bool Equals(object obj) => obj is TagEncryptedParts parts && Equals(parts);

        public bool Equals(TagEncryptedParts other) => Algorithm == other.Algorithm && EqualityComparer<byte[]>.Default.Equals(CipherData, other.CipherData);

        public override int GetHashCode() => HashCode.Combine(Algorithm, CipherData);
    }

    public class TagEncrypted : ILTagExplicit<TagEncryptedParts>
    {
        public TagEncrypted(CipherAlgorithm algorithm, byte[] data) :
            base(ILTagId.Encrypted, new TagEncryptedParts { Algorithm = algorithm, CipherData = data }) {
        }

        public CipherAlgorithm Algorithm => Value.Algorithm;

        public byte[] CipherData => Value.CipherData;

        internal TagEncrypted(Stream s) : base(ILTagId.Encrypted, s) {
        }

        protected override TagEncryptedParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new TagEncryptedParts {
                Algorithm = (CipherAlgorithm)s.BigEndianReadUShort(),
                CipherData = s.DecodeByteArray(),
            });

        protected override byte[] ToBytes(TagEncryptedParts value)
            => TagHelpers.ToBytesHelper(s => s.BigEndianWriteUShort((ushort)value.Algorithm).EncodeByteArray(value.CipherData));
    }
}