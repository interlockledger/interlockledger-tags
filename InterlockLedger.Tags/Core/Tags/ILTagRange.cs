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

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<ILTagRange>))]
[JsonConverter(typeof(JsonCustomConverter<ILTagRange>))]
public class ILTagRange : ILTagExplicit<LimitedRange>, ITextual<ILTagRange>
{
    public ILTagRange() : this(LimitedRange.Empty) { }
    public ILTagRange(LimitedRange range) : base(ILTagId.Range, range) => TextualRepresentation = Value.TextualRepresentation;
    public override object AsJson => this;
    public static ILTagRange Empty { get; } = new ILTagRange();
    public static Regex Mask => LimitedRange.Mask;
    public bool IsEmpty => Value.IsEmpty;
    public string? InvalidityCause => Value.InvalidityCause;
    public ITextual<ILTagRange> Textual => this;
    public static ILTagRange Build(string textualRepresentation) => new(LimitedRange.Build(textualRepresentation));
    internal ILTagRange(Stream s) : base(ILTagId.Range, s) => TextualRepresentation = Value.TextualRepresentation;
    protected override LimitedRange FromBytes(byte[] bytes) =>
        FromBytesHelper(bytes, s => new LimitedRange(s.ILIntDecode(), s.BigEndianReadUShort()));
    protected override byte[] ToBytes(LimitedRange value) =>
        TagHelpers.ToBytesHelper(s => s.ILIntEncode(Value.Start).BigEndianWriteUShort(Value.Count));
    static ILTagRange ITextual<ILTagRange>.InvalidBy(string cause) => new(LimitedRange.InvalidBy(cause));
    public bool Equals(ILTagRange? other) => base.Equals(other);
}