/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IInterlockUpdatableSigningKeyServices
    {
        IKeyParameters CreateKeyParameters(Algorithm algorithm, KeyStrength strength);

        InterlockUpdatableSigningKeyData CreateUsing(KeyPurpose[] purposes, IKeyParameters keyParameters, string name, string description, KeyStrength strength, string password);

        byte[] Encrypt(byte[] secret, string password);

        InterlockUpdatableSigningKey Open(InterlockUpdatableSigningKeyData key, string password);
    }
}
