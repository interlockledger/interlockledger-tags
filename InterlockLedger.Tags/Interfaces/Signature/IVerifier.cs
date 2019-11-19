/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IVerifier
    {
        IEnumerable<Algorithm> SupportedAlgorithms { get; }

        bool Verify(byte[] dataToVerify, TagSignature signature);
    }
}