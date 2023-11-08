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

using Org.BouncyCastle.Utilities;

namespace InterlockLedger.Tags;
public class TagSignatureParts : IEquatable<TagSignatureParts>
{
    public Algorithm Algorithm;
    public byte[] Data = Array.Empty<byte>();

    public static bool operator !=(TagSignatureParts left, TagSignatureParts right) => !(left == right);

    public static bool operator ==(TagSignatureParts left, TagSignatureParts right) => left.Equals(right);

    public override bool Equals(object? obj) => obj is TagSignatureParts parts && Equals(parts);

    public bool Equals(TagSignatureParts? other) => other is not null && Algorithm == other.Algorithm && EqualityComparer<byte[]>.Default.Equals(Data, other.Data);

    public override int GetHashCode() => HashCode.Combine(Algorithm, Data);
}

public class TagSignature : ILTagOfExplicit<TagSignatureParts>
{
    public TagSignature(Algorithm algorithm, byte[] data) : base(ILTagId.Signature, new TagSignatureParts { Algorithm = algorithm, Data = data }) {
    }

    [JsonIgnore]
    public Algorithm Algorithm => Value!.Algorithm;

    [JsonIgnore]
    public byte[] Data => Value!.Data;

    internal TagSignature(Stream s) : base(ILTagId.Signature, s) => Value.Required();

    protected override TagSignatureParts? ValueFromStream(Stream s) => new() {
        Algorithm = (Algorithm)s.BigEndianReadUShort(),
        Data = s.ReadAllBytesAsync().Result
    };
    protected override Stream ValueToStream(Stream s) => s.BigEndianWriteUShort((ushort)Value!.Algorithm).WriteBytes(Value.Data);
}