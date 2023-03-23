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

public class InterlockKey : ILTagExplicit<InterlockKey.Parts>, IEquatable<InterlockKey>, IBaseKey
{
    public InterlockKey(KeyPurpose[] purposes, string name, TagPubKey pubKey, BaseKeyId keyId, IEnumerable<AppPermissions>? permissions, KeyStrength? strength = null, string? description = null)
        : this(new Parts(purposes,
            name,
            description,
            pubKey.Required(),
            strength ?? pubKey.Strength,
            keyId,
            permissions)) { }

    public InterlockKey(IBaseKey key)
        : this(new Parts(
            key.Required().Purposes,
            key.Name,
            key.Description,
            key.PublicKey,
            key.Strength,
            key.Id,
            key.Permissions)) {
    }

    [JsonIgnore]
    public override object AsJson => Value;
    public string? Description => Value.Description;
    public BaseKeyId Id => Value.Id;
    public BaseKeyId Identity => Value.Identity;
    public string Name => Value.Name;
    public IEnumerable<AppPermissions> Permissions => Value.Permissions;
    public TagPubKey PublicKey => Value.PublicKey;
    public KeyPurpose[] Purposes => Value.Purposes;
    public KeyStrength Strength => Value.Strength;
    public ushort Version => Value.Version;

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public bool Equals(InterlockKey? other) => other is not null && Id == other.Id;

    public bool Matches(BaseKeyId senderId, TagPubKey publicKey) => Id == senderId && PublicKey.Equals(publicKey);

    public string ToShortString() => Value.ToShortString();

    public override string ToString() => $@"InterlockKey
-- Version: {Version}
{Value}";

    public class Parts
    {
        public const ushort InterlockKeyVersion = 0x0004;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Parts() {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Parts(KeyPurpose[] purposes, string name, string? description, TagPubKey pubKey, KeyStrength strength, BaseKeyId keyId, IEnumerable<AppPermissions>? permissions) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Version = InterlockKeyVersion;
            Name = name.Required();
            Purposes = purposes.Required().Distinct().ToArray();
            PublicKey = pubKey.Required();
            Description = description;
            Strength = strength;
            Permissions = permissions.Safe().Distinct();
            if (Actionable && Permissions.None())
                Purposes = Purposes.Where(pu => pu != KeyPurpose.Action).ToArray(); // Remove Action Purpose
            Identity = new KeyId(TagHash.HashSha256Of(_hashable));
            Id = keyId ?? Identity;
        }

        [JsonIgnore]
        public bool Actionable => Purposes?.Contains(KeyPurpose.Action) ?? false;

        public string? Description { get; init; }
        public BaseKeyId Id { get; init; }
        public BaseKeyId Identity { get; init; }
        public string Name { get; init; }
        public IEnumerable<AppPermissions> Permissions { get; set; } = NoPermissions;
        public TagPubKey PublicKey { get; set; }
        public KeyPurpose[] Purposes { get; set; } = Array.Empty<KeyPurpose>();
        public KeyStrength Strength { get; init; }
        public ushort Version { get; init; }

        public string ToShortString() => $"{Name.Safe(),-58} [{_displayablePurposes}] {GetPermissions(" ", firstSep: string.Empty)}";

        public override string ToString() =>
        $@"-- Key '{Name}' - {Description}
++ Id: {Id}
++ using {PublicKey.Algorithm} [{PublicKey.TextualRepresentation}]
++ with purposes: {_displayablePurposes}  {GetPermissions(Environment.NewLine + "++++ ", formatter: p => p.VerboseRepresentation).ToLowerInvariant()}
++ from: {Identity}
++ with strength {Strength}";

        internal static AppPermissions[] NoPermissions = Array.Empty<AppPermissions>();

        internal IEnumerable<ulong> FirstActions {
            get => Permissions.FirstOrDefault().ActionIds ?? Array.Empty<ulong>();
            set => Permissions = new AppPermissions(_firstAppId, value).ToEnumerable();
        }

        internal ulong FirstAppId {
            get => Permissions.FirstOrDefault().AppId;
            set => _firstAppId = value;
        }

        internal ILTagArrayOfILInt PurposesAsILInts => new(AsUlongs(Purposes));

        internal ulong[] PurposesAsUlongs {
            get => AsUlongs(Purposes);
            set => Purposes = value.Safe().Select(u => (KeyPurpose)u).Distinct().ToArray();
        }

        private ulong _firstAppId;

        private string _displayablePurposes => Purposes.ToStringAsList();

        private byte[] _hashable => PublicKey.EncodedBytes.Append(GetPermissions(string.Empty).UTF8Bytes())
            .Append(PurposesAsILInts.EncodedBytes);

        private static ulong[] AsUlongs(IEnumerable<KeyPurpose> purposes) => purposes.Select(p => (ulong)p).ToArray();

        private string GetPermissions(string separator, Func<AppPermissions, string>? formatter = null, string? firstSep = null)
            => Actionable
                ? Permissions.None()
                    ? "No actions"
                    : $"{firstSep ?? separator}{Permissions.JoinedBy(separator, formatter!)}"
                : string.Empty;
    }

    internal InterlockKey(Stream s) : base(ILTagId.InterlockKey, s) {
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    protected override Parts FromBytes(byte[] bytes) =>
        FromBytesHelper(bytes, s => {
            var version = s.DecodeUShort();
            var result = new Parts {
                Version = version,                                // Field index 0 //
                Name = s.DecodeString(),                          // Field index 1 //
                PurposesAsUlongs = s.DecodeILIntArray(),          // Field index 2 //
                Id = InterlockId.Resolve<BaseKeyId>(s),           // Field index 3 //
                Identity = InterlockId.Resolve<BaseKeyId>(s),     // Field index 4 //
                Description = s.DecodeString(),                   // Field index 5 //
                PublicKey = s.Decode<TagPubKey>().Required(),     // Field index 6 //
                FirstAppId = version > 0 ? s.DecodeILInt() : 0,   // Field index 7  - since version 1 //
                Strength = version > 1 ? (KeyStrength)s.DecodeILInt() : KeyStrength.Normal, // Field index 8 - since version 2 //
                FirstActions = version > 2 ? s.DecodeILIntArray() : Enumerable.Empty<ulong>(), // Field index 9 - since version 3 //
            };
            if (version > 3)
                result.Permissions = s.DecodeTagArray<AppPermissions.Tag>().Select(t => t.Value).Distinct().ToArray();
            return result;
        });

    protected override byte[] ToBytes(Parts value)
        => TagHelpers.ToBytesHelper(s => {
            s.EncodeUShort(Value.Version);              // Field index 0 //
            s.EncodeString(Value.Name);                 // Field index 1 //
            s.EncodeILIntArray(Value.PurposesAsUlongs); // Field index 2 //
            s.EncodeInterlockId(Value.Id);              // Field index 3 //
            s.EncodeInterlockId(Value.Identity);        // Field index 4 //
            s.EncodeString(Value.Description);          // Field index 5 //
            s.EncodeTag(Value.PublicKey);               // Field index 6 //
            s.EncodeILInt(Value.FirstAppId);            // Field index 7 //
            s.EncodeILInt((ulong)value.Strength);       // Field index 8 //
            s.EncodeILIntArray(Value.FirstActions);     // Field index 9 - since version 3 //
            s.EncodeTagArray(Value.Permissions.Select(p => p.AsTag)); // Field index 10 - since version 4 //
        });

    private bool _disposedValue;

    private InterlockKey(Parts parts) : base(ILTagId.InterlockKey, parts) {
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~InterlockKey()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }
}