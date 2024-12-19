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
