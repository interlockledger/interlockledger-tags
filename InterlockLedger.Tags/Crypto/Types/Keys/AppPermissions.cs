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

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace InterlockLedger.Tags;
[TypeConverter(typeof(TypeCustomConverter<AppPermissions>))]
[JsonConverter(typeof(JsonCustomConverter<AppPermissions>))]
public partial struct AppPermissions : ITextual<AppPermissions>, IEquatable<AppPermissions>
{
    public static Regex Mask { get; } = PermissionsListRegex();

    public IEnumerable<ulong> ActionIds;

    public ulong AppId;

    public AppPermissions(ulong appId, params ulong[] actionIds) : this(appId, (IEnumerable<ulong>)actionIds) { }

    public AppPermissions(ulong appId, IEnumerable<ulong> actionIds) {
        AppId = appId;
        ActionIds = actionIds ?? Array.Empty<ulong>();
    }

    public AppPermissions(string textualRepresentation) {
        if (string.IsNullOrWhiteSpace(textualRepresentation) || !Mask.IsMatch(textualRepresentation))
            throw new ArgumentException($"Invalid textual representation '{textualRepresentation}'", nameof(textualRepresentation));
        var parts = textualRepresentation[1..].Split(',').AsOrderedUlongs();
        AppId = parts.First();
        ActionIds = parts.Skip(1).ToArray();
    }

    [JsonIgnore]
    public Tag AsTag => new(this);

    [JsonIgnore]
    public string Formatted {
        get {
            var plural = ActionIds.SafeCount() == 1 ? "" : "s";
            return $"App #{AppId} {(_noActions ? "All Actions" : $"Action{plural} {ActionIds.WithCommas(noSpaces: true)}")}";
        }
    }

    public bool IsEmpty => AppId == 0 && ActionIds.None();

    public bool IsInvalid => false;

    [JsonIgnore]
    public string TextualRepresentation => $"#{AppId}{(_noActions ? string.Empty : ",")}{ActionIds.WithCommas(noSpaces: true)}";

    public static bool operator !=(AppPermissions left, AppPermissions right) => !(left == right);

    public static bool operator ==(AppPermissions left, AppPermissions right) => left.Equals(right);

    public bool CanAct(ulong appId, ulong actionId) => appId == AppId && (_noActions || ActionIds.Contains(actionId));

    public bool Equals(AppPermissions other) => other.AppId == AppId && ActionIds.EqualTo(other.ActionIds);

    public override bool Equals(object obj) => obj is AppPermissions other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(AppId, ActionIds);

    public IEnumerable<AppPermissions> ToEnumerable() => new SingleEnumerable<AppPermissions>(this);

    public override string ToString() => TextualRepresentation;

    public class Tag : ILTagExplicit<AppPermissions>
    {
        public Tag(AppPermissions value) : base(ILTagId.InterlockKeyAppPermission, value) {
        }

        internal Tag(Stream s) : base(ILTagId.InterlockKeyAppPermission, s) {
        }

        protected override AppPermissions FromBytes(byte[] bytes) => FromBytesHelper(bytes,
            s => new AppPermissions(s.DecodeILInt(), s.DecodeILIntArray())
        );

        protected override byte[] ToBytes(AppPermissions value)
            => TagHelpers.ToBytesHelper(s => s.EncodeILInt(Value.AppId).EncodeILIntArray(Value.ActionIds));
    }

    private bool _noActions => ActionIds.None();

    public static AppPermissions Empty { get; } = new AppPermissions(0);
    public static AppPermissions Invalid { get; } = new AppPermissions(0);
    public static string MessageForMissing { get; } = "Missing app permissions";

    [GeneratedRegex("^#[0-9]+(,[0-9]+)*$")]
    private static partial Regex PermissionsListRegex();
    public static AppPermissions Parse(string s, IFormatProvider provider) => AppPermissions.FromString(s);
    public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out AppPermissions result) =>
        ITextual<AppPermissions>.TryParse(s, out result);
    public static AppPermissions FromString(string textualRepresentation) => new(textualRepresentation);
    public static string MessageForInvalid(string textualRepresentation) => $"Invalid app permissions '{textualRepresentation}'";
}