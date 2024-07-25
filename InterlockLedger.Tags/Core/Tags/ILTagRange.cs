// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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

using System.Text.Json;

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeNotNullConverter<ILTagRange>))]
[JsonConverter(typeof(JsonNotNullConverter<ILTagRange>))]
public class ILTagRange : ILTagOfExplicitTextual<LimitedRange>, ITextualLight<ILTagRange>
{
    public ILTagRange() : this(LimitedRange.Empty) { }
    public ILTagRange(LimitedRange range) : base(ILTagId.Range, range) { }
    public static ILTagRange Empty { get; } = new ILTagRange();
    public bool IsEmpty => Value.IsEmpty;
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ILTagRange result) {
        result = Parse(s.Safe(), provider);
        return !result.Value.IsInvalid();
    }
    public static ILTagRange Parse(string s, IFormatProvider? provider) {
        var range = LimitedRange.Parse(s, provider);
        return new(range);
    }
    public static ILTagRange FromJson(object json) {
        var range = json is JsonElement je && je.ValueKind == JsonValueKind.String
                ? LimitedRange.Parse(je.GetString()!, null)
                : json is string js
                    ? LimitedRange.Parse(js, null)
                    : throw new InvalidDataException($"Could not parse '{json}' to an ILTagRange");
        return new ILTagRange(range);
    }
    internal ILTagRange(Stream s) : base(ILTagId.Range, s) { }
    public bool Equals(ILTagRange? other) => base.Equals(other);
    protected override Task<LimitedRange> ValueFromStreamAsync(WrappedReadonlyStream s) => Task.FromResult<LimitedRange>(new(s.ILIntDecode(), s.BigEndianReadUShort()));
    protected override Task<Stream> ValueToStreamAsync(Stream s) => Task.FromResult(s.ILIntEncode(Value.Start).BigEndianWriteUShort(Value.Count));
}