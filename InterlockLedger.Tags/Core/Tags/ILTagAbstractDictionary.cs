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
public abstract class ILTagAbstractDictionary<T> : ILTagOfExplicit<Dictionary<string, T?>?> where T : class
{
    public T? this[string key] => Value?[key];

    protected ILTagAbstractDictionary(ulong tagId, Dictionary<string, T?>? value) : base(tagId, value) {
    }

    protected ILTagAbstractDictionary(ulong alreadyDeserializedTagId, Stream s) : base(alreadyDeserializedTagId, s) {
    }
    protected override bool AreEquivalent(ILTagOf<Dictionary<string, T?>?> other) {
        if (other is null)
            return false;
        var dict = Value;
        if (dict is null)
            return other.Value.None();
        var otherDict = other.Value.Required();
        foreach (var pair in dict) {
            if (otherDict.TryGetValue(pair.Key, out var otherValue)) {
                var value = pair.Value;
                if ((value is not null && !value.Equals(otherValue)) || (value is null && otherValue is not null))
                    return false;
            } else
                return false;
        }
        return true;
    }

    protected abstract T? DecodeValue(Stream s);

    protected abstract void EncodeValue(Stream s, T? value);

    protected override Dictionary<string, T?>? ValueFromStream(WrappedReadonlyStream s) {
        if (s.Length == 0)
            return null;
        var result = new Dictionary<string, T?>();
        var length = (int)s.ILIntDecode();
        for (var i = 0; i < length; i++) {
            result[s.DecodeString()!] = DecodeValue(s);
        }
        return result;
    }
    protected override Stream ValueToStream(Stream s) {
        if (Value is not null) {
            s.ILIntEncode((ulong)Value.Count);
            if (Value.Count > 0)
                foreach (var pair in Value) {
                    s.EncodeString(pair.Key);
                    EncodeValue(s, pair.Value);
                }
        }
        return s;
    }
}