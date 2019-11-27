/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class InterlockKey : ILTagExplicit<InterlockKeyParts>, IEquatable<InterlockKey>
    {
        public InterlockKey(KeyPurpose[] purposes, string name, TagPubKey pubKey, ulong appId, IEnumerable<ulong> actionIds, BaseKeyId keyId, KeyStrength? strength = null, string description = null)
            : this(new InterlockKeyParts(purposes,
                appId,
                actionIds,
                name,
                description,
                pubKey,
                strength ?? pubKey?.Strength ?? throw new ArgumentNullException(nameof(pubKey)),
                keyId)) { }

        public InterlockKey(InterlockKeyParts parts) : base(ILTagId.InterlockKey, parts) {
        }

        public InterlockKey(ISigningKey key)
            : this(new InterlockKeyParts(key.Purposes, key.AppId, key.SpecificActions, key.Name, key.Description, key.CurrentPublicKey, key.Strength, key.Id)) {
        }

        public bool AllActions => Value.Actionable && SpecificActions.None();
        public ulong AppId => Value.AppId;
        public string Description => Value.Description;
        public BaseKeyId Id => Value.Id;
        public BaseKeyId Identity => Value.Identity;
        public string Name => Value.Name;
        public TagPubKey PublicKey => Value.PublicKey;
        public KeyPurpose[] Purposes => Value.Purposes;
        public IEnumerable<ulong> SpecificActions => Value.SpecificActions;
        public ushort Version => Value.Version;

        public bool CanAct(ulong appId, ulong actionId) => appId == AppId && Purposes.Contains(KeyPurpose.Action) && (SpecificActions?.Contains(actionId) != false);

        public override bool Equals(object obj) => Equals(obj as InterlockKey);

        public bool Equals(InterlockKey other) => other != null && Id == other.Id;

        public override int GetHashCode() => 2_108_858_624 + Id.GetHashCode();

        public bool Matches(BaseKeyId senderId, TagPubKey publicKey) => Id == senderId && PublicKey.Equals(publicKey);

        public string ToShortString() => Value.ToShortString();

        public override string ToString() => $@"InterlockKey
-- Version: {Version}
{Value}";

        internal InterlockKey(Stream s) : base(ILTagId.InterlockKey, s) {
        }

        protected override InterlockKeyParts FromBytes(byte[] bytes) =>
            FromBytesHelper(bytes, s => {
                var version = s.DecodeUShort();
                return new InterlockKeyParts {
                    Version = version,                                // Field index 0 //
                    Name = s.DecodeString(),                          // Field index 1 //
                    PurposesAsUlongs = s.DecodeILIntArray(),          // Field index 2 //
                    Id = s.Decode<BaseKeyId>(),                       // Field index 3 //
                    Identity = s.Decode<BaseKeyId>(),                 // Field index 4 //
                    Description = s.DecodeString(),                   // Field index 5 //
                    PublicKey = s.Decode<TagPubKey>(),                // Field index 6 //
                    AppId = version > 0 ? s.DecodeILInt() : 0,        // Field index 7  - since version 1 //
                    Strength = version > 1 ? (KeyStrength)s.DecodeILInt() : KeyStrength.Normal, // Field index 8 - since version 2 //
                    SpecificActions = version > 2 ? s.DecodeILIntArray() : Enumerable.Empty<ulong>(), // Field index 9 - since version 3 //
                };
            });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => {
                s.EncodeUShort(Value.Version);              // Field index 0 //
                s.EncodeString(Value.Name);                 // Field index 1 //
                s.EncodeILIntArray(Value.PurposesAsUlongs); // Field index 2 //
                s.EncodeInterlockId(Value.Id);              // Field index 3 //
                s.EncodeInterlockId(Value.Identity);        // Field index 4 //
                s.EncodeString(Value.Description);          // Field index 5 //
                s.EncodeTag(Value.PublicKey);               // Field index 6 //
                s.EncodeILInt(Value.AppId);                 // Field index 7 //
                s.EncodeILInt((ulong)Value.Strength);       // Field index 8 //
                s.EncodeILIntArray(Value.SpecificActions ?? Enumerable.Empty<ulong>());  // Field index 9 - since version 3 //
            });
    }

    public class InterlockKeyParts
    {
        public const ushort InterlockKeyVersion = 0x0003;
        public ulong AppId;
        public string Description;
        public BaseKeyId Id;
        public BaseKeyId Identity;
        public string Name;
        public TagPubKey PublicKey;
        public KeyPurpose[] Purposes;
        public IEnumerable<ulong> SpecificActions;
        public KeyStrength Strength;
        public ushort Version;

        public InterlockKeyParts() {
        }

        public InterlockKeyParts(KeyPurpose[] purposes, ulong appId, IEnumerable<ulong> actionIds, string name, string description, TagPubKey pubKey, KeyStrength strength, BaseKeyId keyId) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Version = InterlockKeyVersion;
            Name = name;
            Purposes = purposes ?? throw new ArgumentNullException(nameof(purposes));
            PublicKey = pubKey ?? throw new ArgumentNullException(nameof(pubKey));
            Description = description;
            AppId = appId;
            Strength = strength;
            SpecificActions = actionIds ?? Enumerable.Empty<ulong>();
            Identity = new KeyId(TagHash.HashSha256Of(_hashable));
            Id = keyId ?? Identity;
        }

        public bool Actionable => Purposes.Contains(KeyPurpose.Action);

        public string ToShortString() => $"{Name.Safe().PadRight(58)} [{_displayablePurposes}] {_actionsFor} ";

        public override string ToString() =>
        $@"-- Key '{Name}' - {Description}
++ Id: {Id}
++ using {PublicKey.Algorithm} [{PublicKey.TextualRepresentation}]
++ with purposes: {_displayablePurposes}  {_actionsFor.ToLowerInvariant()}
++ from: {Identity}
++ with strength {Strength}";

        internal ILTagArrayOfILTag<ILTagILInt> PurposesAsILInts => new ILTagArrayOfILTag<ILTagILInt>(AsILInts(Purposes));

        internal ulong[] PurposesAsUlongs {
            get => AsUlongs(Purposes);
            set => Purposes = value?.Select(u => (KeyPurpose)u).ToArray();
        }

        private string _actionsFor => Actionable ? AppAndActions() : string.Empty;

        private string _displayablePurposes => Purposes.ToStringAsList();

        private byte[] _hashable => PublicKey.EncodedBytes.Append(_actionsFor.UTF8Bytes()).Append(PurposesAsILInts.EncodedBytes);

        private static ILTagILInt[] AsILInts(KeyPurpose[] purposes) => purposes?.Select(p => new ILTagILInt((ulong)p)).ToArray();

        private static ulong[] AsUlongs(KeyPurpose[] purposes) => purposes?.Select(p => (ulong)p).ToArray();

        private string AppAndActions() {
            var actions = SpecificActions.ToArray();
            if (AppId == 0 && !actions.SafeAny())
                return "All Apps & Actions";
            var plural = (actions.Length == 1 ? "" : "s");
            return $"App #{AppId} {(actions.SafeAny() ? $"Action{plural} {actions.WithCommas(noSpaces: true)}" : "All Actions")}";
        }
    }
}