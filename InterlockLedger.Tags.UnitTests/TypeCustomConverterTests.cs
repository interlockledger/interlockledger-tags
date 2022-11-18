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

using System.ComponentModel;
using System.Globalization;

using NUnit.Framework;

namespace InterlockLedger.Tags;
public class TypeCustomConverterTests
{
    [Test]
    public void CanConvertFrom() {
        var converter = new TypeCustomConverter<AppPermissions>();
        Assert.AreEqual(true, converter.CanConvertFrom(null, typeof(string)));
        Assert.AreEqual(false, converter.CanConvertFrom(null, typeof(int)));
    }

    [Test]
    public void CanConvertTo() {
        var converter = new TypeCustomConverter<AppPermissions>();
        Assert.AreEqual(true, converter.CanConvertTo(null, typeof(string)));
        Assert.AreEqual(false, converter.CanConvertTo(null, typeof(int)));
    }

    [Test]
    public void ConvertFrom() {
        var converter = new TypeCustomConverter<AppPermissions>();
        ConvertFromStringAndAssert(converter, "#3", _permissionsApp3All);
        ConvertFromStringAndAssert(converter, "#4,1000,1001", _permissionsApp4Docs);
    }

    [Test]
    public void ConvertTo() {
        var converter = new TypeCustomConverter<AppPermissions>();
        ConvertToStringAndAssert(converter, _permissionsApp3All);
        ConvertToStringAndAssert(converter, _permissionsApp4Docs);
    }

    [Test]
    public void TypeDescriptor_GetConverter() {
        var converter = TypeDescriptor.GetConverter(typeof(AppPermissions));
        Assert.IsNotNull(converter);
        Assert.IsInstanceOf<TypeCustomConverter<AppPermissions>>(converter);
    }

    private readonly AppPermissions _permissionsApp3All = new(3, null);
    private readonly AppPermissions _permissionsApp4Docs = new(4, 1000, 1001);

    private static void ConvertFromStringAndAssert(TypeCustomConverter<AppPermissions> converter, string value, AppPermissions expectedAppPermissions) {
        Assert.AreEqual(expectedAppPermissions, (AppPermissions)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value));
        Assert.AreEqual(expectedAppPermissions, AppPermissions.FromString(value));
    }

    private static void ConvertToStringAndAssert(TypeCustomConverter<AppPermissions> converter, AppPermissions permissions)
        => Assert.AreEqual(permissions.TextualRepresentation, converter.ConvertTo(null, CultureInfo.InvariantCulture, permissions, typeof(string)));
}