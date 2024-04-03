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

namespace InterlockLedger.Tags;

public record TagSignatureParts(Algorithm Algorithm, byte[] Data) { }

[JsonConverter(typeof(JsonNotNullConverter<TagSignature>))]
public class TagSignature : ILTagOfExplicit<TagSignatureParts>, ITextualLight<TagSignature>
{
    public TagSignature(Algorithm algorithm, byte[] data) : base(ILTagId.Signature, new TagSignatureParts(algorithm, data)) {
    }

    public Algorithm Algorithm => Value!.Algorithm;

    public byte[] Data => Value!.Data;

    internal TagSignature(Stream s) : base(ILTagId.Signature, s) => Value.Required();

    protected override TagSignatureParts? ValueFromStream(WrappedReadonlyStream s) =>
        new((Algorithm)s.BigEndianReadUShort(), s.ReadAllBytesAsync().WaitResult());

    protected override string? BuildTextualRepresentation() => $"Signature!{Data.ToSafeBase64()}#{Algorithm}";
    protected override Stream ValueToStream(Stream s) => s.BigEndianWriteUShort((ushort)Value!.Algorithm).WriteBytes(Value.Data);
    public static TagSignature Parse(string textualRepresentation, IFormatProvider? provider) =>
        TryParse(textualRepresentation, provider, out var value) ? value : throw new InvalidDataException($"Not a {typeof(TagSignature)} representation");
    public static bool TryParse([NotNullWhen(true)] string? textualRepresentation, IFormatProvider? provider, [MaybeNullWhen(false)] out TagSignature result) {
        var parts = textualRepresentation.Safe().Split('!', '#');
        if (parts.Length == 3
               && parts[0].Equals("Signature", StringComparison.OrdinalIgnoreCase)
               && Enum.TryParse(parts[2], ignoreCase: true, out Algorithm algorithm)) {
            result = new(algorithm, parts[1].FromSafeBase64());
            return true;
        }
        result = null;
        return false;
    }

    public bool Equals(TagSignature? other) => other is not null && other.Value == Value;
}