// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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
public class InterlockColorConverterTests
{
    [Test]
    public void CanConvertFrom() {
        var converter = new TypeNotNullConverter<InterlockColor>();
        Assert.Multiple(() => {
            Assert.That(converter.CanConvertFrom(null, typeof(string)), Is.EqualTo(true));
            Assert.That(converter.CanConvertFrom(null, typeof(int)), Is.EqualTo(false));
        });
    }

    [Test]
    public void CanConvertTo() {
        var converter = new TypeNotNullConverter<InterlockColor>();
        Assert.Multiple(() => {
            Assert.That(converter.CanConvertTo(null, typeof(string)), Is.EqualTo(true));
            Assert.That(converter.CanConvertTo(null, typeof(int)), Is.EqualTo(false));
        });
    }

    [Test]
    public void ConvertFrom() {
        var converter = new TypeNotNullConverter<InterlockColor>();
        ConvertFromStringAndAssert(converter, "Aqua", InterlockColor.Aqua);
        ConvertFromStringAndAssert(converter, "Aqua ", InterlockColor.Aqua);
        ConvertFromStringAndAssert(converter, "aqua", InterlockColor.Aqua);
        ConvertFromStringAndAssert(converter, "#00ffff", InterlockColor.Aqua);
        ConvertFromStringAndAssert(converter, "#00ffff80", InterlockColor.Aqua.WithA(128));
        ConvertFromStringAndAssert(converter, "#00ffFF", InterlockColor.Aqua);
        ConvertFromStringAndAssert(converter, "#00ffff  ", InterlockColor.Aqua);
        ConvertFromStringAndAssert(converter, "SlateGray", InterlockColor.SlateGray);
        ConvertFromStringAndAssert(converter, "slategray", InterlockColor.SlateGray);
        ConvertFromStringAndAssert(converter, "slateGray", InterlockColor.SlateGray);
        ConvertFromStringAndAssert(converter, "SLATEGRAY", InterlockColor.SlateGray);
        ConvertFromStringAndAssert(converter, "#00QQff", InterlockColor.Transparent);
        ConvertFromStringAndAssert(converter, "Transparent", InterlockColor.Transparent);
        ConvertFromStringAndAssert(converter, "#FFFFFF00", InterlockColor.Transparent);
        ConvertFromStringAndAssert(converter, "Zany", InterlockColor.Transparent);
        ConvertFromStringAndAssert(converter, "#010203", new InterlockColor(1, 2, 3));
        ConvertFromStringAndAssert(converter, "#01020300", new InterlockColor(1, 2, 3, a: 0));
        ConvertFromStringAndAssert(converter, "#01020380", new InterlockColor(1, 2, 3, a: 128));
        ConvertFromStringAndAssert(converter, "#010203", InterlockColor.From(0x010203FF));
        ConvertFromStringAndAssert(converter, "#010203FF", InterlockColor.From(0x010203FF));
        ConvertFromStringAndAssert(converter, "#01020300", InterlockColor.From(0x01020300));
        ConvertFromStringAndAssert(converter, "#01020380", InterlockColor.From(0x01020380));
        Assert.Multiple(() => {
            Assert.That("Nothing".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
            Assert.That("#1".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
            Assert.That("#12".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
            Assert.That("#123".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
            Assert.That("#1234".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
            Assert.That("#12345".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
            Assert.That("#1234567".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
            Assert.That("#123AZT".Parse<InterlockColor>(), Is.EqualTo(InterlockColor.Transparent));
        });
    }

    [Test]
    public void ConvertTo() {
        var converter = new TypeNotNullConverter<InterlockColor>();
        ConvertToStringAndAssert(converter, InterlockColor.Aqua);
        ConvertToStringAndAssert(converter, InterlockColor.Transparent);
        ConvertToStringAndAssert(converter, InterlockColor.From(0x01020390));
        ConvertToStringAndAssert(converter, new InterlockColor(1, 2, 3, a: 128));
    }

    [Test]
    public void TypeDescriptor_GetConverter() {
        var converter = TypeDescriptor.GetConverter(typeof(InterlockColor));
        Assert.That(converter, Is.Not.Null);
        Assert.That(converter, Is.InstanceOf<TypeNotNullConverter<InterlockColor>>());
    }

    private static void ConvertFromStringAndAssert(TypeNotNullConverter<InterlockColor> converter, string colorValue, InterlockColor expectedColor) =>
        Assert.Multiple(() => {
            Assert.That((InterlockColor)converter.ConvertFrom(null, CultureInfo.InvariantCulture, colorValue), Is.EqualTo(expectedColor));
            Assert.That(colorValue.Parse<InterlockColor>(), Is.EqualTo(expectedColor));
        });

    private static void ConvertToStringAndAssert(TypeNotNullConverter<InterlockColor> converter, InterlockColor color)
        => Assert.That(converter.ConvertTo(null, CultureInfo.InvariantCulture, color, typeof(string)), Is.EqualTo(color.Name));
}