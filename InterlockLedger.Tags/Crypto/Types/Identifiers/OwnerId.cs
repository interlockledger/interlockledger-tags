/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class OwnerId : BaseKeyId
    {
        public const byte TypeId = 1;

        public OwnerId(string textualRepresentation) : base(textualRepresentation) => CheckType(TypeId);

        public OwnerId(TagHash hash) : base(TypeId, hash) { }

        public OwnerId(InterlockIdParts parts) : base(parts) => CheckType(TypeId);

        public string AsBase64 => Value.Data.ToSafeBase64();

        internal static void RegisterResolver() => RegisterResolver(TypeId, _typeName, (parts) => new OwnerId(parts));

        private const string _typeName = "Owner";
    }
}