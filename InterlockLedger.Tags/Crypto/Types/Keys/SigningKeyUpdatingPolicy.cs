/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;

namespace InterlockLedger.Tags
{
    public class SigningKeyUpdatingPolicy
    {
        public static readonly SigningKeyUpdatingPolicy Defaults = new SigningKeyUpdatingPolicy(
            maxSignaturesWithTheSameKey: 8,
            maxAgeOfSignatureKey: TimeSpan.FromDays(7)
        );

        public SigningKeyUpdatingPolicy(ulong maxSignaturesWithTheSameKey, TimeSpan maxAgeOfSignatureKey) {
            MaxSignaturesWithTheSameKey = maxSignaturesWithTheSameKey;
            MaxAgeOfSignatureKey = maxAgeOfSignatureKey;
        }

        public TimeSpan MaxAgeOfSignatureKey { get; private set; }
        public ulong MaxSignaturesWithTheSameKey { get; }
    }
}