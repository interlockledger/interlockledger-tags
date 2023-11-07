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

using System.Buffers;

namespace InterlockLedger.Tags;

public class CachedTagPart : AbstractDisposable
{
    public ulong TagId { get; private set; }
    public ulong Size { get; private set; }
    public long BodyPosition { get; private set; }
    public Task PerformReadingOfStreamAsync(Func<Stream, Task> performAsync) =>
        DoAsync(() => performAsync(_file.OpenRead()));
    public Task CopyToAsync(Stream destination) =>
        DoAsync(() => _file.OpenRead().CopyToAsync(destination));
    internal static Task<CachedTagPart> ExtractTagPartAsync(Stream source, ulong tagId, ulong size) =>
        new CachedTagPart().DoExtractTagPartAsync(source, tagId, size);
    protected override void DisposeManagedResources() {
        if (_file.Exists)
            _file.Delete();
    }
    private async Task<CachedTagPart> DoExtractTagPartAsync(Stream source, ulong tagId, ulong size) {
        var destination = File.Open(_file.FullName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
        TagId = tagId;
        Size = size;
        destination.ILIntEncode(tagId);
        destination.ILIntEncode(size);
        BodyPosition = destination.Position;
        int bufferSize = (int)ulong.Min(128 * 1024, size);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try {
            ulong totalBytes = 0L;
            while (totalBytes < size) {
                int bytesRead = await source.ReadAsync(buffer.AsMemory(0, bufferSize)).ConfigureAwait(continueOnCapturedContext: false);
                totalBytes += (ulong)bytesRead;
                if (bytesRead == 0) {
                    if (totalBytes < size)
                        throw new IOException($"Could not extract tag part with {size} bytes, only {totalBytes} available");
                    break;
                }

                await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead)).ConfigureAwait(continueOnCapturedContext: false);
                await destination.FlushAsync();
            }
        } finally {
            ArrayPool<byte>.Shared.Return(buffer);
            destination.Close();
        }
        return this;
    }

    private CachedTagPart() {
        _file = new FileInfo(Path.GetTempFileName());
        if (_file.Exists)
            throw new IOException("Temporary file already exists");
    }

    private readonly FileInfo _file;
}
