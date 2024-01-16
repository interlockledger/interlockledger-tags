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
public class ILTagTests
{
    [TestCase(new byte[] { 3, 0 }, ExpectedResult = (byte)0, TestName = "AsByte_0")]
    [TestCase(new byte[] { 3, 1 }, ExpectedResult = (byte)1, TestName = "AsByte_1")]
    [TestCase(new byte[] { 3, 255 }, ExpectedResult = (byte)255, TestName = "AsByte_255")]
    public byte AsByte(byte[] bytes) => ((ILTagUInt8)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 16, 3, 1, 2, 3 }, ExpectedResult = new byte[] { 1, 2, 3 }, TestName = "AsByteArray_1_2_3")]
    [TestCase(new byte[] { 16, 1, 2 }, ExpectedResult = new byte[] { 2 }, TestName = "AsByteArray_2")]
    [TestCase(new byte[] { 16, 0 }, ExpectedResult = new byte[0], TestName = "AsByteArray_Zero_Length_Empty_Array")]
    public byte[] AsByteArray(byte[] bytes) => new MemoryStream(bytes).DecodeByteArray();

    [TestCase(new byte[] { 10, 0xF9, 0xFF, 0xFF }, ExpectedResult = 65783ul, TestName = "AsILInt_65783")]
    [TestCase(new byte[] { 10, 0xF8, 0xFF }, ExpectedResult = 503ul, TestName = "AsILInt_503")]
    [TestCase(new byte[] { 10, 32 }, ExpectedResult = 32ul, TestName = "AsILInt_32")]
    public ulong AsILInt(byte[] bytes) => ((ILTagILInt)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 14, 0xF9, 1, 0 }, ExpectedResult = 252, TestName = "AsILIntSigned_252")]
    [TestCase(new byte[] { 14, 0xF8, 0xFF }, ExpectedResult = -252, TestName = "AsILIntSigned_Minus252")]
    [TestCase(new byte[] { 14, 32 }, ExpectedResult = 16, TestName = "AsILIntSigned_16")]
    [TestCase(new byte[] { 14, 31 }, ExpectedResult = -16, TestName = "AsILIntSigned_Minus16")]
    [TestCase(new byte[] { 14, 0 }, ExpectedResult = 0, TestName = "AsILIntSigned_Zero")]
    public long AsILIntSigned(byte[] bytes) => ((ILTagILIntSigned)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 6, 255, 255, 255, 255 }, ExpectedResult = -1, TestName = "AsInt_1")]
    [TestCase(new byte[] { 6, 0, 0, 0, 1 }, ExpectedResult = 0x01000000, TestName = "AsInt_0x01000000")]
    [TestCase(new byte[] { 6, 0, 0, 1, 0 }, ExpectedResult = 0x010000, TestName = "AsInt_0x010000")]
    [TestCase(new byte[] { 6, 0, 1, 0, 0 }, ExpectedResult = 256, TestName = "AsInt_256")]
    [TestCase(new byte[] { 6, 1, 0, 0, 0 }, ExpectedResult = 1, TestName = "AsInt_1")]
    public int AsInt(byte[] bytes) => ((ILTagInt32)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 8, 255, 255, 255, 255, 255, 255, 255, 255 }, ExpectedResult = (long)-1, TestName = "AsLong_1")]
    [TestCase(new byte[] { 8, 0, 0, 0, 0, 1, 0, 0, 0 }, ExpectedResult = 0x0100000000, TestName = "AsLong_0x0100000000")]
    [TestCase(new byte[] { 8, 0, 0, 0, 1, 0, 0, 0, 0 }, ExpectedResult = (long)0x01000000, TestName = "AsLong_0x01000000")]
    [TestCase(new byte[] { 8, 0, 0, 1, 0, 0, 0, 0, 0 }, ExpectedResult = (long)0x010000, TestName = "AsLong_0x010000")]
    [TestCase(new byte[] { 8, 0, 1, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (long)256, TestName = "AsLong_256")]
    [TestCase(new byte[] { 8, 1, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (long)1, TestName = "AsLong_1")]
    public long AsLong(byte[] bytes) => ((ILTagInt64)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 4, 255, 255 }, ExpectedResult = (short)-1, TestName = "AsShort_1")]
    [TestCase(new byte[] { 4, 0, 1 }, ExpectedResult = (short)256, TestName = "AsShort_256")]
    [TestCase(new byte[] { 4, 1, 0 }, ExpectedResult = (short)1, TestName = "AsShort_1")]
    [TestCase(new byte[] { 4, 0, 0 }, ExpectedResult = (short)0, TestName = "AsShort_0")]
    public short AsShort(byte[] bytes) => ((ILTagInt16)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 2, 0 }, ExpectedResult = (sbyte)0, TestName = "AsSignedByte_0")]
    [TestCase(new byte[] { 2, 1 }, ExpectedResult = (sbyte)1, TestName = "AsSignedByte_1")]
    [TestCase(new byte[] { 2, 255 }, ExpectedResult = (sbyte)-1, TestName = "AsSignedByte_1")]
    public sbyte AsSignedByte(byte[] bytes) => ((ILTagInt8)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 17, 6, 0x41, 0xC3, 0xA7, 0xC3, 0xA3, 0x6F }, ExpectedResult = "Ação", TestName = "AsString_Ação")]
    [TestCase(new byte[] { 17, 2, 65, 66 }, ExpectedResult = "AB", TestName = "AsString_AB")]
    [TestCase(new byte[] { 17, 1, 65 }, ExpectedResult = "A", TestName = "AsString_A")]
    [TestCase(new byte[] { 17, 0 }, ExpectedResult = "", TestName = "AsString_NullAsEmpty")]
    public string AsString(byte[] bytes) => TagProvider.DeserializeFrom(new MemoryStream(bytes)).TextualRepresentation;

    [TestCase(new byte[] { 10, 0xF8, 0xFF }, ExpectedResult = "00000000000001F7", TestName = "AsStringMixedTags_ILint:1F7")]
    [TestCase(new byte[] { 17, 6, 0x41, 0xC3, 0xA7, 0xC3, 0xA3, 0x6F }, ExpectedResult = "Ação", TestName = "AsStringMixedTags_Ação")]
    [TestCase(new byte[] { 17, 2, 65, 66 }, ExpectedResult = "AB", TestName = "AsStringMixedTags_AB")]
    [TestCase(new byte[] { 17, 1, 65 }, ExpectedResult = "A", TestName = "AsStringMixedTags_A")]
    [TestCase(new byte[] { 17, 0 }, ExpectedResult = "", TestName = "AsStringMixedTags_NullAsEmpty")]
    public string AsStringMixedTags(byte[] bytes) => TagProvider.DeserializeFrom(new MemoryStream(bytes)).TextualRepresentation;

    [TestCase(new byte[] { 7, 255, 255, 255, 255 }, ExpectedResult = 0xFFFFFFFF, TestName = "AsUInt_0xFFFFFFFF")]
    [TestCase(new byte[] { 7, 0, 0, 0, 1 }, ExpectedResult = (uint)0x01000000, TestName = "AsUInt_0x01000000")]
    [TestCase(new byte[] { 7, 0, 0, 1, 0 }, ExpectedResult = (uint)0x010000, TestName = "AsUInt_0x010000")]
    [TestCase(new byte[] { 7, 0, 1, 0, 0 }, ExpectedResult = (uint)256, TestName = "AsUInt_256")]
    [TestCase(new byte[] { 7, 1, 0, 0, 0 }, ExpectedResult = (uint)1, TestName = "AsUInt_1")]
    public uint AsUInt(byte[] bytes) => ((ILTagUInt32)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 9, 255, 255, 255, 255, 255, 255, 255, 255 }, ExpectedResult = 0xFFFFFFFFFFFFFFFF, TestName = "AsULong_0xFFFFFFFFFFFFFFFF")]
    [TestCase(new byte[] { 9, 0, 0, 0, 0, 1, 0, 0, 0 }, ExpectedResult = (ulong)0x0100000000, TestName = "AsULong_0x0100000000")]
    [TestCase(new byte[] { 9, 0, 0, 0, 1, 0, 0, 0, 0 }, ExpectedResult = (ulong)0x01000000, TestName = "AsULong_0x01000000")]
    [TestCase(new byte[] { 9, 0, 0, 1, 0, 0, 0, 0, 0 }, ExpectedResult = (ulong)0x010000, TestName = "AsULong_0x010000")]
    [TestCase(new byte[] { 9, 0, 1, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (ulong)256, TestName = "AsULong_256")]
    [TestCase(new byte[] { 9, 1, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (ulong)1, TestName = "AsULong_1")]
    public ulong AsULong(byte[] bytes) => ((ILTagUInt64)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 20, 0 }, ExpectedResult = (ulong[])null, TestName = "AsULongArray_null")]
    [TestCase(new byte[] { 20, 1, 0 }, ExpectedResult = new ulong[0], TestName = "AsULongArray_empty")]
    [TestCase(new byte[] { 20, 2, 1, 2 }, ExpectedResult = new ulong[] { 2 }, TestName = "AsULongArray_2")]
    [TestCase(new byte[] { 20, 5, 3, 1, 248, 7, 3 }, ExpectedResult = new ulong[] { 1, 255, 3 }, TestName = "AsULongArray_1_255_3")]
    public ulong[] AsULongArray(byte[] bytes) => ((ILTagArrayOfILInt)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    [TestCase(new byte[] { 5, 255, 255 }, ExpectedResult = (ushort)0xFFFF, TestName = "AsUShort_0xFFFF")]
    [TestCase(new byte[] { 5, 0, 1 }, ExpectedResult = (ushort)256, TestName = "AsUShort_256")]
    [TestCase(new byte[] { 5, 1, 0 }, ExpectedResult = (ushort)1, TestName = "AsUShort_1")]
    [TestCase(new byte[] { 5, 0, 0 }, ExpectedResult = (ushort)0, TestName = "AsUShort_0")]
    public ushort AsUShort(byte[] bytes) => ((ILTagUInt16)TagProvider.DeserializeFrom(new MemoryStream(bytes))).Value;

    //[TestCase(new byte[] { 11, 0, 0, 0, 0 }, ExpectedResult = ILTagId.Binary32, TestName = "DeserializeFrom_Binary32")]
    //[TestCase(new byte[] { 12, 0, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = ILTagId.Binary64, TestName = "DeserializeFrom_Binary64")]
    //[TestCase(new byte[] { 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = ILTagId.Binary128, TestName = "DeserializeFrom_Binary128")]
    //[TestCase(new byte[] { 18, 0 }, ExpectedResult = ILTagId.BigInteger, TestName = "DeserializeFrom_BigInteger")]
    //[TestCase(new byte[] { 19, 0 }, ExpectedResult = ILTagId.BigDecimal, TestName = "DeserializeFrom_BigDecimal")]
    [TestCase(new byte[] { 0 }, ExpectedResult = ILTagId.Null, TestName = "DeserializeFrom_Null")]
    [TestCase(new byte[] { 1, 1 }, ExpectedResult = ILTagId.Bool, TestName = "DeserializeFrom_Bool")]
    [TestCase(new byte[] { 2, 255 }, ExpectedResult = ILTagId.Int8, TestName = "DeserializeFrom_Int8")]
    [TestCase(new byte[] { 3, 255 }, ExpectedResult = ILTagId.UInt8, TestName = "DeserializeFrom_UInt8")]
    [TestCase(new byte[] { 4, 0, 0 }, ExpectedResult = ILTagId.Int16, TestName = "DeserializeFrom_Int16")]
    [TestCase(new byte[] { 5, 0, 0 }, ExpectedResult = ILTagId.UInt16, TestName = "DeserializeFrom_UInt16")]
    [TestCase(new byte[] { 6, 0, 0, 0, 1 }, ExpectedResult = ILTagId.Int32, TestName = "DeserializeFrom_Int32")]
    [TestCase(new byte[] { 7, 0, 0, 0, 1 }, ExpectedResult = ILTagId.UInt32, TestName = "DeserializeFrom_UInt32")]
    [TestCase(new byte[] { 8, 1, 2, 3, 4, 5, 6, 7, 8 }, ExpectedResult = ILTagId.Int64, TestName = "DeserializeFrom_Int64")]
    [TestCase(new byte[] { 9, 1, 2, 3, 4, 5, 6, 7, 8 }, ExpectedResult = ILTagId.UInt64, TestName = "DeserializeFrom_UInt64")]
    [TestCase(new byte[] { 10, 0xF8, 0xFF }, ExpectedResult = ILTagId.ILInt, TestName = "DeserializeFrom_ILInt")]
    [TestCase(new byte[] { 14, 0xF8, 0xFF }, ExpectedResult = ILTagId.ILIntSigned, TestName = "DeserializeFrom_ILIntSigned")]
    [TestCase(new byte[] { 16, 1, 2 }, ExpectedResult = ILTagId.ByteArray, TestName = "DeserializeFrom_ByteArray")]
    [TestCase(new byte[] { 17, 2, 65, 66 }, ExpectedResult = ILTagId.String, TestName = "DeserializeFrom_String")]
    [TestCase(new byte[] { 20, 0 }, ExpectedResult = ILTagId.ILIntArray, TestName = "DeserializeFrom_ILIntArray_Null")]
    [TestCase(new byte[] { 20, 1, 0 }, ExpectedResult = ILTagId.ILIntArray, TestName = "DeserializeFrom_ILIntArray_Empty")]
    public ulong DeserializeFrom(byte[] bytes) {
        using var ms = new MemoryStream(bytes);
        return TagProvider.DeserializeFrom(ms).TagId;
    }

    [Test]
    public void ILTagNullInstanceBytes() => Assert.ByVal(ILTagNull.Instance.EncodedBytes, Is.EquivalentTo(new byte[] { 0 }));

    [TestCase(new byte[] { 0 }, ExpectedResult = true)]
    public bool IsNull(byte[] bytes) {
        var tag = TagProvider.DeserializeFrom(new MemoryStream(bytes));
        return ((ITag)tag).IsNull && !tag.ValueIs<object>(out _);
    }

    [TestCase(252, ExpectedResult = new byte[] { 14, 0xF9, 1, 0 })]
    public byte[] SerializeILIntSigned(long value) => new ILTagILIntSigned(value).EncodedBytes;

    [TestCase(null, ExpectedResult = new byte[] { 20, 0 }, TestName = "SerializeILTagArrayOfILInt_null")]
    [TestCase(new ulong[0], ExpectedResult = new byte[] { 20, 1, 0 }, TestName = "SerializeILTagArrayOfILInt")]
    [TestCase(new ulong[] { 2 }, ExpectedResult = new byte[] { 20, 2, 1, 2 }, TestName = "SerializeILTagArrayOfILInt_2")]
    [TestCase(new ulong[] { 1, 255, 3 }, ExpectedResult = new byte[] { 20, 5, 3, 1, 248, 7, 3 }, TestName = "SerializeILTagArrayOfILInt_1_255_3")]
    public byte[] SerializeILTagArrayOfILInt(ulong[] ilints) => new ILTagArrayOfILInt(ilints).EncodedBytes;

    [TestCase(new byte[0], ExpectedResult = new byte[] { 16, 0 }, TestName = "SerializeILTagByteArray")]
    [TestCase(new byte[] { 2 }, ExpectedResult = new byte[] { 16, 1, 2 }, TestName = "SerializeILTagByteArray_2")]
    [TestCase(new byte[] { 1, 2, 3 }, ExpectedResult = new byte[] { 16, 3, 1, 2, 3 }, TestName = "SerializeILTagByteArray_1_2_3")]
    public byte[] SerializeILTagByteArray(byte[] bytes) => new ILTagByteArray(bytes).EncodedBytes;

    [TestCase(null, ExpectedResult = new byte[] { 17, 0 }, TestName = "SerializeILTagString_null")]
    [TestCase("", ExpectedResult = new byte[] { 17, 0 }, TestName = "SerializeILTagString_empty")]
    [TestCase("A", ExpectedResult = new byte[] { 17, 1, 65 }, TestName = "SerializeILTagString_A")]
    [TestCase("AB", ExpectedResult = new byte[] { 17, 2, 65, 66 }, TestName = "SerializeILTagString_AB")]
    [TestCase("Ação", ExpectedResult = new byte[] { 17, 6, 0x41, 0xC3, 0xA7, 0xC3, 0xA3, 0x6F }, TestName = "SerializeILTagString_Ação")]
    [TestCase("The fool doth think he is wise, but the wise man knows himself to be a fool.",
     ExpectedResult = new byte[] { 17, 76, 0x54, 0x68, 0x65, 0x20, 0x66, 0x6F, 0x6F, 0x6C, 0x20, 0x64, 0x6F, 0x74, 0x68, 0x20, 0x74, 0x68, 0x69, 0x6E, 0x6B, 0x20, 0x68, 0x65, 0x20, 0x69, 0x73, 0x20, 0x77, 0x69, 0x73, 0x65, 0x2C, 0x20, 0x62, 0x75, 0x74, 0x20, 0x74, 0x68, 0x65, 0x20, 0x77, 0x69, 0x73, 0x65, 0x20, 0x6D, 0x61, 0x6E, 0x20, 0x6B, 0x6E, 0x6F, 0x77, 0x73, 0x20, 0x68, 0x69, 0x6D, 0x73, 0x65, 0x6C, 0x66, 0x20, 0x74, 0x6F, 0x20, 0x62, 0x65, 0x20, 0x61, 0x20, 0x66, 0x6F, 0x6F, 0x6C, 0x2E }, TestName = "SerializeILTagString_The_fool_doth_...")]
    public byte[] SerializeILTagString(string value) => new ILTagString(value).EncodedBytes;

    [Test]
    public void ULongAsILintVariations() {
        var tag = (ILTagILInt)TagProvider.DeserializeFrom(new MemoryStream([10, 32]));
        Assert.Multiple(() => {
            Assert.That(tag.Value, Is.EqualTo(32));
            Assert.That(tag.ValueIs<ulong>(out var v) && v == tag.Value, "Not an ulong value");
            Assert.That(tag.ValueIs<ushort>(out var l) || l != default, Is.False, "An ushort value?");
        });
    }

    [Test]
    public void ULongsAsILintArrayVariations() {
        var tag = (ILTagArrayOfILInt)TagProvider.DeserializeFrom(new MemoryStream([20, 5, 3, 1, 248, 7, 3]));
        CollectionAssert.AreEqual(new ulong[] { 1, 255, 3 }, tag.Value);
        Assert.Multiple(() => {
            Assert.That(tag.ValueIs<ulong[]>(out var v) && v.EqualTo(tag.Value), "Not an ulong[] value");
            Assert.That(tag.ValueIs<ulong>(out var l) || l != default, Is.False, "An ushort value?");
            Assert.That(tag.ValueIs<IEnumerable<ulong>>(out var list) && list.EqualTo(tag.Value), "Not an IEnumerable<ulong> value");
        });
    }

    [Test]
    public void UShortVariations() {
        var tag = (ILTagUInt16)TagProvider.DeserializeFrom(new MemoryStream([5, 1, 0]));
        Assert.Multiple(() => {
            Assert.That(tag.Value, Is.EqualTo((ushort)1));
            Assert.That(tag.ValueIs<ushort>(out var v) && v == tag.Value, "Not an ushort value");
            Assert.That(tag.ValueIs<ulong>(out var l) || l != default, Is.False, "An ulong value?");
        });
    }

    [Test]
    public void DeserializeBigArrays() {
        DeserializeFor(32);
        DeserializeFor(1024*256);

        static void DeserializeFor(ulong size) {
            using var stream = new FakeLargeILTagByteArrayStream(size);
            var tag = stream.Decode<ILTagByteArray>();
            Assert.Multiple(() => {
                Assert.That(tag.ValueLength, Is.EqualTo(size));
                using var readStream = tag.OpenReadingStreamAsync().Result;
                var somebytes = readStream.ReadBytes(20);
                Assert.That(somebytes[0], Is.EqualTo((byte)ILTagId.ByteArray));
                CollectionAssert.AreEqual(new byte[10], somebytes[10..]);
            });
            tag.Dispose();
        }
    }
}
