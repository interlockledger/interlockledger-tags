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

namespace InterlockLedger.Tags;
public class AES256Encrypted<T> : AES256Engine where T : ILTag
{
    public const string MissingPasswordMessage = "Missing the password";

    public AES256Encrypted(T value, string password) : this(value, password, null, null) {
    }

    public AES256Encrypted(Stream s) {
        _encrypted = s.Required().Decode<TagEncrypted>();
        if (_encrypted.Algorithm != CipherAlgorithm.AES256)
            throw new InvalidDataException($"Not AES 256 encrypted!!! {_encrypted.Algorithm}");
    }

    public byte[] EncodedBytes => _encrypted.EncodedBytes;

    public T Decrypt(string password) {
        CheckMissingPassword(password);
        var decrypted = Decrypt(_encrypted.CipherData, (st) => ReadHeader(password, st));
        using var s = new MemoryStream(decrypted);
        return TagProvider.DeserializeFrom(s) as T;
    }

    protected AES256Encrypted(T value, string password, byte[] key, byte[] iv) {
        value.Required();
        CheckMissingPassword(password);
        if (password.Length < 6)
            throw new ArgumentException($"Password '{password}' is too weak!!!", nameof(password));
        (byte[] cipherData, _, _) = Encrypt(value.OpenReadingStreamAsync().Result, key, iv, (s, _key, _iv) => WriteHeader(password, s, _key, _iv));
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
        using var ms = new MemoryStream(ownerBytes.Required());
        return new AES256Encrypted<ILTagByteArray>(ms).Decrypt(composedPassword).Value;
    }

    public byte[] Encrypt(byte[] ownerBytes, string composedPassword) =>
        new AES256Encrypted<ILTagByteArray>(new ILTagByteArray(ownerBytes.Required()), composedPassword).EncodedBytes;
}