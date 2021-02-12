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

#nullable enable

using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace InterlockLedger.Tags
{
    public class SenderIdentity : IEquatable<SenderIdentity?>, IIdentifiedPublicKey
    {
        public SenderIdentity(BaseKeyId id, TagPubKey publicKey, string? name) {
            Id = id.Required(nameof(id));
            PublicKey = publicKey.Required(nameof(publicKey));
            Name = name;
            _reader = new Lazy<TagReader>(() => new TagReader(Id.TextualRepresentation, PublicKey));
        }

        public SenderIdentity(IEnumerable<Claim> claims) : this(claims.Sender(), claims.PublicKey(), claims.Name()) { }

        public TagReader AsReader => _reader.Value;
        public BaseKeyId Id { get; }
        public string? Name { get; }
        public TagPubKey PublicKey { get; }

        public static bool operator !=(SenderIdentity? left, SenderIdentity? right) => !(left == right);

        public static bool operator ==(SenderIdentity? left, SenderIdentity? right) => left?.Equals(right) ?? right is null;

        public override bool Equals(object? obj) => Equals(obj as SenderIdentity);

        public bool Equals(SenderIdentity? other) => other != null && Id == other.Id && PublicKey == other.PublicKey;

        public override int GetHashCode() => HashCode.Combine(Id, PublicKey);

        public override string ToString() => $"Sender {Id} with public key {PublicKey}";

        private readonly Lazy<TagReader> _reader;
    }
}