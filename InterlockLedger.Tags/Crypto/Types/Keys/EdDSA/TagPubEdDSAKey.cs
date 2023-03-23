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

public class TagPubEdDSAKey : TagPubKey
{
    public readonly EdDSAParameters Parameters;

    public override KeyStrength Strength => KeyStrength.Normal;

    public TagPubEdDSAKey(EdDSAParameters parameters)
        : this(EncodeParameters(parameters)) {
    }

    public override byte[] Encrypt(byte[] bytes) => EdDSAHelper.Encrypt(bytes, Parameters);

    public override bool Verify<T>(T data, TagSignature signature) => EdDSAHelper.Verify(data, signature, Parameters);

    public override bool Verify(byte[] data, TagSignature signature) => EdDSAHelper.Verify(data, signature, Parameters);

    internal TagPubEdDSAKey(byte[] data) : base(Algorithm.EdDSA, data) => Parameters = DecodeParameters(base.Data);

    private static EdDSAParameters DecodeParameters(byte[] bytes) {
        using var s = new MemoryStream(bytes.NonEmpty());
        return s.Decode<TagEdDSAPublicParameters>()!.Value;
    }

    private static byte[] EncodeParameters(EdDSAParameters parameters) => new TagEdDSAPublicParameters(parameters).EncodedBytes;
}
