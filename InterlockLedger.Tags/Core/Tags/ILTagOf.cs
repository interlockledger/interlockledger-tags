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

namespace InterlockLedger.Tags;

public abstract class ILTagOf<T> : ILTag, IEquatable<ILTagOf<T>>, IEquatable<T>
{
    [JsonIgnore]
    public T Value { get; set; }
    public override object? Content => Value;
    public override bool ValueIs<TV>(out TV? value) where TV : default {
        if (Value is TV tvalue) {
            value = tvalue;
            return true;
        }
        value = default;
        return false;
    }
    protected ILTagOf(ulong tagId, T value) : base(tagId) => Value = value;

    protected ILTagOf(ulong alreadyDeserializedTagId, Stream s, Action<ITag>? setup = null) : base(alreadyDeserializedTagId) {
        setup?.Invoke(this);
        Value = DeserializeInnerAsync(s).WaitResult();
    }
    private protected abstract Task<T> DeserializeInnerAsync(Stream s);
    protected abstract Task<T> ValueFromStreamAsync(WrappedReadonlyStream s);
    protected abstract Task<Stream> ValueToStreamAsync(Stream s);

    public sealed override int GetHashCode() => HashCode.Combine(TagId, Value);
    public sealed override bool Equals(object? obj) => Equals(obj as ILTagOf<T>);
    public bool Equals(ILTagOf<T>? other) => other is not null && TagId == other.TagId && AreEquivalent(other);
    public bool Equals(T? otherValue) =>
        (Value is null && otherValue is null) ||
        (Value is not null && otherValue is not null && Value.Equals(otherValue));

    protected virtual bool AreEquivalent(ILTagOf<T> other) => Equals(other.Value);

    public static bool operator ==(ILTagOf<T> left, ILTagOf<T> right) =>
        left is null ? right is null : left.Equals(right);
    public static bool operator !=(ILTagOf<T> left, ILTagOf<T> right) =>
        !(left == right);
    public static bool operator ==(ILTagOf<T> left, T right) =>
        left is null ? right is null : left.Equals(right);
    public static bool operator !=(ILTagOf<T> left, T right) =>
        !(left == right);
    public static bool operator ==(T left, ILTagOf<T> right) =>
        right is null ? left is null : right.Equals(left);
    public static bool operator !=(T left, ILTagOf<T> right) =>
        !(left == right);
}

