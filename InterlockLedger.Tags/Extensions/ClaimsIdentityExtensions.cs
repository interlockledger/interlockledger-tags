/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Security.Claims;

namespace InterlockLedger.Tags
{
    public static class ClaimsIdentityExtensions
    {
        public static bool HasRole(this ClaimsIdentity identity, string roleName) => identity?.HasClaim(ClaimTypes.Role, roleName) ?? false;

        public static SenderIdentity Sender(this ClaimsIdentity identity) => new SenderIdentity(identity?.Claims);
    }
}