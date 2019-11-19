/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IReader
    {
        string Id { get; }
        TagHash PublickKeyHash { get; }

        (byte[] key, byte[] iv) OpenKeyAndIV(byte[] encryptedKey, byte[] encryptedIV);
    }
}