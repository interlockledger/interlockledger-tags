// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
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

namespace InterlockLedger.Tags;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class EncryptedValue<T>(ulong tagId) : IVersionedEmbeddedValue<EncryptedValue<T>> where T : ILTag
{
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public const int CurrentVersion = 1;

    public EncryptedValue(ulong tagId, CipherAlgorithm cipher, IEncryptor encryptor, T payloadInClearText, IIdentifiedPublicKey author, IEnumerable<TagReader> readers) : this(tagId) {
        author.Required();
        readers.Required();
        byte[] key;
        byte[] iv;
        (CipherText, key, iv) = encryptor.Encrypt(cipher, payloadInClearText.Required());
        ReadingKeys = BuildReadingKeys(readers, key, iv, author.Identifier, author.PublicKey);
        Cipher = cipher;
    }

    public CipherAlgorithm Cipher { get; private set; }
    public byte[] CipherText { get; private set; }
    public IEnumerable<TagReadingKey> ReadingKeys { get; private set; }

    public IEnumerable<DataField> RemainingStateFields { get; } =
        new DataField(nameof(CipherText), ILTagId.ByteArray) { IsOpaque = true }
            .AppendedOf(new DataField(nameof(ReadingKeys), ILTagId.ILTagArray) { ElementTagId = ILTagId.ReadingKey });

    public ulong TagId { get; } = tagId;
    public string TypeDescription => $"EncryptedValueOf{typeof(T).Name}";
    public string TypeName => $"EncryptedValueOf{typeof(T).Name}";
    public ushort Version { get; set; }

    public void DecodeRemainingStateFrom(Stream s) {
        CipherText = s.DecodeByteArray().Required();
        ReadingKeys = s.DecodeTagArray<TagReadingKey>().RequireNonNulls().NonEmpty();
    }

    public T Decrypt(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) {
        byte[] clearText = DecryptRaw(reader, findEngine).Required();
        return (TagProvider.DeserializeFrom(clearText) as T).Required();
    }

    public byte[] DecryptRaw(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) {
        reader.Required();
        findEngine.Required();
        foreach (var readingKey in ReadingKeys.Safe()) {
            if (readingKey.PublicKeyHash.Equals(reader.PublicKeyHash) && readingKey.ReaderId == reader.Id) {
                (byte[] key, byte[] iv) = reader.OpenKeyAndIV(readingKey.EncryptedKey, readingKey.EncryptedIV);
                return findEngine(Cipher).Decrypt(CipherText, key, iv);
            }
        }
        throw new InvalidOperationException($"Reader {reader.Id} does not match any reading key to be able to decrypt this content");
    }

    public void EncodeRemainingStateTo(Stream s) => s.EncodeByteArray(CipherText).EncodeTagArray(ReadingKeys);

    private static TagReadingKey BuildReadingKey(byte[] symmetricKey, byte[] IV, string id, TagPubKey publicKey)
        => new(id, publicKey.Hash, publicKey.Encrypt(symmetricKey), publicKey.Encrypt(IV));

    private static TagReadingKey[] BuildReadingKeys(IEnumerable<TagReader> readers, byte[] symmetricKey, byte[] IV, string id, TagPubKey publicKey) {
        var readingKeys = new List<TagReadingKey> { BuildReadingKey(symmetricKey, IV, id, publicKey) };
        foreach (var reader in readers)
            readingKeys.Add(BuildReadingKey(symmetricKey, IV, reader.Name, reader.PublicKey));
        return [.. readingKeys];
    }
}