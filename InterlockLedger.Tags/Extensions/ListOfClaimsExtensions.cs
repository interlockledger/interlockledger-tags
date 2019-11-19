/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags
{
    public static class ListOfClaimsExtensions
    {
        public static List<Claim> AddCertificate(this List<Claim> claims, X509Certificate2 certificate) {
            if (claims != null && certificate != null) {
                claims.Add(new Claim(_publicKeyClaimType, TagPubKey.Resolve(certificate).TextualRepresentation));
                claims.Add(new Claim(_senderIdClaimType, KeyId.Resolve(certificate).TextualRepresentation));
            }
            return claims;
        }

        public static List<Claim> AddKey(this List<Claim> claims, InterlockKey key) {
            if (claims != null && key != null) {
                claims.Add(new Claim(_publicKeyClaimType, key.PublicKey.TextualRepresentation));
                claims.Add(new Claim(_senderIdClaimType, key.Id.TextualRepresentation));
            }
            return claims;
        }

        public static List<Claim> AddRoles(this List<Claim> claims, string[] roles) {
            if (claims != null && roles != null)
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));
            return claims;
        }

        internal static TagPubKey PublicKey(this IEnumerable<Claim> claims)
            => BuildFrom(ClaimValue(claims, _publicKeyClaimType), textual => TagPubKey.Resolve(textual));

        internal static BaseKeyId Sender(this IEnumerable<Claim> claims)
            => BuildFrom(ClaimValue(claims, _senderIdClaimType), textual => InterlockId.Resolve(textual) as BaseKeyId);

        private const string _publicKeyClaimType = "InterlockLedger.PublicKey";
        private const string _senderIdClaimType = "InterlockLedger.SenderId";

        private static T BuildFrom<T>(string value, Func<string, T> build) where T : class => value == null ? null : build(value);

        private static string ClaimValue(IEnumerable<Claim> claims, string claimType) => claims?.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}