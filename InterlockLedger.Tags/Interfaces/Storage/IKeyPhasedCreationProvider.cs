/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IKeyPhasedCreationProvider : IKeyFileExporter
    {
        IKeyParameters CreateKeyParameters(Algorithm algorithm, KeyStrength strength);

        InterlockSigningKeyData CreateUsing(IKeyParameters emergencyKeyParameters, KeyStrength strength, BaseKeyId identity, string name, string description, string password, ulong appId, IEnumerable<ulong> actionIds, params KeyPurpose[] purposes);
    }
}