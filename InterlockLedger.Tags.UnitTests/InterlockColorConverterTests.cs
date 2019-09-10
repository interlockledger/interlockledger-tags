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
    public class InterlockColorConverterTests
    {
        [Test]
        public void CanConvertFrom() {
            var converter = new InterlockColorConverter();
            Assert.AreEqual(true, converter.CanConvertFrom(null, typeof(string)));
            Assert.AreEqual(false, converter.CanConvertFrom(null, typeof(int)));
        }

        [Test]
        public void CanConvertTo() {
            var converter = new InterlockColorConverter();
            Assert.AreEqual(true, converter.CanConvertTo(null, typeof(string)));
            Assert.AreEqual(false, converter.CanConvertTo(null, typeof(int)));
        }

        [Test]
        public void ConvertFrom() {
            var converter = new InterlockColorConverter();
            ConvertFromStringAndAssert(converter, "Aqua", InterlockColor.Aqua);
            ConvertFromStringAndAssert(converter, "Aqua ", InterlockColor.Aqua);
            ConvertFromStringAndAssert(converter, "aqua", InterlockColor.Aqua);
            ConvertFromStringAndAssert(converter, "#00ffff", InterlockColor.Aqua);
            ConvertFromStringAndAssert(converter, "#00ffff80", InterlockColor.Aqua.WithA(128));
            ConvertFromStringAndAssert(converter, "#00ffFF", InterlockColor.Aqua);
            ConvertFromStringAndAssert(converter, "#00ffff  ", InterlockColor.Aqua);
            ConvertFromStringAndAssert(converter, "SlateGray", InterlockColor.SlateGray);
            ConvertFromStringAndAssert(converter, "slategray", InterlockColor.SlateGray);
            ConvertFromStringAndAssert(converter, "slateGray", InterlockColor.SlateGray);
            ConvertFromStringAndAssert(converter, "SLATEGRAY", InterlockColor.SlateGray);
            ConvertFromStringAndAssert(converter, "#00QQff", InterlockColor.Transparent);
            ConvertFromStringAndAssert(converter, "Transparent", InterlockColor.Transparent);
            ConvertFromStringAndAssert(converter, "#FFFFFF00", InterlockColor.Transparent);
            ConvertFromStringAndAssert(converter, "Zany", InterlockColor.Transparent);
            ConvertFromStringAndAssert(converter, string.Empty, InterlockColor.Transparent);
            ConvertFromStringAndAssert(converter, "#010203", new InterlockColor(1, 2, 3));
            ConvertFromStringAndAssert(converter, "#01020300", new InterlockColor(1, 2, 3, a: 0));
            ConvertFromStringAndAssert(converter, "#01020380", new InterlockColor(1, 2, 3, a: 128));
            ConvertFromStringAndAssert(converter, "#010203", InterlockColor.From(0x010203FF));
            ConvertFromStringAndAssert(converter, "#010203FF", InterlockColor.From(0x010203FF));
            ConvertFromStringAndAssert(converter, "#01020300", InterlockColor.From(0x01020300));
            ConvertFromStringAndAssert(converter, "#01020380", InterlockColor.From(0x01020380));
            Assert.AreEqual(InterlockColor.Transparent, InterlockColor.FromText(null));
        }

        [Test]
        public void ConvertTo() {
            var converter = new InterlockColorConverter();
            ConvertToStringAndAssert(converter, InterlockColor.Aqua);
            ConvertToStringAndAssert(converter, InterlockColor.Transparent);
            ConvertToStringAndAssert(converter, InterlockColor.From(0x01020390));
            ConvertToStringAndAssert(converter, new InterlockColor(1, 2, 3, a: 128));
        }

        [Test]
        public void TypeDescriptor_GetConverter() {
            var converter = TypeDescriptor.GetConverter(typeof(InterlockColor));
            Assert.IsNotNull(converter);
            Assert.IsInstanceOf<InterlockColorConverter>(converter);
        }

        private static void ConvertFromStringAndAssert(InterlockColorConverter converter, string colorValue, InterlockColor expectedColor) {
            Assert.AreEqual(expectedColor, (InterlockColor)converter.ConvertFrom(null, CultureInfo.InvariantCulture, colorValue));
            Assert.AreEqual(expectedColor, InterlockColor.FromText(colorValue));
        }

        private static void ConvertToStringAndAssert(InterlockColorConverter converter, InterlockColor color)
            => Assert.AreEqual(color.Name, converter.ConvertTo(null, CultureInfo.InvariantCulture, color, typeof(string)));
    }
}