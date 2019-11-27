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

using System.Collections.Generic;
using System.IO;

namespace InterlockLedger.Tags
{
    public abstract class VersionedValue<T> : IVersion where T : VersionedValue<T>, new()
    {
        public Payload AsPayload => new Payload((T)this);

        public DataField FieldModel => new DataField(TypeName, TagId, TypeDescription) {
            SubDataFields = _versionField.AppendedOf(RemainingStateFields)
        };

        public ushort Version { get; private set; }

        public T FromStream(Stream s) {
            Version = s.DecodeUShort(); // Field index 0 //
            DecodeRemainingStateFrom(s);
            return (T)this;
        }

        public void ToStream(Stream s) {
            s.EncodeUShort(Version);              // Field index 0 //
            EncodeRemainingStateTo(s);
        }

        public class Payload : ILTagExplicit<T>, IVersion, INamed
        {
            public Payload(ulong alreadyDeserializedTagId, Stream s) : base(alreadyDeserializedTagId, s) {
            }

            public string TypeName => typeof(T).Name;
            public ushort Version => Value.Version;

            internal Payload(T Value) : base(Value.TagId, Value) {
            }

            protected override T FromBytes(byte[] bytes)
                => FromBytesHelper(bytes, new T().FromStream);

            protected override byte[] ToBytes()
                => ToBytesHelper(Value.ToStream);
        }

        protected VersionedValue(ushort version) => Version = version;

        protected abstract IEnumerable<DataField> RemainingStateFields { get; }
        protected abstract ulong TagId { get; }
        protected abstract string TypeDescription { get; }
        protected abstract string TypeName { get; }

        protected abstract void DecodeRemainingStateFrom(Stream s);

        protected abstract void EncodeRemainingStateTo(Stream s);

        private static readonly DataField _versionField = new DataField(nameof(Version), ILTagId.UInt16);
    }
}