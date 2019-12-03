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
    public class SignedValue<T> : VersionedValue<SignedValue<T>> where T : ILTag
    {
        public const int CurrentVersion = 1;

        public SignedValue() : base(ILTagId.SignedValue, CurrentVersion) {
        }

        public ulong ContentTagId => SignedContent.TagId;
        public IEnumerable<TagIdentifiedSignature> FailedSignatures => FailedSignaturesFor(SignedContent.EncodedBytes);

        public IEnumerable<TagIdentifiedSignature> Signatures { get; private set; }

        public T SignedContent { get; private set; }

        protected SignedValue(T payload, IEnumerable<TagIdentifiedSignature> signatures) : this() {
            SignedContent = payload ?? throw new ArgumentNullException(nameof(payload));
            Signatures = signatures ?? throw new ArgumentNullException(nameof(signatures));
        }

        protected override object AsJson => new { TagId, ContentTagId, SignedContent = SignedContent.AsJson, Signatures = Signatures.AsJsonArray() };

        protected override IEnumerable<DataField> RemainingStateFields => new DataField(nameof(SignedContent), ContentTagId)
            .AppendedOf(new DataField(nameof(Signatures), ILTagId.ILTagArray) { ElementTagId = ILTagId.IdentifiedSignature });

        protected override string TypeDescription => $"SignedValueOf{typeof(T).Name}";
        protected override string TypeName => $"SignedValueOf{typeof(T).Name}";

        protected override void DecodeRemainingStateFrom(Stream s) {
            SignedContent = s.Decode<T>();
            Signatures = s.DecodeTagArray<TagIdentifiedSignature>();
        }

        protected override void EncodeRemainingStateTo(Stream s) {
            s.EncodeTag(SignedContent);
            s.EncodeTagArray(Signatures);
        }

        private IEnumerable<TagIdentifiedSignature> FailedSignaturesFor(byte[] encodedBytes) => Signatures.Where(sig => !sig.Verify(encodedBytes)).ToArray();
    }
}