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

using System.Buffers;

namespace InterlockLedger.Tags;
public abstract partial class VersionedValue<T> : AbstractDisposable, IVersion, ICacheableTag where T : notnull, VersionedValue<T>, new()
{
    public sealed class Payload : ILTagOfExplicit<T>
    {
        public Payload(ulong alreadyDeserializedTagId, Stream s)
            : base(alreadyDeserializedTagId, s)
            => Initialize();

        public Payload(ulong alreadyDeserializedTagId, ReadOnlySequence<byte> bytes)
            : base(alreadyDeserializedTagId, new ReadOnlySequenceStream(bytes), it => SetLength(it, (ulong)bytes.Length))
            => Initialize();

        internal Payload(T value) : base(value.Required().TagId, value) { }

        internal bool DisposeValue = false;
        private void Initialize() {
            if (Value is not null) {
                Traits.ValidateTagId(Value.TagId);
                Value._payload = this;
                DisposeValue = true;
            }
        }
        protected override void DisposeManagedResources() {
            if (Value is not null) {
                Value._payload = null;
                if (DisposeValue) {
                    Value.Dispose();
                    Value = null;
                }
            }
        }

        protected override async Task<T?> ValueFromStreamAsync(WrappedReadonlyStream s) =>
            await new T().FromStreamAsync(s).ConfigureAwait(false);
        protected override async Task<Stream> ValueToStreamAsync(Stream s) {
            await Value!.ToStreamAsync(s).ConfigureAwait(false);
            return s;
        }
    }

    [JsonIgnore]
    public Payload AsPayload => _payload ??= new Payload((T)this);

    [JsonIgnore]
    public DataField FieldModel => _fieldModel.Value;

    [JsonIgnore]
    public DataModel PayloadDataModel => _payloadDataModel.Value;

    [JsonIgnore]
    public ulong TagId { get; }

    [JsonIgnore]
    public virtual string TypeName => GetType().Name;

    public ushort Version { get; set; }

    private int _cachedLength;
    private IMemoryOwner<byte>? _memoryOwner;

    public Task CacheFromAsync(Stream source) => CacheFromInternalAsync(source, prefixed: false);
    public Task CachePrefixedFromAsync(Stream source) => CacheFromInternalAsync(source, prefixed: true);

    private async Task CacheFromInternalAsync(Stream source, bool prefixed) {
        if (_memoryOwner is null && !Disposed) {
            ulong length = (ulong)source.Length;
            int prefixSize = TagId.ILIntSize() + length.ILIntSize();
            ulong totalLength = length + (ulong)(prefixed ? prefixSize : 0);
            if (totalLength > int.MaxValue)
                throw new OutOfMemoryException("Tag too big to cache");
            _cachedLength = (int)totalLength;
            _memoryOwner = MemoryPool<byte>.Shared.Rent(_cachedLength);
            var buffer = _memoryOwner.Memory;
            int i = 0;
            if (prefixed) {
                TagId.ILIntEncode(b => buffer.Span[i++] = b);
                length.ILIntEncode(b => buffer.Span[i++] = b);
                if (i != prefixSize)
                    throw new InvalidDataException("Bad prefix size on caching");
            }
            source.Seek(0, SeekOrigin.Begin);
            var readBytes = await source.ReadAsync(buffer[i..]).ConfigureAwait(false);
            if (readBytes != source.Length)
                throw new InvalidOperationException("Could not read all expected bytes from tag to cache");
        }
    }

    public async Task<Stream> OpenCachedReadingStreamAsync() {
        if (Disposed || _memoryOwner is null || _cachedLength <= 0) {
            return new MemoryStream([], writable: false);
        }
        var ms = new MemoryStream(_cachedLength);
        await ms.WriteAsync(_memoryOwner.Memory[0.._cachedLength]).ConfigureAwait(false);
        ms.Position = 0;
        return ms;
    }

    protected override void DisposeManagedResources() {
        _memoryOwner?.Dispose();
        _memoryOwner = null;
        _cachedLength = 0;
        if (_payload is not null && !_payload.DisposeValue) {
            _payload.Dispose();
            _payload = null;
        }
    }

    #region Incompleteness
    [JsonIgnore]
    public bool Incomplete => !string.IsNullOrWhiteSpace(IncompletenessReason);

    [JsonIgnore]
    public string? IncompletenessReason { get; private set; }

    public T MustBeComplete() =>
        Incomplete
            ? throw new InvalidDataException($"Instance of {typeof(T)} is incompletely deserialized")
            : (T)this;
    #endregion

    public async Task<T> FromUnknownAsync(ILTagUnknown unknown) {
        if (unknown.Required().TagId != TagId)
            throw new InvalidCastException($"Wrong tagId! Expecting {TagId} but came {unknown.TagId}");
        if (unknown.Value.None())
            throw new ArgumentException("Empty tagged value not expected!", nameof(unknown));
        using var s = new MemoryStream(unknown.Value);
        return await FromStreamAsync(s);
    }

    public Task<Stream> OpenReadingStreamAsync() => Disposed ? throw new InvalidOperationException("Already disposed") : AsPayload.OpenReadingStreamAsync();

    public bool RegisterAsField(ITagRegistrar registrar)
        => registrar.Required()
            .RegisterILTag(TagId, s => new Payload(TagId, s), TagProvider.NoJson);

    protected static readonly DataField VersionField = new(nameof(Version), ILTagId.UInt16);

    protected bool FromVersion(ushort version) => Version >= version;

    protected VersionedValue(ulong tagId, ushort version) {
        TagId = tagId;
        Version = version;
        _fieldModel = new Lazy<DataField>(() => new DataField(TypeName, TagId, TypeDescription) {
            SubDataFields = VersionField.AppendedOf(RemainingStateFields)
        });
        _payloadDataModel = new Lazy<DataModel>(() => new DataModel {
            PayloadName = TypeName,
            PayloadTagId = TagId,
            DataFields = VersionField.AppendedOf(RemainingStateFields),
            Indexes = PayloadIndexes
        });
    }

    protected virtual DataIndex[]? PayloadIndexes => null;

    protected abstract IEnumerable<DataField> RemainingStateFields { get; }

    protected abstract string TypeDescription { get; }

    protected abstract Task DecodeRemainingStateFromAsync(Stream s);

    protected abstract Task EncodeRemainingStateToAsync(Stream s);

    private readonly Lazy<DataField> _fieldModel;
    private readonly Lazy<DataModel> _payloadDataModel;
    private Payload? _payload;

    private async Task<T> FromStreamAsync(Stream s) {
        try {
            Version = s.DecodeUShort(); // Field index 0 //
            await DecodeRemainingStateFromAsync(s).ConfigureAwait(false);
        } catch (Exception e) {
            IncompletenessReason = e.ToString();
        }
        return (T)this;
    }

    private async Task ToStreamAsync(Stream s) {
        s.EncodeUShort(Version);    // Field index 0 //
        await EncodeRemainingStateToAsync(s).ConfigureAwait(false);
    }

}