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
public class ILTagStringDictionary : ILTagAbstractDictionary<string>
{
    public ILTagStringDictionary(params (string key, string? value)[] pairs) : this(pairs.ToDictionary(p => p.key, pp => pp.value)) {
    }

    public ILTagStringDictionary(Dictionary<string, string?> value) : base(ILTagId.StringDictionary, value) {
    }

    public ILTagStringDictionary(object opaqueValue) : this(Elicit(opaqueValue)) {
    }

    internal ILTagStringDictionary(Stream s) : base(ILTagId.StringDictionary, s) {
    }

    protected override string? DecodeValue(Stream s) => s.Decode<ILTagString>()?.Value;

    protected override void EncodeValue(Stream s, string? value) {
        if (value is null) s.EncodeNull(); else s.EncodeString(value);
    }

    private static Dictionary<string, string?> Elicit(object opaqueValue)
        => (opaqueValue as Dictionary<string, string?>) ?? new Dictionary<string, string?>();
}