/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using System.IO;
using InterlockLedger.Tags;

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