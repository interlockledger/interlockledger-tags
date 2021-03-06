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
    public class SignedValueOfSignableTests
    {
        [TestCase(64ul, new byte[] {
            50, 249, 1, 91,
                5, 1, 0,
                250, 15, 65, 72, 5,
                    5, 1, 0,
                    10, 64,
                21, 249, 1, 74,
                    1,
                    51, 249, 1, 69,
                        5, 1, 0,
                        38, 248, 10,
                            0, 0, 143, 153, 188, 70, 80, 154, 245, 84, 90, 53, 143, 225, 168, 49, 6, 51, 104,
                            239, 33, 23, 141, 13, 7, 252, 56, 153, 50, 193, 102, 7, 48, 94, 239, 32, 82, 184,
                            195, 111, 79, 190, 87, 150, 163, 228, 143, 16, 235, 250, 87, 197, 124, 64, 189, 12,
                            210, 191, 202, 141, 142, 58, 138, 219, 164, 188, 136, 99, 173, 7, 91, 233, 99, 96,
                            97, 221, 209, 213, 17, 239, 183, 47, 89, 119, 112, 186, 72, 120, 79, 65, 233, 214,
                            238, 248, 53, 254, 133, 75, 3, 122, 125, 235, 63, 2, 245, 30, 178, 57, 142, 240, 210,
                            189, 131, 225, 125, 101, 137, 163, 106, 40, 13, 213, 188, 244, 193, 228, 208, 101, 22,
                            74, 159, 11, 247, 223, 234, 156, 128, 165, 161, 82, 97, 212, 106, 236, 135, 154, 38,
                            45, 169, 79, 191, 66, 157, 119, 2, 1, 145, 38, 53, 154, 70, 150, 220, 125, 24, 52, 199,
                            84, 180, 224, 68, 82, 246, 249, 192, 84, 27, 181, 111, 201, 175, 188, 139, 137, 101, 88,
                            137, 177, 255, 74, 71, 59, 240, 248, 21, 176, 52, 177, 109, 182, 28, 178, 195, 244, 169,
                            114, 167, 147, 218, 78, 160, 89, 50, 34, 207, 143, 241, 208, 134, 242, 160, 238, 136, 52,
                            142, 22, 157, 53, 61, 79, 124, 46, 76, 4, 238, 46, 178, 202, 250, 245, 129, 198, 96, 36,
                            89, 182, 151, 50, 148, 88, 93, 251, 237, 82, 32, 56, 44, 187, 43, 35, 1, 1, 0, 227, 176,
                            196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100,
                            155, 147, 76, 164, 149, 153, 27, 120, 82, 184, 85, 37, 248, 21, 0, 0, 40, 248, 16, 16,
                            248, 8, 210, 171, 20, 8, 209, 144, 192, 9, 196, 104, 106, 190, 94, 52, 111, 15, 93, 181,
                            35, 78, 249, 139, 59, 117, 204, 168, 76, 29, 221, 68, 104, 193, 112, 195, 136, 200, 168,
                            152, 173, 206, 212, 152, 74, 167, 40, 233, 91, 117, 166, 78, 170, 13, 25, 39, 225, 3, 75,
                            86, 166, 80, 23, 36, 205, 7, 87, 208, 216, 124, 115, 33, 203, 116, 61, 169, 57, 57, 64,
                            164, 225, 33, 174, 130, 53, 111, 210, 205, 26, 242, 132, 82, 133, 107, 23, 211, 107, 124,
                            199, 210, 196, 6, 145, 132, 96, 101, 142, 193, 30, 113, 67, 190, 223, 84, 11, 104, 199, 66,
                            189, 247, 132, 14, 14, 44, 199, 26, 81, 208, 36, 83, 188, 230, 233, 195, 250, 231, 219,
                            108, 76, 148, 12, 50, 122, 126, 216, 38, 32, 136, 126, 168, 248, 146, 164, 19, 214, 12,
                            7, 152, 83, 40, 78, 37, 19, 2, 246, 184, 180, 194, 157, 248, 87, 211, 200, 92, 198, 59,
                            194, 33, 223, 128, 86, 8, 57, 89, 94, 56, 41, 147, 134, 139, 164, 62, 69, 205, 133, 131,
                            179, 137, 114, 244, 161, 58, 216, 112, 3, 195, 126, 178, 250, 114, 185, 67, 151, 75, 159,
                            54, 99, 76, 55, 255, 222, 26, 215, 58, 204, 87, 69, 150, 101, 170, 250, 109, 32, 51, 151,
                            205, 99, 186, 198, 203, 170, 235, 37, 170, 21, 102, 35, 172, 194, 138, 101, 207, 75, 4,
                            176, 17, 92, 145, 16, 3, 1, 0, 1
        })]
        public void NewSignedValueOfSignableFromStream(ulong ilint, byte[] bytes) {
            using var ms = new MemoryStream(bytes);
            var tag = ms.DecodeAny<SignedValue<TestSignable>>();
            Assert.AreEqual(ILTagId.SignedValue, tag.TagId);
            Assert.AreEqual(TestSignable.FieldTagId, tag.ContentTagId);
            Assert.AreEqual(3, tag.FieldModel.SubDataFields.SafeCount());
            Assert.AreEqual(nameof(SignedValue<TestSignable>.SignedContent), tag.FieldModel.SubDataFields.Skip(1).First().Name);
            var signaturesFieldModel = tag.FieldModel.SubDataFields.Last();
            Assert.AreEqual(ILTagId.IdentifiedSignature, signaturesFieldModel.ElementTagId);
            Assert.AreEqual(4, signaturesFieldModel.SubDataFields.SafeCount());
            Assert.AreEqual(nameof(SignedValue<TestSignable>.Signatures), signaturesFieldModel.Name);
            var signedContent = tag.SignedContent;
            Assert.AreEqual(ilint, signedContent.SomeILInt);
            Assert.AreEqual(new TestSignable().FieldModel, signedContent.FieldModel);
            Assert.AreEqual(2, signedContent.FieldModel.SubDataFields.SafeCount());
            Assert.AreEqual(nameof(TestSignable.SomeILInt), signedContent.FieldModel.SubDataFields.Last().Name);
        }

        [TestCase(64ul, ExpectedResult = new byte[] {
            50, 249, 1, 91,
                5, 1, 0,
                250, 15, 65, 72, 5,
                    5, 1, 0,
                    10, 64,
                21, 249, 1, 74,
                    1,
                    51, 249, 1, 69,
                        5, 1, 0,
                        38, 248, 10,
                            0, 0, 143, 153, 188, 70, 80, 154, 245, 84, 90, 53, 143, 225, 168, 49, 6, 51, 104,
                            239, 33, 23, 141, 13, 7, 252, 56, 153, 50, 193, 102, 7, 48, 94, 239, 32, 82, 184,
                            195, 111, 79, 190, 87, 150, 163, 228, 143, 16, 235, 250, 87, 197, 124, 64, 189, 12,
                            210, 191, 202, 141, 142, 58, 138, 219, 164, 188, 136, 99, 173, 7, 91, 233, 99, 96,
                            97, 221, 209, 213, 17, 239, 183, 47, 89, 119, 112, 186, 72, 120, 79, 65, 233, 214,
                            238, 248, 53, 254, 133, 75, 3, 122, 125, 235, 63, 2, 245, 30, 178, 57, 142, 240, 210,
                            189, 131, 225, 125, 101, 137, 163, 106, 40, 13, 213, 188, 244, 193, 228, 208, 101, 22,
                            74, 159, 11, 247, 223, 234, 156, 128, 165, 161, 82, 97, 212, 106, 236, 135, 154, 38,
                            45, 169, 79, 191, 66, 157, 119, 2, 1, 145, 38, 53, 154, 70, 150, 220, 125, 24, 52, 199,
                            84, 180, 224, 68, 82, 246, 249, 192, 84, 27, 181, 111, 201, 175, 188, 139, 137, 101, 88,
                            137, 177, 255, 74, 71, 59, 240, 248, 21, 176, 52, 177, 109, 182, 28, 178, 195, 244, 169,
                            114, 167, 147, 218, 78, 160, 89, 50, 34, 207, 143, 241, 208, 134, 242, 160, 238, 136, 52,
                            142, 22, 157, 53, 61, 79, 124, 46, 76, 4, 238, 46, 178, 202, 250, 245, 129, 198, 96, 36,
                            89, 182, 151, 50, 148, 88, 93, 251, 237, 82, 32, 56, 44, 187, 43, 35, 1, 1, 0, 227, 176,
                            196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100,
                            155, 147, 76, 164, 149, 153, 27, 120, 82, 184, 85, 37, 248, 21, 0, 0, 40, 248, 16, 16,
                            248, 8, 210, 171, 20, 8, 209, 144, 192, 9, 196, 104, 106, 190, 94, 52, 111, 15, 93, 181,
                            35, 78, 249, 139, 59, 117, 204, 168, 76, 29, 221, 68, 104, 193, 112, 195, 136, 200, 168,
                            152, 173, 206, 212, 152, 74, 167, 40, 233, 91, 117, 166, 78, 170, 13, 25, 39, 225, 3, 75,
                            86, 166, 80, 23, 36, 205, 7, 87, 208, 216, 124, 115, 33, 203, 116, 61, 169, 57, 57, 64,
                            164, 225, 33, 174, 130, 53, 111, 210, 205, 26, 242, 132, 82, 133, 107, 23, 211, 107, 124,
                            199, 210, 196, 6, 145, 132, 96, 101, 142, 193, 30, 113, 67, 190, 223, 84, 11, 104, 199, 66,
                            189, 247, 132, 14, 14, 44, 199, 26, 81, 208, 36, 83, 188, 230, 233, 195, 250, 231, 219,
                            108, 76, 148, 12, 50, 122, 126, 216, 38, 32, 136, 126, 168, 248, 146, 164, 19, 214, 12,
                            7, 152, 83, 40, 78, 37, 19, 2, 246, 184, 180, 194, 157, 248, 87, 211, 200, 92, 198, 59,
                            194, 33, 223, 128, 86, 8, 57, 89, 94, 56, 41, 147, 134, 139, 164, 62, 69, 205, 133, 131,
                            179, 137, 114, 244, 161, 58, 216, 112, 3, 195, 126, 178, 250, 114, 185, 67, 151, 75, 159,
                            54, 99, 76, 55, 255, 222, 26, 215, 58, 204, 87, 69, 150, 101, 170, 250, 109, 32, 51, 151,
                            205, 99, 186, 198, 203, 170, 235, 37, 170, 21, 102, 35, 172, 194, 138, 101, 207, 75, 4,
                            176, 17, 92, 145, 16, 3, 1, 0, 1
        })]
        public byte[] SerializeSignedValueOfSignable(ulong someILInt) {
            var signedValue = new TestSignable(someILInt).SignWith(TestFakeSigner.FixedKeysInstance);
            var encodedBytes = signedValue.AsPayload.EncodedBytes;
            TestContext.WriteLine(encodedBytes.AsLiteral());
            return encodedBytes;
        }
    }
}