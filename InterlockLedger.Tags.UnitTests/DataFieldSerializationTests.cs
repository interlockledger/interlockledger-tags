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

using System.IO;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    public class DataFieldSerializationTests
    {
        [Test]
        public void SerializeAndDeserialize() {
            TestWith(_dataFieldWithEnumeration);
            TestWith(_dataFieldWithoutEnumeration);
        }

        private static readonly DataField _dataFieldWithEnumeration =
                    new DataField() {
                        TagId = 10,
                        Name = "DeliveryStatus",
                        Version = 3,
                        Description = "Enumerated Status for Delivery",
                        Enumeration = new EnumerationDictionary {
                            [0] = new EnumerationDetails("WaitingApproval", "Waiting for sale to be completed (paid)"),
                            [1] = new EnumerationDetails("Canceled", "Sale canceled won't be delivered"),
                            [2] = new EnumerationDetails("Handling", "Locating itens in stock"),
                            [3] = new EnumerationDetails("Sent", "Sent to delivery provider"),
                            [4] = new EnumerationDetails("Done", "Client received the itens")
                        }
                    };

        private static readonly DataField _dataFieldWithoutEnumeration =
                    new DataField() {
                        TagId = 10,
                        Name = "BuildStatus",
                        Version = 3,
                        Description = "Status for Build"
                    };

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
