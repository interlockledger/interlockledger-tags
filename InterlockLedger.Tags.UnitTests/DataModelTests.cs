/******************************************************************************************************************************
 
Copyright (c) 2018-2020 InterlockLedger Network
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

using System;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class DataModelTests
    {
        [Test]
        public void BadCompatibilityElementTagIdChangedDataModel() => Assert.IsFalse(_bad_ElementTagIdChangedDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityFieldTagIdChangedDataModel() => Assert.IsFalse(_bad_FieldTagIdChangedDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityIndexUniquenessDataModel() => Assert.IsFalse(_bad_IndexUniquenessDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityLessIndexedDataModel() => Assert.IsFalse(_bad_LessIndexedDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityOpaquedDataModel() => Assert.IsFalse(_bad_OpaquedDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityRenamedDataModel() => Assert.IsFalse(_bad_RenamedDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityRenamedFieldDataModel() => Assert.IsFalse(_bad_RenamedFieldDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityRenamedIndexDataModel() => Assert.IsFalse(_bad_RenamedIndexDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityRenumberedDataModel() => Assert.IsFalse(_bad_RenumberedDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityShorterDataModel() => Assert.IsFalse(_bad_ShorterDataModel.IsCompatible(_baseDataModel));

        [Test]
        public void BadCompatibilityUnblobedAndIndexedDataModel() {
            Assert.IsTrue(_bad_UnblobedAndIndexedMisversionedDataModel.IsCompatible(_baseDataModel));
            Assert.IsFalse(_bad_UnblobedAndIndexedMisversionedDataModel.IsCompatible(_ok_UnblobedDataModel));
        }

        [Test]
        public void GoodCompatibilityUnblobedAndIndexedDataModel() {
            Assert.IsTrue(_ok_UnblobedAndIndexedDataModel.IsCompatible(_baseDataModel));
            Assert.IsTrue(_ok_UnblobedAndIndexedDataModel.IsCompatible(_ok_UnblobedDataModel));
            Assert.IsTrue(_ok_UnblobedWithEnumerationDataModel.IsCompatible(_baseDataModel));
            Assert.IsTrue(_ok_UnblobedWithEnumerationDataModel.IsCompatible(_ok_UnblobedDataModel));
        }

        [Test]
        public void GoodCompatibilityUnblobedDataModel() => Assert.IsTrue(_ok_UnblobedDataModel.IsCompatible(_baseDataModel));

        private static readonly DataModel _bad_ElementTagIdChangedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, ElementTagId = 12, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_FieldTagIdChangedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 12, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_IndexUniquenessDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { IsUnique = true, ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_LessIndexedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = Array.Empty<DataIndex>()
        };

        private static readonly DataModel _bad_OpaquedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1, IsOpaque = true },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_RenamedDataModel = new DataModel {
            PayloadName = "Test_Renamed",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_RenamedFieldDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Vendor", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_RenamedIndexDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { Name = "History", ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_RenumberedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 101,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_ShorterDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _bad_UnblobedAndIndexedMisversionedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 6, Name= "Balance", Version = 1} }},
                },
            Indexes = new DataIndex[] {
                    new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" },
                    new DataIndex { ElementsAsString = "Blob.Balance" }}
        };

        private static readonly DataModel _baseDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _ok_UnblobedAndIndexedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 6, Name= "Balance", Version = 2} }},
                },
            Indexes = new DataIndex[] {
                    new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" },
                    new DataIndex { ElementsAsString = "Blob.Balance" }}
        };

        private static readonly DataModel _ok_UnblobedDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 6, Name= "Balance", Version = 2} }},
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };

        private static readonly DataModel _ok_UnblobedWithEnumerationDataModel = new DataModel {
            PayloadName = "Test",
            PayloadTagId = 1,
            DataFields = new DataField[] {
                    new DataField() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new DataField() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 4, Name= "Id", Version = 1} } },
                    new DataField() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = new DataField [] { new DataField() { TagId = 6, Name= "Balance", Version = 2} }},
                    new DataField() { TagId = 10, Name= "DeliveryStatus", Version = 3, Description = "Enumerated Status for Delivery",
                        Enumeration = new EnumerationDictionary {
                            [0] = new EnumerationDetails("WaitingApproval", "Waiting for sale to be completed (paid)"),
                            [1] = new EnumerationDetails("Canceled", "Sale canceled won't be delivered"),
                            [2] = new EnumerationDetails("Handling", "Locating itens in stock"),
                            [3] = new EnumerationDetails("Sent", "Sent to delivery provider"),
                            [4] = new EnumerationDetails("Done", "Client received the itens")
                        }
                    }
                },
            Indexes = new DataIndex[] { new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" } }
        };
    }
}
