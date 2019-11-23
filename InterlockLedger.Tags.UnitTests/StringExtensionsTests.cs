/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void WithSuffix() {
            Assert.IsNull(((string)null).WithSuffix(".json"));
            Assert.AreEqual(".json", "".WithSuffix(".json"));
            Assert.AreEqual("a.json", "a".WithSuffix(".json"));
            Assert.AreEqual("b.JSON", "b.JSON".WithSuffix(".json"));
            Assert.AreEqual("c", "c".WithSuffix(null));
            Assert.AreEqual("c", "c".WithSuffix(""));
            Assert.AreEqual("c", "c".WithSuffix("     "));
            Assert.AreEqual("d!KKK", "d".WithSuffix("KKK", '!'));
            Assert.AreEqual("d!KKK", "d".WithSuffix("!KKK", '!'));
            Assert.AreEqual("d!KKK!", "d".WithSuffix("!!KKK!", '!'));
            Assert.AreEqual("e#001", "e# ".WithSuffix("001", '#'));
            Assert.AreEqual("file.txt", "file ".WithSuffix("txt"));
            Assert.AreEqual(".file.txt", ".file ".WithSuffix("txt", '.'));
            Assert.AreEqual("file.txt", "file. ".WithSuffix("txt"));
        }
    }
}