/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public class EncryptedValue<T> : VersionedValue<EncryptedValue<T>> where T : ILTag
    {
        public const int CurrentVersion = 1;

        public EncryptedValue() : base(CurrentVersion) {
        }

        public CipherAlgorithm Cipher { get; }

        public byte[] CipherText { get; private set; }

        public IEnumerable<TagReadingKey> ReadingKeys { get; private set; }

        public T Decrypt(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) {
            byte[] clearText = DecryptRaw(reader, findEngine);
            return clearText is null ? null : ILTag.DeserializeFrom(clearText) as T;
        }

        public byte[] DecryptBlob(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) => (Decrypt(reader, findEngine) as ILTagByteArray)?.Value;

        public byte[] DecryptRaw(IReader reader, Func<CipherAlgorithm,ISymmetricEngine> findEngine) {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (findEngine is null)
                throw new ArgumentNullException(nameof(findEngine));
            var readingKey = ReadingKeys.FirstOrDefault(rk => rk.PublickKeyHash == reader.PublickKeyHash && rk.ReaderId == reader.Id);
            if (readingKey is null)
                return null;
            (byte[] key, byte[] iv) = reader.OpenKeyAndIV(readingKey.EncryptedKey, readingKey.EncryptedIV);
            return findEngine(Cipher)?.Decrypt(CipherText, key, iv);
        }

        protected EncryptedValue(CipherAlgorithm cipher, T payloadInClearText, ISigner author, IEnumerable<TagReader> readers) : base(CurrentVersion) {
            if (author is null)
                throw new ArgumentNullException(nameof(author));
            if (readers is null)
                throw new ArgumentNullException(nameof(readers));
            byte[] key;
            byte[] iv;
            (CipherText, key, iv) = author.Encrypt(cipher, payloadInClearText ?? throw new ArgumentNullException(nameof(payloadInClearText)));
            ReadingKeys = BuildReadingKeys(readers, key, iv, author.Id.TextualRepresentation, author.PublicKey);
            Cipher = cipher;
        }

        protected override IEnumerable<DataField> RemainingStateFields { get; } =
            new DataField(nameof(CipherText), ILTagId.ByteArray) { IsOpaque = true }
            .AppendedOf(new DataField(nameof(ReadingKeys), ILTagId.ILTagArray) { ElementTagId = ILTagId.ReadingKey });

        protected override ulong TagId => ILTagId.Encrypted;
        protected override string TypeDescription => $"EncryptedValueOf{typeof(T).Name}";
        protected override string TypeName => $"EncryptedValueOf{typeof(T).Name}";

        protected override void DecodeRemainingStateFrom(Stream s) {
            CipherText = s.DecodeByteArray();
            ReadingKeys = s.DecodeTagArray<TagReadingKey>();
        }

        protected override void EncodeRemainingStateTo(Stream s) {
            s.EncodeByteArray(CipherText);
            s.EncodeTagArray(ReadingKeys);
        }

        private static TagReadingKey BuildReadingKey(byte[] symmetricKey, byte[] IV, string id, TagPubKey publicKey)
            => new TagReadingKey(id, publicKey.Hash, publicKey.Encrypt(symmetricKey), publicKey.Encrypt(IV));

        private static TagReadingKey[] BuildReadingKeys(IEnumerable<TagReader> readers, byte[] symmetricKey, byte[] IV, string id, TagPubKey publicKey) {
            var readingKeys = new List<TagReadingKey> { BuildReadingKey(symmetricKey, IV, id, publicKey) };
            foreach (var reader in readers)
                readingKeys.Add(BuildReadingKey(symmetricKey, IV, reader.Id, reader.PublicKey));
            return readingKeys.ToArray();
        }
    }
}