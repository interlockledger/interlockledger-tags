/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

namespace InterlockLedger.Tags
{
    public class TagIdentifiedSignature : ILTagExplicit<TagIdentifiedSignature.Parts>
    {
        public TagIdentifiedSignature(TagSignature signature, BaseKeyId id, TagPubKey publicKey) : base(ILTagId.IdentifiedSignature, new Parts(signature, id, publicKey)) {
        }

        public TagPubKey PublicKey => Value.PublicKey;
        public TagSignature Signature => Value.Signature;
        public BaseKeyId SignerId => Value.SignerId;

        public bool Verify(byte[] data) => PublicKey.Verify(data, Signature);

        public struct Parts : IEquatable<Parts>
        {
            public readonly TagPubKey PublicKey;
            public readonly TagSignature Signature;
            public readonly BaseKeyId SignerId;

            public Parts(TagSignature signature, BaseKeyId id, TagPubKey publicKey) {
                Signature = signature ?? throw new ArgumentNullException(nameof(signature));
                SignerId = id ?? throw new System.ArgumentNullException(nameof(id));
                PublicKey = publicKey ?? throw new System.ArgumentNullException(nameof(publicKey));
            }

            public static bool operator !=(Parts left, Parts right) => !(left == right);

            public static bool operator ==(Parts left, Parts right) => left.Equals(right);

            public override bool Equals(object obj) => obj is Parts parts && Equals(parts);

            public bool Equals(Parts other) => EqualityComparer<TagPubKey>.Default.Equals(PublicKey, other.PublicKey) && EqualityComparer<TagSignature>.Default.Equals(Signature, other.Signature) && EqualityComparer<BaseKeyId>.Default.Equals(SignerId, other.SignerId);

            public override int GetHashCode() {
                var hashCode = -1433170334;
                hashCode = (hashCode * -1521134295) + EqualityComparer<TagPubKey>.Default.GetHashCode(PublicKey);
                hashCode = (hashCode * -1521134295) + EqualityComparer<TagSignature>.Default.GetHashCode(Signature);
                hashCode = (hashCode * -1521134295) + EqualityComparer<BaseKeyId>.Default.GetHashCode(SignerId);
                return hashCode;
            }

            public override string ToString() => $"Signature by signer {SignerId} with public key {PublicKey}";
        }

        internal TagIdentifiedSignature(Stream s) : base(ILTagId.IdentifiedSignature, s) {
        }

        protected override Parts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => new Parts(s.Decode<TagSignature>(), s.Decode<BaseKeyId>(), s.Decode<TagPubKey>()));

        protected override byte[] ToBytes()
            => ToBytesHelper(s => s.EncodeTag(Value.Signature).EncodeTag(Value.SignerId).EncodeTag(Value.PublicKey));
    }
}