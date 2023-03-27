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

using System.ComponentModel;
using System.Globalization;

namespace InterlockLedger.Tags;
[TestFixture]
public class InterlockIdConverterTests
{
    [Test]
    public void CanConvertFrom() {
        var converter = BuildConverter();
        Assert.AreEqual(true, converter.CanConvertFrom(null, typeof(string)));
        Assert.AreEqual(false, converter.CanConvertFrom(null, typeof(int)));
    }

    private static TypeCustomConverter<InterlockId> BuildConverter() => new();

    [Test]
    public void CanConvertTo() {
        var converter = BuildConverter();
        Assert.AreEqual(true, converter.CanConvertTo(null, typeof(string)));
        Assert.AreEqual(false, converter.CanConvertTo(null, typeof(int)));
    }

    [Test]
    public void ConvertFrom() {
        var converter = BuildConverter();
        var id = converter.ConvertFrom(null, CultureInfo.InvariantCulture, "Key!AAA#SHA3_512") as InterlockId;
        Assert.IsNotNull(id);
        Assert.AreEqual("Key!AAA#SHA3_512", id.ToString());
        Assert.AreEqual(HashAlgorithm.SHA3_512, id.Algorithm);
        Assert.AreEqual(new byte[] { 0, 0 }, id.Data);
        Assert.AreEqual(4, id.Type);
    }

    [Test]
    public void ConvertTo() {
        var converter = BuildConverter();
        Assert.AreEqual("Key!47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU",
            converter.ConvertTo(null, CultureInfo.InvariantCulture, new KeyId(TagHash.Empty), typeof(string)));
    }

    [Test]
    public void TypeDescriptor_GetConverter() {
        var converter = TypeDescriptor.GetConverter(typeof(InterlockId));
        Assert.IsNotNull(converter);
        Assert.IsInstanceOf<TypeCustomConverter<InterlockId>>(converter);
    }
}