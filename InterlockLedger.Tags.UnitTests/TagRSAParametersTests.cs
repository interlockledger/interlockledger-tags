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

[TestFixture]
public class TagRSAParametersTests
{
    [TestCase(new byte[] { 41, 26, 16, 1, 6, 16, 1, 5, 16, 1, 7, 16, 1, 8, 16, 1, 2, 16, 1, 3, 16, 1, 4, 16, 1, 1, 10, 0 })]
    [TestCase(new byte[] { 41, 24, 16, 1, 6, 16, 1, 5, 16, 1, 7, 16, 1, 8, 16, 1, 2, 16, 1, 3, 16, 1, 4, 16, 1, 1 })]
    public void NewTagRSAParametersFromStream(byte[] bytes) {
        using var ms = new MemoryStream(bytes);
        var tag = ms.Decode<TagRSAParameters>();
        Assert.That(tag.TagId, Is.EqualTo(ILTagId.RSAParameters));
        var value = tag.Value.Parameters;
        Assert.That(value.D, Is.EqualTo(new byte[] { 1 }));
        Assert.That(value.DP, Is.EqualTo(new byte[] { 2 }));
        Assert.That(value.DQ, Is.EqualTo(new byte[] { 3 }));
        Assert.That(value.InverseQ, Is.EqualTo(new byte[] { 4 }));
        Assert.That(value.Exponent, Is.EqualTo(new byte[] { 5 }));
        Assert.That(value.Modulus, Is.EqualTo(new byte[] { 6 }));
        Assert.That(value.P, Is.EqualTo(new byte[] { 7 }));
        Assert.That(value.Q, Is.EqualTo(new byte[] { 8 }));
        Assert.That(tag.Value.Strength, Is.EqualTo(KeyStrength.Normal));
    }

    [TestCase(ExpectedResult = new byte[] { 41, 26, 16, 1, 6, 16, 1, 5, 16, 1, 7, 16, 1, 8, 16, 1, 2, 16, 1, 3, 16, 1, 4, 16, 1, 1, 10, 0 })]
    public byte[] SerializeTagRSAParameters() =>
        new TagRSAParameters(new RSAParameters {
            D = new byte[] { 1 },
            DP = new byte[] { 2 },
            DQ = new byte[] { 3 },
            InverseQ = new byte[] { 4 },
            Exponent = new byte[] { 5 },
            Modulus = new byte[] { 6 },
            P = new byte[] { 7 },
            Q = new byte[] { 8 },
        }, KeyStrength.Normal).EncodedBytes;
}
