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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class AES256EncryptedTests
    {
        [Test]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Delegate is for testing exception throwing")]
        public void ConstructorParameterValidation() {
            AssertArgumentNullException(() => new AES256Encrypted<ILTagBool>(null, "password"), "value");
            AssertPasswordMissing(() => new AES256Encrypted<ILTagBool>(ILTagBool.False, null));
            AssertArgumentNullException(() => new AES256Encrypted<ILTagBool>(null), "s");
        }

        [Test]
        public void DecryptParameterValidation() {
            var tag = new AES256Encrypted<ILTagBool>(new MemoryStream(new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            72, 104, 36, 141, 30, 66, 29, 203, 15, 6, 42, 29, 52, 96, 232, 3 }));
            AssertPasswordMissing(() => tag.Decrypt(null));
        }

        [TestCase(new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            72, 104, 36, 141, 30, 66, 29, 203, 15, 6, 42, 29, 52, 96, 232, 3 }, true, "password", TestName = "AES256Encrypted<True>FromStream")]
        [TestCase(new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            82, 117, 243, 216, 107, 79, 184, 104, 69, 147, 19, 62, 191, 165, 60, 211 }, false, "password", TestName = "AES256Encrypted<False>FromStream")]
        public void NewAES256EncryptedFromStream(byte[] bytes, bool value, string password) {
            var tag = new AES256Encrypted<ILTagBool>(new MemoryStream(bytes));
            var clear = tag.Decrypt(password);
            Assert.AreEqual(value, AsBool(clear));
        }

        [TestCase(true, "password", ExpectedResult = new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            72, 104, 36, 141, 30, 66, 29, 203, 15, 6, 42, 29, 52, 96, 232, 3 }, TestName = "AES256Encrypted<True>EncodedBytes")]
        [TestCase(false, "password", ExpectedResult = new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            82, 117, 243, 216, 107, 79, 184, 104, 69, 147, 19, 62, 191, 165, 60, 211 }, TestName = "AES256Encrypted<False>EncodedBytes")]
        public byte[] SerializeAES256Encrypted(bool value, string password) {
            var result = new TestableAES256Encrypted(value ? ILTagBool.True : ILTagBool.False, password);
            var clear = result.Decrypt(password);
            Assert.AreEqual(value, AsBool(clear));
            return result.EncodedBytes;
        }

        private static bool AsBool(ILTagBool tag) => tag?.Value ?? throw new InvalidDataException("Not an ILTagBool");

        private static void AssertArgumentNullException(TestDelegate code, string paramName)
            => Assert.AreEqual(paramName, Assert.Throws<ArgumentNullException>(code).ParamName);

        private static void AssertPasswordMissing(TestDelegate code) {
            const string expectedMessageStart = AES256Encrypted<ILTagBool>.MissingPasswordMessage;
            var ae = Assert.Throws<ArgumentException>(code);
            Assert.AreEqual("password", ae.ParamName);
            Assert.IsTrue(ae.Message.StartsWith(expectedMessageStart, StringComparison.Ordinal), $"Exception message doesn't start with {expectedMessageStart}");
        }

        private class TestableAES256Encrypted : AES256Encrypted<ILTagBool>
        {
            public TestableAES256Encrypted(ILTagBool value, string password) : base(value, password, new byte[32], new byte[16]) {
            }
        }
    }
}