/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags
{
    public static class X509Certificate2Extensions
    {
        public static string FullName(this X509Certificate2 certificate) {
            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));
            return certificate.SubjectName.Format(false);
        }

        public static KeyStrength KeyStrengthGuess(this X509Certificate2 certificate) {
            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));
            return certificate.GetRSAPublicKey().KeyStrengthGuess();
        }

        public static TagPubKey PubKey(this X509Certificate2 certificate) {
            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));
            return new TagPubRSAKey(certificate.GetRSAPublicKey().ExportParameters(false));
        }

        public static string SimpleName(this X509Certificate2 certificate) {
            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));
            return certificate.FriendlyName.WithDefault(certificate.DottedName());
        }

        private static string DottedName(this X509Certificate2 certificate) {
            if (certificate is null)
                throw new ArgumentNullException(nameof(certificate));
            return certificate.SubjectName.Name.Split(',').Select(part => part.Split('=').Last()).Reverse().JoinedBy(".");
        }
    }
}