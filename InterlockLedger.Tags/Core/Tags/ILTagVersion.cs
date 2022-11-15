// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
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

#nullable enable


namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<ILTagVersion>))]
[JsonConverter(typeof(JsonCustomConverter<ILTagVersion>))]
public partial class ILTagVersion : ILTagExplicit<Version>, ITextual<ILTagVersion>, IEquatable<ILTagVersion>
{

    public ILTagVersion(Version version) : base(ILTagId.Version, version) => TextualRepresentation = version.ToString();

    public override object AsJson => TextualRepresentation;

    public bool IsEmpty { get; }
    public bool IsInvalid { get; }
    public string TextualRepresentation { get; }
    public static ILTagVersion Empty { get; } = new();
    public static Regex Mask { get; } = Version_Regex();
    public static string MessageForMissing => "Version is missing";
    public string? InvalidityCause { get; }

    public static ILTagVersion Parse(string s, IFormatProvider? provider) => FromString(s);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ILTagVersion result) =>
        ITextual<ILTagVersion>.TryParse(s, out result);

    public static ILTagVersion FromString(string textualRepresentation) {
        try {
            var parsedVersion = Version.Parse(textualRepresentation);
            return new(parsedVersion);
        } catch (Exception ex) {
            return new(ex.Message);
        }
    }

    public static string MessageForInvalid(string? textualRepresentation) => $"Invalid version '{textualRepresentation}'";
    public bool EqualsForValidInstances(ILTagVersion other) => TextualRepresentation == other.TextualRepresentation;
    public static ILTagVersion InvalidBy(string cause) => new(cause);

    public static ILTagVersion FromJson(object o) => FromString((string)o);

    public static bool operator !=(ILTagVersion left, ILTagVersion right) => !(left == right);

    public static bool operator ==(ILTagVersion left, ILTagVersion right) => left?.Equals(right) ?? right is null;

    public override bool Equals(object? obj) => Equals(obj as ILTagVersion);

    public bool Equals(ILTagVersion? other) => _traits.EqualsForAnyInstances(other ?? Empty);

    public override int GetHashCode() => HashCode.Combine(TextualRepresentation);
    public override string Formatted => $"{TextualRepresentation}{_traits.InvalidityCoda}";
    private ITextual<ILTagVersion> _traits => this;
    private ILTagVersion(string? invalidityCause) : this(BlankVersion) {
        TextualRepresentation = "?";
        IsInvalid = true;
        InvalidityCause = invalidityCause;
    }

    private ILTagVersion() : this(BlankVersion) {
        TextualRepresentation = string.Empty;
        IsEmpty = true;
    }

    public static Version BlankVersion => _blankVersion ??= Version.Parse("0.0.0.0".AsSpan());
    private static Version? _blankVersion;

    internal ILTagVersion(Stream s) : base(ILTagId.Version, s) => TextualRepresentation = Value.ToString();

    protected override Version FromBytes(byte[] bytes) {
        return FromBytesHelper(bytes, s => Build(s.BigEndianReadInt(), s.BigEndianReadInt(), s.BigEndianReadInt(), s.BigEndianReadInt()));

        static Version Build(int major, int minor, int build, int revision)
            => revision >= 0
                ? new Version(major, minor, build, revision)
                : build >= 0
                    ? new Version(major, minor, build)
                    : new Version(major, minor);
    }

    protected override byte[] ToBytes(Version value) =>
        TagHelpers.ToBytesHelper(s => s.BigEndianWriteInt(value.Major)
                                       .BigEndianWriteInt(value.Minor)
                                       .BigEndianWriteInt(value.Build)
                                       .BigEndianWriteInt(value.Revision));

    [GeneratedRegex("""^\d+(\.\d+){1,3}$""")]
    private static partial Regex Version_Regex();
}