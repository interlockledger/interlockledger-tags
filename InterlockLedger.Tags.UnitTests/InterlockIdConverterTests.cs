/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    [TestFixture]
    public class InterlockIdConverterTests
    {
        [Test]
        public void CanConvertFrom() {
            var converter = new InterlockIdConverter();
            Assert.AreEqual(true, converter.CanConvertFrom(null, typeof(string)));
            Assert.AreEqual(false, converter.CanConvertFrom(null, typeof(int)));
        }

        [Test]
        public void CanConvertTo() {
            var converter = new InterlockIdConverter();
            Assert.AreEqual(true, converter.CanConvertTo(null, typeof(string)));
            Assert.AreEqual(false, converter.CanConvertTo(null, typeof(int)));
        }

        [Test]
        public void ConvertFrom() {
            var converter = new InterlockIdConverter();
            var id = converter.ConvertFrom(null, CultureInfo.InvariantCulture, "Key!AAA#SHA3_512") as InterlockId;
            Assert.IsNotNull(id);
            Assert.AreEqual("Key!AAA#SHA3_512", id.ToString());
            Assert.AreEqual(HashAlgorithm.SHA3_512, id.Algorithm);
            Assert.AreEqual(new byte[] { 0, 0 }, id.Data);
            Assert.AreEqual(4, id.Type);
        }

        [Test]
        public void ConvertTo() {
            var converter = new InterlockIdConverter();
            Assert.AreEqual("Key!47DEQpj8HBSa-_TImW-5JCeuQeRkm5NMpJWZG3hSuFU",
                converter.ConvertTo(null, CultureInfo.InvariantCulture, new KeyId(TagHash.Empty), typeof(string)));
        }

        [Test]
        public void TypeDescriptor_GetConverter() {
            var converter = TypeDescriptor.GetConverter(typeof(InterlockId));
            Assert.IsNotNull(converter);
            Assert.IsInstanceOf<InterlockIdConverter>(converter);
        }
    }
}