/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;

namespace InterlockLedger.Tags
{
    public class Payload<T> : ILTagExplicit<T>, IVersion, INamed where T : IRecordData<T>, new()
    {
        public Payload(ulong tagId, T jsonTestTaggedData) : base(tagId, jsonTestTaggedData) {
        }

        public Payload(ulong alreadyDeserializedTagId, Stream s) : base(alreadyDeserializedTagId, s) {
        }

        public override object AsJson => Value.AsJson;
        public string TypeName => typeof(T).Name;
        public ushort Version => Value.Version;

        public override string ToString() => Value.ToString();

        protected override T FromBytes(byte[] bytes)
           => FromBytesHelper(bytes, s => TryBuildFrom(() => new T().FromStream(s)));

        protected override byte[] ToBytes()
            => ToBytesHelper(s => Value.ToStream(s));

        private static TR TryBuildFrom<TR>(Func<TR> func) {
            try {
                return func();
            } catch (InvalidDataException e) {
                throw new InvalidDataException($"Not a properly encoded Payload of {typeof(T).Name}", e);
            }
        }
    }
}
