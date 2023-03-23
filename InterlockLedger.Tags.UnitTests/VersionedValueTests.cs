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

using NUnit.Framework;

using System.Buffers;

namespace InterlockLedger.Tags;
public class VersionedValueTests
{
    [Test]
    public void SerializeDeserialize() {
        var vvt = new VersionedValueForTests("Yeah!");
        var bytes = vvt.AsPayload.EncodedBytes;
        byte[] expectedBytes = new byte[] { 27, 10, 5, 1, 0, 17, 5, (byte)'Y', (byte)'e', (byte)'a', (byte)'h', (byte)'!' };
        CollectionAssert.AreEqual(expectedBytes, bytes);
        TagProvider.RegisterDeserializer(27, s => new VersionedValueForTests.Payload(27, s));
        AssertThings(vvt, (VersionedValueForTests.Payload)TagProvider.DeserializeFrom(new MemoryStream(bytes)));
        AssertThings(vvt, new VersionedValueForTests.Payload(27, new ReadOnlySequence<byte>(bytes[2..])));

        static void AssertThings(VersionedValueForTests vvt, VersionedValue<VersionedValueForTests>.Payload tag) {
            Assert.AreEqual((ushort)1, tag.Version, "Version");
            Assert.AreEqual((ulong)10, tag.ValueLength, "ValueLength");
            var data = tag.Value;
            Assert.AreEqual((ushort)1, data.Version, "Version");
            Assert.AreSame(tag, data.AsPayload, "AsPayload");
            Assert.AreNotSame(vvt, data, "vvt_x_data");
            Assert.AreEqual(vvt.UserMessage, data.UserMessage, "Not the right UserMessage");
        }
    }

    public class VersionedValueForTests : VersionedValue<VersionedValueForTests>
    {
        public VersionedValueForTests() : base(27, 1) { }

        public VersionedValueForTests(string userMessage) : base(27, 1) => UserMessage = userMessage;

        public override object AsJson { get; }
        public override string Formatted => UserMessage;
        public override string TypeName => nameof(VersionedValueForTests);
        public string UserMessage { get; private set; }

        public override VersionedValueForTests FromJson(object json) => throw new System.NotImplementedException();

        protected override IEnumerable<DataField> RemainingStateFields { get; }
        protected override string TypeDescription { get; }

        protected override void DecodeRemainingStateFrom(Stream s) => UserMessage = s.DecodeString();

        protected override void EncodeRemainingStateTo(Stream s) => s.EncodeString(UserMessage);
    }
}