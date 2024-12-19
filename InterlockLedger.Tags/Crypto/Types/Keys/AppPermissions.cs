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

namespace InterlockLedger.Tags;

[TypeConverter(typeof(TypeCustomConverter<AppPermissions>))]
[JsonConverter(typeof(JsonCustomConverter<AppPermissions>))]
public partial class AppPermissions : ITextual<AppPermissions>, IComparable<AppPermissions>
{
    public ulong AppId { get; }
    public IEnumerable<ulong> ActionIds { get; }
    public string? InvalidityCause { get; private init; }
    public string TextualRepresentation { get; }
    public Tag AsTag => new(this);
    public string VerboseRepresentation {
        get {
            var plural = ActionIds.SafeCount() == 1 ? "" : "s";
            return $"App #{AppId} {(_noActions ? "All Actions" : $"Action{plural} {ActionIds.WithCommas(noSpaces: true)}")}";
        }
    }
    public bool IsEmpty => AppId == 0 && ActionIds.None();
    public static AppPermissions Empty { get; } = new AppPermissions(0);
    public static Regex Mask { get; } = PermissionsListRegex();

    public AppPermissions(ulong appId, params ulong[] actionIds) : this(appId, (IEnumerable<ulong>)actionIds) { }

    public AppPermissions(ulong appId, IEnumerable<ulong> actionIds) {
        AppId = appId;
        ActionIds = (actionIds ?? []).Order();
        _noActions = ActionIds.None();
        _actionsSum = ActionIds.Sum(i => (int)i);
        TextualRepresentation = $"#{AppId}{(_noActions ? string.Empty : ",")}{ActionIds.WithCommas(noSpaces: true)}";
    }

    private AppPermissions(string invalidityCause) {
        AppId = ulong.MaxValue;
        ActionIds = [];
        _noActions = true;
        _actionsSum = 0;
        InvalidityCause = invalidityCause.Required();
        TextualRepresentation = $"#?{InvalidityCause}";
    }

    public static AppPermissions InvalidBy(string cause) => new(cause);

    public bool CanAct(ulong appId, ulong actionId) => appId == AppId && (_noActions || ActionIds.Contains(actionId));

    public IEnumerable<AppPermissions> ToEnumerable() => new SingleEnumerable<AppPermissions>(this);


    public static AppPermissions Parse(string s, IFormatProvider? provider) {
        if (s.IsBlank() || s[0] != '#')
            return InvalidBy(Mask.InvalidityByNotMatching(s));
        if (s.Length > 2 && s[1] == '?')
            return new AppPermissions(invalidityCause: s[2..]);
        var parts = s[1..].Split(',').AsOrderedUlongs();
        return new AppPermissions(parts.First(), parts.Skip(1));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out AppPermissions result) {
        result = Parse(s.Safe(), provider);
        return !result.IsInvalid();
    }

    public bool Equals(AppPermissions? other) => other is not null && other.AppId == AppId && ActionIds.EqualTo(other.ActionIds);

    public override bool Equals(object? obj) => obj is AppPermissions other && Textual.Equals(other);

    public ITextual<AppPermissions> Textual => this;

    public int CompareTo(AppPermissions? other) => Equals(other) ? 0 : other is null ? 1 : AppId.CompareTo(other.AppId);

    public override int GetHashCode() => HashCode.Combine(AppId, _actionsSum, InvalidityCause);

    public override string ToString() => Textual.FullRepresentation();

    public static bool operator ==(AppPermissions left, AppPermissions right) => left.Equals(right);

    public static bool operator !=(AppPermissions left, AppPermissions right) => !(left == right);

    public static bool operator <(AppPermissions left, AppPermissions right) => left.CompareTo(right) < 0;

    public static bool operator <=(AppPermissions left, AppPermissions right) => left.CompareTo(right) <= 0;

    public static bool operator >(AppPermissions left, AppPermissions right) => left.CompareTo(right) > 0;

    public static bool operator >=(AppPermissions left, AppPermissions right) => left.CompareTo(right) >= 0;

    private readonly bool _noActions;
    private readonly int _actionsSum;

    [GeneratedRegex("^#[0-9]+(,[0-9]+)*$")]
    private static partial Regex PermissionsListRegex();

    public class Tag : ILTagOfExplicit<AppPermissions>
    {
        public Tag(AppPermissions value) : base(ILTagId.InterlockKeyAppPermission, value) {
            if (value.Textual.IsInvalid())
                throw new InvalidOperationException(value.ToString());
        }

        internal Tag(Stream s) : base(ILTagId.InterlockKeyAppPermission, s) {
        }

        protected override Task<AppPermissions?> ValueFromStreamAsync(WrappedReadonlyStream s) => Task.FromResult<AppPermissions?>(new(s.DecodeILInt(), s.DecodeILIntArray()));

        protected override Task<Stream> ValueToStreamAsync(Stream s) => Task.FromResult(s.EncodeILInt(Value!.AppId).EncodeILIntArray(Value.ActionIds));
    }
}
