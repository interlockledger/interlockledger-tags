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
public static class TagProvider
{
    public static ILTag DeserializeFrom(Stream s) {
        if (s.HasBytes()) {
            var tagId = s.DecodeTagId();
            return !_deserializers.ContainsKey(tagId)
                ? new ILTagUnknown(tagId, s)
                : _deserializers[tagId].fromStream(s);
        }
        return ILTagNull.Instance;
    }

    public static ILTag DeserializeFrom(byte[] bytes) {
        using var ms = new MemoryStream(bytes);
        return DeserializeFrom(ms);
    }

    public static ILTag DeserializeFromJson(ulong tagId, object? payload)
        => payload is null
            ? ILTagNull.Instance
            : !_deserializers.ContainsKey(tagId)
                ? throw new ArgumentException($"Unknown tagId: {tagId}", nameof(tagId))
                : _deserializers[tagId].fromJson(payload);

    public static bool HasDeserializer(ulong id) => _deserializers.ContainsKey(id);

    public static ILTag NoJson(object json) => throw new InvalidOperationException($"Can't deserialize from json");

    public static void RegisterDeserializer(ulong id, Func<Stream, ILTag> deserializer, Func<object, ILTag>? jsonDeserializer = null) {
        if (HasDeserializer(id))
            throw new ArgumentException($"Can't redefine deserializer for id: {id}", nameof(id));
        _deserializers[id] = (
            deserializer.Required(),
            jsonDeserializer ?? NoJson
        );
    }

    public static void RegisterDeserializersFrom(params ITagDeserializersProvider[] providers) {
        foreach (var provider in providers)
            foreach (var (id, deserializer, jsonDeserializer) in provider.Deserializers)
                RegisterDeserializer(id, deserializer, jsonDeserializer);
    }

    private static readonly Dictionary<ulong, (Func<Stream, ILTag> fromStream, Func<object, ILTag> fromJson)> _deserializers
    = new() {
        [ILTagId.Null] = (_ => ILTagNull.Instance, _ => ILTagNull.Instance),
        [ILTagId.Bool] = (s => s.ReadSingleByte() != 0 ? ILTagBool.True : ILTagBool.False, o => (bool)o ? ILTagBool.True : ILTagBool.False),
        [ILTagId.Int8] = (s => new ILTagInt8(s, ILTagId.Int8), o => new ILTagInt8(Convert.ToSByte(o, CultureInfo.InvariantCulture))),
        [ILTagId.UInt8] = (s => new ILTagUInt8(s, ILTagId.UInt8), o => new ILTagUInt8(Convert.ToByte(o, CultureInfo.InvariantCulture))),
        [ILTagId.Int16] = (s => new ILTagInt16(s, ILTagId.Int16), o => new ILTagInt16(Convert.ToInt16(o, CultureInfo.InvariantCulture))),
        [ILTagId.UInt16] = (s => new ILTagUInt16(s, ILTagId.UInt16), o => new ILTagUInt16(Convert.ToUInt16(o, CultureInfo.InvariantCulture))),
        [ILTagId.Int32] = (s => new ILTagInt32(s, ILTagId.Int32), o => new ILTagInt32(Convert.ToInt32(o, CultureInfo.InvariantCulture))),
        [ILTagId.UInt32] = (s => new ILTagUInt32(s, ILTagId.UInt32), o => new ILTagUInt32(Convert.ToUInt32(o, CultureInfo.InvariantCulture))),
        [ILTagId.Int64] = (s => new ILTagInt64(s, ILTagId.Int64), o => new ILTagInt64(Convert.ToInt64(o, CultureInfo.InvariantCulture))),
        [ILTagId.UInt64] = (s => new ILTagUInt64(s, ILTagId.UInt64), o => new ILTagUInt64(Convert.ToUInt64(o, CultureInfo.InvariantCulture))),
        [ILTagId.ILInt] = (s => new ILTagILInt(s, ILTagId.ILInt), o => new ILTagILInt(Convert.ToUInt64(o, CultureInfo.InvariantCulture))),
        [ILTagId.ILIntSigned] = (s => new ILTagILIntSigned(s, ILTagId.ILIntSigned), o => new ILTagILIntSigned(Convert.ToInt64(o, CultureInfo.InvariantCulture))),
        [ILTagId.Timestamp] = (s => new ILTagTimestamp(s, ILTagId.Timestamp), o => new ILTagTimestamp(DateTimeOffset.Parse(Convert.ToString(o).Safe(), CultureInfo.InvariantCulture))),
        [ILTagId.Binary32] = (s => new ILTagBinary32(s), NoJson),
        [ILTagId.Binary64] = (s => new ILTagBinary64(s), NoJson),
        [ILTagId.Binary128] = (s => new ILTagBinary128(s), NoJson),
        [ILTagId.ByteArray] = (s => new ILTagByteArray(s), o => new ILTagByteArray(o)),
        [ILTagId.String] = (s => new ILTagString(s), o => new ILTagString(o as string)),
        [ILTagId.BigInteger] = (s => new ILTagBigInteger(s), NoJson),
        [ILTagId.BigDecimal] = (s => new ILTagBigDecimal(s), NoJson),
        [ILTagId.ILIntArray] = (s => new ILTagArrayOfILInt(s), ILTagArrayOfILInt.FromJson),
        [ILTagId.ILTagArray] = (s => new ILTagArrayOfILTag<ILTag>(s), ILTagArrayOfILTag<ILTag>.FromJson),
        [ILTagId.Sequence] = (s => new ILTagSequence(s), o => new ILTagSequence(o)),
        [ILTagId.Version] = (s => new ILTagVersion(s), ILTagVersion.FromJson),
        [ILTagId.Range] = (s => new ILTagRange(s), o => ILTagRange.Build((string)o)),
        [ILTagId.Dictionary] = (s => new ILTagDictionary<ILTag>(s), o => new ILTagDictionary<ILTag>(o)),
        [ILTagId.StringDictionary] = (s => new ILTagStringDictionary(s), o => new ILTagStringDictionary(o)),
        [ILTagId.PubKey] = (TagPubKey.Resolve, o => TagPubKey.Build((string)o)),
        [ILTagId.Signature] = (s => new TagSignature(s), NoJson),
        [ILTagId.Hash] = (s => new TagHash(s), o => TagHash.Build((string)o)),
        [ILTagId.PublicRSAParameters] = (s => new TagPublicRSAParameters(s), NoJson),
        [ILTagId.RSAParameters] = (s => new TagRSAParameters(s), NoJson),
        [ILTagId.Encrypted] = (s => new TagEncrypted(s), NoJson),
        [ILTagId.InterlockId] = (InterlockId.DeserializeAndResolve, o => InterlockId.Build((string)o)),
        [ILTagId.InterlockKey] = (s => new InterlockKey(s), NoJson),
        [ILTagId.InterlockSigningKey] = (s => new InterlockSigningKeyData(s), NoJson),
        [ILTagId.InterlockUpdatableSigningKey] = (s => new InterlockUpdatableSigningKeyData(s), NoJson),
        [ILTagId.Hmac] = (s => new TagHmac(s), o => TagHmac.Build((string)o)),
        [ILTagId.Certificate] = (s => new TagCertificate(s), NoJson),
        [ILTagId.SignedValue] = (s => s.SignedValueFromStream(), NoJson),
        [ILTagId.IdentifiedSignature] = (s => new IdentifiedSignature.Payload(ILTagId.IdentifiedSignature, s), NoJson),
        [ILTagId.Reader] = (s => new TagReader(s), NoJson),
        [ILTagId.ReadingKey] = (s => new TagReadingKey(s), NoJson),
        [ILTagId.EncryptedBlob] = (s => new EncryptedBlob.Payload(ILTagId.EncryptedBlob, s), NoJson),
        [ILTagId.InterlockKeyAppPermission] = (s => new AppPermissions.Tag(s), NoJson),
        [ILTagId.EncryptedText] = (s => new EncryptedText.Payload(ILTagId.EncryptedText, s), NoJson),
        [ILTagId.ECParameters] = (s => new ECDsaParameters.Payload(ILTagId.ECParameters, s), NoJson),
        [ILTagId.DataModel] = (s => new ILTagDataModel(s), NoJson),
        [ILTagId.DataField] = (s => new ILTagDataField(s), NoJson),
        [ILTagId.DataIndex] = (s => new ILTagDataIndex(s), NoJson),
    };
}