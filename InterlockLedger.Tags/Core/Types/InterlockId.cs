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

[TypeConverter(typeof(TypeCustomConverter<InterlockId>))]
[JsonConverter(typeof(JsonCustomConverter<InterlockId>))]
public partial class InterlockId : ILTagOfExplicit<InterlockId.Parts>, IComparable<InterlockId>, ITextual<InterlockId>
{

    private InterlockId() : this(DefaultType, HashAlgorithm.Invalid, []) { }
    public static IEnumerable<string> AllTypes => Parts.AllTypes;
    public static HashAlgorithm DefaultAlgorithm => Parts.DefaultAlgorithm;

    public static byte DefaultType {
        get => Parts.DefaultType;
        set {
            if (Parts.DefaultType != 0 && Parts.DefaultType != value)
                throw new InvalidOperationException("InterlockId default type is already set");
            Parts.DefaultType = value;
        }
    }

    public HashAlgorithm Algorithm => Value!.Algorithm;
    public byte[]? Data => Value?.Data;
    public byte Type => Value!.Type;
    public static InterlockId Empty { get; } = new(new Parts(DefaultType, TagHash.Empty));
    public static Regex Mask { get; } = InterlockIdRegex();
    public string? InvalidityCause { get; protected init; }
    public bool IsEmpty => Data.EqualTo(TagHash.Empty.Data);
    public int CompareTo(InterlockId? other) => SafeCompare(this, other);
    [JsonIgnore]
    public ITextual<InterlockId> Textual => this;
    [JsonIgnore]
    public string AsBase64 => Value!.Data.Safe().ToSafeBase64();

    public static InterlockId InvalidBy(string cause) => new() { InvalidityCause = cause };
    public static InterlockId Build(string textualRepresentation) => new Parts(textualRepresentation).Resolve();
    public bool Equals(InterlockId? other) => base.Equals(other);
    public sealed override string ToString() => TextualRepresentation;
    public string ToFullString() => Value!.ToFullString();

    public static bool operator <(InterlockId a, InterlockId b) => SafeCompare(a, b) < 0;
    public static bool operator <=(InterlockId a, InterlockId b) => SafeCompare(a, b) <= 0;
    public static bool operator >(InterlockId a, InterlockId b) => SafeCompare(a, b) > 0;
    public static bool operator >=(InterlockId a, InterlockId b) => SafeCompare(a, b) >= 0;
    public static T Resolve<T>(Stream s) where T : InterlockId =>
        s.ILIntDecode() == ILTagId.InterlockId
            ? (T)DeserializeAndResolve(s)
            : throw new InvalidDataException("Not an InterlockId");

    protected InterlockId(string textualRepresentation) : this(new Parts(textualRepresentation)) { }
    protected InterlockId(byte type, HashAlgorithm algorithm, byte[]? data) : this(new Parts(type, algorithm, data)) { }
    protected InterlockId(Parts parts) : base(ILTagId.InterlockId, parts) { }

    private InterlockId(ulong alreadyDeserializedTagId, Stream s) : base(alreadyDeserializedTagId, s) { }
    protected static void RegisterResolver(byte type, string typeName, Func<Parts, InterlockId> resolver) =>
        Parts.RegisterResolver(type, typeName.Required(), resolver.Required());
    protected void CheckType(byte type) {
        if (Type != type)
            throw new InvalidDataException($"This is not a {GetType().Name}");
    }
    internal static InterlockId DeserializeAndResolve(Stream s) =>
        new InterlockId(ILTagId.InterlockId, s).Value!.Resolve();
    private static int SafeCompare(InterlockId? a, InterlockId? b) {
        if (a is null)
            return b is null ? 0 : -1;
        if (b is null)
            return 1;
        var tc = a.Type.CompareTo(b.Type);
        if (tc != 0) return tc;
        var ta = a.Algorithm.CompareTo(b.Algorithm);
        return ta != 0
            ? ta
            : a.Data.None()
                ? b.Data.None()
                    ? 0
                    : -1
                : b.Data.None()
                    ? 1
                    : a.Data.CompareTo(b.Data);
    }

    [GeneratedRegex("""^(\w+\!)?(?:[A-Za-z0-9_-]{4}?)*(?:[A-Za-z0-9_-]{2,3})?(#\w+)?$""")]
    private static partial Regex InterlockIdRegex();
    protected override Parts? ValueFromStream(StreamSpan s) => Parts.FromStream(s);
    protected override Stream ValueToStream(Stream s) => Value!.ToStream(s);
}
