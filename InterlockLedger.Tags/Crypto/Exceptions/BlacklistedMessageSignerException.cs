/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class BlacklistedMessageSignerException : InterlockLedgerException
    {
        public BlacklistedMessageSignerException(SenderIdentity senderIdentity) : base($"{senderIdentity} is blacklisted!") {
        }
    }
}