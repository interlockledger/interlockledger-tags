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
#pragma warning disable CA2231 // Overload operator equals on overriding value type Equals
public partial struct AppPermissions : ITextual<AppPermissions>, IEquatable<AppPermissions>
#pragma warning restore CA2231 // Overload operator equals on overriding value type Equals
{
    public ulong AppId;
    public IEnumerable<ulong> ActionIds;

    public AppPermissions() : this(ulong.MaxValue) { }

    public AppPermissions(ulong appId, params ulong[] actionIds) : this(appId, (IEnumerable<ulong>)actionIds) { }

    public AppPermissions(ulong appId, IEnumerable<ulong> actionIds) {
        AppId = appId;
        ActionIds = actionIds ?? Array.Empty<ulong>();
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
    public string InvalidityCause { get; init; }
    public string TextualRepresentation => Textual.IsInvalid
        ? $"#?{Textual.InvalidityCause}"
        : $"#{AppId}{(_noActions ? string.Empty : ",")}{ActionIds.WithCommas(noSpaces: true)}";

    public static AppPermissions Empty { get; } = new AppPermissions(0);
    public static Regex Mask { get; } = PermissionsListRegex();

    public static AppPermissions FromString(string textualRepresentation) {
        if (textualRepresentation.IsBlank() || !Mask.IsMatch(textualRepresentation))
            throw new ArgumentException($"Invalid textual representation '{textualRepresentation}'", nameof(textualRepresentation));
        var parts = textualRepresentation[1..].Split(',').AsOrderedUlongs();
        return new AppPermissions(parts.First(), parts.Skip(1));
    }

     public bool EqualsForValidInstances(AppPermissions other) => other.AppId == AppId && ActionIds.EqualTo(other.ActionIds);

    public override bool Equals(object obj) => obj is AppPermissions other && Equals(other);
    public bool Equals(AppPermissions other) => Textual.EqualForAnyInstances(other);
    public ITextual<AppPermissions> Textual => this;
    public override int GetHashCode() => HashCode.Combine(AppId, ActionIds);
    public override string ToString() => Textual.FullRepresentation;

    private bool _noActions => ActionIds.None();

    [GeneratedRegex("^#[0-9]+(,[0-9]+)*$")]
    private static partial Regex PermissionsListRegex();
}