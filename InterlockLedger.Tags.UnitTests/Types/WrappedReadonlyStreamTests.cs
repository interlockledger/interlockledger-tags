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

internal class DisposableMemoryStream(byte[] bytes) : MemoryStream(bytes)
{
    public bool Disposed { get; private set; }
    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        Disposed = true;
    }
}
public class WrappedReadonlyStreamTests
{
    [Test]
    public void RejectNonSeekableOriginalStream() {
        var e = Assert.Throws<ArgumentException>(() => new WrappedReadonlyStream(new NonSeekMemoryStream([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]), 1, 10, closeWrappedStreamOnDispose: false));
        Assert.That(e?.Message, Is.EqualTo("original stream needs to be seekable"));
    }

    [Test]
    public void RejectNegativeOriginOnOriginalStream() {
        var e = Assert.Throws<ArgumentOutOfRangeException>(() => new WrappedReadonlyStream(new MemoryStream([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]), -1, 10, closeWrappedStreamOnDispose: false));
        StringAssert.Contains("offset ('-1') must be a non-negative value. (Parameter 'offset')", e?.Message);
    }
    [Test]
    public void AssertDisposalOfOriginalStreamOnCondition() {
        var ms = new DisposableMemoryStream([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
        using (var wrs = new WrappedReadonlyStream(ms, 1, 10, closeWrappedStreamOnDispose: false)) {
            Assert.That(wrs.Length, Is.EqualTo(10L));
            CollectionAssert.AreEqual(new byte[] { 2, 3, 4 }, wrs.ReadBytes(3));
        }
        Assert.That(ms.Position, Is.EqualTo(4L));
        Assert.False(ms.Disposed, "Original stream was disposed");
        using (var wrs = new WrappedReadonlyStream(ms, 0, -1, closeWrappedStreamOnDispose: false)) {
            Assert.That(wrs.Length, Is.EqualTo(12L));
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, wrs.ReadBytes(4));
        }
        Assert.That(ms.Position, Is.EqualTo(4L));
        Assert.False(ms.Disposed, "Original stream was disposed");
        using (var wrs = new WrappedReadonlyStream(ms)) {
            Assert.That(wrs.Length, Is.EqualTo(8L));
            CollectionAssert.AreEqual(new byte[] { 5, 6, 7, 8, 9 }, wrs.ReadBytes(5));
        }
        Assert.That(ms.Position, Is.EqualTo(9L));
        Assert.False(ms.Disposed, "Original stream was disposed");
        using (var wrs = new WrappedReadonlyStream(ms, 1, 10, closeWrappedStreamOnDispose: true)) {
            Assert.That(wrs.Length, Is.EqualTo(10L));
            wrs.Position = 5;
            byte[] buffer = new byte[6];
            int count = wrs.Read(buffer, 0, buffer.Length);
            Assert.That(count, Is.EqualTo(5));
            CollectionAssert.AreEqual(new byte[] { 7, 8, 9, 10, 11, 0 }, buffer);
            Assert.That(ms.Position, Is.EqualTo(11L));
        }
        Assert.True(ms.Disposed, "Original stream was not disposed");
    }

    [Test]
    public void RejectNullOriginalStream() {
        var e = Assert.Throws<ArgumentException>(() => new WrappedReadonlyStream(null!, 0, 10, closeWrappedStreamOnDispose: false));
        Assert.That(e?.Message, Is.EqualTo("Required (Parameter 's')"));
    }
}
