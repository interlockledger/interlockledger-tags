/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
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
