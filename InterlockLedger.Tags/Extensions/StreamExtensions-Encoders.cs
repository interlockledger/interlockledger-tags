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
public static partial class StreamExtensions
{
    public static TS EncodeAny<T, TS>(this TS s, T? value) where T : ITaggable where TS : Stream
        => s.EncodeTag(value?.AsILTag);

    public static TS EncodeArray<T, TS>(this TS s, IEnumerable<T>? values) where T : class, ITaggableOf<T> where TS : Stream
        => s.EncodeTag(new ILTagArrayOfILTag<ILTagOf<T>>(values.Safe().Select(v => v.AsTag).ToArray()));

    public static TS EncodeBool<TS>(this TS s, bool value) where TS : Stream
        => s.EncodeTag(value ? ILTagBool.True : ILTagBool.False);

    public static TS EncodeByte<TS>(this TS s, byte value) where TS : Stream
        => s.EncodeTag(new ILTagUInt8(value));

    public static TS EncodeByteArray<TS>(this TS s, byte[]? value) where TS : Stream
        => value == null ? s.EncodeNull() : s.EncodeTag(new ILTagByteArray(value));

    public static TS EncodeByteArray<TS>(this TS s, Span<byte> value) where TS : Stream
        => s.EncodeTag(new ILTagByteArray(value));

    public static TS EncodeColor<TS>(this TS s, InterlockColor value) where TS : Stream
        => s.EncodeTag(new ILTagUInt32(value.RGBA));

    public static TS EncodeDateTimeOffset<TS>(this TS s, DateTimeOffset value) where TS : Stream
        => s.EncodeILInt(value.AsMilliseconds());

    public static TS EncodeDictionary<TS>(this TS s, Dictionary<string, string?>? dictionary) where TS : Stream
        => s.EncodeTag(new ILTagStringDictionary(dictionary));

    public static TS EncodeILInt<TS>(this TS s, ulong value) where TS : Stream
        => s.EncodeTag(new ILTagILInt(value));

    public static TS EncodeILIntArray<TS>(this TS s, IEnumerable<ulong>? values) where TS : Stream
        => s.EncodeTag(new ILTagArrayOfILInt(values?.ToArray()));

    public static TS EncodeInt<TS>(this TS s, int value) where TS : Stream
        => s.EncodeTag(new ILTagInt32(value));

    public static TS EncodeNull<TS>(this TS s) where TS : Stream {
        _ = ILTagNull.Instance.SerializeIntoAsync(s).WaitResult()!;
        return s;
    }

    public static TS EncodeSequence<TS>(this TS s, IEnumerable<ILTag>? values) where TS : Stream
        => s.EncodeTag(new ILTagSequence(values.Safe().ToArray()));

    public static TS EncodeString<TS>(this TS s, string? value) where TS : Stream
        => s.EncodeTag(new ILTagString(value));

    public static TS EncodeTag<TS>(this TS s, ILTag? tag) where TS : Stream {
        _ = tag is null ? s.EncodeNull() : tag.SerializeIntoAsync(s).WaitResult()!;
        return s;
    }

    public static TS EncodeTagArray<T, TS>(this TS s, IEnumerable<T>? values) where T : ILTag where TS : Stream
        => s.EncodeTag(new ILTagArrayOfILTag<T>(values.Safe().ToArray()));

    public static TS EncodeTimestamp<TS>(this TS s, DateTimeOffset value) where TS : Stream
        => s.EncodeTag(new ILTagTimestamp(value));

    public static TS EncodeUShort<TS>(this TS s, ushort value) where TS : Stream
        => s.EncodeTag(new ILTagUInt16(value));

    public static TS EncodeVersion<TS>(this TS s, Version version) where TS : Stream
        => s.EncodeTag(new ILTagVersion(version));
}