/******************************************************************************************************************************

Copyright (c) 2018-2020 InterlockLedger Network
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
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public class IdentifiedSignature : VersionedValue<IdentifiedSignature>
    {
        public const string Description = "A signature identified with the signerId and the corresponding PublickKey";
        public const ushort ImplementedVersion = 1;

        public IdentifiedSignature(TagSignature signature, BaseKeyId id, TagPubKey publicKey) : this() {
            Signature = signature ?? throw new ArgumentNullException(nameof(signature));
            SignerId = id ?? throw new System.ArgumentNullException(nameof(id));
            PublicKey = publicKey ?? throw new System.ArgumentNullException(nameof(publicKey));
        }

        public IdentifiedSignature() : base(ILTagId.IdentifiedSignature, ImplementedVersion) {
        }

        [JsonIgnore]
        public static IEnumerable<DataField> DataFields { get; } = VersionField.AppendedOf(_remainingDataFields);

        public TagPubKey PublicKey { get; set; }
        public TagSignature Signature { get; set; }
        public BaseKeyId SignerId { get; set; }

        public override string TypeName => nameof(IdentifiedSignature);

        public bool Verify(byte[] data) => PublicKey.Verify(data, Signature);

        protected override object AsJson => new { TagId, Version, Signature, SignerId, PublicKey };

        protected override IEnumerable<DataField> RemainingStateFields => _remainingDataFields;

        protected override string TypeDescription => Description;

        protected override void DecodeRemainingStateFrom(Stream s) {
            Signature = s.Decode<TagSignature>();
            SignerId = s.DecodeBaseKeyId();
            PublicKey = s.Decode<TagPubKey>();
        }

        protected override void EncodeRemainingStateTo(Stream s) {
            s.EncodeTag(Signature);
            s.EncodeTag(SignerId);
            s.EncodeTag(PublicKey);
        }

        protected override IdentifiedSignature FromJson(object json) => throw new NotImplementedException();

        private static readonly DataField[] _remainingDataFields = new DataField[] {
            new DataField { Name = nameof(Signature), TagId = ILTagId.Signature },
            new DataField { Name = nameof(SignerId), TagId = ILTagId.InterlockId },
            new DataField { Name = nameof(PublicKey), TagId = ILTagId.PubKey}
        };
    }
}
