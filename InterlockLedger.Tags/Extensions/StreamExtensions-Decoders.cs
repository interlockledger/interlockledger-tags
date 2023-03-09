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

namespace InterlockLedger.Tags;
public static partial class StreamExtensions
{
    public static T? Decode<T>(this Stream s) where T : ILTag {
        var tag = s.DecodeTag();
        return tag.Traits.IsNull
            ? null
            : tag as T
                ?? throw new InvalidDataException($"Not a {typeof(T).Name} was {tag?.GetType().Name}:{tag}");
    }

    public static T? DecodeAny<T>(this Stream s) where T : class, ITaggable {
        var tag = s.DecodeTag();
        return tag.Traits.IsNull
            ? null
            : tag is ILTagOfExplicit<T> t
                ? t.Value
                : throw new InvalidDataException($"Not a tagged form of {typeof(T).Name} was {tag?.GetType().Name}:{tag}");
    }

    public static T[]? DecodeArray<T>(this Stream s) where T : ITaggableOf<T> =>
        DecodeArrayInternal(s, s => new ILTagArrayOfILTag<ILTagOfExplicit<T>>(s));

    public static T[]? DecodeArray<T, TT>(this Stream s, Func<Stream, TT> decoder) where T : notnull where TT : ILTagOfExplicit<T> =>
        DecodeArrayInternal(s, s => new ILTagArrayOfILTag<ILTagOfExplicit<T>>(s, decoder));

    private static T[]? DecodeArrayInternal<T>(Stream s, Func<Stream, ILTagArrayOfILTag<ILTagOfExplicit<T>>> decodeTagArray) where T : notnull {
        var tagId = s.DecodeTagId();
        return tagId != ILTagId.ILTagArray
            ? throw new InvalidDataException($"Not {typeof(ILTagArrayOfILTag<ILTagOfExplicit<T>>).Name}")
            : (decodeTagArray(s).Value?.SkipDefaults().Select(element => element.Value).SkipDefaults().ToArray());
    }

    public static bool DecodeBool(this Stream s) => s.Decode<ILTagBool>()?.Value ?? false;

    public static byte DecodeByte(this Stream s) => s.Decode<ILTagUInt8>()?.Value ?? 0;

    public static byte[]? DecodeByteArray(this Stream s) => s.Decode<ILTagByteArray>()?.Value;

    public static InterlockColor DecodeColor(this Stream s) {
        var tagValue = s.Decode<ILTagUInt32>();
        return tagValue is null ? InterlockColor.Transparent : InterlockColor.From(tagValue.Value);
    }

    [Obsolete("Use DecodeTimestamp: needs field to be defined as ILTagTimestamp")]
    public static DateTimeOffset DecodeDateTimeOffset(this Stream s) => s.DecodeILInt().AsDateTime();

    [Obsolete("Use DecodeTimestamp: needs field to be defined as ILTagTimestamp")]
    public static DateTimeOffset DecodeDateTimeOffset(this Stream s, bool doIt) => doIt ? s.DecodeDateTimeOffset() : DateTimeOffset.UnixEpoch;
    public static Dictionary<string, string?> DecodeDictionary(this Stream s) => s.Decode<ILTagStringDictionary>()!.Value;

    public static Dictionary<string, T?> DecodeDictionary<T>(this Stream s) where T : ILTag {
        var tagId = s.DecodeTagId();
        return tagId == ILTagId.Dictionary
            ? new ILTagDictionary<T>(s).Value
            : throw new InvalidDataException($"Not a {typeof(ILTagDictionary<T>).Name}");
    }

    public static ulong DecodeILInt(this Stream s) => s.Decode<ILTagILInt>()?.Value ?? 0;

    public static ulong[] DecodeILIntArray(this Stream s) => s.Decode<ILTagArrayOfILInt>()?.Value ?? Array.Empty<ulong>();

    public static int DecodeInt(this Stream s) => s.Decode<ILTagInt32>()?.Value ?? 0;

    public static ILTag?[] DecodeSequence(this Stream s) {
        var tagId = s.DecodeTagId();
        return tagId == ILTagId.Sequence ? new ILTagSequence(s).Value : throw new InvalidDataException($"Not a {nameof(ILTagSequence)}");
    }

    public static string? DecodeString(this Stream s) => s.Decode<ILTagString>()?.Value;

    public static Dictionary<string, string?> DecodeStringDictionary(this Stream s) {
        var tagId = s.DecodeTagId();
        return tagId == ILTagId.StringDictionary
            ? new ILTagStringDictionary(s).Value
            : throw new InvalidDataException($"Not a {nameof(ILTagStringDictionary)}");
    }

    public static ILTag DecodeTag(this Stream s)
        => TagProvider.DeserializeFrom(s);

    public static T[] DecodeTagArray<T>(this Stream s) where T : ILTag {
        var tagId = s.DecodeTagId();
        return tagId == ILTagId.ILTagArray
            ? new ILTagArrayOfILTag<T>(s).Value
            : throw new InvalidDataException($"Not a {typeof(ILTagArrayOfILTag<T>).Name}");
    }

    public static ulong DecodeTagId(this Stream s)
        => s.ILIntDecode();
    public static DateTimeOffset? DecodeTimestamp(this Stream s)
        => s.Decode<ILTagTimestamp>()?.Value;
    public static ushort DecodeUShort(this Stream s)
        => s.Decode<ILTagUInt16>()?.Value ?? 0;

    public static Version? DecodeVersion(this Stream s)
        => s.Decode<ILTagVersion>()?.Value;
}