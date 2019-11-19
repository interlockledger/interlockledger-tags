/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public abstract class Owner : ISigningKey, IPasswordProvider
    {
        public ulong AppId => 0;
        public InterlockKey AsInterlockKey => new InterlockKey(this);
        public TagPubKey CurrentPublicKey { get; protected set; }
        public string Description { get; protected set; }
        public string Email { get; protected set; }
        public BaseKeyId Id { get; protected set; }
        public string Name { get; protected set; }
        public OwnerId OwnerId => (OwnerId)Id;
        public KeyPurpose[] Purposes => keyPurposes;
        public Algorithm SignAlgorithm { get; protected set; }
        public IEnumerable<ulong> SpecificActions => Enumerable.Empty<ulong>();
        public KeyStrength Strength { get; protected set; }

        public string PasswordFor(InterlockId id) {
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            return Convert.ToBase64String(Sign(id.EncodedBytes).Data);
        }

        public abstract TagSignature Sign(byte[] data);

        public string ToListing() => $"'{Name}' {Id}";

        public string ToShortString() => $"Owner {Name} using {SignAlgorithm} with {Strength} strength ({Id})";

        public override string ToString() => ToShortString() + $"\r\n-- {Description}\r\n-- Email {Email}\r\n-- {CurrentPublicKey}\r\n-- Purposes {keyPurposes.ToStringAsList()}";
        public abstract byte[] Decrypt(byte[] bytes);

        protected static readonly KeyPurpose[] keyPurposes = new KeyPurpose[] { KeyPurpose.KeyManagement, KeyPurpose.Action, KeyPurpose.ClaimSigner, KeyPurpose.Protocol };
    }
}