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

[TypeConverter(typeof(TypeNotNullConverter<ILTagTimestamp>))]
[JsonConverter(typeof(JsonNotNullConverter<ILTagTimestamp>))]
public class ILTagTimestamp : ILTagOfImplicit<DateTimeOffset>, ITextualLight<ILTagTimestamp>
{
    private const string _fixedFormat = "u";
    private string? _textualRepresentation;

    public string TextualRepresentation => _textualRepresentation ??= BuildTextualRepresentation();
    internal ILTagTimestamp(Stream s, ulong alreadyDeserializedTagId) : base(ILTagId.Timestamp, s) => Traits.ValidateTagId(alreadyDeserializedTagId);

    protected override Task<DateTimeOffset> ValueFromStreamAsync(WrappedReadonlyStream s) =>
        Task.FromResult(DateTimeOffset.FromUnixTimeMilliseconds(s.ILIntDecode().AsSignedILInt()));

    protected override Task<Stream> ValueToStreamAsync(Stream s) {
        s.ILIntEncode(Value.ToUnixTimeMilliseconds().AsUnsignedILInt());
        return Task.FromResult(s);
    }
    public ILTagTimestamp(DateTimeOffset value) : base(ILTagId.Timestamp, value) { }
    private string BuildTextualRepresentation() => Value.ToString(_fixedFormat, CultureInfo.InvariantCulture);
    public bool Equals(ILTagTimestamp? other) => other is not null && other.Value == Value;
    public static ILTagTimestamp Parse(string s, IFormatProvider? provider) => new(DateTimeOffset.ParseExact(s, _fixedFormat, CultureInfo.InvariantCulture));
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ILTagTimestamp result) {
        result = Parse(s.Safe(), provider);
        return true;
    }
}