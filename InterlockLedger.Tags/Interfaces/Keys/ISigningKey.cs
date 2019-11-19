/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface ISigningKey
    {
        ulong AppId { get; }
        TagPubKey CurrentPublicKey { get; }
        string Description { get; }
        BaseKeyId Id { get; }
        string Name { get; }
        KeyPurpose[] Purposes { get; }
        Algorithm SignAlgorithm { get; }
        IEnumerable<ulong> SpecificActions { get; }
        KeyStrength Strength { get; }

        byte[] Decrypt(byte[] bytes);

        TagSignature Sign(byte[] data);

        string ToShortString();
    }
}