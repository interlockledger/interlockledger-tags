/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface ISymmetricCipher
    {
        byte[] Decrypt(byte[] ownerBytes, string composedPassword);

        byte[] Encrypt(byte[] ownerBytes, string composedPassword);
    }
}