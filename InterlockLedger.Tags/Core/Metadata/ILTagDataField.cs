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

namespace InterlockLedger.Tags;

public class ILTagDataField : ILTagExplicit<DataField>
{
    public ILTagDataField(DataField field) : base(ILTagId.DataField, field) {
    }

    public ILTagDataField(Stream s) : base(ILTagId.DataField, s) {
    }

    protected override DataField FromBytes(byte[] bytes) {
        ushort serVersion = 0;
        return FromBytesHelper(bytes, s => new DataField {
            Version = s.DecodeUShort(),
            TagId = s.DecodeILInt(),
            Name = s.DecodeString() ?? "*ERROR*",
            IsOptional_Deprecated = s.DecodeBool(),
            IsOpaque = s.DecodeBool(),
            ElementTagId = s.DecodeILInt(),
            SubDataFields = s.DecodeTagArray<ILTagDataField>()?.Select(t => t.Value),
            Cast = s.HasBytes() ? (CastType)s.DecodeByte() : CastType.None,
            SerializationVersion = serVersion = s.HasBytes() ? s.DecodeUShort() : (ushort)0,
            Description = (serVersion > 1) ? s.DecodeString().TrimToNull() : null,
            EnumerationDefinition = (serVersion > 2) ? DecodeEnumeration(s) : null,
            EnumerationAsFlags = (serVersion > 3) && s.DecodeBool(),
            IsDeprecated = (serVersion > 4) && s.DecodeBool(),
        });
    }

    protected override byte[] ToBytes(DataField value)
        => TagHelpers.ToBytesHelper(s => {
            s.EncodeUShort(Value.Version);
            s.EncodeILInt(Value.TagId);
            s.EncodeString(Value.Name);
            s.EncodeBool(false);
            s.EncodeBool(Value.IsOpaque);
            s.EncodeILInt(Value.ElementTagId);
            s.EncodeTagArray(Value.SubDataFields?.Select(df => new ILTagDataField(df)));
            s.EncodeByte((byte)value.Cast);
            s.EncodeUShort(Value.SerializationVersion);
            s.EncodeString(Value.Description);
            EncodeEnumeration(s, value.EnumerationDefinition);
            s.EncodeBool(Value.EnumerationAsFlags);
            s.EncodeBool(Value.IsDeprecated);
        });

    private static EnumerationDictionary DecodeEnumeration(Stream s) {
        var triplets = s.DecodeArray<Triplet, Triplet.Tag>(s => new Triplet.Tag(s));
        return new EnumerationDictionary(triplets.Safe().Where(t => t is not null).ToDictionary(t => t!.Value, t => new EnumerationDetails(t.Name, t.Description)));
    }

    private static void EncodeEnumeration(Stream s, EnumerationDictionary? enumeration)
        => s.EncodeTagArray(enumeration?.Select(p => new Triplet(p.Key, p.Value.Name, p.Value.Description).AsTag));

    private class Triplet : EnumerationDetails
    {
        public readonly ulong Value;

        public Triplet(ulong value, string? name, string? description) : base(name.Required(), description.Required()) => Value = value;

        public Tag AsTag => new(this);

        public class Tag : ILTagOfExplicit<Triplet>
        {
            public Tag(Triplet v) : base(0, v) {
            }

            public Tag(Stream s) : base(s.DecodeTagId(), s) {
            }

            protected override Triplet ValueFromStream(StreamSpan s) =>
                new(s.DecodeILInt(), s.DecodeString(), s.DecodeString());
            protected override Task<Stream> ValueToStreamAsync(Stream s) {
                s.EncodeILInt(Value.Value);
                s.EncodeString(Value.Name);
                s.EncodeString(Value.Description);
                return Task.FromResult(s);
            }
        }
    }
}