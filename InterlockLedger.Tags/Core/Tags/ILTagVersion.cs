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

[TypeConverter(typeof(TypeCustomConverter<ILTagVersion>))]
[JsonConverter(typeof(JsonCustomConverter<ILTagVersion>))]
public partial class ILTagVersion : ILTagOfExplicit<Version>, ITextual<ILTagVersion>, IComparable<ILTagVersion>
{
    protected override string? BuildTextualRepresentation() => InvalidityCause ?? Value?.ToString();
    public ILTagVersion(Version version) : base(ILTagId.Version, version) { }
    public bool IsEmpty { get; private init; }
    public static ILTagVersion Empty { get; } = new() { IsEmpty = true };
    public static Regex Mask { get; } = Version_Regex();
    public string? InvalidityCause { get; private init; }
    public static ILTagVersion Build(string textualRepresentation) {
        try {
            var parsedVersion = Version.Parse(textualRepresentation);
            return new(parsedVersion);
        } catch (Exception ex) {
            return InvalidBy(ex.Message);
        }
    }
    public static ILTagVersion InvalidBy(string cause) => new() { InvalidityCause = cause };
    public bool Equals(ILTagVersion? other) => base.Equals(other);
    protected override bool AreEquivalent(ILTagOf<Version?> other) => TextualRepresentation == other.TextualRepresentation;
    public static ILTagVersion FromJson(object o) => o is Version version ? new(version) : Build((string)o);
    public ITextual<ILTagVersion> Textual => this;

    public int CompareTo(ILTagVersion? other) => Value?.CompareTo(other?.Value) ?? -1;

    public static bool operator <(ILTagVersion left, ILTagVersion right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;
    public static bool operator <=(ILTagVersion left, ILTagVersion right) =>
        left is null || left.CompareTo(right) <= 0;
    public static bool operator >(ILTagVersion left, ILTagVersion right) =>
        left is not null && left.CompareTo(right) > 0;
    public static bool operator >=(ILTagVersion left, ILTagVersion right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    internal ILTagVersion(Stream s) : base(ILTagId.Version, s) { }
    private static Version BuildVersion(int major, int minor, int build, int revision)
        => revision >= 0
            ? new Version(major, minor, build, revision)
            : build >= 0
                ? new Version(major, minor, build)
                : new Version(major, minor);

    [GeneratedRegex("""^\d+(\.\d+){1,3}$""")]
    private static partial Regex Version_Regex();

    protected override Version? ValueFromStream(Stream s) =>
        BuildVersion(s.BigEndianReadInt(), s.BigEndianReadInt(), s.BigEndianReadInt(), s.BigEndianReadInt());
    protected override Stream ValueToStream(Stream s) =>
        s.BigEndianWriteInt(Value!.Major)
         .BigEndianWriteInt(Value.Minor)
         .BigEndianWriteInt(Value.Build)
         .BigEndianWriteInt(Value.Revision);

    private ILTagVersion() : this(Version.Parse("0.0.0.0".AsSpan())) { }
}