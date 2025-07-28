// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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

using System.Security.Cryptography;


namespace InterlockLedger.Tags;

public static class ECParametersExtensions
{
    public static Stream EncodeHashAlgorithm(this Stream s, HashAlgorithmName? hashAlgorithm) =>
        s.EncodeString(hashAlgorithm?.Name);

    public static HashAlgorithmName? DecodeHashAlgorithm(this WrappedReadonlyStream s) {
        var name = s.DecodeString();
        return name is null ? null : new HashAlgorithmName(name);
    }

    public static Stream EncodeCurveType(this Stream s, ECCurve.ECCurveType curveType) =>
        s.EncodeILInt((ulong)curveType);

    public static ECCurve.ECCurveType DecodeCurveType(this WrappedReadonlyStream s) =>
        (ECCurve.ECCurveType)s.DecodeILInt();


    public static Stream EncodeECParametersPublicParts(this Stream s, ECParameters parameters) {
        s.Required();
        if (parameters.Curve.Oid is null) {
            s.EncodeString(null) // No OID
             .EncodeByteArray(parameters.Curve.A)
             .EncodeByteArray(parameters.Curve.B)
             .EncodeByteArray(parameters.Curve.Cofactor)
             .EncodeCurveType(parameters.Curve.CurveType)
             .EncodeByteArray(parameters.Curve.G.X)
             .EncodeByteArray(parameters.Curve.G.Y)
             .EncodeHashAlgorithm(parameters.Curve.Hash)
             .EncodeByteArray(parameters.Curve.Order)
             .EncodeByteArray(parameters.Curve.Polynomial)
             .EncodeByteArray(parameters.Curve.Prime)
             .EncodeByteArray(parameters.Curve.Seed);
        } else {
            s.EncodeString(parameters.Curve.Oid.Value); // OID
        }
        s.EncodeByteArray(parameters.Q.X)
         .EncodeByteArray(parameters.Q.Y);
        return s;
    }
    public static Stream EncodeECParameters(this Stream s, ECParameters parameters) =>
        s.EncodeByteArray(parameters.D.Required())
         .EncodeECParametersPublicParts(parameters);

    public static ECParameters DecodeECParameters(this WrappedReadonlyStream s) {
        var privateKey = s.DecodeByteArray().Required("ECParameters.PrivateKey");
        var publicParts = s.DecodeECParametersPublicParts();
        var result = new ECParameters {
            D = privateKey,
            Curve = publicParts.Curve,
            Q = publicParts.Q
        };
        result.Validate();
        return result;
    }
    public static ECParameters DecodeECParametersPublicParts(this WrappedReadonlyStream s) {
        var oid = s.DecodeString();
        ECCurve Curve;
        if (string.IsNullOrEmpty(oid)) {
            Curve = new ECCurve() {
                A = s.DecodeByteArray(),
                B = s.DecodeByteArray(),
                Cofactor = s.DecodeByteArray(),
                CurveType = s.DecodeCurveType(),
                G = new ECPoint {
                    X = s.DecodeByteArray(),
                    Y = s.DecodeByteArray()
                },
                Hash = s.DecodeHashAlgorithm(),
                Order = s.DecodeByteArray(),
                Polynomial = s.DecodeByteArray(),
                Prime = s.DecodeByteArray(),
                Seed = s.DecodeByteArray()
            };
        } else {
            // New format
            Curve = ECCurve.CreateFromOid(new Oid(oid));
        }
        var result = new ECParameters {
            Curve = Curve,
            // Q is the public key point
            Q = new ECPoint {
                X = s.DecodeByteArray(),
                Y = s.DecodeByteArray()
            }
        };
        return result;
    }

}
