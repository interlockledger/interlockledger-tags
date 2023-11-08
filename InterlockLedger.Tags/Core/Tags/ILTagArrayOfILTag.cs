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

public class ILTagArrayOfILTag<T> : ILTagOfExplicit<T?[]> where T : ILTag
{
    public ILTagArrayOfILTag(T?[]? value) : this(ILTagId.ILTagArray, value) { }
    public ILTagArrayOfILTag(IEnumerable<T?> value) : this(ILTagId.ILTagArray, value?.ToArray()) { }
    public T? this[int i] => Value?[i];

    [JsonIgnore]
    public int Length => Value?.Length ?? 0;

    public IEnumerable<TV> GetValues<TV>() => Value.Safe().Select(t => t is ILTagOf<TV> tv ? tv.Value : default).SkipDefaults()!;
    protected override bool AreEquivalent(ILTagOf<T?[]?> other) => Value.EquivalentTo(other.Value.Safe());
    internal ILTagArrayOfILTag(Stream s) : base(ILTagId.ILTagArray, s) { }

    internal ILTagArrayOfILTag(Stream s, Func<Stream, T?> decoder) :
        base(ILTagId.ILTagArray, s, (it) => { ((ILTagArrayOfILTag<T>)it)._decoder = decoder; }) { }

    private protected ILTagArrayOfILTag(ulong tagId, T?[]? Value) : base(tagId, Value) { }

    private protected ILTagArrayOfILTag(ulong tagId, Stream s) : base(tagId, s, null) {
    }

    protected override T?[]? ValueFromStream(Stream s) {
        if (s.Length > s.Position) {
            try {
                var arrayLength = (int)s.ILIntDecode();
                var list = new List<T>(arrayLength);
                for (var i = 0; i < arrayLength; i++) {
                    try {
                        var item = _decoder(s);
                        if (item is not null)
                            list.Add(item);
                        else
                            break;
                    } catch (Exception e) {
                        Console.WriteLine($"Error decoding ILTagArrayOfILTag<{typeof(T).FullName}>, expecting {arrayLength} items but failed at item {i} with: {e}");
                        break;
                    }
                }
                return [.. list];
            } catch (Exception e) {
                Console.WriteLine($"Error decoding ILTagArrayOfILTag<{typeof(T).FullName}>, failed with: {e}");
            }
        }
        return null;
    }

    protected override Stream ValueToStream(Stream s) {
        if (Value is not null) {
            s.ILIntEncode((ulong)Value.Length);
            if (Value.Length > 0) {
                foreach (var tag in Value)
                    s.EncodeTag(tag);
            }
        }
        return s;

    }

    private Func<Stream, T?> _decoder = s => AllowNull(s.DecodeTag());

    private static T? AllowNull(ILTag tag) => (tag.Traits.IsNull || tag is not T) ? default : (T)tag;

}