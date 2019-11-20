/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class IEnumerableOfTExtensionsTests
    {
        [Test]
        public void AppendedOf() {
            Assert.That(() => ((object)null).AppendedOf(null), Is.EquivalentTo(new object[] { null }), "Null concat Null should be equal to [Null]");
            Assert.That(() => ((object)null).AppendedOf(Enumerable.Empty<object>()), Is.EquivalentTo(new object[] { null }), "Null concat Empty should be equal to [Null]");
            Assert.That(() => ((string)null).AppendedOf(new string[] { "A" }), Is.EquivalentTo(new string[] { null, "A" }), "Null concat [A] should be equal to [Null, A]");
            Assert.That(() => 1.AppendedOf(null), Is.EquivalentTo(new int[] { 1 }), "1 concat Null should be equal to [1]");
            Assert.That(() => 1.AppendedOf((IEnumerable<int>)new int[] { 2, 3 }), Is.EquivalentTo(new int[] { 1, 2, 3 }), "1 concat [2,3] should be equal to [1, 2, 3]");
        }

        [Test]
        public void AppendedOfParams() {
            Assert.That(() => ((object)null).AppendedOf(), Is.EquivalentTo(new object[] { null }), "Null concat Empty should be equal to [Null]");
            Assert.That(() => ((string)null).AppendedOf("A"), Is.EquivalentTo(new string[] { null, "A" }), "Null concat [A] should be equal to [Null, A]");
            Assert.That(() => 1.AppendedOf(), Is.EquivalentTo(new int[] { 1 }), "1 concat Empty should be equal to [1]");
            Assert.That(() => 1.AppendedOf(2, 3), Is.EquivalentTo(new int[] { 1, 2, 3 }), "1 concat [2,3] should be equal to [1, 2, 3]");
        }

        [Test]
        public void SafeConcat() {
            Assert.That(() => ((IEnumerable<object>)null).SafeConcat(null), Is.Empty, "Null concat Null should be equal to Empty");
            Assert.That(() => ((IEnumerable<object>)null).SafeConcat(Enumerable.Empty<object>()), Is.Empty, "Null concat Empty should be equal to Empty");
            Assert.That(() => (Enumerable.Empty<object>()).SafeConcat(null), Is.Empty, "Empty concat Null should be equal to Empty");
            Assert.That(() => (Enumerable.Empty<object>()).SafeConcat(Enumerable.Empty<object>()), Is.Empty, "Empty concat Empty should be equal to Empty");
            Assert.That(() => ((IEnumerable<int>)null).SafeConcat(new int[] { 1 }), Is.EquivalentTo(new int[] { 1 }), "Null concat [1] should be equal to [1]");
            Assert.That(() => new int[] { 1 }.SafeConcat(new int[] { 2, 3 }), Is.EquivalentTo(new int[] { 1, 2, 3 }), "[1] concat [2,3] should be equal to [1, 2, 3]");
        }
    }
}