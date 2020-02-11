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
using System.Linq;

namespace InterlockLedger.Tags
{
    public abstract class Owner : ISigningKey, IPasswordProvider
    {
        public InterlockKey AsInterlockKey => new InterlockKey(this);
        public TagPubKey CurrentPublicKey { get; protected set; }
        public string Description { get; protected set; }
        public string Email { get; protected set; }
        public BaseKeyId Id { get; protected set; }
        public string Name { get; protected set; }
        public OwnerId OwnerId => (OwnerId)Id;
        public IEnumerable<AppPermissions> Permissions { get; } = InterlockKeyParts.NoPermissions;
        public KeyPurpose[] Purposes => keyPurposes;
        public Algorithm SignAlgorithm { get; protected set; }
        public KeyStrength Strength { get; protected set; }

        public abstract byte[] Decrypt(byte[] bytes);

        public string PasswordFor(InterlockId id) {
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            return Convert.ToBase64String(Sign(id.EncodedBytes).Data);
        }

        public abstract TagSignature Sign(byte[] data);

        public string ToListing() => $"'{Name}' {Id}";

        public string ToShortString() => $"Owner {Name} using {SignAlgorithm} with {Strength} strength ({Id})";

        public override string ToString() => ToShortString() + $"\r\n-- {Description}\r\n-- Email {Email}\r\n-- {CurrentPublicKey}\r\n-- Purposes {keyPurposes.ToStringAsList()}";

        protected static readonly KeyPurpose[] keyPurposes = new KeyPurpose[] { KeyPurpose.KeyManagement, KeyPurpose.Action, KeyPurpose.ClaimSigner, KeyPurpose.Protocol };
    }
}