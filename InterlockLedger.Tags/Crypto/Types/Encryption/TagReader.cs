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

namespace InterlockLedger.Tags;
public class TagReader : ILTagOfExplicit<TagReader.Parts>, IIdentifiedPublicKey
{
    public TagReader(string name, TagPubKey publicKey) :
        base(ILTagId.Reader, new Parts(name.Required(), publicKey.Required())) {
    }

    public TagReader(IIdentifiedPublicKey ipk) : this(ipk.Required().Identifier, ipk.PublicKey) => Id = ipk.Id;

    public string Name => Value!.Name;
    public TagPubKey PublicKey => Value!.PublicKey;

    public BaseKeyId? Id { get; }

    public class Parts : IEquatable<Parts>
    {
        public readonly string Name;
        public readonly TagPubKey PublicKey;

        public Parts(string name, TagPubKey publicKey) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must provide a non-empty name for this reader", nameof(name));
            Name = name;
            PublicKey = publicKey.Required();
        }

        public static bool operator !=(Parts left, Parts right) => !(left == right);

        public static bool operator ==(Parts left, Parts right) => left.Equals(right);

        public override bool Equals(object? obj) => obj is Parts parts && Equals(parts);

        public bool Equals(Parts? other) => other is not null && PublicKey.Equals(other.PublicKey) && Name == other.Name;

        public override int GetHashCode() => HashCode.Combine(PublicKey, Name);

        public override string ToString() => $"Reader '{Name}' with public key {PublicKey}";
    }

    internal TagReader(Stream s) : base(ILTagId.Reader, s) => Value.Required();

    protected override  Task<Parts?> ValueFromStreamAsync(WrappedReadonlyStream s)
        => Task.FromResult<Parts?>(new(s.DecodeString().Required(), s.Decode<TagPubKey>().Required()));
    protected override  Task<Stream> ValueToStreamAsync(Stream s)
        => Task.FromResult( s.EncodeString(Value!.Name).EncodeTag(Value.PublicKey));
}