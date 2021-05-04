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
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class EncryptedTextTests
    {
        [TestCase(new byte[] {
            57, 249, 1, 131,
                5, 1, 0,
                16, 16, 45, 38, 94, 51, 98, 67, 115, 113, 145, 151, 44, 131, 220, 188, 143, 165,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49, 79, 119, 110, 101, 114, 33, 52, 55, 68, 69, 81, 112, 106, 56, 72, 66, 83, 97, 45, 95, 84, 73,
                            109, 87, 45, 53, 74, 67, 101, 117, 81, 101, 82, 107, 109, 53, 78, 77, 112, 74, 87, 90, 71, 51, 104, 83, 117, 70, 85,
                        39, 34, 1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36, 106, 221, 140, 204, 78,
                            212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8,
                            82, 16, 14, 36, 39, 24, 53, 6, 26, 100, 251, 115, 71, 218, 232, 246, 143, 199, 224, 217, 227, 220, 175, 79, 98, 146, 6,
                            176, 225, 30, 251, 172, 1, 119, 247, 216, 230, 219, 81, 160, 189, 163, 82, 184, 233, 13, 130, 112, 53, 32, 215, 47, 223,
                            222, 1, 61, 91, 24, 22, 73, 227, 62, 203, 150, 150, 250, 127, 202, 78, 242, 147, 8, 34, 108, 113, 75, 75, 42, 33, 184,
                            125, 64, 139, 121, 110, 220, 54, 46, 105, 194, 78, 46, 188, 222, 134, 222, 200, 28, 160, 92, 144, 217, 242, 118, 207, 76,
                            46, 240, 238, 252, 95, 164, 150, 227, 79, 176, 90, 229, 68, 32, 118, 82, 249, 231, 234, 216, 148, 229, 238, 204,
                            171, 8, 56, 121, 164, 110, 188, 27, 139, 23, 5, 25, 177, 130, 248, 175, 85, 206, 214, 74, 1, 177, 128, 75, 197,
                            187, 108, 6, 117, 3, 25, 148, 99, 212, 112, 77, 173, 33, 198, 154, 239, 57, 34, 59, 103, 178, 72, 211, 31, 215, 115,
                            150, 174, 220, 87, 245, 249, 141, 169, 55, 1, 213, 145, 140, 210, 50, 81, 139, 114, 226, 184, 73, 112, 69, 218, 171,
                            205, 150, 252, 123, 165, 117, 107, 218, 163, 15, 104, 238, 214, 115, 138, 141, 56, 127, 136, 227, 71, 23, 185, 148,
                            197, 13, 26, 133, 37, 220, 214, 182, 107, 62, 249, 138, 214, 183, 57, 174, 164, 59, 183, 41, 171, 123, 205, 240, 120, 172,
                        16, 248, 8,
                            167, 131, 17, 154, 49, 165, 169, 53, 94, 152, 236, 2, 140, 238, 107, 62, 81, 228, 61, 71, 61, 77, 66, 142, 147,
                            22, 241, 216, 109, 219, 7, 84, 35, 160, 42, 99, 153, 186, 109, 155, 248, 68, 199, 23, 151, 189, 119, 32, 140,
                            189, 220, 25, 37, 221, 83, 232, 181, 115, 205, 37, 159, 11, 173, 54, 25, 10, 219, 11, 241, 127, 184, 231, 150,
                            64, 226, 212, 112, 140, 30, 78, 186, 5, 62, 28, 177, 92, 159, 162, 78, 57, 20, 253, 48, 8, 182, 143, 191, 65,
                            11, 130, 98, 31, 112, 155, 177, 111, 115, 125, 187, 91, 77, 86, 108, 241, 182, 90, 176, 186, 13, 76, 227, 120,
                            205, 1, 47, 184, 85, 207, 84, 177, 171, 134, 69, 62, 66, 149, 83, 217, 19, 29, 196, 99, 118, 45, 36, 69, 222,
                            86, 220, 123, 114, 105, 109, 185, 138, 224, 98, 156, 146, 15, 157, 88, 226, 222, 77, 223, 59, 88, 241, 246, 191,
                            138, 178, 156, 205, 114, 215, 122, 105, 164, 191, 224, 21, 142, 226, 74, 97, 55, 158, 67, 59, 139, 65, 123, 109,
                            206, 42, 122, 163, 132, 8, 249, 205, 20, 121, 23, 152, 80, 142, 218, 245, 237, 68, 61, 99, 113, 241, 135, 245,
                            252, 230, 176, 244, 81, 126, 160, 231, 251, 248, 130, 223, 122, 126, 148, 223, 56, 217, 99, 115, 62, 150, 209,
                            158, 220, 134, 149, 146, 236, 238, 240, 107, 229, 28, 121, 197, 201
            }, CipherAlgorithm.AES256, "Hello Test", TestName = "NewEncryptedTextFromStream")]
        [TestCase(new byte[] {
            57, 249, 1, 131,
                5, 1, 0,
                16, 16, 214, 3, 127, 106, 43, 231, 37, 192, 161, 148, 20, 105, 50, 187, 229, 188,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49, 79, 119, 110, 101, 114, 33, 52, 55, 68, 69, 81, 112, 106, 56, 72, 66, 83, 97, 45, 95, 84, 73,
                            109, 87, 45, 53, 74, 67, 101, 117, 81, 101, 82, 107, 109, 53, 78, 77, 112, 74, 87, 90, 71, 51, 104, 83, 117,
                            70, 85,
                        39, 34, 1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36, 106, 221,
                            140, 204, 78, 212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8,
                            148, 212, 141, 12, 157, 190, 19, 118, 241, 157, 136, 164, 57, 75, 15, 213, 64, 58, 184, 216, 220, 228, 192, 144, 83, 131, 206, 255, 167, 235, 32, 129, 220, 150, 218, 112, 152, 107, 64, 96, 185, 106, 226, 193, 76, 234, 34, 247, 68, 203, 2, 203, 201, 224, 143, 209, 106, 128, 30, 218, 14, 23, 136, 15, 222, 243, 11, 19, 89, 11, 138, 173, 105, 174, 234, 99, 185, 250, 1, 161, 88, 194, 42, 66, 160, 73, 35, 234, 115, 95, 183, 192, 18, 249, 234, 106, 27, 238, 56, 109, 164, 237, 126, 214, 125, 97, 249, 134, 75, 138, 44, 114, 39, 143, 250, 96, 8, 48, 116, 23, 119, 43, 216, 170, 53, 63, 243, 242, 12, 197, 117, 162, 189, 103, 189, 105, 34, 234, 198, 194, 141, 107, 249, 185, 247, 236, 232, 106, 212, 66, 109, 178, 85, 143, 162, 208, 156, 103, 231, 126, 94, 77, 41, 114, 233, 125, 15, 67, 165, 120, 145, 237, 58, 100, 171, 140, 100, 217, 55, 80, 106, 133, 77, 137, 99, 91, 188, 32, 185, 88, 237, 84, 147, 43, 184, 225, 164, 36, 45, 143, 158, 159, 8, 234, 119, 207, 141, 219, 142, 240, 93, 153, 60, 216, 241, 241, 10, 73, 80, 248, 84, 238, 119, 198, 99, 147, 47, 107, 152, 145, 67, 121, 245, 120, 233, 242, 189, 137, 93, 115, 119, 174, 55, 124, 19, 95, 171, 244, 190, 204, 83, 31, 53, 3, 231, 194,
                        16, 248, 8,
                            108, 206, 164, 236, 29, 84, 185, 160, 54, 209, 74, 2, 144, 222, 121, 161, 249, 96, 38, 111, 138, 223, 157, 247, 170, 23, 124, 189, 230, 182, 200, 65, 3, 101, 145, 214, 165, 53, 230, 55, 46, 149, 175, 82, 98, 230, 144, 169, 48, 205, 240, 103, 85, 154, 144, 3, 209, 66, 124, 250, 8, 21, 201, 167, 106, 118, 163, 18, 190, 218, 156, 200, 107, 71, 152, 30, 78, 105, 65, 169, 127, 245, 231, 79, 4, 85, 79, 1, 10, 24, 138, 21, 48, 15, 185, 90, 225, 36, 212, 126, 76, 159, 166, 156, 111, 124, 56, 82, 136, 197, 94, 133, 154, 117, 23, 50, 116, 93, 184, 190, 112, 159, 180, 11, 205, 246, 219, 96, 83, 169, 97, 73, 15, 45, 21, 72, 25, 234, 147, 145, 29, 55, 16, 204, 189, 177, 6, 61, 133, 58, 195, 174, 159, 94, 44, 189, 56, 177, 244, 139, 230, 52, 206, 213, 221, 183, 152, 127, 0, 1, 176, 75, 208, 180, 202, 221, 98, 95, 204, 216, 196, 37, 216, 11, 177, 103, 82, 109, 223, 53, 198, 175, 201, 252, 48, 162, 54, 131, 137, 208, 98, 196, 30, 56, 149, 133, 124, 35, 241, 121, 109, 148, 20, 94, 178, 50, 147, 203, 251, 89, 54, 4, 139, 169, 181, 64, 234, 76, 225, 104, 232, 157, 128, 37, 127, 253, 125, 171, 191, 222, 241, 204, 186, 79, 7, 233, 222, 217, 38, 15, 95, 35, 111, 4, 60, 57
        }, CipherAlgorithm.AES256, "Hello Test 00", TestName = "NewEncryptedTextFromStreamFromZeroes")]
        public void NewEncryptedTextFromStream(byte[] bytes, CipherAlgorithm algorithm, string clearText) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<EncryptedText.Payload>();
            Assert.AreEqual(ILTagId.EncryptedText, tag.TagId);
            Assert.AreEqual(algorithm, tag.Value.Cipher);
            var clearBlob = tag.Value.DecryptText(TestFakeSigner.FixedKeysInstance, _ => new AES256Engine());
            Assert.IsNotNull(clearBlob);
            Assert.AreEqual(clearText.Length, clearBlob.Length);
        }

        [TestCase(CipherAlgorithm.AES256, "Hello Test 00", ExpectedResult = new byte[] {
            57, 249, 1, 131,
                5, 1, 0,
                16, 16, 214, 3, 127, 106, 43, 231, 37, 192, 161, 148, 20, 105, 50, 187, 229, 188,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49, 79, 119, 110, 101, 114, 33, 52, 55, 68, 69, 81, 112, 106, 56, 72, 66, 83, 97, 45, 95, 84, 73,
                            109, 87, 45, 53, 74, 67, 101, 117, 81, 101, 82, 107, 109, 53, 78, 77, 112, 74, 87, 90, 71, 51, 104, 83, 117,
                            70, 85,
                        39, 34, 1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36, 106, 221,
                            140, 204, 78, 212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8,
        }, TestName = "SerializeEncryptedTextAsZeroes")]
        [TestCase(CipherAlgorithm.AES256, "Hello Test", ExpectedResult = new byte[] {
            57, 249, 1, 131,
                5, 1, 0,
                16, 16, 45, 38, 94, 51, 98, 67, 115, 113, 145, 151, 44, 131, 220, 188, 143, 165,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49, 79, 119, 110, 101, 114, 33, 52, 55, 68, 69, 81, 112, 106, 56, 72, 66, 83, 97, 45, 95, 84, 73,
                            109, 87, 45, 53, 74, 67, 101, 117, 81, 101, 82, 107, 109, 53, 78, 77, 112, 74, 87, 90, 71, 51, 104, 83, 117, 70, 85,
                        39, 34, 1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36,
                            106, 221, 140, 204, 78, 212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8,
        }, TestName = "SerializeEncryptedText")]
        public byte[] SerializeEncryptedText(CipherAlgorithm algorithm, string clearText) {
            var encodedBytes = new EncryptedText(algorithm,
                                                 clearText,
                                                 TestFakeSigner.FixedKeysInstance,
                                                 TestFakeSigner.FixedKeysInstance,
                                                 Array.Empty<TagReader>()).AsPayload.ToEncodedBytes();
            TestContext.WriteLine(encodedBytes.AsLiteral());
            return encodedBytes.PartOf(124);
        }

        [Test]
        public void ValidateFieldDefinition() {
            var fd = EncryptedText.FieldDefinition;
            Assert.IsNotNull(fd);
            Assert.AreEqual(nameof(EncryptedText), fd.Name);
            Assert.AreEqual(ILTagId.EncryptedText, fd.TagId);
            Assert.IsTrue(fd.HasSubFields, "Must have subfields");
            Assert.IsFalse(fd.IsEnumeration, "Should not be an enumeration");
            Assert.AreEqual(3, fd.SubDataFields.SafeCount());
            var fieldVersion = fd.SubDataFields.First();
            Assert.AreEqual(nameof(EncryptedText.Version), fieldVersion.Name);
            Assert.AreEqual(ILTagId.UInt16, fieldVersion.TagId);
            Assert.AreEqual((ushort)1, fieldVersion.Version);
            Assert.IsTrue(fieldVersion.IsVersion);
            Assert.IsFalse(fieldVersion.IsDeprecated.GetValueOrDefault());
            Assert.IsFalse(fieldVersion.IsOpaque.GetValueOrDefault());
            var fieldCipherText = fd.SubDataFields.Skip(1).First();
            Assert.AreEqual(nameof(EncryptedText.CipherText), fieldCipherText.Name);
            Assert.AreEqual(ILTagId.ByteArray, fieldCipherText.TagId);
            Assert.AreEqual((ushort)1, fieldCipherText.Version);
            Assert.IsFalse(fieldCipherText.IsVersion);
            Assert.IsFalse(fieldCipherText.IsDeprecated.GetValueOrDefault());
            Assert.IsTrue(fieldCipherText.IsOpaque.GetValueOrDefault());
            var fieldKeys = fd.SubDataFields.Skip(2).First();
            Assert.AreEqual(nameof(EncryptedText.ReadingKeys), fieldKeys.Name);
            Assert.AreEqual(ILTagId.ILTagArray, fieldKeys.TagId);
            Assert.AreEqual((ushort)1, fieldKeys.Version);
            Assert.IsFalse(fieldKeys.IsVersion);
            Assert.IsFalse(fieldKeys.IsDeprecated.GetValueOrDefault());
            Assert.IsFalse(fieldKeys.IsOpaque.GetValueOrDefault());
            Assert.AreEqual(ILTagId.ReadingKey, fieldKeys.ElementTagId.GetValueOrDefault());
        }
    }
}