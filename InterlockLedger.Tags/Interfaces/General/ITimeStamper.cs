/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public enum TimeStampStatus
    {
        OK,
        TooOld,
        AheadOfTime
    }

    public interface ITimeStamper
    {
        ulong Nonce { get; }
        DateTimeOffset Now { get; }
        TagHash Session { get; }

        void SwitchSession(SenderIdentity senderIdentity);

        TimeStampStatus Validate(DateTimeOffset timeStamp, SenderIdentity senderIdentity);
    }
}