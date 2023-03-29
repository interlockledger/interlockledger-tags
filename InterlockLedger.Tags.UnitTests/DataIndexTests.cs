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
public class DataIndexTests
{
    [Test]
    public void VerifyReversibilityOfElementsAsString() {
        var d = new DataIndex { ElementsAsString = "Field" };
        Assert.Multiple(() => {
            Assert.That(d.ElementsAsString, Is.EqualTo("Field"));
            Assert.That(d.Elements.SafeCount(), Is.EqualTo(1));
        });
        var firstElement = d.Elements.First();
        Assert.Multiple(() => {
            Assert.That(firstElement.FieldPath, Is.EqualTo("Field"));
            Assert.That(firstElement.DescendingOrder, Is.EqualTo(false));
            Assert.That(firstElement.Function, Is.Null);
        });
        d = new DataIndex { ElementsAsString = "Field2:-" };
        Assert.Multiple(() => {
            Assert.That(d.ElementsAsString, Is.EqualTo("Field2:-"));
            Assert.That(d.Elements.SafeCount(), Is.EqualTo(1));
        });
        firstElement = d.Elements.First();
        Assert.Multiple(() => {
            Assert.That(firstElement.FieldPath, Is.EqualTo("Field2"));
            Assert.That(firstElement.DescendingOrder, Is.EqualTo(true));
            Assert.That(firstElement.Function, Is.Null);
        });
        d = new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" };
        Assert.Multiple(() => {
            Assert.That(d.ElementsAsString, Is.EqualTo("DateOfPurchase:+:Day|Buyer.Id"));
            Assert.That(d.Elements.SafeCount(), Is.EqualTo(2));
        });
        firstElement = d.Elements.First();
        Assert.Multiple(() => {
            Assert.That(firstElement.FieldPath, Is.EqualTo("DateOfPurchase"));
            Assert.That(firstElement.DescendingOrder, Is.EqualTo(false));
            Assert.That(firstElement.Function, Is.EqualTo("Day"));
        });
        var secondElement = d.Elements.Skip(1).First();
        Assert.Multiple(() => {
            Assert.That(secondElement.FieldPath, Is.EqualTo("Buyer.Id"));
            Assert.That(secondElement.DescendingOrder, Is.EqualTo(false));
            Assert.That(secondElement.Function, Is.Null);
        });
    }
}