// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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

public sealed class TestFakeSigner : Owner, IUpdatingSigner, ITimeStamper, IHasher, IEncryptor
{
    public static readonly TestFakeSigner FixedKeysInstance = new(generateEmptySignatures: false);
    public static readonly TestFakeSigner Instance = new(generateEmptySignatures: true);
    public static readonly DateTimeOffset KnownTimeStamp = new(2017, 10, 25, 0, 0, 0, TimeSpan.Zero);

    public HashAlgorithm DefaultHashAlgorithm => HashAlgorithm.SHA256;

    public TagPubKey EmergencyPublicKey => PublicKey;
    BaseKeyId IUpdatingSigner.Identity => Id;
    public ISigningKey Key => _key;
    public DateTimeOffset LastSignatureTimeStamp => KnownTimeStamp;
    public TagPubKey NextPublicKey => PublicKey;
    public ulong Nonce => 13;
    public DateTimeOffset Now => KnownTimeStamp;
    public TagHash Session => TagHash.Empty;
    public ulong SignaturesWithCurrentKey => 14;
    public ITimeStamper TimeStamper => this;
    public IReader Reader { get; }
    public IUpdatingSigner DestroyKeys() => this;
    public override void Dispose() { }
    public (byte[] cypherText, byte[] key, byte[] iv) Encrypt<T>(CipherAlgorithm cipher, T clearText) where T : ILTag
            => cipher != CipherAlgorithm.AES256
            ? throw new InvalidOperationException("Only AES256 is valid for now")
            : new AES256Engine().Encrypt(clearText.OpenReadingStreamAsync().Result, key: _fakeCipherKey, iv: _fakeCipherIV);
    public (byte[] cypherText, byte[] key, byte[] iv) EncryptRaw(CipherAlgorithm cipher, byte[] clearText)
        => cipher != CipherAlgorithm.AES256
            ? throw new InvalidOperationException("Only AES256 is valid for now")
            : new AES256Engine().Encrypt(clearText, key: _fakeCipherKey, iv: _fakeCipherIV);
    public TagHash Hash(byte[] data, HashAlgorithm hashAlgorithm) => new(hashAlgorithm, []);
    public TagHash Hash(byte[] data) => Hash(data, DefaultHashAlgorithm);
    public void RegenerateKeys() {
        // For testing it is not needed
    }

    public TagSignature Sign(byte[] data, KeyPurpose purpose, ulong? appId = null)
        => _generateEmptySignatures ? new(Algorithm.RSA, []) : Key.Sign(data);

    public override TagSignature Sign(byte[] data) => Sign(data, KeyPurpose.Any);

    public override TagSignature Sign<T>(T data)
        => _generateEmptySignatures ? new(Algorithm.RSA, []) : Key.Sign(data);
    public IdentifiedSignature SignWithId(byte[] data)
        => _generateEmptySignatures ? new(new(Algorithm.RSA, []), (KeyId)Id, PublicKey) : Key.SignWithId(data);

    public void SwitchSession(SenderIdentity senderIdentity) {
        // Method intentionally left empty.
    }

    public TimeStampStatus Validate(DateTimeOffset timeStamp, SenderIdentity senderIdentity) => TimeStampStatus.OK;

    private static readonly byte[] _fakeCipherIV = new byte[16];
    private static readonly byte[] _fakeCipherKey = new byte[32];
    private readonly bool _generateEmptySignatures;

    private readonly TagRSAParameters _tagRSAParameters;
    private readonly RSAInterlockSigningKey _key;

    public static InterlockSigningKeyData DummyFor(TagPubKey pubKey, string name) =>
     new(
         [KeyPurpose.Protocol],
         _fakePermissions,
         name,
         [],
         pubKey);

    private static readonly AppPermissions[] _fakePermissions =
        Enumerable.Range(0, 101).Select(i => new AppPermissions((ulong)i)).ToArray();

    private TestFakeSigner(bool generateEmptySignatures) {
        Name = "Fake Owner";
        Id = new OwnerId(TagHash.Empty);
        _tagRSAParameters = TagRSAParameters.DecodeFromBytes(
                    [ 41, 249, 3, 157, 16, 248, 8, 210, 171, 20, 8, 209, 144, 192, 9, 196, 104, 106, 190, 94, 52, 111, 15,
                    93, 181, 35, 78, 249, 139, 59, 117, 204, 168, 76, 29, 221, 68, 104, 193, 112, 195, 136, 200, 168, 152, 173,
                    206, 212, 152, 74, 167, 40, 233, 91, 117, 166, 78, 170, 13, 25, 39, 225, 3, 75, 86, 166, 80, 23, 36, 205, 7,
                    87, 208, 216, 124, 115, 33, 203, 116, 61, 169, 57, 57, 64, 164, 225, 33, 174, 130, 53, 111, 210, 205, 26, 242,
                    132, 82, 133, 107, 23, 211, 107, 124, 199, 210, 196, 6, 145, 132, 96, 101, 142, 193, 30, 113, 67, 190, 223, 84,
                    11, 104, 199, 66, 189, 247, 132, 14, 14, 44, 199, 26, 81, 208, 36, 83, 188, 230, 233, 195, 250, 231, 219, 108,
                    76, 148, 12, 50, 122, 126, 216, 38, 32, 136, 126, 168, 248, 146, 164, 19, 214, 12, 7, 152, 83, 40, 78, 37, 19,
                    2, 246, 184, 180, 194, 157, 248, 87, 211, 200, 92, 198, 59, 194, 33, 223, 128, 86, 8, 57, 89, 94, 56, 41, 147,
                    134, 139, 164, 62, 69, 205, 133, 131, 179, 137, 114, 244, 161, 58, 216, 112, 3, 195, 126, 178, 250, 114, 185,
                    67, 151, 75, 159, 54, 99, 76, 55, 255, 222, 26, 215, 58, 204, 87, 69, 150, 101, 170, 250, 109, 32, 51, 151,
                    205, 99, 186, 198, 203, 170, 235, 37, 170, 21, 102, 35, 172, 194, 138, 101, 207, 75, 4, 176, 17, 92, 145, 16,
                    3, 1, 0, 1, 16, 128, 252, 237, 157, 200, 155, 249, 33, 68, 48, 113, 218, 21, 70, 176, 83, 20, 217, 90, 7, 66,
                    71, 205, 104, 148, 159, 133, 189, 194, 2, 112, 8, 111, 0, 197, 46, 30, 23, 98, 255, 206, 89, 144, 181, 137,
                    14, 210, 63, 73, 139, 230, 57, 152, 240, 177, 208, 239, 115, 168, 124, 215, 224, 241, 91, 167, 124, 10, 173,
                    19, 152, 97, 155, 68, 34, 235, 184, 62, 87, 21, 21, 195, 9, 147, 165, 173, 228, 191, 218, 38, 10, 59, 84, 165,
                    184, 162, 232, 128, 83, 207, 138, 157, 90, 78, 249, 230, 26, 219, 233, 169, 249, 103, 243, 18, 69, 174, 160,
                    188, 26, 80, 154, 4, 101, 9, 43, 59, 166, 78, 47, 179, 16, 128, 213, 58, 18, 34, 222, 91, 26, 84, 21, 237, 56,
                    184, 20, 42, 71, 44, 13, 136, 123, 14, 53, 107, 130, 204, 139, 45, 47, 143, 81, 42, 249, 232, 236, 163, 75,
                    242, 64, 223, 80, 7, 207, 126, 67, 55, 29, 34, 8, 168, 166, 131, 83, 179, 39, 100, 154, 222, 72, 87, 207, 27,
                    204, 94, 138, 94, 31, 96, 171, 117, 116, 239, 5, 48, 108, 9, 146, 209, 251, 176, 28, 197, 221, 61, 101, 145,
                    85, 48, 109, 39, 238, 129, 247, 213, 131, 225, 124, 186, 85, 9, 164, 68, 161, 172, 40, 233, 21, 24, 138, 112,
                    196, 3, 7, 189, 233, 101, 123, 193, 95, 109, 186, 15, 151, 196, 77, 207, 71, 10, 128, 171, 16, 128, 82, 163,
                    188, 171, 0, 51, 77, 40, 63, 127, 227, 150, 146, 11, 40, 138, 38, 94, 33, 3, 9, 252, 214, 79, 193, 51, 108,
                    133, 200, 80, 28, 161, 80, 42, 28, 224, 94, 25, 205, 164, 249, 100, 171, 187, 197, 104, 242, 158, 176, 36,
                    31, 235, 149, 177, 51, 168, 25, 45, 18, 229, 98, 44, 218, 26, 134, 15, 226, 239, 5, 25, 215, 38, 83, 22, 155,
                    147, 90, 214, 155, 206, 167, 1, 99, 223, 198, 94, 221, 3, 18, 210, 193, 220, 135, 208, 74, 145, 43, 81, 35,
                    100, 56, 78, 151, 158, 20, 102, 136, 25, 46, 81, 69, 125, 81, 225, 53, 201, 95, 251, 183, 230, 249, 176, 30,
                    61, 22, 32, 115, 187, 16, 128, 108, 51, 232, 202, 42, 254, 30, 49, 55, 99, 71, 26, 26, 153, 141, 190, 108, 43,
                    171, 14, 125, 203, 77, 247, 208, 84, 160, 194, 224, 148, 167, 119, 44, 198, 125, 30, 181, 14, 221, 132, 233,
                    37, 144, 164, 98, 51, 72, 35, 149, 68, 37, 112, 79, 120, 61, 34, 185, 161, 93, 167, 36, 161, 129, 35, 220, 86,
                    105, 11, 212, 200, 10, 97, 21, 34, 18, 144, 94, 97, 115, 104, 113, 41, 219, 229, 209, 78, 30, 198, 89, 193, 56,
                    107, 240, 93, 183, 182, 178, 186, 142, 210, 137, 28, 93, 50, 82, 147, 62, 133, 148, 226, 88, 198, 101, 175, 43,
                    10, 233, 11, 60, 148, 247, 22, 21, 202, 46, 169, 196, 187, 16, 128, 159, 124, 252, 45, 233, 145, 82, 87, 171,
                    46, 227, 136, 2, 250, 125, 232, 211, 58, 152, 153, 151, 65, 209, 61, 20, 166, 171, 195, 28, 27, 226, 37, 52, 86,
                    47, 149, 235, 159, 123, 52, 91, 221, 146, 135, 251, 11, 172, 62, 17, 86, 193, 183, 44, 75, 60, 64, 134, 105, 230,
                    131, 74, 85, 46, 229, 192, 83, 248, 55, 89, 82, 238, 120, 225, 3, 172, 29, 234, 124, 42, 133, 233, 247, 70, 44,
                    88, 113, 72, 160, 175, 47, 18, 137, 41, 173, 151, 121, 74, 126, 190, 3, 235, 55, 89, 155, 183, 130, 120, 138,
                    45, 118, 9, 223, 204, 196, 0, 226, 169, 237, 128, 46, 199, 94, 32, 180, 161, 204, 35, 202, 16, 248, 8, 187, 217,
                    60, 198, 24, 72, 32, 85, 21, 67, 158, 91, 29, 153, 87, 166, 182, 2, 60, 247, 192, 224, 124, 38, 176, 85, 41, 122,
                    63, 193, 46, 63, 191, 19, 60, 54, 224, 207, 51, 188, 136, 41, 59, 248, 14, 44, 247, 22, 211, 50, 181, 195, 116,
                    56, 241, 21, 132, 234, 64, 230, 141, 156, 141, 14, 13, 41, 246, 192, 17, 209, 130, 159, 166, 19, 237, 40, 182,
                    17, 207, 133, 191, 27, 240, 22, 68, 60, 11, 217, 80, 80, 93, 19, 127, 109, 80, 29, 53, 249, 36, 62, 79, 39, 96,
                    229, 110, 3, 153, 231, 153, 102, 235, 176, 255, 115, 112, 170, 124, 197, 230, 132, 185, 243, 7, 158, 218, 112,
                    228, 209, 254, 166, 129, 40, 34, 142, 128, 40, 9, 88, 215, 65, 106, 147, 131, 117, 144, 144, 7, 145, 198, 123,
                    220, 139, 196, 234, 112, 217, 250, 41, 189, 251, 177, 154, 45, 169, 218, 193, 181, 234, 25, 21, 249, 162, 68, 151,
                    147, 198, 75, 107, 83, 151, 29, 121, 154, 187, 119, 245, 158, 123, 190, 254, 19, 126, 41, 169, 138, 11, 178, 150,
                    40, 242, 166, 182, 189, 87, 86, 198, 114, 74, 119, 19, 24, 250, 60, 155, 229, 126, 50, 94, 99, 135, 147, 231, 45,
                    206, 127, 156, 224, 235, 28, 167, 252, 48, 185, 192, 224, 251, 45, 33, 128, 38, 170, 214, 83, 218, 243, 27, 222, 119,
                    203, 110, 138, 19, 81, 164, 102, 57 ]);
        _generateEmptySignatures = generateEmptySignatures;
        PublicKey = _tagRSAParameters.PublicKey;
        _key = new RSAInterlockSigningKey(DummyFor(PublicKey, Name), _tagRSAParameters);
        Reader = new ReaderWrapper(_key, Id.TextualRepresentation, PublicKey.Hash);
    }
}