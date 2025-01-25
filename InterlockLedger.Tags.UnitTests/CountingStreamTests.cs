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

using System;
using System.IO;
using NUnit.Framework;

namespace InterlockLedger.Tags;

[TestFixture]
public class CountingStreamTests
{
    [Test]
    public void CanRead_ShouldReturnFalse()
    {
        // Arrange
        var stream = new CountingStream();

        // Act
        var canRead = stream.CanRead;

        // Assert
        Assert.That(canRead, Is.False);
    }

    [Test]
    public void CanSeek_ShouldReturnFalse()
    {
        // Arrange
        var stream = new CountingStream();

        // Act
        var canSeek = stream.CanSeek;

        // Assert
        Assert.That(canSeek, Is.False);
    }

    [Test]
    public void CanWrite_ShouldReturnTrue()
    {
        // Arrange
        var stream = new CountingStream();

        // Act
        var canWrite = stream.CanWrite;

        // Assert
        Assert.That(canWrite, Is.True);
    }

    [Test]
    public void Length_ShouldReturnZeroInitially()
    {
        // Arrange
        var stream = new CountingStream();

        // Act
        var length = stream.Length;

        // Assert
        Assert.That(length, Is.EqualTo(0));
    }

    [Test]
    public void Position_ShouldBeSettable()
    {
        // Arrange
        var stream = new CountingStream();
        var newPosition = 10L;

        // Act
        stream.Position = newPosition;

        // Assert
        Assert.That(stream.Position, Is.EqualTo(newPosition));
    }

    [Test]
    public void Flush_ShouldNotThrow()
    {
        // Arrange
        var stream = new CountingStream();

        // Act & Assert
        Assert.DoesNotThrow(() => stream.Flush());
    }

    [Test]
    public void Read_ShouldThrowNotSupportedException()
    {
        // Arrange
        var stream = new CountingStream();
        var buffer = new byte[10];

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => stream.Read(buffer, 0, buffer.Length));
    }

    [Test]
    public void Seek_ShouldThrowNotSupportedException()
    {
        // Arrange
        var stream = new CountingStream();

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
    }

    [Test]
    public void SetLength_ShouldThrowNotSupportedException()
    {
        // Arrange
        var stream = new CountingStream();

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => stream.SetLength(10));
    }

    [Test]
    public void Write_ShouldIncreaseLength()
    {
        // Arrange
        var stream = new CountingStream();
        var buffer = new byte[10];

        // Act
        stream.Write(buffer, 0, buffer.Length);

        // Assert
        Assert.That(stream.Length, Is.EqualTo(buffer.Length));
    }
}
