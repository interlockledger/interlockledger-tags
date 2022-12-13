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

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<AppPermissions>))]
[JsonConverter(typeof(JsonCustomConverter<AppPermissions>))]
public partial struct AppPermissions : ITextual<AppPermissions>, IComparable<AppPermissions>
{
    public ulong AppId;
    public IEnumerable<ulong> ActionIds;

    public AppPermissions(ulong appId, params ulong[] actionIds) : this(appId, (IEnumerable<ulong>)actionIds) { }

    public AppPermissions(ulong appId, IEnumerable<ulong> actionIds) {
        AppId = appId;
        ActionIds = actionIds ?? Array.Empty<ulong>();
        TextualRepresentation = BuildTextualRepresentation();
    }

    public bool CanAct(ulong appId, ulong actionId) => appId == AppId && (_noActions || ActionIds.Contains(actionId));
    public IEnumerable<AppPermissions> ToEnumerable() => new SingleEnumerable<AppPermissions>(this);

    public Tag AsTag => new(this);

    public string VerboseRepresentation {
        get {
            var plural = ActionIds.SafeCount() == 1 ? "" : "s";
            return $"App #{AppId} {(_noActions ? "All Actions" : $"Action{plural} {ActionIds.WithCommas(noSpaces: true)}")}";
        }
    }
    public bool IsEmpty => AppId == 0 && ActionIds.None();
    public string? InvalidityCause { get; private init; }
    public string TextualRepresentation { get; private init; }
    private string BuildTextualRepresentation() => Textual.IsInvalid
            ? $"#?{InvalidityCause}"
            : $"#{AppId}{(_noActions ? string.Empty : ",")}{ActionIds.WithCommas(noSpaces: true)}";

    public static AppPermissions Empty { get; } = new AppPermissions(0);
    public static Regex Mask { get; } = PermissionsListRegex();
    private static readonly string _invalidTextualRepresentation = "#?";
    public static AppPermissions InvalidBy(string cause) =>
        new(ulong.MaxValue) { InvalidityCause = cause, TextualRepresentation = _invalidTextualRepresentation };
    public static AppPermissions Build(string textualRepresentation) {
        if (textualRepresentation.FirstOrDefault() != '#')
            InvalidBy(Mask.InvalidityByNotMatching(textualRepresentation));
        var parts = textualRepresentation[1..].Split(',').AsOrderedUlongs();
        return new AppPermissions(parts.First(), parts.Skip(1));
    }
    public bool Equals(AppPermissions other) => other.AppId == AppId && ActionIds.EqualTo(other.ActionIds);

    public override bool Equals(object? obj) => obj is AppPermissions other && Textual.Equals(other);
    public ITextual<AppPermissions> Textual => this;
    public override int GetHashCode() => HashCode.Combine(AppId, ActionIds);
    public override string ToString() => Textual.FullRepresentation;
    public static bool operator ==(AppPermissions left, AppPermissions right) =>
         left.Equals(right);
    public static bool operator !=(AppPermissions left, AppPermissions right) =>
        !(left == right);
    public static bool operator <(AppPermissions left, AppPermissions right) =>
        left.CompareTo(right) < 0;
    public static bool operator <=(AppPermissions left, AppPermissions right) =>
        left.CompareTo(right) <= 0;
    public static bool operator >(AppPermissions left, AppPermissions right) =>
        left.CompareTo(right) > 0;
    public static bool operator >=(AppPermissions left, AppPermissions right) =>
        left.CompareTo(right) >= 0;
    private bool _noActions => ActionIds.None();


    [GeneratedRegex("^#[0-9]+(,[0-9]+)*$")]
    private static partial Regex PermissionsListRegex();
    public int CompareTo(AppPermissions other) => Equals(other) ? 0 : AppId.CompareTo(other.AppId);

}