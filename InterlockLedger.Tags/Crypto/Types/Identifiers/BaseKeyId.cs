/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [JsonConverter(typeof(JsonInterlockIdConverter))]
    public class BaseKeyId : InterlockId
    {
        public static BaseKeyId OptionalResolve(Stream s) {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            return s.Position < s.Length ? Resolve(s) as BaseKeyId : null;
        }

        public static void RegisterKeyIdTypes() {
            OwnerId.RegisterResolver();
            KeyId.RegisterResolver();
        }

        protected BaseKeyId(string textualRepresentation) : base(textualRepresentation) {
        }

        protected BaseKeyId(byte type, TagHash hash) : base(type, hash?.Algorithm ?? throw new ArgumentNullException(nameof(hash)), hash.Data) {
        }

        protected BaseKeyId(InterlockIdParts parts) : base(parts) {
        }
    }
}