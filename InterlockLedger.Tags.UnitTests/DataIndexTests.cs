/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

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
