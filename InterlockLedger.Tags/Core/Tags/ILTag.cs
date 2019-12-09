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
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public abstract class ILTag : ITag
    {
        [JsonIgnore]
        public abstract object AsJson { get; }

        [JsonIgnore]
        public byte[] EncodedBytes => SerializeToByteArray();

        [JsonIgnore]
        public virtual string Formatted => EncodedBytes.ToSafeBase64();

        [JsonIgnore]
        public bool IsNull => TagId == ILTagId.Null;

        public ulong TagId { get; set; }

        public static ILTag DeserializeFrom(Stream s) {
            if (s.HasBytes()) {
                var tagId = s.DecodeTagId();
                if (!_deserializers.ContainsKey(tagId))
                    return new ILTagUnknown(tagId, s);
                return _deserializers[tagId].fromStream(s);
            }
            return ILTagNull.Instance;
        }

        public static ILTag DeserializeFrom(byte[] bytes) {
            using var ms = new MemoryStream(bytes);
            return DeserializeFrom(ms);
        }

        public static ILTag DeserializeFromJson(ulong tagId, object payload) {
            if (payload is null)
                return ILTagNull.Instance;
            if (!_deserializers.ContainsKey(tagId))
                throw new ArgumentException($"Unknown tagId: {tagId}", nameof(tagId));
            return _deserializers[tagId].fromJson(payload);
        }

        public static bool HasDeserializer(ulong id) => _deserializers.ContainsKey(id);

        public static ILTag NoJson(object json) => throw new InvalidOperationException($"Can't deserialize from json");

        public static void RegisterDeserializer(ulong id, Func<Stream, ILTag> deserializer, Func<object, ILTag> jsonDeserializer = null) {
            if (HasDeserializer(id))
                throw new ArgumentException($"Can't redefine deserializer for id: {id}", nameof(id));
            _deserializers[id] = (
                deserializer ?? throw new ArgumentNullException(nameof(deserializer)),
                jsonDeserializer ?? NoJson
            );
        }

        public static void RegisterDeserializersFrom(params ITagDeserializersProvider[] providers) {
            foreach (var provider in providers)
                foreach (var (id, deserializer, jsonDeserializer) in provider.Deserializers)
                    RegisterDeserializer(id, deserializer, jsonDeserializer);
        }

        public T As<T>() where T : ILTag => this as T ?? throw new InvalidDataException($"Not an {typeof(T).Name}");

        public string AsString() => (TagId == ILTagId.String) ? Formatted : ToString();

        public Stream SerializeInto(Stream s) {
            try {
                s.ILIntEncode(TagId);
                SerializeInner(s);
            } finally {
                s.Flush();
            }
            return s;
        }

        public override string ToString() => GetType().Name + $"#{TagId}:" + Formatted;

        protected ILTag(ulong tagId) => TagId = tagId;

        protected abstract void SerializeInner(Stream s);

        private static readonly Dictionary<ulong, (Func<Stream, ILTag> fromStream, Func<object, ILTag> fromJson)> _deserializers
            = new Dictionary<ulong, (Func<Stream, ILTag> fromStream, Func<object, ILTag> fromJson)> {
                [ILTagId.Null] = (_ => ILTagNull.Instance, _ => ILTagNull.Instance),
                [ILTagId.Bool] = (s => s.ReadSingleByte() != 0 ? ILTagBool.True : ILTagBool.False, o => (bool)o ? ILTagBool.True : ILTagBool.False),
                [ILTagId.Int8] = (s => new ILTagInt8(s, ILTagId.Int8), o => new ILTagInt8(Convert.ToSByte(o))),
                [ILTagId.UInt8] = (s => new ILTagUInt8(s, ILTagId.UInt8), o => new ILTagUInt8(Convert.ToByte(o))),
                [ILTagId.Int16] = (s => new ILTagInt16(s, ILTagId.Int16), o => new ILTagInt16(Convert.ToInt16(o))),
                [ILTagId.UInt16] = (s => new ILTagUInt16(s, ILTagId.UInt16), o => new ILTagUInt16(Convert.ToUInt16(o))),
                [ILTagId.Int32] = (s => new ILTagInt32(s, ILTagId.Int32), o => new ILTagInt32(Convert.ToInt32(o))),
                [ILTagId.UInt32] = (s => new ILTagUInt32(s, ILTagId.UInt32), o => new ILTagUInt32(Convert.ToUInt32(o))),
                [ILTagId.Int64] = (s => new ILTagInt64(s, ILTagId.Int64), o => new ILTagInt64(Convert.ToInt64(o))),
                [ILTagId.UInt64] = (s => new ILTagUInt64(s, ILTagId.UInt64), o => new ILTagUInt64(Convert.ToUInt64(o))),
                [ILTagId.ILInt] = (s => new ILTagILInt(s, ILTagId.ILInt), o => new ILTagILInt(Convert.ToUInt64(o))),
                [ILTagId.Binary32] = (s => new ILTagBinary32(s), NoJson),
                [ILTagId.Binary64] = (s => new ILTagBinary64(s), NoJson),
                [ILTagId.Binary128] = (s => new ILTagBinary128(s), NoJson),
                [ILTagId.ByteArray] = (s => new ILTagByteArray(s), o => new ILTagByteArray(o)),
                [ILTagId.String] = (s => new ILTagString(s), o => new ILTagString(o as string)),
                [ILTagId.BigInteger] = (s => new ILTagBigInteger(s), NoJson),
                [ILTagId.BigDecimal] = (s => new ILTagBigDecimal(s), NoJson),
                [ILTagId.ILIntArray] = (s => new ILTagArrayOfILInt(s), o => new ILTagArrayOfILInt(o)),
                [ILTagId.ILTagArray] = (s => new ILTagArrayOfILTag<ILTag>(s), o => new ILTagArrayOfILTag<ILTag>(o)),
                [ILTagId.Sequence] = (s => new ILTagSequence(s), o => new ILTagSequence(o)),
                [ILTagId.Version] = (s => new ILTagVersion(s), o => ILTagVersion.FromJson(o)),
                [ILTagId.Range] = (s => new ILTagRange(s), o => new ILTagRange(LimitedRange.Resolve((string)o))),
                [ILTagId.Dictionary] = (s => new ILTagDictionary<ILTag>(s), o => new ILTagDictionary<ILTag>(o)),
                [ILTagId.StringDictionary] = (s => new ILTagStringDictionary(s), o => new ILTagStringDictionary(o)),
                [ILTagId.PubKey] = (s => TagPubKey.Resolve(s), o => TagPubKey.Resolve((string)o)),
                [ILTagId.Signature] = (s => new TagSignature(s), NoJson),
                [ILTagId.Hash] = (s => new TagHash(s), o => TagHash.From((string)o)),
                [ILTagId.PublicRSAParameters] = (s => new TagPublicRSAParameters(s), NoJson),
                [ILTagId.RSAParameters] = (s => new TagRSAParameters(s), NoJson),
                [ILTagId.Encrypted] = (s => new TagEncrypted(s), NoJson),
                [ILTagId.InterlockId] = (s => InterlockId.DeserializeAndResolve(s), o => InterlockId.Resolve((string)o)),
                [ILTagId.InterlockKey] = (s => new InterlockKey(s), NoJson),
                [ILTagId.InterlockSigningKey] = (s => new InterlockSigningKeyData(s), NoJson),
                [ILTagId.InterlockUpdatableSigningKey] = (s => new InterlockUpdatableSigningKeyData(s), NoJson),
                [ILTagId.Hmac] = (s => new TagHmac(s), NoJson),
                [ILTagId.Certificate] = (s => new TagCertificate(s), NoJson),
                [ILTagId.SignedValue] = (s => new SignedValue<ILTag>.Payload(ILTagId.SignedValue, s), NoJson),
                [ILTagId.IdentifiedSignature] = (s => new TagIdentifiedSignature(s), NoJson),
                [ILTagId.Reader] = (s => new TagReader(s), NoJson),
                [ILTagId.ReadingKey] = (s => new TagReadingKey(s), NoJson),
                [ILTagId.EncryptedBlob] = (s => new EncryptedBlob.Payload(ILTagId.EncryptedBlob, s), NoJson),
                [ILTagId.DataModel] = (s => new ILTagDataModel(s), NoJson),
                [ILTagId.DataField] = (s => new ILTagDataField(s), NoJson),
                [ILTagId.DataIndex] = (s => new ILTagDataIndex(s), NoJson),
            };

        private byte[] SerializeToByteArray() {
            using var stream = new MemoryStream();
            SerializeInto(stream);
            stream.Flush();
            return stream.GetBuffer().PartOf((int)stream.Length);
        }
    }
}