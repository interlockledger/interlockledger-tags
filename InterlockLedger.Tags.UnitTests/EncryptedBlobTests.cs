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
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
#pragma warning disable CA1062 // Validate arguments of public methods

    [TestFixture]
    public class EncryptedBlobTests
    {
        [TestCase(new byte[] {
            55, 249, 1, 131,
                5, 1, 0,
                16, 16, 228, 155, 155, 36, 5, 133, 124, 106, 244, 135, 239, 56, 50, 213, 198, 74,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49, 79, 119, 110, 101, 114, 33, 52, 55, 68, 69, 81, 112, 106, 56, 72, 66, 83, 97, 45, 95, 84, 73,
                            109, 87, 45, 53, 74, 67, 101, 117, 81, 101, 82, 107, 109, 53, 78, 77, 112, 74, 87, 90, 71, 51, 104, 83, 117, 70, 85,
                        39, 34, 1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36,
                            106, 221, 140, 204, 78, 212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8,
                            163, 195, 126, 95, 1, 176, 176, 240, 7, 241, 176, 236, 147, 41, 208, 95, 46, 72, 19, 80, 39, 250, 154, 192, 85, 182, 70, 110, 72,
                            104, 211, 193, 2, 153, 29, 165, 190, 20, 123, 43, 109, 12, 158, 104, 150, 177, 9, 125, 55, 5, 88, 90, 178, 45, 20, 220, 179, 143,
                            10, 240, 159, 111, 185, 174, 146, 16, 238, 135, 249, 107, 144, 172, 139, 33, 88, 142, 218, 225, 169, 237, 87, 41, 24, 197, 118,
                            185, 237, 186, 64, 33, 117, 113, 34, 28, 134, 172, 141, 105, 71, 243, 50, 111, 153, 206, 19, 79, 43, 48, 6, 195, 106, 253, 103,
                            144, 24, 229, 41, 100, 14, 232, 242, 26, 93, 243, 167, 14, 96, 140, 255, 227, 39, 247, 102, 187, 226, 111, 142, 171, 12, 148,
                            165, 1, 28, 8, 250, 196, 184, 19, 169, 108, 252, 165, 110, 103, 101, 236, 208, 232, 120, 243, 255, 20, 64, 137, 102, 1, 32, 146,
                            63, 102, 93, 248, 173, 100, 237, 129, 22, 4, 27, 43, 118, 41, 233, 42, 177, 186, 222, 105, 172, 20, 137, 123, 135, 183, 75, 31,
                            5, 151, 182, 165, 120, 255, 6, 59, 13, 111, 83, 119, 49, 27, 58, 248, 113, 182, 173, 249, 109, 37, 134, 238, 119, 107, 139, 109,
                            102, 128, 217, 43, 21, 22, 234, 84, 213, 229, 230, 125, 74, 92, 138, 27, 155, 233, 125, 103, 215, 53, 186, 237, 35, 168, 36, 210,
                            146, 193, 129, 88,
                        16, 248, 8,
                            182, 56, 240, 52, 75, 117, 52, 31, 216, 253, 181, 130, 174, 152, 190, 83, 207, 189, 8, 238, 34, 83, 214, 131, 69, 241, 233, 246,
                            114, 57, 252, 11, 171, 61, 39, 158, 83, 40, 154, 235, 99, 194, 4, 236, 217, 16, 109, 243, 116, 80, 127, 146, 88, 166, 210, 1,
                            210, 184, 137, 30, 166, 114, 102, 235, 12, 176, 224, 232, 4, 72, 73, 201, 55, 146, 65, 242, 161, 20, 217, 24, 232, 3, 55, 60,
                            186, 184, 214, 141, 205, 143, 153, 50, 155, 75, 210, 134, 223, 231, 98, 227, 255, 154, 2, 68, 78, 23, 26, 47, 190, 164, 219, 235,
                            51, 187, 206, 223, 175, 233, 124, 148, 30, 55, 217, 171, 203, 218, 120, 59, 176, 23, 236, 19, 60, 137, 33, 40, 49, 109, 193, 104,
                            12, 112, 147, 182, 200, 85, 234, 2, 217, 71, 239, 74, 233, 137, 109, 195, 5, 21, 148, 3, 140, 57, 199, 64, 53, 35, 185, 166, 69,
                            241, 127, 228, 133, 113, 188, 241, 87, 22, 58, 198, 34, 132, 113, 201, 89, 47, 204, 160, 251, 8, 135, 238, 14, 82, 136, 71, 233,
                            142, 87, 138, 98, 117, 135, 141, 67, 104, 21, 150, 250, 115, 203, 24, 142, 69, 105, 124, 142, 104, 153, 176, 100, 38, 75, 112,
                            147, 206, 83, 148, 57, 70, 57, 48, 10, 132, 62, 3, 149, 214, 32, 237, 89, 48, 36, 213, 184, 123, 156, 188, 91, 223, 193, 202, 97,
                            83, 152, 241
        }, CipherAlgorithm.AES256, new byte[] { 0, 0 }, TestName = "NewEncryptedBlobFromStreamFromZeroes")]
        [TestCase(new byte[] {
            55, 249, 1, 131,
                5, 1, 0,
                16, 16, 115, 55, 111, 187, 246, 84, 208, 104, 110, 14, 132, 0, 20, 119, 16, 107,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49,
                            79, 119, 110, 101, 114, 33, 52, 55, 68, 69,
                            81, 112, 106, 56, 72, 66, 83, 97, 45, 95,
                            84, 73, 109, 87, 45, 53, 74, 67, 101, 117,
                            81, 101, 82, 107, 109, 53, 78, 77, 112, 74,
                            87, 90, 71, 51, 104, 83, 117, 70, 85,
                        39, 34,
                            1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36, 106, 221, 140, 204, 78, 212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8, 103, 213, 209, 139, 137, 226, 36, 209, 8, 147, 15, 0, 78, 79, 57, 87, 87, 149, 127, 100, 248, 30, 10, 172, 23, 142, 209, 38, 16, 235,
                            134, 205, 137, 134, 65, 79, 23, 158, 203, 189, 55, 249, 98, 79, 144, 102, 198, 136, 215, 182, 6, 85, 11, 202, 32, 92, 94, 231, 215, 138, 71,
                            135, 82, 195, 96, 240, 1, 14, 68, 155, 47, 225, 135, 76, 40, 105, 218, 114, 245, 16, 96, 183, 138, 15, 175, 87, 19, 235, 66, 55, 233, 74, 147,
                            44, 45, 162, 63, 80, 87, 252, 11, 156, 207, 13, 98, 188, 110, 66, 175, 49, 27, 99, 46, 112, 242, 218, 0, 144, 223, 8, 57, 172, 89, 55, 66, 175,
                            88, 160, 17, 176, 251, 216, 30, 211, 115, 204, 8, 156, 178, 198, 187, 245, 215, 100, 174, 79, 243, 223, 81, 116, 73, 80, 18, 129, 19, 177, 49,
                            61, 114, 199, 181, 249, 175, 182, 115, 188, 204, 132, 130, 64, 167, 150, 52, 22, 105, 246, 198, 88, 37, 95, 251, 38, 34, 114, 127, 112, 64, 180,
                            74, 231, 156, 96, 97, 5, 167, 243, 180, 152, 209, 84, 112, 109, 41, 158, 44, 205, 112, 245, 111, 210, 244, 210, 153, 97, 217, 119, 231, 230,
                            209, 77, 138, 137, 2, 96, 214, 221, 170, 48, 51, 155, 8, 116, 62, 58, 102, 229, 247, 144, 212, 1, 91, 66, 221, 48, 243, 198, 56, 20, 42, 45, 81,
                            112, 165, 82, 168, 204,
                        16, 248, 8, 52, 222, 211, 102, 243, 219, 148, 234, 117, 97, 48, 130, 10, 74, 105, 117, 54, 37, 67, 202, 227, 51, 84, 242, 74, 67, 104, 227, 181,
                            57, 51, 25, 181, 135, 249, 39, 48, 30, 224, 55, 252, 164, 202, 154, 84, 52, 71, 63, 118, 61, 137, 185, 74, 22, 3, 56, 59, 93, 7, 28, 54, 149, 8,
                            170, 99, 33, 45, 131, 157, 219, 210, 68, 151, 103, 138, 249, 20, 6, 44, 114, 211, 62, 158, 60, 163, 246, 111, 108, 79, 213, 253, 191, 35, 48,
                            148, 111, 10, 249, 192, 208, 166, 171, 12, 115, 93, 250, 159, 209, 40, 36, 132, 198, 178, 95, 171, 141, 218, 8, 24, 136, 168, 40, 34, 174, 121,
                            42, 48, 234, 94, 170, 96, 246, 91, 25, 35, 249, 1, 49, 192, 86, 90, 94, 116, 64, 112, 165, 220, 24, 199, 11, 31, 48, 69, 10, 240, 216, 51, 96,
                            224, 132, 18, 151, 128, 187, 150, 15, 37, 78, 72, 31, 138, 96, 248, 8, 180, 66, 194, 132, 156, 78, 104, 112, 38, 34, 52, 10, 209, 32, 181, 94,
                            153, 29, 242, 20, 176, 87, 83, 70, 104, 40, 137, 230, 245, 144, 215, 192, 139, 33, 127, 249, 255, 141, 225, 106, 51, 41, 119, 212, 202, 150,
                            134, 211, 110, 36, 124, 101, 19, 246, 18, 196, 175, 113, 136, 226, 141, 27, 139, 123, 127, 226, 98, 125, 36, 40, 118, 210, 6, 146, 235, 150, 46,
                            152, 218, 113, 174, 82
        }, CipherAlgorithm.AES256, new byte[] { }, TestName = "NewEncryptedBlobFromStream")]
        public void NewEncryptedBlobFromStream(byte[] bytes, CipherAlgorithm algorithm, byte[] data) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.Decode<EncryptedBlob.Payload>();
            Assert.AreEqual(ILTagId.EncryptedBlob, tag.TagId);
            Assert.AreEqual(algorithm, tag.Value.Cipher);
            var clearBlob = tag.Value.DecryptBlob(TestFakeSigner.FixedKeysInstance, _ => new AES256Engine());
            Assert.IsNotNull(clearBlob);
            Assert.AreEqual(data.Length, clearBlob.Length);
        }

        [TestCase(CipherAlgorithm.AES256, new byte[] { 0, 0 }, ExpectedResult = new byte[] {
            55, 249, 1, 131,
                5, 1, 0,
                16, 16, 228, 155, 155, 36, 5, 133, 124, 106, 244, 135, 239, 56, 50, 213, 198, 74,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49, 79, 119, 110, 101, 114, 33, 52, 55, 68, 69, 81, 112, 106, 56, 72, 66, 83, 97, 45, 95, 84, 73,
                            109, 87, 45, 53, 74, 67, 101, 117, 81, 101, 82, 107, 109, 53, 78, 77, 112, 74, 87, 90, 71, 51, 104, 83, 117, 70, 85,
                        39, 34, 1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36,
                            106, 221, 140, 204, 78, 212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8,
        }, TestName = "SerializeEncryptedBlobAsZeroes")]
        [TestCase(CipherAlgorithm.AES256, new byte[] { }, ExpectedResult = new byte[] {
            55, 249, 1, 131,
                5, 1, 0,
                16, 16, 115, 55, 111, 187, 246, 84, 208, 104, 110, 14, 132, 0, 20, 119, 16, 107,
                21, 249, 1, 106, 1,
                    53, 249, 1, 101,
                        17, 49,
                            79, 119, 110, 101, 114, 33, 52, 55, 68, 69,
                            81, 112, 106, 56, 72, 66, 83, 97, 45, 95,
                            84, 73, 109, 87, 45, 53, 74, 67, 101, 117,
                            81, 101, 82, 107, 109, 53, 78, 77, 112, 74,
                            87, 90, 71, 51, 104, 83, 117, 70, 85,
                        39, 34,
                            1, 0, 90, 234, 31, 9, 108, 223, 128, 59, 77, 2, 215, 129, 119, 28, 194, 164, 198, 5, 74, 36, 106, 221, 140, 204, 78, 212, 55, 139, 19, 26, 150, 206,
                        16, 248, 8,
        }, TestName = "SerializeEncryptedBlob")]
        public byte[] SerializeEncryptedBlob(CipherAlgorithm algorithm, byte[] data) {
            var encodedBytes = new EncryptedBlob(algorithm, data, TestFakeSigner.FixedKeysInstance, Array.Empty<TagReader>()).AsPayload.EncodedBytes;
            return encodedBytes.PartOf(124);
        }

        [Test]
        public void ValidateFieldDefinition() {
            var fd = EncryptedBlob.FieldDefinition;
            Assert.IsNotNull(fd);
            Assert.AreEqual(nameof(EncryptedBlob), fd.Name);
            Assert.AreEqual(ILTagId.EncryptedBlob, fd.TagId);
            Assert.IsTrue(fd.HasSubFields, "Must have subfields");
            Assert.IsFalse(fd.IsEnumeration, "Should not be an enumeration");
            Assert.AreEqual(3, fd.SubDataFields.SafeCount());
            var fieldVersion = fd.SubDataFields.First();
            Assert.AreEqual(nameof(EncryptedBlob.Version), fieldVersion.Name);
            Assert.AreEqual(ILTagId.UInt16, fieldVersion.TagId);
            Assert.AreEqual((ushort)1, fieldVersion.Version);
            Assert.IsTrue(fieldVersion.IsVersion);
            Assert.IsFalse(fieldVersion.IsOptional.GetValueOrDefault());
            Assert.IsFalse(fieldVersion.IsOpaque.GetValueOrDefault());
            var fieldCipherText = fd.SubDataFields.Skip(1).First();
            Assert.AreEqual(nameof(EncryptedBlob.CipherText), fieldCipherText.Name);
            Assert.AreEqual(ILTagId.ByteArray, fieldCipherText.TagId);
            Assert.AreEqual((ushort)1, fieldCipherText.Version);
            Assert.IsFalse(fieldCipherText.IsVersion);
            Assert.IsFalse(fieldCipherText.IsOptional.GetValueOrDefault());
            Assert.IsTrue(fieldCipherText.IsOpaque.GetValueOrDefault());
            var fieldKeys = fd.SubDataFields.Skip(2).First();
            Assert.AreEqual(nameof(EncryptedBlob.ReadingKeys), fieldKeys.Name);
            Assert.AreEqual(ILTagId.ILTagArray, fieldKeys.TagId);
            Assert.AreEqual((ushort)1, fieldKeys.Version);
            Assert.IsFalse(fieldKeys.IsVersion);
            Assert.IsFalse(fieldKeys.IsOptional.GetValueOrDefault());
            Assert.IsFalse(fieldKeys.IsOpaque.GetValueOrDefault());
            Assert.AreEqual(ILTagId.ReadingKey, fieldKeys.ElementTagId.GetValueOrDefault());
        }
    }
}