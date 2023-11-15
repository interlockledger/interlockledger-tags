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

namespace InterlockLedger.Tags;
public enum Preload
{
    DontCare,

    //May,
    Must
}

public class RecordLoader<T, TV> : AbstractDisposable, IRecordLoader<T, TV> where T : PreloadableTag<T, TV>, new() where TV : notnull
{
    public RecordLoader(Func<Stream> getReadStream, Func<ulong, ulong, ulong, ulong, Preload> shouldPreload, Stream s, ulong payloadTagId = 0, ulong applicationId = 0) {
        _offset = s.Position;
        _container = null;
        ulong tagId = s.ILIntDecode();
        if (tagId != _tagId)
            throw new InvalidDataException($"Expecting a container tagId {_tagId} (for {typeof(T).FullName}) but came {tagId}");
        ulong length = s.ILIntDecode();
        long headSize = s.Position - _offset;
        _getReadStream = getReadStream.Required();
        PayloadTagId = payloadTagId;
        ApplicationId = applicationId;
        PreLoadContainer(s);
        switch (shouldPreload.Required()((ulong)_offset, length, ApplicationId, PayloadTagId)) {
        case Preload.DontCare:
            _container = null;
            CanUnload = true;
            break;

        case Preload.Must:
            CanUnload = false;
            break;

        default:
            break;
        }
        if (payloadTagId != 0)
            s.Dispose();
        else {
            long nextOffset = _offset + headSize + (long)length;
            if (s.Position != nextOffset)
                _ = s.Seek(nextOffset, SeekOrigin.Begin);
        }
    }


    public ulong ApplicationId { get; private set; }
    public ulong PayloadTagId { get; private set; }
    public bool CanUnload { get; set; }

    public T? Load(Stream s) {
        if (_container is null)
            PreLoadContainer(s);
        var record = _container;
        if (CanUnload)
            _container = null;
        return record;
    }

    Stream IPreloadableTagStorage.PayloadReadStream {
        get {
            if (_preloadOffset <= _offset)
                throw new InvalidOperationException("TagStorage not correctly Marked");
            var source = _getReadStream();
            return new StreamSpan(source, _preloadOffset, (ulong)_preloadLength, closeWrappedStreamOnDispose: true);
        }
    }
    Stream IPreloadableTagStorage.CopyTo(Stream s) {
        using var span = ((IPreloadableTagStorage)this).PayloadReadStream;
        span.CopyTo(s);
        return s;
    }
    void IPreloadableTagStorage.Mark(StreamSpan s) {
        _preloadOffset = s.OriginalPosition;
        _preloadLength = s.Length;
    }

    protected override void DisposeManagedResources() => _container?.Dispose();

    private readonly long _offset;
    private readonly Func<Stream> _getReadStream;
    private long _preloadLength;
    private long _preloadOffset;
    private T? _container;
    private static ulong _tagId { get; } = new T().TagId;

    private void PreLoadContainer(Stream s) {
        if (s.Position != _offset)
            _ = s.Seek(_offset, SeekOrigin.Begin);
        _container = DecodePreloading(s, this);
        if (_container is not null) {
            PayloadTagId = _container.PayloadTagId;
            ApplicationId = _container.ApplicationId;
        }

        static T DecodePreloading(Stream s, IPreloadableTagStorage storage) {
            ulong tagId = s.DecodeTagId();
            var tag = new T().Preload(s, storage);
            return tag is null || tagId != tag.TagId ? throw new InvalidDataException($"Not a {typeof(T).Name}") : tag;
        }
    }
}
