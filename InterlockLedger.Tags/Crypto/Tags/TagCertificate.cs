/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags
{
    public class TagCertificate : ILTagExplicit<X509Certificate2>
    {
        public TagCertificate(X509Certificate2 value) : base(ILTagId.Certificate, value) {
        }

        public TagCertificate(Stream s) : base(ILTagId.Certificate, s) {
        }

        protected override X509Certificate2 FromBytes(byte[] bytes) => new X509Certificate2(bytes);

        protected override byte[] ToBytes() => Value.RawData;
    }
}