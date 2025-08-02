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

namespace InterlockLedger.Tags;

public class StreamSpanTests
{
    [Test]
    public void TestAutoSkippingToEndOfSpanOnDispose() {
        using var baseStream = new MemoryStream(new byte[100]);
        _ = baseStream.Seek(10, SeekOrigin.Begin);
        Assert.That(baseStream.Position, Is.EqualTo(10L));
        baseStream.WriteByte(30);
        _ = baseStream.Seek(10, SeekOrigin.Begin);
        Assert.That(baseStream.Position, Is.EqualTo(10L));
        using (var sp = new StreamSpan(baseStream, (ulong)baseStream.ReadByte())) {
            Assert.That(sp.Length, Is.EqualTo(30L));
            Assert.Multiple(() => {
                Assert.That(sp.Position, Is.EqualTo(0L));
                Assert.That(baseStream.Position, Is.EqualTo(11L));
                Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
            });
            _ = sp.ReadBytes(20);
            Assert.Multiple(() => {
                Assert.That(sp.Position, Is.EqualTo(20L));
                Assert.That(baseStream.Position, Is.EqualTo(31L));
                Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
            });
            if (sp.CanSeek) {
                sp.Position = 30;
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(41L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                sp.Position = 5;
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(16L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                _ = sp.Seek(30, SeekOrigin.Begin);
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(41L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                sp.Position = 5;
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(16L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                _ = sp.Seek(0, SeekOrigin.End);
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(41L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                sp.Position = 5;
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(16L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                _ = sp.Seek(25, SeekOrigin.Current);
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(41L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                sp.Position = 5;
                Assert.Multiple(() => {
                    Assert.That(baseStream.Position, Is.EqualTo(16L));
                    Assert.That(sp.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                });
                using var ss = new StreamSpan(sp, 20);
                Assert.Multiple(() => {
                    Assert.That(ss.Position, Is.EqualTo(0L));
                    Assert.That(sp.Position, Is.EqualTo(5L));
                    Assert.That(ss.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                    Assert.That(baseStream.Position, Is.EqualTo(16L));
                });
                _ = ss.Seek(8, SeekOrigin.Current);
                Assert.Multiple(() => {
                    Assert.That(ss.Position, Is.EqualTo(8L));
                    Assert.That(sp.Position, Is.EqualTo(13L));
                    Assert.That(ss.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
                    Assert.That(baseStream.Position, Is.EqualTo(24L));
                });
            }
        }

        Assert.That(baseStream.Position, Is.EqualTo(41L));
        using (var sp2 = new StreamSpan(baseStream, (ulong)(baseStream.Length - baseStream.Position))) {
            Assert.Multiple(() => {
                Assert.That(sp2.Position, Is.EqualTo(0L));
                Assert.That(baseStream.Position, Is.EqualTo(41L));
                Assert.That(sp2.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
            });
            _ = sp2.ReadBytes(20);
            Assert.Multiple(() => {
                Assert.That(sp2.Position, Is.EqualTo(20L));
                Assert.That(baseStream.Position, Is.EqualTo(61L));
                Assert.That(sp2.PositionOnOriginalStream, Is.EqualTo(baseStream.Position));
            });
        }

        Assert.That(baseStream.Position, Is.EqualTo(baseStream.Length));
    }

    [Test]
    public void RejectNonSeekableOriginalStream() {
        var e = Assert.Throws<ArgumentException>(() => new StreamSpan(new NonSeekMemoryStream([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]), 10));
        Assert.That(e?.Message, Is.EqualTo("original stream needs to be seekable"));
    }

    [Test]
    public void RejectNegativeOriginOnOriginalStream() {
        var e = Assert.Throws<ArgumentOutOfRangeException>(() => new StreamSpan(new MemoryStream([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]), -1, 10));
        Assert.That(e?.Message, Does.Contain("offset ('-1') must be a non-negative value. (Parameter 'offset')"));
    }

    [Test]
    public void RejectNullOriginalStream() {
        var e = Assert.Throws<ArgumentException>(() => new StreamSpan(null!, 10));
        Assert.That(e?.Message, Is.EqualTo("Required (Parameter 's')"));
    }
}