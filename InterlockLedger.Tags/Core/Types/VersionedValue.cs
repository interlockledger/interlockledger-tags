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
public abstract class VersionedValue<T> : IVersion, ITaggableOf<T> where T : notnull, VersionedValue<T>, new()
{
    [JsonIgnore]
    public ILTag AsILTag => AsPayload;

    [JsonIgnore]
    public Payload AsPayload => _payload ??= new Payload((T)this);

    [JsonIgnore]
    public ILTagOf<T> AsTag => AsPayload!;

    [JsonIgnore]
    public DataField FieldModel => _fieldModel.Value;

    [JsonIgnore]
    public abstract string Formatted { get; }

    [JsonIgnore]
    public bool Incomplete => !string.IsNullOrWhiteSpace(IncompletenessReason);

    [JsonIgnore]
    public string? IncompletenessReason { get; private set; }

    [JsonIgnore]
    public virtual bool KeepEncodedBytesInMemory => true;

    [JsonIgnore]
    public DataModel PayloadDataModel => _payloadDataModel.Value;

    [JsonIgnore]
    public ulong TagId { get; }

    public abstract string TypeName { get; }

    public ushort Version { get; set; }

    public T MustBeComplete() =>
        Incomplete
            ? throw new InvalidDataException($"Instance of {typeof(T)} is incompletely deserialized")
            : (T)this;

    public void Changed() => _payload = null;

    public T FromUnknown(ILTagUnknown unknown) {
        if (unknown.Required().TagId != TagId)
            throw new InvalidCastException($"Wrong tagId! Expecting {TagId} but came {unknown.TagId}");
        if (unknown.Value.None())
            throw new ArgumentException("Empty tagged value not expected!", nameof(unknown));
        using var s = new MemoryStream(unknown.Value);
        return FromStream(s);
    }

    public Task<Stream> OpenReadingStreamAsync() => AsPayload.OpenReadingStreamAsync();

    public bool RegisterAsField(ITagRegistrar registrar)
        => registrar.Required()
            .RegisterILTag(TagId, s => new Payload(TagId, s), TagProvider.NoJson);

    public sealed class Payload : ILTagOfExplicit<T>, IVersion, INamed
    {
        public Payload(ulong alreadyDeserializedTagId, Stream s)
            : base(alreadyDeserializedTagId, s)
            => Initialize();

        public Payload(ulong alreadyDeserializedTagId, ReadOnlySequence<byte> bytes)
            : base(alreadyDeserializedTagId, new ReadOnlySequenceStream(bytes), it => SetLength(it, (ulong)bytes.Length))
            => Initialize();

        protected override bool KeepEncodedBytesInMemory => Value!.KeepEncodedBytesInMemory;

        public string TypeName => typeof(T).Name;

        public ushort Version => Value!.Version;

        public override string ToString() => Value?.ToString() ?? "?";

        internal Payload(T value) : base(value.Required().TagId, value) {
        }

        protected override ulong CalcValueLength() => Value!.CalcValueLength();

        protected override T ValueFromStream(WrappedReadonlyStream s) => new T().FromStream(s);

        protected override Stream ValueToStream(Stream s) {
            Value!.ToStream(s);
            return s;
        }

        private void Initialize() {
            if (Value is not null) {
                Traits.ValidateTagId(Value.TagId);
                Value._payload = this;
            }
        }
    }

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

    protected virtual Stream BuildTempStream() => new MemoryStream();

    protected virtual ulong CalcValueLength() {
        using var stream = new MemoryStream();
        ToStream(stream);
        stream.Flush();
        return (ulong)stream.Length;
    }

    protected abstract void DecodeRemainingStateFrom(Stream s);

    protected abstract void EncodeRemainingStateTo(Stream s);

    private readonly Lazy<DataField> _fieldModel;
    private readonly Lazy<DataModel> _payloadDataModel;
    private Payload? _payload;

    private T FromStream(Stream s) {
        try {
            Version = s.DecodeUShort(); // Field index 0 //
            DecodeRemainingStateFrom(s);
        } catch (Exception e) {
            IncompletenessReason = e.ToString();
        }
        return (T)this;
    }

    private void ToStream(Stream s) {
        s.EncodeUShort(Version);    // Field index 0 //
        EncodeRemainingStateTo(s);
    }
}