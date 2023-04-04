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

public class AES256EncryptedTests
{
    [Test]
    [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Delegate is for testing exception throwing")]
    public void ConstructorParameterValidation() {
        AssertRequiredException(() => new AES256Encrypted<ILTagBool>(null, "password"), "value");
        AssertRequiredException(() => new AES256Encrypted<ILTagBool>(ILTagBool.False, null), "password");
        AssertRequiredException(() => new AES256Encrypted<ILTagBool>(null), "s");
    }

    [Test]
    public void DecryptParameterValidation() {
        var tag = new AES256Encrypted<ILTagBool>(new MemoryStream(new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            72, 104, 36, 141, 30, 66, 29, 203, 15, 6, 42, 29, 52, 96, 232, 3 }));
        AssertRequiredException(() => tag.Decrypt(null), "password");
    }

    [Test]
    public void EncodedBytesFalse() => Assert.That(() =>
                SerializeAES256Encrypted(false, "password"), Is.EquivalentTo(new byte[] { 42, 68, 0, 0, 16, 64,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
                82, 117, 243, 216, 107, 79, 184, 104, 69, 147, 19, 62, 191, 165, 60, 211 }));

    [Test]
    public void EncodedBytesTrue() => Assert.That(() =>
                SerializeAES256Encrypted(true, "password"), Is.EquivalentTo(new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            72, 104, 36, 141, 30, 66, 29, 203, 15, 6, 42, 29, 52, 96, 232, 3 }));

    [Test]
    public void FromStreamFalse() => NewAES256EncryptedFromStream(new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            82, 117, 243, 216, 107, 79, 184, 104, 69, 147, 19, 62, 191, 165, 60, 211 }, false, "password");

    [Test]
    public void FromStreamTrue() => NewAES256EncryptedFromStream(new byte[] { 42, 68, 0, 0, 16, 64,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100, 112, 97, 115, 115, 119, 111, 114, 100,
            72, 104, 36, 141, 30, 66, 29, 203, 15, 6, 42, 29, 52, 96, 232, 3 }, true, "password");

    private static bool AsBool(ILTagBool tag) => tag?.Value ?? throw new InvalidDataException("Not an ILTagBool");

 

    private static void NewAES256EncryptedFromStream(byte[] bytes, bool value, string password) {
        var tag = new AES256Encrypted<ILTagBool>(new MemoryStream(bytes));
        var clear = tag.Decrypt(password);
        Assert.That(AsBool(clear), Is.EqualTo(value));
    }

    private static byte[] SerializeAES256Encrypted(bool value, string password) {
        var result = new TestableAES256Encrypted(value ? ILTagBool.True : ILTagBool.False, password);
        var clear = result.Decrypt(password);
        Assert.That(AsBool(clear), Is.EqualTo(value));
        return result.EncodedBytes;
    }

    private class TestableAES256Encrypted : AES256Encrypted<ILTagBool>
    {
        public TestableAES256Encrypted(ILTagBool value, string password) : base(value, password, new byte[32], new byte[16]) {
        }
    }
}