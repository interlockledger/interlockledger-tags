/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;

namespace InterlockLedger.Tags
{
    public interface IUpdatableSigningKey : ISigningKey
    {
        BaseKeyId Identity { get; }
        DateTimeOffset LastSignatureTimeStamp { get; }
        TagPubKey NextPublicKey { get; }
        ulong SignaturesWithCurrentKey { get; }

        void GenerateNextKeys();

        TagSignature SignAndUpdate(byte[] data, Func<byte[], byte[]> encrypt = null);
    }
}