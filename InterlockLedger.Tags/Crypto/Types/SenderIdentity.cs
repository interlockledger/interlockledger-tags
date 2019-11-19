/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using System.Security.Claims;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class SenderIdentity
    {
        public readonly BaseKeyId Id;
        public readonly TagPubKey PublicKey;

        public SenderIdentity(BaseKeyId id, TagPubKey publicKey) {
            Id = id;
            PublicKey = publicKey;
        }

        public SenderIdentity(IEnumerable<Claim> claims) : this(claims.Sender(), claims.PublicKey()) { }
        public override string ToString() => $"Sender {Id} with public key {PublicKey}";
    }
}