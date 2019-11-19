/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags
{
    public class KeyId : BaseKeyId
    {
        public KeyId(string textualRepresentation) : base(textualRepresentation) => CheckType(_keyId);

        public KeyId(TagHash hash) : base(_keyId, hash) {
        }

        public KeyId(InterlockIdParts parts) : base(parts) => CheckType(_keyId);

        public static KeyId Resolve(X509Certificate2 certificate) => new KeyId(TagHash.HashFrom(certificate));

        internal static void RegisterResolver() => RegisterResolver(_keyId, _typeName, (parts) => new KeyId(parts));

        private const byte _keyId = 4;
        private const string _typeName = "Key";
    }
}