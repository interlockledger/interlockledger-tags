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
    public class TagReader : ILTagExplicit<TagReader.Parts>, IIdentifiedPublicKey
    {
        public TagReader(string name, TagPubKey publicKey) :
            base(ILTagId.Reader, new Parts(name.Required(nameof(name)), publicKey.Required(nameof(publicKey)))) { }

        public TagReader(IIdentifiedPublicKey ipk) : this(ipk.Required(nameof(ipk)).Identifier, ipk.PublicKey) { }

        BaseKeyId IIdentifiedPublicKey.Id { get; }
        public string Name => Value.Name;
        public TagPubKey PublicKey => Value.PublicKey;

        public struct Parts : IEquatable<Parts>
        {
            public readonly string Name;
            public readonly TagPubKey PublicKey;

            public Parts(string name, TagPubKey publicKey) {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Must provide a non-empty name for this reader", nameof(name));
                Name = name;
                PublicKey = publicKey.Required(nameof(publicKey));
            }

            public static bool operator !=(Parts left, Parts right) => !(left == right);

            public static bool operator ==(Parts left, Parts right) => left.Equals(right);

            public override bool Equals(object obj) => obj is Parts parts && Equals(parts);

            public bool Equals(Parts other) => EqualityComparer<TagPubKey>.Default.Equals(PublicKey, other.PublicKey) && Name == other.Name;

            public override int GetHashCode() => HashCode.Combine(PublicKey, Name);

            public override string ToString() => $"Reader '{Name}' with public key {PublicKey}";
        }

        internal TagReader(Stream s) : base(ILTagId.Reader, s) {
        }

        protected override Parts FromBytes(byte[] bytes)
            => FromBytesHelper(bytes, s => new Parts(s.DecodeString(), s.Decode<TagPubKey>()));

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.EncodeString(Value.Name).EncodeTag(Value.PublicKey));
    }
}