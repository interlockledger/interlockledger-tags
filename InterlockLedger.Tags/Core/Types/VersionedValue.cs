/******************************************************************************************************************************

Copyright (c) 2018-2020 InterlockLedger Network
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
    public abstract class VersionedValue<T> : IVersion where T : VersionedValue<T>, new()
    {
        [JsonIgnore]
        public Payload AsPayload => _payload.Value;

        [JsonIgnore]
        public DataField FieldModel => _fieldModel.Value;

        [JsonIgnore]
        public DataModel PayloadDataModel => _payloadDataModel.Value;

        public ulong TagId { get => _tagId; set { if (value != 0 && value != _tagId) throw new InvalidDataException($"Invalid value for TagId: {_tagId}"); } }

        [JsonIgnore]
        public abstract string TypeName { get; }

        public ushort Version { get; set; }

        public T FromStream(Stream s) {
            Version = s.DecodeUShort(); // Field index 0 //
            DecodeRemainingStateFrom(s);
            return (T)this;
        }

        public T FromUnknown(ILTagUnknown unknown) {
            if (unknown is null)
                throw new ArgumentNullException(nameof(unknown));
            if (unknown.TagId != _tagId)
                throw new InvalidCastException($"Wrong tagId! Expecting {_tagId} but came {unknown.TagId}");
            if (unknown.Value.None())
                throw new ArgumentException("Empty tagged value not expected!", nameof(unknown));
            using var s = new MemoryStream(unknown.Value);
            return FromStream(s);
        }

        public SignedValue<Payload> SignWith(ISigningContext context) {
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            return new SignedValue<Payload>(AsPayload, context.Key.SignWithId(AsPayload.EncodedBytes).AsSingle());
        }

        public void ToStream(Stream s) {
            s.EncodeUShort(Version);    // Field index 0 //
            EncodeRemainingStateTo(s);
        }

        public class Payload : ILTagExplicit<T>, IVersion, INamed
        {
            public Payload(ulong alreadyDeserializedTagId, Stream s) : base(alreadyDeserializedTagId, s) => ValidateTagId(Value.TagId);

            public override object AsJson => Value.AsJson;
            public string TypeName => typeof(T).Name;
            public ushort Version => Value.Version;

            public T FromJson(object o) => new T().FromJson(o);

            public override string ToString() => Value.ToString();

            internal Payload(T Value) : base(Value.TagId, Value) {
            }

            protected override T FromBytes(byte[] bytes) => FromBytesHelper(bytes, new T().FromStream);

            protected override byte[] ToBytes() => ToBytesHelper(Value.ToStream);
        }

        protected VersionedValue(ulong tagId, ushort version) {
            _tagId = tagId;
            Version = version;
            _payload = new Lazy<Payload>(() => new Payload((T)this));
            _fieldModel = new Lazy<DataField>(() => new DataField(TypeName, TagId, TypeDescription) {
                SubDataFields = _versionField.AppendedOf(RemainingStateFields)
            });
            _payloadDataModel = new Lazy<DataModel>(() => new DataModel {
                PayloadName = TypeName,
                PayloadTagId = TagId,
                DataFields = _versionField.AppendedOf(RemainingStateFields),
                Indexes = PayloadIndexes
            });
        }

        protected abstract object AsJson { get; }

        protected virtual DataIndex[] PayloadIndexes => Array.Empty<DataIndex>();

        protected abstract IEnumerable<DataField> RemainingStateFields { get; }

        protected abstract string TypeDescription { get; }

        protected static bool RegisterAsField(ITagRegistrar registrar, ulong fieldTagId) {
            if (registrar is null)
                throw new ArgumentNullException(nameof(registrar));
            return registrar.RegisterILTag(fieldTagId, s => BuildPayload(fieldTagId, s), PayloadFromJson, null);
        }

        protected abstract void DecodeRemainingStateFrom(Stream s);

        protected abstract void EncodeRemainingStateTo(Stream s);

        protected abstract T FromJson(object json);

        private static readonly DataField _versionField = new DataField(nameof(Version), ILTagId.UInt16);

        private readonly Lazy<DataField> _fieldModel;

        private readonly Lazy<Payload> _payload;

        private readonly Lazy<DataModel> _payloadDataModel;

        private readonly ulong _tagId;

        private static Payload BuildPayload(ulong fieldTagId, Stream s) => new Payload(fieldTagId, s);

        private static Payload PayloadFromJson(object o) => new T().FromJson(o).AsPayload;
    }
}
