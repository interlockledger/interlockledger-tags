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
public class SignedValue<T> : VersionedValue<SignedValue<T>> where T : Signable<T>, new()
{
    public const int CurrentVersion = 1;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SignedValue() : base(ILTagId.SignedValue, CurrentVersion) {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [JsonIgnore]
    public ulong ContentTagId => SignedContent.TagId;

    [JsonIgnore]
    public IEnumerable<IdentifiedSignature> FailedSignatures => FailedSignaturesFor(SignedContent);

    [JsonIgnore]
    public override string Formatted => SignedContent.Formatted + $"\nWith {Signatures.SafeCount()} signatures";
    public IEnumerable<IdentifiedSignature> Signatures { get; private set; }

    public T SignedContent { get; private set; }

    public override string TypeName => $"SignedValueOf{SignedContent?.TypeName}";

    public bool IsSignedBy(BaseKeyId validSigner, TagPubKey validPubKey)
        => SignedContent is not null
        && Signatures.Safe().Any(sig => sig.SignerId == validSigner
                                        && sig.PublicKey == validPubKey
                                        && sig.Verify(SignedContent));

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal SignedValue(ushort version, T signedContent, Stream s) : base(ILTagId.SignedValue, version)
        => Init(signedContent, s.DecodeArray<IdentifiedSignature>().OrEmpty()!);

    internal SignedValue(T signedContent, IEnumerable<IdentifiedSignature> signatures) : this()
        => Init(signedContent, signatures);
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override IEnumerable<DataField> RemainingStateFields =>
        new T().FieldModel.WithName(nameof(SignedContent))
        .AppendedOf(new DataField(nameof(Signatures), ILTagId.ILTagArray) {
            ElementTagId = ILTagId.IdentifiedSignature,
            SubDataFields = new IdentifiedSignature().FieldModel.SubDataFields,
        });

    protected override string TypeDescription => $"SignedValueOf{typeof(T).Name}";

    protected override void DecodeRemainingStateFrom(Stream s) {
        SignedContent = s.DecodeAny<T>().Required();
        Signatures = s.DecodeArray<IdentifiedSignature>().RequireNonNulls().NonEmpty();
    }

    protected override void EncodeRemainingStateTo(Stream s) => s.EncodeAny(SignedContent).EncodeArray(Signatures);

    private IEnumerable<IdentifiedSignature> FailedSignaturesFor(T data)
        => Signatures.Where(sig => !sig.Verify(data)).ToArray();

    private void Init(T signedContent, IEnumerable<IdentifiedSignature> signatures) {
        SignedContent = signedContent.Required();
        Signatures = signatures;
        if (Signatures.None())
            throw new InvalidDataException("At least one signature must be provided");
    }
}