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

namespace InterlockLedger.Tags
{
    public abstract class VersionedValue<T> : IVersion where T : VersionedValue<T>, new()
    {
        public Payload AsPayload => _payload.Value;
        public DataField FieldModel => _fieldModel.Value;
        public DataModel PayloadDataModel => _payloadDataModel.Value;
        public ushort Version { get; protected set; }

        public T FromStream(Stream s) {
            Version = s.DecodeUShort(); // Field index 0 //
            DecodeRemainingStateFrom(s);
            return (T)this;
        }

        public void ToStream(Stream s) {
            s.EncodeUShort(Version);    // Field index 0 //
            EncodeRemainingStateTo(s);
        }

        public class Payload : ILTagExplicit<T>, IVersion, INamed
        {
            public Payload(ulong alreadyDeserializedTagId, Stream s) : base(alreadyDeserializedTagId, s) => ValidateTagId(Value.TagId);

            public string TypeName => typeof(T).Name;
            public override object AsJson => Value.AsJson;
            public ushort Version => Value.Version;

            public T FromJson(object o) => new T().FromJson(o);

            internal Payload(T Value) : base(Value.TagId, Value) { }

            protected override T FromBytes(byte[] bytes) => FromBytesHelper(bytes, new T().FromStream);

            protected override byte[] ToBytes() => ToBytesHelper(Value.ToStream);
        }

        protected VersionedValue(ulong tagId, ushort version) {
            TagId = tagId;
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

        protected ulong TagId { get; }

        protected abstract string TypeDescription { get; }

        protected abstract string TypeName { get; }

        protected abstract void DecodeRemainingStateFrom(Stream s);

        protected abstract void EncodeRemainingStateTo(Stream s);

        protected abstract T FromJson(object json);

        private static readonly DataField _versionField = new DataField(nameof(Version), ILTagId.UInt16);
        private readonly Lazy<DataField> _fieldModel;
        private readonly Lazy<Payload> _payload;
        private readonly Lazy<DataModel> _payloadDataModel;
    }
}