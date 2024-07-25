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

using System.Security.Cryptography.X509Certificates;

namespace InterlockLedger.Tags;

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
        Assert.Multiple(() => {
            Assert.That(tag.TagId, Is.EqualTo(ILTagId.SignedValue));
            Assert.That(tag.ContentTagId, Is.EqualTo(TestSignable.FieldTagId));
            Assert.That(tag.FieldModel.SubDataFields.SafeCount(), Is.EqualTo(3));
            Assert.That(tag.FieldModel.SubDataFields.Skip(1).First().Name, Is.EqualTo(nameof(SignedValue<TestSignable>.SignedContent)));
        });
        var signaturesFieldModel = tag.FieldModel.SubDataFields.Last();
        Assert.Multiple(() => {
            Assert.That(signaturesFieldModel.ElementTagId, Is.EqualTo(ILTagId.IdentifiedSignature));
            Assert.That(signaturesFieldModel.SubDataFields.SafeCount(), Is.EqualTo(4));
            Assert.That(signaturesFieldModel.Name, Is.EqualTo(nameof(SignedValue<TestSignable>.Signatures)));
        });
        var signedContent = tag.SignedContent;
        Assert.Multiple(() => {
            Assert.That(signedContent.SomeILInt, Is.EqualTo(ilint));
            Assert.That(signedContent.FieldModel, Is.EqualTo(new TestSignable().FieldModel));
            Assert.That(signedContent.FieldModel.SubDataFields.SafeCount(), Is.EqualTo(2));
            Assert.That(signedContent.FieldModel.SubDataFields.Last().Name, Is.EqualTo(nameof(TestSignable.SomeILInt)));
        });
    }

    [TestCase(64ul, ExpectedResult = new byte[] {
        50, 249, 1, 91,
            5, 1, 0,
            250, 15, 65, 72, 5, 5, 1, 0, 10, 64, 21, 249, 1, 74, 1, 51, 249, 1, 69, 5, 1, 0, 38, 248, 10, 0, 0, 143, 153, 188, 70, 80, 154, 245, 84, 90, 53, 143, 225, 168, 49, 6, 51, 104, 239, 33, 23, 141, 13, 7, 252, 56, 153, 50, 193, 102, 7, 48, 94, 239, 32, 82, 184, 195, 111, 79, 190, 87, 150, 163, 228, 143, 16, 235, 250, 87, 197, 124, 64, 189, 12, 210, 191, 202, 141, 142, 58, 138, 219, 164, 188, 136, 99, 173, 7, 91, 233, 99, 96, 97, 221, 209, 213, 17, 239, 183, 47, 89, 119, 112, 186, 72, 120, 79, 65, 233, 214, 238, 248, 53, 254, 133, 75, 3, 122, 125, 235, 63, 2, 245, 30, 178, 57, 142, 240, 210, 189, 131, 225, 125, 101, 137, 163, 106, 40, 13, 213, 188, 244, 193, 228, 208, 101, 22, 74, 159, 11, 247, 223, 234, 156, 128, 165, 161, 82, 97, 212, 106, 236, 135, 154, 38, 45, 169, 79, 191, 66, 157, 119, 2, 1, 145, 38, 53, 154, 70, 150, 220, 125, 24, 52, 199, 84, 180, 224, 68, 82, 246, 249, 192, 84, 27, 181, 111, 201, 175, 188, 139, 137, 101, 88, 137, 177, 255, 74, 71, 59, 240, 248, 21, 176, 52, 177, 109, 182, 28, 178, 195, 244, 169, 114, 167, 147, 218, 78, 160, 89, 50, 34, 207, 143, 241, 208, 134, 242, 160, 238, 136, 52, 142, 22, 157, 53, 61, 79, 124, 46, 76, 4, 238, 46, 178, 202, 250, 245, 129, 198, 96, 36, 89, 182, 151, 50, 148, 88, 93, 251, 237, 82, 32, 56, 44, 187, 43, 35, 4, 1, 0, 252, 247, 7, 54, 65, 105, 72, 217, 0, 220, 229, 31, 9, 197, 50, 87, 255, 239, 135, 81, 223, 184, 67, 118, 113, 110, 232, 216, 224, 61, 86, 28, 37, 248, 21, 0, 0, 40, 248, 16, 16, 248, 8, 210, 171, 20, 8, 209, 144, 192, 9, 196, 104, 106, 190, 94, 52, 111, 15, 93, 181, 35, 78, 249, 139, 59, 117, 204, 168, 76, 29, 221, 68, 104, 193, 112, 195, 136, 200, 168, 152, 173, 206, 212, 152, 74, 167, 40, 233, 91, 117, 166, 78, 170, 13, 25, 39, 225, 3, 75, 86, 166, 80, 23, 36, 205, 7, 87, 208, 216, 124, 115, 33, 203, 116, 61, 169, 57, 57, 64, 164, 225, 33, 174, 130, 53, 111, 210, 205, 26, 242, 132, 82, 133, 107, 23, 211, 107, 124, 199, 210, 196, 6, 145, 132, 96, 101, 142, 193, 30, 113, 67, 190, 223, 84, 11, 104, 199, 66, 189, 247, 132, 14, 14, 44, 199, 26, 81, 208, 36, 83, 188, 230, 233, 195, 250, 231, 219, 108, 76, 148, 12, 50, 122, 126, 216, 38, 32, 136, 126, 168, 248, 146, 164, 19, 214, 12, 7, 152, 83, 40, 78, 37, 19, 2, 246, 184, 180, 194, 157, 248, 87, 211, 200, 92, 198, 59, 194, 33, 223, 128, 86, 8, 57, 89, 94, 56, 41, 147, 134, 139, 164, 62, 69, 205, 133, 131, 179, 137, 114, 244, 161, 58, 216, 112, 3, 195, 126, 178, 250, 114, 185, 67, 151, 75, 159, 54, 99, 76, 55, 255, 222, 26, 215, 58, 204, 87, 69, 150, 101, 170, 250, 109, 32, 51, 151, 205, 99, 186, 198, 203, 170, 235, 37, 170, 21, 102, 35, 172, 194, 138, 101, 207, 75, 4, 176, 17, 92, 145, 16, 3, 1, 0, 1
        })]
    public byte[] SerializeSignedValueOfSignable(ulong someILInt) {
        var signedValue = new TestSignable(someILInt).SignWith(TestFakeSigner.FixedKeysInstance);
        var encodedBytes = signedValue.AsPayload.EncodedBytes();
        TestContext.WriteLine(encodedBytes.AsLiteral());
        return encodedBytes;
    }

    [Test]
    public void SignWithRSA() {
        byte[] bytes = [48, 130, 9, 82, 2, 1, 3, 48, 130, 9, 14, 6, 9, 42, 134, 72, 134, 247, 13, 1, 7, 1, 160, 130, 8, 255, 4, 130, 8, 251, 48, 130, 8, 247, 48, 130, 5, 144, 6, 9, 42, 134, 72, 134, 247, 13, 1, 7, 1, 160, 130, 5, 129, 4, 130, 5, 125, 48, 130, 5, 121, 48, 130, 5, 117, 6, 11, 42, 134, 72, 134, 247, 13, 1, 12, 10, 1, 2, 160, 130, 4, 238, 48, 130, 4, 234, 48, 28, 6, 10, 42, 134, 72, 134, 247, 13, 1, 12, 1, 3, 48, 14, 4, 8, 59, 163, 36, 118, 113, 253, 1, 122, 2, 2, 7, 208, 4, 130, 4, 200, 97, 195, 114, 245, 9, 207, 65, 204, 182, 225, 100, 207, 109, 193, 94, 95, 161, 42, 44, 184, 137, 255, 29, 41, 81, 64, 2, 192, 239, 141, 232, 129, 11, 224, 39, 34, 254, 124, 169, 92, 22, 140, 14, 244, 73, 31, 71, 5, 49, 121, 228, 130, 5, 217, 230, 162, 183, 176, 249, 187, 202, 255, 218, 58, 26, 164, 107, 153, 252, 87, 238, 121, 14, 138, 220, 48, 252, 119, 76, 122, 151, 136, 170, 38, 229, 19, 186, 3, 109, 53, 50, 24, 204, 223, 189, 176, 182, 24, 44, 90, 13, 177, 19, 99, 69, 251, 25, 124, 16, 96, 12, 187, 158, 80, 74, 33, 2, 164, 53, 211, 222, 55, 36, 43, 5, 206, 205, 95, 178, 179, 58, 148, 62, 50, 135, 149, 164, 115, 163, 194, 170, 64, 225, 150, 207, 23, 141, 51, 203, 184, 165, 158, 142, 153, 49, 111, 134, 27, 114, 48, 5, 148, 69, 203, 105, 18, 62, 111, 95, 31, 244, 139, 182, 37, 24, 195, 163, 92, 51, 255, 116, 16, 246, 10, 69, 80, 119, 252, 187, 144, 231, 198, 123, 181, 197, 207, 152, 34, 109, 53, 112, 181, 150, 218, 112, 169, 62, 3, 166, 142, 168, 66, 23, 16, 143, 102, 213, 100, 163, 26, 126, 198, 105, 104, 34, 75, 101, 163, 108, 42, 142, 123, 46, 68, 53, 234, 37, 150, 199, 124, 81, 11, 146, 159, 33, 238, 139, 150, 4, 212, 119, 125, 122, 52, 99, 204, 15, 85, 200, 45, 94, 112, 6, 182, 52, 88, 234, 134, 108, 167, 220, 117, 249, 233, 19, 75, 183, 120, 2, 1, 34, 85, 83, 33, 142, 23, 17, 211, 111, 56, 61, 235, 177, 11, 0, 45, 56, 163, 171, 208, 214, 129, 44, 252, 145, 130, 66, 241, 230, 26, 25, 187, 24, 242, 205, 93, 233, 96, 87, 124, 161, 233, 69, 91, 63, 149, 149, 94, 23, 91, 212, 85, 15, 137, 89, 53, 30, 87, 113, 136, 181, 142, 229, 254, 198, 85, 108, 119, 21, 58, 34, 50, 185, 124, 80, 237, 0, 140, 225, 114, 192, 151, 92, 60, 158, 90, 129, 24, 21, 243, 168, 1, 53, 144, 201, 99, 230, 35, 149, 163, 190, 127, 98, 120, 60, 254, 204, 220, 98, 12, 148, 28, 251, 38, 130, 228, 58, 73, 180, 166, 148, 93, 198, 209, 39, 187, 244, 104, 107, 217, 164, 217, 59, 90, 153, 151, 1, 17, 199, 132, 223, 132, 78, 29, 55, 227, 94, 173, 198, 183, 149, 7, 91, 41, 109, 167, 209, 126, 165, 35, 122, 78, 98, 15, 82, 220, 103, 39, 92, 227, 177, 237, 213, 167, 153, 56, 173, 5, 0, 7, 161, 74, 157, 25, 152, 123, 194, 213, 105, 136, 158, 99, 145, 148, 109, 102, 39, 10, 23, 168, 170, 69, 249, 175, 30, 191, 41, 192, 127, 130, 236, 126, 120, 224, 245, 58, 184, 185, 255, 215, 226, 92, 114, 164, 156, 18, 31, 231, 225, 162, 101, 35, 52, 50, 192, 197, 197, 186, 63, 102, 220, 64, 43, 147, 194, 11, 232, 183, 179, 132, 63, 233, 237, 52, 207, 107, 110, 185, 1, 137, 242, 116, 32, 182, 82, 56, 218, 88, 31, 145, 210, 173, 227, 23, 115, 20, 138, 35, 77, 14, 72, 49, 64, 157, 255, 36, 215, 21, 49, 96, 245, 152, 115, 37, 240, 107, 160, 174, 71, 79, 211, 11, 184, 55, 218, 91, 70, 240, 88, 48, 21, 227, 208, 60, 52, 106, 47, 245, 103, 210, 75, 249, 222, 100, 247, 56, 52, 219, 228, 131, 184, 1, 220, 167, 78, 84, 77, 155, 62, 241, 113, 168, 76, 156, 239, 174, 153, 217, 77, 88, 151, 43, 192, 193, 246, 241, 116, 39, 90, 173, 37, 74, 36, 89, 70, 50, 191, 96, 125, 16, 71, 121, 153, 135, 105, 112, 170, 173, 120, 15, 35, 118, 244, 181, 207, 242, 65, 5, 175, 208, 25, 29, 105, 190, 178, 25, 163, 60, 119, 135, 180, 103, 84, 190, 183, 52, 30, 108, 108, 42, 43, 184, 162, 92, 94, 149, 225, 12, 169, 137, 51, 14, 112, 135, 51, 122, 17, 245, 12, 182, 53, 250, 90, 231, 40, 0, 175, 21, 53, 196, 111, 214, 120, 189, 119, 212, 122, 229, 160, 134, 104, 36, 100, 95, 254, 34, 144, 72, 140, 63, 24, 218, 129, 252, 47, 11, 176, 31, 155, 120, 184, 31, 54, 72, 50, 115, 245, 5, 109, 57, 27, 62, 103, 189, 52, 217, 88, 249, 181, 197, 189, 213, 72, 145, 57, 233, 188, 147, 53, 197, 230, 176, 56, 82, 131, 169, 126, 252, 179, 2, 161, 51, 210, 212, 192, 54, 159, 172, 184, 22, 237, 235, 47, 39, 20, 57, 136, 45, 193, 138, 154, 227, 244, 85, 149, 106, 150, 106, 115, 243, 216, 219, 20, 5, 167, 200, 129, 73, 216, 41, 41, 238, 112, 191, 152, 230, 233, 186, 66, 109, 243, 88, 103, 202, 177, 84, 136, 111, 23, 207, 105, 107, 63, 179, 89, 56, 128, 85, 252, 26, 40, 69, 138, 82, 183, 188, 23, 142, 61, 26, 39, 222, 117, 177, 53, 232, 152, 223, 13, 158, 216, 16, 207, 241, 158, 65, 249, 237, 19, 168, 198, 178, 59, 160, 135, 113, 193, 238, 77, 125, 104, 190, 101, 43, 88, 193, 241, 230, 85, 99, 159, 13, 80, 134, 18, 217, 137, 0, 218, 247, 59, 59, 181, 100, 211, 178, 80, 62, 149, 107, 104, 225, 3, 86, 252, 172, 167, 36, 183, 124, 205, 54, 45, 104, 64, 76, 165, 104, 216, 83, 18, 38, 77, 188, 190, 152, 12, 117, 71, 170, 184, 209, 60, 224, 136, 128, 162, 205, 235, 44, 242, 44, 75, 139, 11, 25, 112, 159, 175, 85, 202, 116, 217, 114, 60, 26, 1, 208, 188, 94, 217, 192, 4, 112, 20, 27, 142, 165, 250, 205, 125, 7, 107, 221, 214, 58, 99, 183, 24, 187, 81, 3, 78, 2, 58, 149, 179, 234, 99, 195, 113, 185, 122, 167, 83, 170, 213, 191, 170, 151, 228, 39, 150, 39, 118, 20, 77, 132, 165, 110, 166, 190, 64, 12, 78, 107, 169, 204, 22, 70, 200, 32, 253, 193, 22, 89, 234, 77, 170, 148, 209, 116, 80, 245, 181, 218, 214, 90, 81, 48, 24, 59, 207, 186, 253, 23, 111, 28, 168, 83, 32, 199, 228, 0, 158, 201, 190, 13, 97, 239, 58, 82, 233, 142, 38, 44, 70, 100, 136, 70, 14, 207, 25, 245, 47, 202, 80, 118, 56, 96, 159, 150, 210, 14, 115, 119, 39, 239, 72, 166, 112, 47, 189, 161, 249, 178, 63, 237, 17, 123, 155, 88, 221, 56, 187, 187, 97, 69, 64, 177, 125, 122, 4, 99, 246, 178, 64, 247, 75, 63, 254, 172, 51, 157, 235, 251, 27, 40, 92, 39, 10, 88, 162, 177, 163, 217, 242, 32, 96, 90, 144, 146, 202, 9, 13, 168, 0, 166, 51, 80, 133, 200, 191, 193, 190, 246, 127, 198, 82, 60, 26, 13, 19, 245, 96, 165, 205, 94, 214, 157, 57, 41, 27, 51, 166, 143, 220, 209, 236, 172, 34, 146, 170, 210, 108, 20, 170, 117, 17, 183, 254, 97, 87, 81, 49, 116, 48, 19, 6, 9, 42, 134, 72, 134, 247, 13, 1, 9, 21, 49, 6, 4, 4, 1, 0, 0, 0, 48, 93, 6, 9, 43, 6, 1, 4, 1, 130, 55, 17, 1, 49, 80, 30, 78, 0, 77, 0, 105, 0, 99, 0, 114, 0, 111, 0, 115, 0, 111, 0, 102, 0, 116, 0, 32, 0, 83, 0, 111, 0, 102, 0, 116, 0, 119, 0, 97, 0, 114, 0, 101, 0, 32, 0, 75, 0, 101, 0, 121, 0, 32, 0, 83, 0, 116, 0, 111, 0, 114, 0, 97, 0, 103, 0, 101, 0, 32, 0, 80, 0, 114, 0, 111, 0, 118, 0, 105, 0, 100, 0, 101, 0, 114, 48, 130, 3, 95, 6, 9, 42, 134, 72, 134, 247, 13, 1, 7, 6, 160, 130, 3, 80, 48, 130, 3, 76, 2, 1, 0, 48, 130, 3, 69, 6, 9, 42, 134, 72, 134, 247, 13, 1, 7, 1, 48, 28, 6, 10, 42, 134, 72, 134, 247, 13, 1, 12, 1, 3, 48, 14, 4, 8, 109, 23, 210, 175, 190, 253, 49, 139, 2, 2, 7, 208, 128, 130, 3, 24, 213, 6, 195, 115, 190, 53, 197, 175, 210, 51, 105, 138, 79, 215, 112, 153, 144, 147, 216, 44, 185, 129, 94, 199, 127, 137, 141, 146, 169, 171, 28, 208, 177, 11, 36, 137, 127, 101, 135, 128, 207, 239, 227, 8, 120, 148, 89, 25, 221, 166, 200, 178, 138, 226, 137, 143, 159, 209, 29, 128, 73, 120, 198, 82, 16, 195, 128, 7, 125, 253, 221, 121, 212, 9, 64, 242, 217, 105, 213, 121, 216, 81, 160, 41, 151, 243, 187, 70, 116, 63, 88, 149, 8, 113, 18, 160, 148, 97, 61, 25, 30, 43, 89, 23, 255, 10, 138, 120, 233, 18, 32, 103, 79, 165, 172, 207, 219, 166, 149, 100, 237, 143, 33, 110, 130, 41, 197, 147, 248, 100, 5, 23, 108, 173, 205, 164, 176, 40, 69, 159, 184, 74, 176, 169, 26, 241, 10, 42, 107, 80, 182, 120, 82, 17, 149, 51, 242, 216, 200, 125, 39, 221, 247, 209, 9, 133, 236, 74, 149, 17, 107, 63, 100, 205, 200, 123, 67, 90, 55, 150, 212, 84, 87, 209, 83, 153, 209, 54, 132, 149, 205, 126, 79, 145, 47, 153, 207, 78, 200, 231, 35, 24, 70, 60, 180, 223, 195, 251, 73, 53, 85, 163, 69, 75, 29, 51, 89, 195, 197, 85, 211, 236, 42, 40, 224, 88, 219, 30, 115, 147, 233, 22, 20, 134, 108, 7, 148, 5, 218, 244, 245, 232, 180, 88, 40, 12, 181, 47, 180, 176, 27, 142, 123, 25, 216, 246, 63, 70, 121, 42, 188, 171, 119, 206, 11, 171, 132, 76, 56, 249, 19, 83, 191, 54, 219, 49, 211, 93, 151, 247, 53, 94, 142, 6, 113, 132, 147, 117, 170, 106, 192, 224, 172, 218, 205, 109, 216, 102, 200, 88, 134, 81, 197, 87, 48, 93, 216, 72, 117, 176, 229, 124, 53, 191, 9, 40, 196, 204, 31, 116, 207, 15, 136, 43, 210, 14, 31, 165, 211, 179, 139, 245, 136, 218, 48, 238, 48, 248, 16, 239, 99, 57, 160, 209, 151, 68, 219, 175, 155, 59, 187, 175, 115, 15, 214, 20, 104, 201, 101, 8, 58, 132, 207, 99, 63, 227, 80, 255, 146, 109, 77, 48, 205, 191, 56, 152, 208, 234, 59, 127, 30, 41, 1, 23, 47, 8, 183, 185, 143, 136, 39, 235, 59, 242, 231, 125, 80, 206, 114, 92, 205, 50, 83, 141, 165, 60, 176, 17, 154, 169, 81, 228, 241, 37, 207, 121, 31, 166, 51, 72, 54, 121, 97, 154, 82, 249, 21, 74, 156, 81, 156, 237, 238, 173, 114, 181, 245, 83, 49, 135, 86, 206, 231, 13, 117, 205, 166, 153, 70, 114, 192, 69, 169, 40, 58, 243, 82, 88, 169, 58, 202, 2, 16, 14, 126, 212, 237, 153, 91, 74, 39, 247, 73, 209, 182, 65, 228, 22, 21, 241, 92, 253, 32, 28, 32, 104, 192, 255, 181, 2, 89, 227, 111, 171, 212, 120, 180, 222, 153, 198, 142, 211, 255, 99, 17, 86, 200, 147, 3, 121, 56, 249, 22, 25, 35, 205, 91, 225, 47, 44, 218, 236, 218, 158, 206, 150, 133, 108, 234, 228, 162, 57, 41, 174, 59, 68, 237, 213, 66, 255, 17, 215, 13, 141, 116, 168, 133, 185, 185, 11, 169, 203, 154, 207, 89, 235, 189, 190, 69, 212, 108, 222, 146, 97, 35, 5, 143, 70, 207, 176, 228, 18, 48, 232, 188, 127, 251, 3, 174, 75, 69, 150, 51, 250, 148, 105, 202, 55, 21, 200, 237, 165, 252, 92, 67, 205, 228, 73, 25, 54, 28, 96, 91, 126, 139, 155, 253, 225, 165, 174, 204, 42, 17, 18, 36, 101, 65, 215, 84, 24, 72, 181, 183, 109, 205, 71, 33, 163, 253, 43, 243, 211, 214, 106, 177, 166, 220, 131, 49, 95, 44, 236, 201, 37, 142, 130, 161, 111, 97, 212, 59, 94, 64, 115, 215, 126, 159, 252, 4, 61, 233, 199, 39, 101, 209, 170, 43, 193, 0, 149, 95, 31, 227, 150, 219, 20, 62, 251, 56, 73, 183, 212, 172, 195, 104, 166, 21, 37, 95, 120, 148, 85, 132, 110, 158, 38, 57, 33, 213, 108, 44, 202, 243, 74, 199, 217, 82, 188, 157, 167, 132, 61, 8, 138, 211, 213, 196, 156, 69, 236, 60, 38, 7, 210, 196, 110, 84, 136, 133, 227, 196, 105, 201, 172, 161, 143, 99, 67, 16, 223, 208, 23, 246, 8, 177, 98, 210, 187, 184, 172, 31, 248, 138, 118, 114, 99, 77, 212, 171, 199, 199, 16, 176, 19, 70, 73, 185, 125, 100, 183, 207, 140, 76, 253, 79, 56, 249, 210, 185, 86, 135, 44, 166, 128, 250, 55, 198, 87, 246, 177, 164, 123, 48, 59, 48, 31, 48, 7, 6, 5, 43, 14, 3, 2, 26, 4, 20, 28, 69, 65, 151, 26, 229, 60, 27, 28, 164, 41, 5, 160, 37, 230, 146, 12, 85, 106, 67, 4, 20, 194, 39, 19, 227, 225, 7, 162, 152, 158, 101, 36, 199, 44, 42, 20, 183, 109, 146, 233, 179, 2, 2, 7, 208];
        using var x509Certificate = new X509Certificate2(bytes, "password");
        var data = new InterlockSigningKeyData(
            [KeyPurpose.Protocol],
            [],
            $"{nameof(SignWithRSA)} test key",
            bytes,
            TagPubKey.Resolve(x509Certificate),
            encryptedContentType: EncryptedContentType.EmbeddedCertificate);
        var rsakey = new RSACertificateSigningKey(data, x509Certificate);
        var context = new SigningContext(rsakey, TestFakeSigner.Instance);
        var signedValue = new TestSignable(112).SignWith(context);
    }
}