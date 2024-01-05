// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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
public class KeyParameters(RSAParameters parameters, KeyStrength strength)
{
    public readonly RSAParameters Parameters = parameters;
    public readonly KeyStrength Strength = strength;

    internal KeyParameters(Stream s) : this(
        new RSAParameters {
            Modulus = s.DecodeByteArray(),
            Exponent = s.DecodeByteArray(),
            P = s.DecodeByteArray(),
            Q = s.DecodeByteArray(),
            DP = s.DecodeByteArray(),
            DQ = s.DecodeByteArray(),
            InverseQ = s.DecodeByteArray(),
            D = s.DecodeByteArray()
        },
        DecodeStrength(s)) { }

    internal Stream EncodeTo(Stream s) => s
        .EncodeByteArray(Parameters.Modulus)
        .EncodeByteArray(Parameters.Exponent)
        .EncodeByteArray(Parameters.P)
        .EncodeByteArray(Parameters.Q)
        .EncodeByteArray(Parameters.DP)
        .EncodeByteArray(Parameters.DQ)
        .EncodeByteArray(Parameters.InverseQ)
        .EncodeByteArray(Parameters.D)
        .EncodeILInt((ulong)Strength);

    private static KeyStrength DecodeStrength(Stream s) => (KeyStrength)(s.HasBytes() ? s.DecodeILInt() : 0);
}

public class TagRSAParameters : ILTagOfExplicit<KeyParameters>, IKeyParameters
{
    public TagRSAParameters(RSAParameters parameters, KeyStrength strength) : base(ILTagId.RSAParameters, new KeyParameters(parameters, strength)) {
    }

    public TagPubKey PublicKey => new TagPubRSAKey(Value!.Parameters);

    public KeyStrength Strength => Value!.Strength;

    public static TagRSAParameters DecodeFromBytes(byte[] encodedBytes) {
        using var s = new MemoryStream(encodedBytes);
        return s.Decode<TagRSAParameters>().Required();
    }

    internal TagRSAParameters(Stream s) : base(ILTagId.RSAParameters, s) => Value.Required();

    protected override KeyParameters? ValueFromStream(WrappedReadonlyStream s) => new(s);
    protected override Stream ValueToStream(Stream s) => Value!.EncodeTo(s);
}