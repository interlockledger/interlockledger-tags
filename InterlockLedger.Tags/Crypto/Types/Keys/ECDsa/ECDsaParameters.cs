// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
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
#nullable enable

using System.Security.Cryptography;

namespace InterlockLedger.Tags;
public class ECDsaParameters : VersionedValue<ECDsaParameters>, IKeyParameters
{
    public ECDsaParameters(ECParameters parameters) : this() {
        Parameters = parameters;
        Strength = GuessStrength(parameters.Curve);
        HashAlgorithm = ChooseHashAlgo(parameters.Curve);
    }

    public ECDsaParameters() : base(ILTagId.ECParameters, _version) {
    }

    public override object AsJson => new { TagId, Version, Strength, Parameters.Q, Parameters.Curve.Oid };

    public byte[] EncodedBytes => AsPayload.EncodedBytes;

    public override string Formatted => "";

    public HashAlgorithm HashAlgorithm { get; private set; }

    public bool HasPrivateKey => Parameters.D.SafeAny();

    public ECParameters Parameters { get; private set; }

    public TagPubKey PublicKey => _pk ??= new TagPubECKey(this);

    public KeyStrength Strength { get; private set; }

    public override string TypeName => nameof(ECParameters);

    public static ECCurve ChooseCurve(KeyStrength strength) => strength switch {
        KeyStrength.Normal => ECCurve.NamedCurves.brainpoolP160r1,
        KeyStrength.Strong => ECCurve.NamedCurves.brainpoolP192r1,
        KeyStrength.ExtraStrong => ECCurve.NamedCurves.brainpoolP224r1,
        KeyStrength.MegaStrong => ECCurve.NamedCurves.brainpoolP256r1,
        KeyStrength.SuperStrong => ECCurve.NamedCurves.brainpoolP320r1,
        KeyStrength.HyperStrong => ECCurve.NamedCurves.brainpoolP384r1,
        KeyStrength.UltraStrong => ECCurve.NamedCurves.brainpoolP512r1,
        _ => throw new NotSupportedException(),
    };

    public static HashAlgorithm ChooseHashAlgo(ECCurve curve) => curve.Oid.FriendlyName switch {
        "brainpoolP384r1" or "brainpoolP320r1" => HashAlgorithm.SHA384,
        "brainpoolP512r1" => HashAlgorithm.SHA512,
        _ => HashAlgorithm.SHA256,
    };

    public static KeyStrength GuessStrength(ECCurve curve) => curve.Oid.FriendlyName switch {
        "brainpoolP160r1" => KeyStrength.Normal,
        "brainpoolP192r1" => KeyStrength.Strong,
        "brainpoolP224r1" => KeyStrength.ExtraStrong,
        "brainpoolP320r1" => KeyStrength.SuperStrong,
        "brainpoolP256r1" => KeyStrength.MegaStrong,
        "brainpoolP384r1" => KeyStrength.HyperStrong,
        "brainpoolP512r1" => KeyStrength.UltraStrong,
        _ => throw new NotSupportedException(),
    };

    public override ECDsaParameters FromJson(object json) => throw new NotSupportedException();

    internal ECDsaParameters(ECParameters parameters, KeyStrength strength, HashAlgorithm hashAlgorithm) : base(ILTagId.ECParameters, _version) {
        Parameters = parameters;
        Strength = strength;
        HashAlgorithm = hashAlgorithm;
    }

    internal byte[] EncodedPublicBytes {
        get {
            var pubParameters = Parameters;
            pubParameters.D = null;
            var clone = new ECDsaParameters {
                Parameters = pubParameters,
                Strength = Strength,
                HashAlgorithm = HashAlgorithm
            };
            return clone.EncodedBytes;
        }
    }

    internal ECDsaParameters DecodeFrom(Stream s) {
        Strength = (KeyStrength)s.DecodeILInt();
        HashAlgorithm = (HashAlgorithm)s.DecodeILInt();
        Parameters = new ECParameters {
            Q = new ECPoint() { X = s.DecodeByteArray(), Y = s.DecodeByteArray() },
            D = s.DecodeByteArray(),
            Curve = s.DecodeString().FromJson<ECCurve>()
        };
        return this;
    }

    protected override IEnumerable<DataField> RemainingStateFields { get; } = Enumerable.Empty<DataField>();
    protected override string TypeDescription => "Elliptic Curve Key Parameters";

    protected override void DecodeRemainingStateFrom(Stream s) => DecodeFrom(s);

    protected override void EncodeRemainingStateTo(Stream s) => EncodeTo(s, onlyPublic: false);

    private const ushort _version = 1;
    private TagPubECKey? _pk;

    private Stream EncodeTo(Stream s, bool onlyPublic) => s
        .EncodeILInt((ulong)Strength)
        .EncodeILInt((ulong)HashAlgorithm)
        .EncodeByteArray(Parameters.Q.X)
        .EncodeByteArray(Parameters.Q.Y)
        .EncodeByteArray(onlyPublic ? null : Parameters.D)
        .EncodeString(Parameters.Curve.AsJson());
}