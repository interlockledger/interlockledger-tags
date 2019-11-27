/******************************************************************************************************************************
 
Copyright (c) 2018-2019 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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