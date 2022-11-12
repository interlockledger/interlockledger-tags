// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2021 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;
public static class ListOfClaimsExtensions
{
    public static List<Claim> AddCertificate(this List<Claim> claims, X509Certificate2 certificate) {
        if (claims != null && certificate != null) {
            claims.Add(new Claim(_publicKeyClaimType, TagPubKey.Resolve(certificate).TextualRepresentation));
            claims.Add(new Claim(_senderIdClaimType, KeyId.Resolve(certificate).TextualRepresentation));
            claims.Add(new Claim(_senderNameClaimType, certificate.FriendlyName));
        }
        return claims;
    }

    public static List<Claim> AddKey(this List<Claim> claims, InterlockKey key) {
        if (claims != null && key != null) {
            claims.Add(new Claim(_publicKeyClaimType, key.PublicKey.TextualRepresentation));
            claims.Add(new Claim(_senderIdClaimType, key.Id.TextualRepresentation));
            claims.Add(new Claim(_senderNameClaimType, key.Name));
        }
        return claims;
    }

    public static List<Claim> AddRoles(this List<Claim> claims, string[] roles) {
        if (claims != null && roles != null)
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        return claims;
    }

    internal static string Name(this IEnumerable<Claim> claims)
        => BuildFrom(ClaimValue(claims, _senderNameClaimType), textual => textual);

    internal static TagPubKey PublicKey(this IEnumerable<Claim> claims)
                => BuildFrom(ClaimValue(claims, _publicKeyClaimType), textual => TagPubKey.FromString(textual));

    internal static BaseKeyId Sender(this IEnumerable<Claim> claims)
        => BuildFrom(ClaimValue(claims, _senderIdClaimType), textual => InterlockId.FromString(textual) as BaseKeyId);

    private const string _publicKeyClaimType = "InterlockLedger.PublicKey";
    private const string _senderIdClaimType = "InterlockLedger.SenderId";
    private const string _senderNameClaimType = "InterlockLedger.SenderName";

    private static T BuildFrom<T>(string value, Func<string, T> build) where T : class => value == null ? null : build(value);

    private static string ClaimValue(IEnumerable<Claim> claims, string claimType) => claims?.FirstOrDefault(c => c.Type == claimType)?.Value;
}