/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.IO;
using NUnit.Framework;

namespace InterlockLedger.Tags
{

    [TestFixture]
    public class ILTagTests
    {
        [TestCase(new byte[] { 3, 0 }, ExpectedResult = (byte)0, TestName = "AsByte 0")]
        [TestCase(new byte[] { 3, 1 }, ExpectedResult = (byte)1, TestName = "AsByte 1")]
        [TestCase(new byte[] { 3, 255 }, ExpectedResult = (byte)255, TestName = "AsByte 255")]
        public byte AsByte(byte[] bytes) => ((ILTagUInt8)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 16, 3, 1, 2, 3 }, ExpectedResult = new byte[] { 1, 2, 3 }, TestName = "AsByteArray [1,2,3]")]
        [TestCase(new byte[] { 16, 1, 2 }, ExpectedResult = new byte[] { 2 }, TestName = "AsByteArray [2]")]
        [TestCase(new byte[] { 16, 0 }, ExpectedResult = new byte[0], TestName = "AsByteArray Zero Length => Empty Array")]
        public byte[] AsByteArray(byte[] bytes) => new MemoryStream(bytes).DecodeByteArray();

        [TestCase(new byte[] { 10, 0xF9, 0xFF, 0xFF }, ExpectedResult = 65783, TestName = "AsILInt 65783")]
        [TestCase(new byte[] { 10, 0xF8, 0xFF }, ExpectedResult = 503, TestName = "AsILInt 503")]
        [TestCase(new byte[] { 10, 32 }, ExpectedResult = 32, TestName = "AsILInt 32")]
        public ulong AsILInt(byte[] bytes) => ((ILTagILInt)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 6, 255, 255, 255, 255 }, ExpectedResult = -1, TestName = "AsInt -1")]
        [TestCase(new byte[] { 6, 0, 0, 0, 1 }, ExpectedResult = 0x01000000, TestName = "AsInt 0x01000000")]
        [TestCase(new byte[] { 6, 0, 0, 1, 0 }, ExpectedResult = 0x010000, TestName = "AsInt 0x010000")]
        [TestCase(new byte[] { 6, 0, 1, 0, 0 }, ExpectedResult = 256, TestName = "AsInt 256")]
        [TestCase(new byte[] { 6, 1, 0, 0, 0 }, ExpectedResult = 1, TestName = "AsInt 1")]
        public int AsInt(byte[] bytes) => ((ILTagInt32)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 8, 255, 255, 255, 255, 255, 255, 255, 255 }, ExpectedResult = (long)-1, TestName = "AsLong -1")]
        [TestCase(new byte[] { 8, 0, 0, 0, 0, 1, 0, 0, 0 }, ExpectedResult = 0x0100000000, TestName = "AsLong 0x0100000000")]
        [TestCase(new byte[] { 8, 0, 0, 0, 1, 0, 0, 0, 0 }, ExpectedResult = (long)0x01000000, TestName = "AsLong 0x01000000")]
        [TestCase(new byte[] { 8, 0, 0, 1, 0, 0, 0, 0, 0 }, ExpectedResult = (long)0x010000, TestName = "AsLong 0x010000")]
        [TestCase(new byte[] { 8, 0, 1, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (long)256, TestName = "AsLong 256")]
        [TestCase(new byte[] { 8, 1, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (long)1, TestName = "AsLong 1")]
        public long AsLong(byte[] bytes) => ((ILTagInt64)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 4, 255, 255 }, ExpectedResult = (short)-1, TestName = "AsShort -1")]
        [TestCase(new byte[] { 4, 0, 1 }, ExpectedResult = (short)256, TestName = "AsShort 256")]
        [TestCase(new byte[] { 4, 1, 0 }, ExpectedResult = (short)1, TestName = "AsShort 1")]
        [TestCase(new byte[] { 4, 0, 0 }, ExpectedResult = (short)0, TestName = "AsShort 0")]
        public short AsShort(byte[] bytes) => ((ILTagInt16)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 2, 0 }, ExpectedResult = (byte)0, TestName = "AsSignedByte 0")]
        [TestCase(new byte[] { 2, 1 }, ExpectedResult = (byte)1, TestName = "AsSignedByte 1")]
        [TestCase(new byte[] { 2, 255 }, ExpectedResult = (sbyte)(-1), TestName = "AsSignedByte -1")]
        public sbyte AsSignedByte(byte[] bytes) => ((ILTagInt8)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 17, 6, 0x41, 0xC3, 0xA7, 0xC3, 0xA3, 0x6F }, ExpectedResult = "Ação", TestName = "AsString 'Ação'")]
        [TestCase(new byte[] { 17, 2, 65, 66 }, ExpectedResult = "AB", TestName = "AsString 'AB'")]
        [TestCase(new byte[] { 17, 1, 65 }, ExpectedResult = "A", TestName = "AsString 'A'")]
        [TestCase(new byte[] { 17, 0 }, ExpectedResult = "", TestName = "AsString ''")]
        public string AsString(byte[] bytes) => ILTag.DeserializeFrom(new MemoryStream(bytes)).AsString();

        [TestCase(new byte[] { 10, 0xF8, 0xFF }, ExpectedResult = "ILTagILInt#10:00000000000001F7", TestName = "AsStringMixedTags(ILint) 'ILTagILInt#10:00000000000001F7'")]
        [TestCase(new byte[] { 17, 6, 0x41, 0xC3, 0xA7, 0xC3, 0xA3, 0x6F }, ExpectedResult = "Ação", TestName = "AsStringMixedTags 'Ação'")]
        [TestCase(new byte[] { 17, 2, 65, 66 }, ExpectedResult = "AB", TestName = "AsStringMixedTags 'AB'")]
        [TestCase(new byte[] { 17, 1, 65 }, ExpectedResult = "A", TestName = "AsStringMixedTags 'A'")]
        [TestCase(new byte[] { 17, 0 }, ExpectedResult = "", TestName = "AsStringMixedTags ''")]
        public string AsStringMixedTags(byte[] bytes) => ILTag.DeserializeFrom(new MemoryStream(bytes)).AsString();

        [TestCase(new byte[] { 7, 255, 255, 255, 255 }, ExpectedResult = 0xFFFFFFFF, TestName = "AsUInt 0xFFFFFFFF")]
        [TestCase(new byte[] { 7, 0, 0, 0, 1 }, ExpectedResult = (uint)0x01000000, TestName = "AsUInt 0x01000000")]
        [TestCase(new byte[] { 7, 0, 0, 1, 0 }, ExpectedResult = (uint)0x010000, TestName = "AsUInt 0x010000")]
        [TestCase(new byte[] { 7, 0, 1, 0, 0 }, ExpectedResult = (uint)256, TestName = "AsUInt 256")]
        [TestCase(new byte[] { 7, 1, 0, 0, 0 }, ExpectedResult = (uint)1, TestName = "AsUInt 1")]
        public uint AsUInt(byte[] bytes) => ((ILTagUInt32)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 9, 255, 255, 255, 255, 255, 255, 255, 255 }, ExpectedResult = 0xFFFFFFFFFFFFFFFF, TestName = "AsULong 0xFFFFFFFFFFFFFFFF")]
        [TestCase(new byte[] { 9, 0, 0, 0, 0, 1, 0, 0, 0 }, ExpectedResult = (ulong)0x0100000000, TestName = "AsULong 0x0100000000")]
        [TestCase(new byte[] { 9, 0, 0, 0, 1, 0, 0, 0, 0 }, ExpectedResult = (ulong)0x01000000, TestName = "AsULong 0x01000000")]
        [TestCase(new byte[] { 9, 0, 0, 1, 0, 0, 0, 0, 0 }, ExpectedResult = (ulong)0x010000, TestName = "AsULong 0x010000")]
        [TestCase(new byte[] { 9, 0, 1, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (ulong)256, TestName = "AsULong 256")]
        [TestCase(new byte[] { 9, 1, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = (ulong)1, TestName = "AsULong 1")]
        public ulong AsULong(byte[] bytes) => ((ILTagUInt64)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 20, 0 }, ExpectedResult = null, TestName = "AsULongArray null")]
        [TestCase(new byte[] { 20, 1, 0 }, ExpectedResult = new ulong[0], TestName = "AsULongArray empty")]
        [TestCase(new byte[] { 20, 2, 1, 2 }, ExpectedResult = new ulong[] { 2 }, TestName = "AsULongArray [2]")]
        [TestCase(new byte[] { 20, 5, 3, 1, 248, 7, 3 }, ExpectedResult = new ulong[] { 1, 255, 3 }, TestName = "AsULongArray [1, 255, 3]")]
        public ulong[] AsULongArray(byte[] bytes) => ((ILTagArrayOfILInt)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 5, 255, 255 }, ExpectedResult = (ushort)0xFFFF, TestName = "AsUShort 0xFFFF")]
        [TestCase(new byte[] { 5, 0, 1 }, ExpectedResult = (ushort)256, TestName = "AsUShort 256")]
        [TestCase(new byte[] { 5, 1, 0 }, ExpectedResult = (ushort)1, TestName = "AsUShort 1")]
        [TestCase(new byte[] { 5, 0, 0 }, ExpectedResult = (ushort)0, TestName = "AsUShort 0")]
        public ushort AsUShort(byte[] bytes) => ((ILTagUInt16)ILTag.DeserializeFrom(new MemoryStream(bytes))).Value;

        [TestCase(new byte[] { 0 }, ExpectedResult = ILTagId.Null, TestName = "DeserializeFrom(Null)")]
        [TestCase(new byte[] { 1, 1 }, ExpectedResult = ILTagId.Bool, TestName = "DeserializeFrom(Bool)")]
        [TestCase(new byte[] { 2, 255 }, ExpectedResult = ILTagId.Int8, TestName = "DeserializeFrom(Int8)")]
        [TestCase(new byte[] { 3, 255 }, ExpectedResult = ILTagId.UInt8, TestName = "DeserializeFrom(UInt8)")]
        [TestCase(new byte[] { 4, 0, 0 }, ExpectedResult = ILTagId.Int16, TestName = "DeserializeFrom(Int16)")]
        [TestCase(new byte[] { 5, 0, 0 }, ExpectedResult = ILTagId.UInt16, TestName = "DeserializeFrom(UInt16)")]
        [TestCase(new byte[] { 6, 0, 0, 0, 1 }, ExpectedResult = ILTagId.Int32, TestName = "DeserializeFrom(Int32)")]
        [TestCase(new byte[] { 7, 0, 0, 0, 1 }, ExpectedResult = ILTagId.UInt32, TestName = "DeserializeFrom(UInt32)")]
        [TestCase(new byte[] { 8, 1, 2, 3, 4, 5, 6, 7, 8 }, ExpectedResult = ILTagId.Int64, TestName = "DeserializeFrom(Int64)")]
        [TestCase(new byte[] { 9, 1, 2, 3, 4, 5, 6, 7, 8 }, ExpectedResult = ILTagId.UInt64, TestName = "DeserializeFrom(UInt64)")]
        [TestCase(new byte[] { 10, 0xF8, 0xFF }, ExpectedResult = ILTagId.ILInt, TestName = "DeserializeFrom(ILInt)")]
        //[TestCase(new byte[] { 11, 0, 0, 0, 0 }, ExpectedResult = ILTagId.Binary32, TestName = "DeserializeFrom(Binary32)")]
        //[TestCase(new byte[] { 12, 0, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = ILTagId.Binary64, TestName = "DeserializeFrom(Binary64)")]
        //[TestCase(new byte[] { 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, ExpectedResult = ILTagId.Binary128, TestName = "DeserializeFrom(Binary128)")]
        [TestCase(new byte[] { 16, 1, 2 }, ExpectedResult = ILTagId.ByteArray, TestName = "DeserializeFrom(ByteArray)")]
        [TestCase(new byte[] { 17, 2, 65, 66 }, ExpectedResult = ILTagId.String, TestName = "DeserializeFrom(String)")]
        //[TestCase(new byte[] { 18, 0 }, ExpectedResult = ILTagId.BigInteger, TestName = "DeserializeFrom(BigInteger)")]
        //[TestCase(new byte[] { 19, 0 }, ExpectedResult = ILTagId.BigDecimal, TestName = "DeserializeFrom(BigDecimal)")]
        [TestCase(new byte[] { 20, 0 }, ExpectedResult = ILTagId.ILIntArray, TestName = "DeserializeFrom(ILIntArray) Null")]
        [TestCase(new byte[] { 20, 1, 0 }, ExpectedResult = ILTagId.ILIntArray, TestName = "DeserializeFrom(ILIntArray) Empty")]
        public ulong DeserializeFrom(byte[] bytes) {
            using var ms = new MemoryStream(bytes);
            return ILTag.DeserializeFrom(ms).TagId;
        }

        [Test]
        public void ILTagNullInstanceBytes() => Assert.ByVal(ILTagNull.Instance.EncodedBytes, Is.EquivalentTo(new byte[] { 0 }));

        [TestCase(new byte[] { 0 }, ExpectedResult = true)]
        public bool IsNull(byte[] bytes) => ILTag.DeserializeFrom(new MemoryStream(bytes)).IsNull;

        [TestCase(null, ExpectedResult = new byte[] { 20, 0 }, TestName = "SerializeILTagArrayOfILInt null")]
        [TestCase(new ulong[0], ExpectedResult = new byte[] { 20, 1, 0 }, TestName = "SerializeILTagArrayOfILInt []")]
        [TestCase(new ulong[] { 2 }, ExpectedResult = new byte[] { 20, 2, 1, 2 }, TestName = "SerializeILTagArrayOfILInt [2]")]
        [TestCase(new ulong[] { 1, 255, 3 }, ExpectedResult = new byte[] { 20, 5, 3, 1, 248, 7, 3 }, TestName = "SerializeILTagArrayOfILInt [1, 255, 3]")]
        public byte[] SerializeILTagArrayOfILInt(ulong[] ilints) => new ILTagArrayOfILInt(ilints).EncodedBytes;

        [TestCase(null, ExpectedResult = new byte[] { 16, 0 }, TestName = "SerializeILTagByteArray null")]
        [TestCase(new byte[0], ExpectedResult = new byte[] { 16, 0 }, TestName = "SerializeILTagByteArray []")]
        [TestCase(new byte[] { 2 }, ExpectedResult = new byte[] { 16, 1, 2 }, TestName = "SerializeILTagByteArray [2]")]
        [TestCase(new byte[] { 1, 2, 3 }, ExpectedResult = new byte[] { 16, 3, 1, 2, 3 }, TestName = "SerializeILTagByteArray [1, 2, 3]")]
        public byte[] SerializeILTagByteArray(byte[] bytes) => new ILTagByteArray(bytes).EncodedBytes;

        [TestCase(null, ExpectedResult = new byte[] { 17, 0 }, TestName = "SerializeILTagString null")]
        [TestCase("", ExpectedResult = new byte[] { 17, 0 }, TestName = "SerializeILTagString empty")]
        [TestCase("A", ExpectedResult = new byte[] { 17, 1, 65 }, TestName = "SerializeILTagString 'A'")]
        [TestCase("AB", ExpectedResult = new byte[] { 17, 2, 65, 66 }, TestName = "SerializeILTagString 'AB'")]
        [TestCase("Ação", ExpectedResult = new byte[] { 17, 6, 0x41, 0xC3, 0xA7, 0xC3, 0xA3, 0x6F }, TestName = "SerializeILTagString 'Ação'")]
        [TestCase("The fool doth think he is wise, but the wise man knows himself to be a fool.",
         ExpectedResult = new byte[] { 17, 76, 0x54, 0x68, 0x65, 0x20, 0x66, 0x6F, 0x6F, 0x6C, 0x20, 0x64, 0x6F, 0x74, 0x68, 0x20, 0x74, 0x68, 0x69, 0x6E, 0x6B, 0x20, 0x68, 0x65, 0x20, 0x69, 0x73, 0x20, 0x77, 0x69, 0x73, 0x65, 0x2C, 0x20, 0x62, 0x75, 0x74, 0x20, 0x74, 0x68, 0x65, 0x20, 0x77, 0x69, 0x73, 0x65, 0x20, 0x6D, 0x61, 0x6E, 0x20, 0x6B, 0x6E, 0x6F, 0x77, 0x73, 0x20, 0x68, 0x69, 0x6D, 0x73, 0x65, 0x6C, 0x66, 0x20, 0x74, 0x6F, 0x20, 0x62, 0x65, 0x20, 0x61, 0x20, 0x66, 0x6F, 0x6F, 0x6C, 0x2E }, TestName = "SerializeILTagString 'The fool doth ...'")]
        public byte[] SerializeILTagString(string value) => new ILTagString(value).EncodedBytes;

    }
}