/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;

namespace InterlockLedger.Tags
{
    public interface ISymmetricEngine
    {
        byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv);

        byte[] Decrypt(byte[] cipherData, Func<MemoryStream, (byte[] key, byte[] iv)> readHeader);

        (byte[] cipherData, byte[] key, byte[] iv) Encrypt(byte[] clearData,
                                                           Action<MemoryStream, byte[], byte[]> writeHeader = null,
                                                           byte[] key = null,
                                                           byte[] iv = null);
    }
}