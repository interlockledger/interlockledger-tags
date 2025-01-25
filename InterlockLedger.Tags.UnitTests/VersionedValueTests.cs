// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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

using System.Buffers;

namespace InterlockLedger.Tags;
public class VersionedValueTests
{
    [Test]
    public void SerializeDeserialize() {
        var vvt = new VersionedValueForTests("Yeah!");
        var payload = vvt.AsPayload; // creates payload from vvt
        try {
            var bytes = payload.EncodedBytes();
            byte[] expectedBytes = [27, 10, 5, 1, 0, 17, 5, (byte)'Y', (byte)'e', (byte)'a', (byte)'h', (byte)'!'];
            Assert.That(bytes, Is.EqualTo(expectedBytes).AsCollection);
            TagProvider.RegisterDeserializer(27, s => new VersionedValueForTests.Payload(27, s));
            AssertThings(vvt, (VersionedValueForTests.Payload)TagProvider.DeserializeFrom(new MemoryStream(bytes)));
            AssertThings(vvt, new VersionedValueForTests.Payload(27, new ReadOnlySequence<byte>(bytes[2..])));
        } finally {
            vvt.Dispose();
            Assert.That(payload.Disposed);
        }

        static void AssertThings(VersionedValueForTests vvt, VersionedValue<VersionedValueForTests>.Payload tag) {
            var data = tag.Value;
            Assert.That(data, Is.Not.Null);
            try {
                Assert.Multiple(() => {
                    Assert.That(data.Version, Is.EqualTo((ushort)1), "Version");
                    Assert.That(tag.ValueLength, Is.EqualTo((ulong)10), "ValueLength");
                });
                Assert.Multiple(() => {
                    Assert.That(data.Version, Is.EqualTo((ushort)1), "Version");
                    Assert.That(data.AsPayload, Is.SameAs(tag), "AsPayload");
                    Assert.That(data, Is.Not.SameAs(vvt), "vvt_x_data");
                    Assert.That(data.UserMessage, Is.EqualTo(vvt.UserMessage), "Not the right UserMessage");
                });
            } finally {
                tag.Dispose();
                Assert.Multiple(() => {
                    Assert.That(data.Disposed);
                    Assert.That(!vvt.Disposed);
                });
            }

        }
    }

    public class VersionedValueForTests : VersionedValue<VersionedValueForTests>
    {
        public VersionedValueForTests() : base(27, 1) { }

        public VersionedValueForTests(string userMessage) : base(27, 1) => UserMessage = userMessage;

        public string UserMessage { get; private set; }
        protected override IEnumerable<DataField> RemainingStateFields { get; }
        protected override string TypeDescription { get; }
        protected override Task DecodeRemainingStateFromAsync(Stream s) {
            UserMessage = s.DecodeString();
            return Task.CompletedTask;
        }
        protected override Task EncodeRemainingStateToAsync(Stream s) {
            s.EncodeString(UserMessage);
            return Task.CompletedTask;
        }
    }
}