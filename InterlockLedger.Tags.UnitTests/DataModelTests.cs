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
public class DataModelTests
{
    [Test]
    public void BadCompatibilityElementTagIdChangedDataModel() => Assert.That(_bad_ElementTagIdChangedDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityFieldTagIdChangedDataModel() => Assert.That(_bad_FieldTagIdChangedDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityIndexUniquenessDataModel() => Assert.That(_bad_IndexUniquenessDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityLessIndexedDataModel() => Assert.That(_bad_LessIndexedDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityOpaquedDataModel() => Assert.That(_bad_OpaquedDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityRenamedDataModel() => Assert.That(_bad_RenamedDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityRenamedFieldDataModel() => Assert.That(_bad_RenamedFieldDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityRenamedIndexDataModel() => Assert.That(_bad_RenamedIndexDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityRenumberedDataModel() => Assert.That(_bad_RenumberedDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityShorterDataModel() => Assert.That(_bad_ShorterDataModel.IsCompatible(_baseDataModel), Is.False);

    [Test]
    public void BadCompatibilityUnblobedAndIndexedDataModel() =>
        Assert.Multiple(() => {
            Assert.That(_bad_UnblobedAndIndexedMisversionedDataModel.IsCompatible(_baseDataModel));
            Assert.That(_bad_UnblobedAndIndexedMisversionedDataModel.IsCompatible(_ok_UnblobedDataModel), Is.False);
        });

    [Test]
    public void GoodCompatibilityUnblobedAndIndexedDataModel() =>
        Assert.Multiple(() => {
            Assert.That(_ok_UnblobedAndIndexedDataModel.IsCompatible(_baseDataModel));
            Assert.That(_ok_UnblobedAndIndexedDataModel.IsCompatible(_ok_UnblobedDataModel));
            Assert.That(_ok_UnblobedWithEnumerationDataModel.IsCompatible(_baseDataModel));
            Assert.That(_ok_UnblobedWithEnumerationDataModel.IsCompatible(_ok_UnblobedDataModel));
        });

    [Test]
    public void GoodCompatibilityUnblobedDataModel() => Assert.That(_ok_UnblobedDataModel.IsCompatible(_baseDataModel));

    [Test]
    public void HasField() {
        ShouldHave("DateOfPurchase");
        ShouldHave("dateofpurchase");
        ShouldHave("Buyer");
        ShouldHave("Buyer.Id");
        ShouldHave("Blob");
        ShouldHave("Blob.Balance");
        ShouldNotHave("Date_Of_Purchase");
        ShouldNotHave("Buyer.Balance");
        ShouldNotHave(null);
        ShouldNotHave("");
        ShouldNotHave("    ");
        ShouldNotHave(" .  ");
        ShouldNotHave("Buyer.");
        ShouldNotHave("Buyer..ID");

        static void ShouldHave(string fieldName)
            => Assert.That(_ok_UnblobedDataModel.HasField(fieldName), $"Should have '{fieldName}' field");

        static void ShouldNotHave(string fieldName)
            => Assert.That(_ok_UnblobedDataModel.HasField(fieldName), Is.False, $"Should not have '{fieldName}' field");
    }

    private static readonly DataModel _bad_ElementTagIdChangedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, ElementTagId = 12, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_FieldTagIdChangedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 12, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_IndexUniquenessDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { IsUnique = true, ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_LessIndexedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = []
    };

    private static readonly DataModel _bad_OpaquedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1, IsOpaque = true },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_RenamedDataModel = new() {
        PayloadName = "Test_Renamed",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_RenamedFieldDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Vendor", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_RenamedIndexDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { Name = "History", ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_RenumberedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 101,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_ShorterDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _bad_UnblobedAndIndexedMisversionedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = [new() { TagId = 6, Name= "Balance", Version = 1}]},
                ],
        Indexes = [
                    new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" },
                    new() { ElementsAsString = "Blob.Balance" }]
    };

    private static readonly DataModel _baseDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 5, Name= "Blob", IsOpaque = true, Version = 1},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _ok_UnblobedAndIndexedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = [new() { TagId = 6, Name= "Balance", Version = 2}]},
                ],
        Indexes = [
                    new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" },
                    new() { ElementsAsString = "Blob.Balance" }]
    };

    private static readonly DataModel _ok_UnblobedDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = [new() { TagId = 6, Name= "Balance", Version = 2}]},
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };

    private static readonly DataModel _ok_UnblobedWithEnumerationDataModel = new() {
        PayloadName = "Test",
        PayloadTagId = 1,
        DataFields = [
                    new() { TagId = 2, Name= "DateOfPurchase", Cast = CastType.DateTime, Version = 1},
                    new() { TagId = 3, Name= "Buyer", Version = 1,
                        SubDataFields  = [new() { TagId = 4, Name= "Id", Version = 1}] },
                    new() { TagId = 8, Name= "Blob", Version = 2,
                        SubDataFields  = [new() { TagId = 6, Name= "Balance", Version = 2}]},
                    new() { TagId = 10, Name= "DeliveryStatus", Version = 3, Description = "Enumerated Status for Delivery",
                        EnumerationDefinition = new EnumerationDictionary {
                            [0] = new EnumerationDetails("WaitingApproval", "Waiting for sale to be completed (paid)"),
                            [1] = new EnumerationDetails("Canceled", "Sale canceled won't be delivered"),
                            [2] = new EnumerationDetails("Handling", "Locating itens in stock"),
                            [3] = new EnumerationDetails("Sent", "Sent to delivery provider"),
                            [4] = new EnumerationDetails("Done", "Client received the itens")
                        }
                    }
                ],
        Indexes = [new() { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" }]
    };
}