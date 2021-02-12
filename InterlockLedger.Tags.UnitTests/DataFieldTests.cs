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

using System.IO;
using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    public class DataFieldTests
    {
        [Test]
        public void Cloning() {
            AssertCloneIsEqualTo(_dataFieldWithEnumeration);
            AssertCloneIsEqualTo(_dataFieldWithFlagsEnumeration);
            AssertCloneIsEqualTo(_dataFieldWithoutEnumeration);

            static void AssertCloneIsEqualTo(DataField source) {
                var clone = source.WithName(source.Name);
                Assert.AreEqual(source, clone);
                Assert.AreNotSame(source, clone);
                Assert.AreNotEqual(source, clone.WithName("testing"));
            }
        }

        [Test]
        public void EnumerationFromString() {
            foreach (ulong value in _dataFieldWithEnumeration.EnumerationDefinition.Keys)
                Assert.AreEqual(value, FromStringAsNumber(_dataFieldWithEnumeration.EnumerationDefinition[value].Name));
            Assert.AreEqual(10, FromStringAsNumber("?10"));
            Assert.IsNull(FromStringAsNumber("?"));
            var e = Assert.Throws<InvalidDataException>(() => FromStringAsNumber("Donr"));
            Assert.AreEqual("Value 'Donr' is not valid for the enumeration for field [DeliveryStatus]", e.Message);
            static ulong? FromStringAsNumber(string text) => EnumFromStringAsNumber(_dataFieldWithEnumeration, text);
        }

        [Test]
        public void EnumerationFromStringFlags() {
            foreach (ulong value in _dataFieldWithFlagsEnumeration.EnumerationDefinition.Keys)
                Assert.AreEqual(value, FromStringAsNumber(_dataFieldWithFlagsEnumeration.EnumerationDefinition[value].Name));
            Assert.AreEqual(7ul, FromStringAsNumber(AllFlags()));
            Assert.AreEqual(7ul, FromStringAsNumber(AllFlags(reversed: true)));
            Assert.AreEqual(15ul, FromStringAsNumber(AllFlags() + "|?8"));
            Assert.IsNull(FromStringAsNumber("?"));
            var e = Assert.Throws<InvalidDataException>(() => FromStringAsNumber("CheckAll"));
            Assert.AreEqual("Value 'CheckAll' is not valid for the enumeration for field [Policy]", e.Message);
            static ulong? FromStringAsNumber(string text) => EnumFromStringAsNumber(_dataFieldWithFlagsEnumeration, text);
        }

        [Test]
        public void EnumerationToString() {
            foreach (ulong value in _dataFieldWithEnumeration.EnumerationDefinition.Keys)
                Assert.AreEqual(_dataFieldWithEnumeration.EnumerationDefinition[value].Name, _dataFieldWithEnumeration.EnumerationToString(value));
            Assert.AreEqual("?10", _dataFieldWithEnumeration.EnumerationToString(10));
            Assert.AreEqual("?", _dataFieldWithoutEnumeration.EnumerationToString(1));
        }

        [Test]
        public void EnumerationToStringFlags() {
            foreach (ulong value in _dataFieldWithFlagsEnumeration.EnumerationDefinition.Keys)
                Assert.AreEqual(_dataFieldWithFlagsEnumeration.EnumerationDefinition[value].Name, _dataFieldWithFlagsEnumeration.EnumerationToString(value));
            Assert.AreEqual(AllFlags(), _dataFieldWithFlagsEnumeration.EnumerationToString(7));
            Assert.AreEqual("?8", _dataFieldWithFlagsEnumeration.EnumerationToString(8));
            Assert.AreEqual(AllFlags() + "|?8", _dataFieldWithFlagsEnumeration.EnumerationToString(15));
            Assert.AreEqual(AllFlags() + "|?40", _dataFieldWithFlagsEnumeration.EnumerationToString(47));
        }

        [Test]
        public void SerializeAndDeserialize() {
            TestWith(_dataFieldWithEnumeration);
            TestWith(_dataFieldWithoutEnumeration);
            TestWith(_dataFieldWithFlagsEnumeration);
        }

        private static readonly DataField _dataFieldWithEnumeration =
            new DataField() {
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
             new DataField() {
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
            new DataField() {
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
            Assert.IsInstanceOf<ILTagDataField>(tagValue);
            var value = ((ILTagDataField)tagValue).Value;
            Assert.AreEqual(dataField, value);
        }
    }
}