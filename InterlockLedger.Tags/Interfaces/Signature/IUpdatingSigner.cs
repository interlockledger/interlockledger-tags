/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;

namespace InterlockLedger.Tags
{
    public interface IUpdatingSigner : ISigner
    {
        BaseKeyId Identity { get; }
        DateTimeOffset LastSignatureTimeStamp { get; }
        TagPubKey NextPublicKey { get; }
        ulong SignaturesWithCurrentKey { get; }

        void RegenerateKeys();
        IUpdatingSigner DestroyKeys();
    }
}