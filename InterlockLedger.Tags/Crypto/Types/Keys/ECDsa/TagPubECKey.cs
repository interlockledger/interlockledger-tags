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

using System;
using System.IO;
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
    public class TagPubECKey : TagPubKey
    {
        public TagPubECKey(ECDsaParameters parameters) : base(Algorithm.EcDSA, parameters.EncodedPublicBytes) => _kp = parameters;

        public override KeyStrength Strength => _kp.Strength;

        public override byte[] Encrypt(byte[] bytes) => throw new InvalidOperationException("ECDsa does not permit encryption");
        // ECDsaHelper.Encrypt(bytes, _kp.Parameters);

        public override bool Verify<T>(T data, TagSignature signature)
            => ECDsaHelper.Verify(data, signature, _kp.Parameters);

        public override bool Verify(byte[] data, TagSignature signature)
            => ECDsaHelper.Verify(data, signature, _kp.Parameters);

        internal TagPubECKey(ECParameters parameters) : this(new ECDsaParameters(parameters)) {
        }

        internal static TagPubECKey From(byte[] data) => new(data.Required(nameof(data)));

        protected TagPubECKey(byte[] data) : base(Algorithm.EcDSA, data) {
            using var ms = new MemoryStream(data);
            _kp = ms.DecodeAny<ECDsaParameters>();
        }

        private readonly ECDsaParameters _kp;
    }
}