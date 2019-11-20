/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.IO;

namespace InterlockLedger.Tags
{
    public class AES256Encrypted<T> : AES256Engine where T : ILTag
    {
        public const string MissingPasswordMessage = "Missing the password";

        public AES256Encrypted(T value, string password) : this(value, password, null, null) {
        }

        public AES256Encrypted(Stream s) {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            _encrypted = s.Decode<TagEncrypted>();
            if (_encrypted.Algorithm != CipherAlgorithm.AES256)
                throw new InvalidDataException($"Not AES 256 encrypted!!! {_encrypted.Algorithm}");
        }

        public byte[] EncodedBytes => _encrypted.EncodedBytes;

        public T Decrypt(string password) {
            CheckMissingPassword(password);
            var decrypted = Decrypt(_encrypted.CipherData, (st) => ReadHeader(password, st));
            using var s = new MemoryStream(decrypted);
            return ILTag.DeserializeFrom(s) as T;
        }

        protected AES256Encrypted(T value, string password, byte[] key, byte[] iv) {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            CheckMissingPassword(password);
            if (password.Length < 6)
                throw new ArgumentException($"Password '{password}' is too weak!!!", nameof(password));
            (byte[] cipherData, _, _) = Encrypt(value.EncodedBytes, (s, _key, _iv) => WriteHeader(password, s, _key, _iv), key, iv);
            _encrypted = new TagEncrypted(CipherAlgorithm.AES256, cipherData);
        }

        private readonly TagEncrypted _encrypted;

        private static void CheckMissingPassword(string password) {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(MissingPasswordMessage, nameof(password));
        }

        private static byte[] KeyFumble(byte[] key, string password) {
            for (var i = 0; i < key.Length; i++) key[i] ^= (byte)password[i % password.Length];
            return key;
        }

        private static byte[] KeyUnfumble(byte[] key, string password) {
            for (var i = 0; i < key.Length; i++) key[i] ^= (byte)password[i % password.Length];
            return key;
        }

        private static (byte[] key, byte[] iv) ReadHeader(string password, MemoryStream s) {
            var iv = s.ReadExactly(16);
            var key = KeyUnfumble(s.ReadExactly(32), password);
            return (key, iv);
        }

        private static void WriteHeader(string password, MemoryStream s, byte[] key, byte[] iv)
            => s.WriteBytes(iv).WriteBytes(KeyFumble(key, password));
    }

    public class AESCipher : ISymmetricCipher
    {
        public byte[] Decrypt(byte[] ownerBytes, string composedPassword) {
            Check(ownerBytes);
            using var ms = new MemoryStream(ownerBytes);
            return new AES256Encrypted<ILTagByteArray>(ms).Decrypt(composedPassword).Value;
        }

        public byte[] Encrypt(byte[] ownerBytes, string composedPassword) {
            Check(ownerBytes);
            return new AES256Encrypted<ILTagByteArray>(new ILTagByteArray(ownerBytes), composedPassword).EncodedBytes;
        }

        private static void Check(byte[] ownerBytes) {
            if (ownerBytes is null)
                throw new ArgumentNullException(nameof(ownerBytes));
        }
    }
}