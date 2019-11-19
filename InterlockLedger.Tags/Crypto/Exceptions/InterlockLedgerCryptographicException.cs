/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;

namespace InterlockLedger.Tags
{
    public class InterlockLedgerCryptographicException : InterlockLedgerException
    {
        public InterlockLedgerCryptographicException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
