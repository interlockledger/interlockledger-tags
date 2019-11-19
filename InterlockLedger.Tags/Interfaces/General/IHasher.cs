/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IHasher
    {
        HashAlgorithm DefaultHashAlgorithm { get; }

        TagHash Hash(byte[] data);

        TagHash Hash(byte[] data, HashAlgorithm hashAlgorithm);
    }
}