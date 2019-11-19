/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class SigningContext
    {
        public SigningContext(InterlockSigningKey key, ITimeStamper timeStamper) {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            TimeStamper = timeStamper ?? throw new ArgumentNullException(nameof(timeStamper));
        }

        public InterlockSigningKey Key { get; }
        public ITimeStamper TimeStamper { get; }
    }
}