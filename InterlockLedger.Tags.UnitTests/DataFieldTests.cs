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
public class DataFieldTests
{
    [Test]
    public void Cloning() {
        AssertCloneIsEqualTo(_dataFieldWithEnumeration);
        AssertCloneIsEqualTo(_dataFieldWithFlagsEnumeration);
        AssertCloneIsEqualTo(_dataFieldWithoutEnumeration);

        static void AssertCloneIsEqualTo(DataField source) {
            var clone = source.WithName(source.Name);
            Assert.Multiple(() => {
                Assert.That(clone, Is.EqualTo(source));
                Assert.That(clone, Is.Not.SameAs(source));
                Assert.That(clone.WithName("testing"), Is.Not.EqualTo(source));
            });
        }
    }

    [Test]
    public void EnumerationFromString() {
        Assert.Multiple(() => {
            foreach (ulong value in _dataFieldWithEnumeration.EnumerationDefinition.Keys)
                Assert.That(FromStringAsNumber(_dataFieldWithEnumeration.EnumerationDefinition[value].Name), Is.EqualTo(value));
            Assert.That(FromStringAsNumber("?10"), Is.EqualTo(10));
            Assert.That(FromStringAsNumber("?"), Is.Null);
            var e = Assert.Throws<InvalidDataException>(() => FromStringAsNumber("Donr"));
            Assert.That(e.Message, Is.EqualTo("Value 'Donr' is not valid for the enumeration for field [DeliveryStatus]"));
        });
        static ulong? FromStringAsNumber(string text) => EnumFromStringAsNumber(_dataFieldWithEnumeration, text);
    }

    [Test]
    public void EnumerationFromStringFlags() {
        Assert.Multiple(() => {
            foreach (ulong value in _dataFieldWithFlagsEnumeration.EnumerationDefinition.Keys)
                Assert.That(FromStringAsNumber(_dataFieldWithFlagsEnumeration.EnumerationDefinition[value].Name), Is.EqualTo(value));
            Assert.That(FromStringAsNumber(AllFlags()), Is.EqualTo(7ul));
            Assert.That(FromStringAsNumber(AllFlags(reversed: true)), Is.EqualTo(7ul));
            Assert.That(FromStringAsNumber(AllFlags() + "|?8"), Is.EqualTo(15ul));
            Assert.That(FromStringAsNumber("?"), Is.Null);
        });
        var e = Assert.Throws<InvalidDataException>(() => FromStringAsNumber("CheckAll"));
        Assert.That(e.Message, Is.EqualTo("Value 'CheckAll' is not valid for the enumeration for field [Policy]"));

        static ulong? FromStringAsNumber(string text) => EnumFromStringAsNumber(_dataFieldWithFlagsEnumeration, text);
    }

    [Test]
    public void EnumerationToString() =>
        Assert.Multiple(() => {
            foreach (ulong value in _dataFieldWithEnumeration.EnumerationDefinition.Keys)
                Assert.That(_dataFieldWithEnumeration.EnumerationToString(value), Is.EqualTo(_dataFieldWithEnumeration.EnumerationDefinition[value].Name));
            Assert.That(_dataFieldWithEnumeration.EnumerationToString(10), Is.EqualTo("?10"));
            Assert.That(_dataFieldWithoutEnumeration.EnumerationToString(1), Is.EqualTo("?"));
        });

    [Test]
    public void EnumerationToStringFlags() =>
        Assert.Multiple(() => {
            foreach (ulong value in _dataFieldWithFlagsEnumeration.EnumerationDefinition.Keys)
                Assert.That(_dataFieldWithFlagsEnumeration.EnumerationToString(value), Is.EqualTo(_dataFieldWithFlagsEnumeration.EnumerationDefinition[value].Name));
            Assert.That(_dataFieldWithFlagsEnumeration.EnumerationToString(7), Is.EqualTo(AllFlags()));
            Assert.That(_dataFieldWithFlagsEnumeration.EnumerationToString(8), Is.EqualTo("?8"));
            Assert.That(_dataFieldWithFlagsEnumeration.EnumerationToString(15), Is.EqualTo(AllFlags() + "|?8"));
            Assert.That(_dataFieldWithFlagsEnumeration.EnumerationToString(47), Is.EqualTo(AllFlags() + "|?40"));
        });

    [Test]
    public void SerializeAndDeserializeWithEnumeration() => TestWith(_dataFieldWithEnumeration);

    [Test]
    public void SerializeAndDeserializeWithoutEnumeration() => TestWith(_dataFieldWithoutEnumeration);

    [Test]
    public void SerializeAndDeserializeWithFlagsEnumeration() => TestWith(_dataFieldWithFlagsEnumeration);

    private static readonly DataField _dataFieldWithEnumeration =
        new() {
            TagId = 10,
            Name = "DeliveryStatus",
            Version = 3,
            Description = "Enumerated Status for Delivery",
            EnumerationDefinition = new EnumerationDictionary {
                [0] = new EnumerationDetails("WaitingApproval", "Waiting for sale to be completed (paid)"),
                [1] = new EnumerationDetails("Canceled", "Sale canceled won't be delivered"),
                [2] = new EnumerationDetails("Handling", "Locating itens in stock"),
                [3] = new EnumerationDetails("Sent", "Sent to delivery provider"),
                [4] = new EnumerationDetails("Done", "Client received the itens")
            }
        };

    private static readonly DataField _dataFieldWithFlagsEnumeration =
         new() {
             TagId = 10,
             Name = "Policy",
             Version = 3,
             Description = "Policy Flags",
             EnumerationAsFlags = true,
             EnumerationDefinition = new EnumerationDictionary {
                 [1] = new EnumerationDetails("CheckMemory", "Check if memory is enough"),
                 [2] = new EnumerationDetails("CheckData", "Check if input is the proper format"),
                 [4] = new EnumerationDetails("CheckConnection", "Check if connection is alive")
             }
         };

    private static readonly DataField _dataFieldWithoutEnumeration =
        new() {
            TagId = 10,
            Name = "BuildStatus",
            Version = 3,
            Description = "Status for Build"
        };

    private static string AllFlags(bool reversed = false) => _dataFieldWithFlagsEnumeration.EnumerationDefinition
        .OrderBy(kp => reversed ? Reverse(kp.Key) : kp.Key)
        .Select(i => i.Value.Name)
        .JoinedBy("|");

    private static ulong? EnumFromStringAsNumber(DataField datafield, string text) => (datafield.EnumerationFromString(text) as ILTagILInt)?.Value;

    private static ulong Reverse(ulong key) => ulong.MaxValue - key;

    private static void TestWith(DataField dataField) {
        var encodedBytes = new ILTagDataField(dataField).EncodedBytes;
        using var ms = new MemoryStream(encodedBytes);
        var tagValue = ms.DecodeTag();
        Assert.That(tagValue, Is.InstanceOf<ILTagDataField>());
        var value = ((ILTagDataField)tagValue).Value;
        Assert.That(value, Is.EqualTo(dataField));
    }
}