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
using System.Linq;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public class SignedValue<T> : VersionedValue<SignedValue<T>> where T : Signable<T>, new()
    {
        public const int CurrentVersion = 1;

        public SignedValue() : base(ILTagId.SignedValue, CurrentVersion) {
        }

        public ulong ContentTagId => SignedContent.TagId;

        public IEnumerable<IdentifiedSignature> FailedSignatures => FailedSignaturesFor(SignedContent.AsILTag.EncodedBytes);

        public IEnumerable<IdentifiedSignature> Signatures { get; private set; }

        public T SignedContent { get; private set; }

        public override string TypeName => $"SignedValueOf{SignedContent?.TypeName}";

        public bool IsSignedBy(BaseKeyId validSigner, TagPubKey validPubKey) {
            if (SignedContent is null || Signatures.None())
                return false;
            byte[] encodedBytes = SignedContent.AsILTag.EncodedBytes;
            return Signatures.Any(sig => sig.SignerId == validSigner && sig.PublicKey == validPubKey && sig.Verify(encodedBytes));
        }

        internal SignedValue(ushort version, T signedContent, Stream s) : base(ILTagId.SignedValue, version)
            => Init(signedContent, s.DecodeArray<IdentifiedSignature>());

        internal SignedValue(T signedContent, IEnumerable<IdentifiedSignature> signatures) : this()
            => Init(signedContent, signatures);

        protected override object AsJson => new {
            TagId,
            ContentTagId,
            SignedContent = SignedContent.AsILTag.AsJson,
            Signatures = Signatures.AsJsonArray()
        };

        protected override IEnumerable<DataField> RemainingStateFields =>
            new T().FieldModel?.WithName(nameof(SignedContent))
            .AppendedOf(new DataField(nameof(Signatures), ILTagId.ILTagArray) {
                ElementTagId = ILTagId.IdentifiedSignature,
                SubDataFields = IdentifiedSignature.DataFields,
            });

        protected override string TypeDescription => $"SignedValueOf{typeof(T).Name}";

        protected override void DecodeRemainingStateFrom(Stream s) {
            SignedContent = s.DecodeAny<T>();
            Signatures = s.DecodeArray<IdentifiedSignature>();
        }

        protected override void EncodeRemainingStateTo(Stream s) => s.EncodeAny(SignedContent).EncodeArray(Signatures);

        protected override SignedValue<T> FromJson(object json) => throw new NotImplementedException();

        private IEnumerable<IdentifiedSignature> FailedSignaturesFor(byte[] encodedBytes)
            => Signatures.Where(sig => !sig.Verify(encodedBytes)).ToArray();

        private void Init(T signedContent, IEnumerable<IdentifiedSignature> signatures) {
            SignedContent = signedContent ?? throw new ArgumentNullException(nameof(signedContent));
            Signatures = signatures;
            if (Signatures.None())
                throw new InvalidDataException("At least one signature must be provided");
            if (FailedSignatures.SafeAny())
                throw new InvalidDataException("Some signatures don't match the payload");
        }
    }
}
