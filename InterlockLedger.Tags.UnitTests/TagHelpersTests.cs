/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class TagHelpersTests
    {
        [Test]
        public void DecodedTest() {
            var tag = new ILTagILInt(14);
            var decoded = tag.EncodedBytes.Decoded();
            Assert.AreEqual(tag, decoded);
        }

        [Test]
        public void ToBytesHelperTest() {
            var tag = new ILTagILInt(23);
            var bytes = TagHelpers.ToBytesHelper((s) => s.EncodeILInt(23));
            Assert.IsNotNull(bytes);
            Assert.That(tag.EncodedBytes, Is.EquivalentTo(bytes));
            var e = Assert.Throws<ArgumentNullException>(() => TagHelpers.ToBytesHelper(null));
            Assert.AreEqual("serialize", e.ParamName);
        }

        [Test]
        public void ToTagArrayFromTest() {
            var data = new string[] { "1", "2", "3", "5", "7", "11" };
            var tagArray = data.ToTagArrayFrom(v => new ILTagString(v));
            Assert.IsNotNull(tagArray);
            Assert.AreEqual(data.Length, tagArray.Value.Length);
            Assert.That(tagArray.GetValues<string>(), Is.EquivalentTo(data));
            var e = Assert.Throws<ArgumentNullException>(() => data.ToTagArrayFrom(null));
            Assert.AreEqual("convert", e.ParamName);
        }

        [Test]
        public void ToTagArrayTest() {
            var data = new ulong[] { 1, 2, 3, 5, 7, 11 };
            var tagArray = data.ToTagArray(v => new ILTagILInt(v));
            Assert.IsNotNull(tagArray);
            Assert.AreEqual(data.Length, tagArray.Value.Length);
            Assert.That(tagArray.GetValues<ulong>(), Is.EquivalentTo(data));
            var e = Assert.Throws<ArgumentNullException>(() => data.ToTagArray<ulong, ILTagILInt>(null));
            Assert.AreEqual("convert", e.ParamName);
        }
    }
}