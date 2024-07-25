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

[TestFixture]
public class TagEdDSAParametersTests
{
    [TestCase(new byte[] { 59, 32, 172, 110, 35, 93, 197, 226, 49, 6, 11, 110, 227, 117, 164, 193, 115, 109, 36, 125, 119, 213, 221, 91, 214, 32, 153, 224, 151, 133, 221, 164, 157, 147 }, false, TestName = "NewTagEdDSAParametersFromStream Without Private Key")]
    [TestCase(new byte[] { 59, 64, 172, 110, 35, 93, 197, 226, 49, 6, 11, 110, 227, 117, 164, 193, 115, 109, 36, 125, 119, 213, 221, 91, 214, 32, 153, 224, 151, 133, 221, 164, 157, 147, 179, 48, 50, 122, 188, 119, 20, 20, 226, 50, 35, 209, 8, 248, 0, 157, 198, 62, 52, 93, 18, 185, 246, 125, 90, 179, 146, 90, 244, 239, 221, 45 }, true, TestName = "NewTagEdDSAParametersFromStream With Private Key")]
    public void NewTagEdDSAParametersFromStream(byte[] bytes, bool hasPrivate) {
        using var ms = new MemoryStream(bytes);
        var tag = ms.Decode<TagEdDSAParameters>();
        Assert.That(tag, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(tag.TagId, Is.EqualTo(ILTagId.EdDSAParameters));
            Assert.That(tag.Strength, Is.EqualTo(KeyStrength.Normal));
        });
        var parameters = tag.Value;
        Assert.That(parameters, Is.Not.Null);
        Assert.That(parameters.HasPrivatePart, Is.EqualTo(hasPrivate));
    }

    [TestCase(new byte[] { 172, 110, 35, 93, 197, 226, 49, 6, 11, 110, 227, 117, 164, 193, 115, 109, 36, 125, 119, 213, 221, 91, 214, 32, 153, 224, 151, 133, 221, 164, 157, 147 }, ExpectedResult = new byte[] { 59, 32, 172, 110, 35, 93, 197, 226, 49, 6, 11, 110, 227, 117, 164, 193, 115, 109, 36, 125, 119, 213, 221, 91, 214, 32, 153, 224, 151, 133, 221, 164, 157, 147 }, TestName = "SerializeTagEdDSAParameters Without Private Key")]
    [TestCase(new byte[] { 172, 110, 35, 93, 197, 226, 49, 6, 11, 110, 227, 117, 164, 193, 115, 109, 36, 125, 119, 213, 221, 91, 214, 32, 153, 224, 151, 133, 221, 164, 157, 147, 179, 48, 50, 122, 188, 119, 20, 20, 226, 50, 35, 209, 8, 248, 0, 157, 198, 62, 52, 93, 18, 185, 246, 125, 90, 179, 146, 90, 244, 239, 221, 45 }, ExpectedResult = new byte[] { 59, 64, 172, 110, 35, 93, 197, 226, 49, 6, 11, 110, 227, 117, 164, 193, 115, 109, 36, 125, 119, 213, 221, 91, 214, 32, 153, 224, 151, 133, 221, 164, 157, 147, 179, 48, 50, 122, 188, 119, 20, 20, 226, 50, 35, 209, 8, 248, 0, 157, 198, 62, 52, 93, 18, 185, 246, 125, 90, 179, 146, 90, 244, 239, 221, 45 }, TestName = "SerializeTagEdDSAParameters With Private Key")]
    public byte[] SerializeTagEdDSAParameters(byte[] bytes) {
        byte[] encodedBytes = new TagEdDSAParameters(new EdDSAParameters(bytes)).EncodedBytes();
        TestContext.WriteLine(encodedBytes.AsLiteral());
        return encodedBytes;
    }
}