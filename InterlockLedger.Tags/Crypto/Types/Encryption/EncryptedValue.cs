// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2021 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public class EncryptedValue<T> : IVersionedEmbeddedValue<EncryptedValue<T>> where T : ILTag
    {
        public const int CurrentVersion = 1;

        public EncryptedValue(ulong tagId) => TagId = tagId;

        public EncryptedValue(ulong tagId, CipherAlgorithm cipher, IEncryptor encryptor, T payloadInClearText, IIdentifiedPublicKey author, IEnumerable<TagReader> readers) : this(tagId) {
            if (author is null)
                throw new ArgumentNullException(nameof(author));
            if (readers is null)
                throw new ArgumentNullException(nameof(readers));
            byte[] key;
            byte[] iv;
            (CipherText, key, iv) = encryptor.Encrypt(cipher, payloadInClearText.Required(nameof(payloadInClearText)));
            ReadingKeys = BuildReadingKeys(readers, key, iv, author.Identifier, author.PublicKey);
            Cipher = cipher;
        }

        public object AsJson => new { TagId, Version, Cipher, CipherText, ReadingKeys = ReadingKeys.AsJsonArray() };
        public CipherAlgorithm Cipher { get; private set; }
        public byte[] CipherText { get; private set; }
        public IEnumerable<TagReadingKey> ReadingKeys { get; private set; }

        public IEnumerable<DataField> RemainingStateFields { get; } =
            new DataField(nameof(CipherText), ILTagId.ByteArray) { IsOpaque = true }
                .AppendedOf(new DataField(nameof(ReadingKeys), ILTagId.ILTagArray) { ElementTagId = ILTagId.ReadingKey });

        public ulong TagId { get; }
        public string TypeDescription => $"EncryptedValueOf{typeof(T).Name}";
        public string TypeName => $"EncryptedValueOf{typeof(T).Name}";
        public ushort Version { get; set; }

        public void DecodeRemainingStateFrom(Stream s) {
            CipherText = s.DecodeByteArray();
            ReadingKeys = s.DecodeTagArray<TagReadingKey>();
        }

        public T Decrypt(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) {
            byte[] clearText = DecryptRaw(reader, findEngine);
            return clearText is null ? null : TagProvider.DeserializeFrom(clearText) as T;
        }

        public byte[] DecryptRaw(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (findEngine is null)
                throw new ArgumentNullException(nameof(findEngine));
            var readingKey = ReadingKeys.FirstOrDefault(rk => rk.PublicKeyHash == reader.PublicKeyHash && rk.ReaderId == reader.Id);
            if (readingKey is null)
                return null;
            (byte[] key, byte[] iv) = reader.OpenKeyAndIV(readingKey.EncryptedKey, readingKey.EncryptedIV);
            return findEngine(Cipher)?.Decrypt(CipherText, key, iv);
        }

        public void EncodeRemainingStateTo(Stream s) => s.EncodeByteArray(CipherText).EncodeTagArray(ReadingKeys);

        public EncryptedValue<T> FromJson(object o) => new(TagId) {
            Cipher = CipherAlgorithm.AES256, // TODO deserialize from o
        };

        private static TagReadingKey BuildReadingKey(byte[] symmetricKey, byte[] IV, string id, TagPubKey publicKey)
            => new(id, publicKey.Hash, publicKey.Encrypt(symmetricKey), publicKey.Encrypt(IV));

        private static TagReadingKey[] BuildReadingKeys(IEnumerable<TagReader> readers, byte[] symmetricKey, byte[] IV, string id, TagPubKey publicKey) {
            var readingKeys = new List<TagReadingKey> { BuildReadingKey(symmetricKey, IV, id, publicKey) };
            foreach (var reader in readers)
                readingKeys.Add(BuildReadingKey(symmetricKey, IV, reader.Name, reader.PublicKey));
            return readingKeys.ToArray();
        }
    }
}