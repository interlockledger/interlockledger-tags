/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IInterlockSigningKeyServices
    {
        IKeyParameters CreateKeyParameters(Algorithm algorithm, KeyStrength strength);

        InterlockSigningKeyData CreateUsing(KeyPurpose[] purposes, ulong appId, IEnumerable<ulong> actionIds, IKeyParameters keyParameters, string name, string description, KeyStrength strength, string password);

        InterlockSigningKeyData CreateUsing(KeyPurpose[] purposes, ulong appId, IEnumerable<ulong> actionIds, byte[] certificateBytes, string password);

        InterlockSigningKey Open(InterlockSigningKeyData key, string password);

        bool SupportsAlgorithm(Algorithm algorithm);
    }
}
