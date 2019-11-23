/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
    public class AES256Engine : ISymmetricEngine
    {
        public byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv) {
            if (cipherData is null)
                throw new ArgumentNullException(nameof(cipherData));
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            return Decrypt(cipherData, readHeader: _ => (key, iv));
        }

        public byte[] Decrypt(byte[] cipherData, Func<MemoryStream, (byte[] key, byte[] iv)> readHeader) {
            if (cipherData is null)
                throw new ArgumentNullException(nameof(cipherData));
            if (readHeader is null)
                throw new ArgumentNullException(nameof(readHeader));
            using var source = new MemoryStream(cipherData);
            (byte[] key, byte[] iv) = readHeader(source);
            using var algorithm = new RijndaelManaged {
                KeySize = 256,
                BlockSize = 128,
                IV = iv,
                Key = key,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            using var cs = new CryptoStream(source, algorithm.CreateDecryptor(), CryptoStreamMode.Read);
            using var dest = new MemoryStream();
            cs.CopyTo(dest);
            return dest.ToArray();
        }

        public (byte[] cipherData, byte[] key, byte[] iv) Encrypt(byte[] clearData,
                                                                  Action<MemoryStream, byte[], byte[]> writeHeader = null,
                                                                  byte[] key = null,
                                                                  byte[] iv = null) {
            using var algorithm = BuildAlgorithm(key, iv);
            if (clearData is null)
                throw new ArgumentNullException(nameof(clearData));
            using var ms = new MemoryStream();
            writeHeader?.Invoke(ms, algorithm.Key, algorithm.IV);
            using (var cs = new CryptoStream(ms, algorithm.CreateEncryptor(), CryptoStreamMode.Write)) {
                cs.Write(clearData, 0, clearData.Length);
                cs.Close();
            }
            return (ms.ToArray(), algorithm.Key, algorithm.IV);
        }

        private SymmetricAlgorithm BuildAlgorithm(byte[] key, byte[] iv) {
            var AES = new RijndaelManaged {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            if (iv == null || iv.Length != 16)
                AES.GenerateIV();
            else
                AES.IV = iv;
            if (key == null || key.Length != 32)
                AES.GenerateKey();
            else
                AES.Key = key;
            return AES;
        }
    }
}