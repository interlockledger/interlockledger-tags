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

using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class DataIndexTests
    {
        [Test]
        public void VerifyReversibilityOfElementsAsString() {
            var d = new DataIndex { ElementsAsString = "Field" };
            Assert.AreEqual("Field", d.ElementsAsString);
            Assert.AreEqual(1, d.Elements.SafeCount());
            var firstElement = d.Elements.First();
            Assert.AreEqual("Field", firstElement.FieldPath);
            Assert.AreEqual(false, firstElement.DescendingOrder);
            Assert.IsNull(firstElement.Function);
            d = new DataIndex { ElementsAsString = "Field2:-" };
            Assert.AreEqual("Field2:-", d.ElementsAsString);
            Assert.AreEqual(1, d.Elements.SafeCount());
            firstElement = d.Elements.First();
            Assert.AreEqual("Field2", firstElement.FieldPath);
            Assert.AreEqual(true, firstElement.DescendingOrder);
            Assert.IsNull(firstElement.Function);
            d = new DataIndex { ElementsAsString = "DateOfPurchase:+:Day|Buyer.Id" };
            Assert.AreEqual("DateOfPurchase:+:Day|Buyer.Id", d.ElementsAsString);
            Assert.AreEqual(2, d.Elements.SafeCount());
            firstElement = d.Elements.First();
            Assert.AreEqual("DateOfPurchase", firstElement.FieldPath);
            Assert.AreEqual(false, firstElement.DescendingOrder);
            Assert.AreEqual("Day", firstElement.Function);
            var secondElement = d.Elements.Skip(1).First();
            Assert.AreEqual("Buyer.Id", secondElement.FieldPath);
            Assert.AreEqual(false, secondElement.DescendingOrder);
            Assert.IsNull(secondElement.Function);
        }
    }
}