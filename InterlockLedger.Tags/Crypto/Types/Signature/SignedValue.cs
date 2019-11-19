/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
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

        public SignedValue() : base(CurrentVersion) {
        }

        public IEnumerable<TagIdentifiedSignature> FailedSignatures => FailedSignaturesFor(SignedContent.EncodedBytes);

        public IEnumerable<TagIdentifiedSignature> Signatures { get; private set; }

        public T SignedContent { get; private set; }

        protected SignedValue(T payload, IEnumerable<TagIdentifiedSignature> signatures) : base(CurrentVersion) {
            SignedContent = payload ?? throw new ArgumentNullException(nameof(payload));
            Signatures = signatures ?? throw new ArgumentNullException(nameof(signatures));
        }

        protected virtual ulong ContentTagId => 0;

        protected override IEnumerable<DataField> RemainingStateFields => new DataField(nameof(SignedContent), ContentTagId)
            .AppendedOf(new DataField(nameof(Signatures), ILTagId.ILTagArray) { ElementTagId = ILTagId.IdentifiedSignature });

        protected override ulong TagId => ILTagId.SignedValue;
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