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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public static partial class StreamExtensions
    {
        public static T Decode<T>(this Stream s) where T : ILTag {
            var tag = s.DecodeTag();
            return tag.Traits.IsNull
                ? null
                : tag as T
                    ?? throw new InvalidDataException($"Not a {typeof(T).Name} was {tag?.GetType().Name}:{tag}");
        }

        public static T DecodeAny<T>(this Stream s) where T : class, ITaggable {
            var tag = s.DecodeTag();
            return tag.Traits.IsNull
                ? null
                : tag is ILTagOfExplicit<T> t
                    ? t.Value
                    : throw new InvalidDataException($"Not a tagged form of {typeof(T).Name} was {tag?.GetType().Name}:{tag}");
        }

        public static T[] DecodeArray<T>(this Stream s) where T : class, ITaggableOf<T> {
            var tagId = s.DecodeTagId();
            return tagId == ILTagId.ILTagArray
                ? (new ILTagArrayOfILTag<ILTagOfExplicit<T>>(s).Value?.Select(element => element?.Value).ToArray())
                : throw new InvalidDataException($"Not {typeof(ILTagArrayOfILTag<ILTagOfExplicit<T>>).Name}");
        }

        public static T[] DecodeArray<T, TT>(this Stream s, Func<Stream, TT> decoder) where TT : ILTagOfExplicit<T> {
            var tagId = s.DecodeTagId();
            return tagId == ILTagId.ILTagArray
                ? (new ILTagArrayOfILTag<TT>(s, decoder).Value?.Select(element => element.Value).ToArray())
                : throw new InvalidDataException($"Not a {typeof(ILTagArrayOfILTag<TT>).Name}");
        }

        public static bool DecodeBool(this Stream s) => s.Decode<ILTagBool>().Value;

        public static byte DecodeByte(this Stream s) => s.Decode<ILTagUInt8>().Value;

        public static byte[] DecodeByteArray(this Stream s) => s.Decode<ILTagByteArray>()?.Value;

        public static InterlockColor DecodeColor(this Stream s) => InterlockColor.From(s.Decode<ILTagUInt32>().Value);

        public static DateTimeOffset DecodeDateTimeOffset(this Stream s) => s.DecodeILInt().AsDateTime();

        public static Dictionary<string, string> DecodeDictionary(this Stream s) => s.Decode<ILTagStringDictionary>()?.Value;

        public static Dictionary<string, T> DecodeDictionary<T>(this Stream s) where T : ILTag {
            var tagId = s.DecodeTagId();
            return tagId == ILTagId.Dictionary
                ? new ILTagDictionary<T>(s).Value
                : throw new InvalidDataException($"Not a {typeof(ILTagDictionary<T>).Name}");
        }

        public static ulong DecodeILInt(this Stream s) => s.Decode<ILTagILInt>().Value;

        public static ulong[] DecodeILIntArray(this Stream s) => s.Decode<ILTagArrayOfILInt>()?.Value;

        public static int DecodeInt(this Stream s) => s.Decode<ILTagInt32>().Value;

        public static bool? DecodeNullableBool(this Stream s) => NullableIfDefault(s.Decode<ILTagBool>().Value);

        public static byte? DecodeNullableByte(this Stream s) => NullableIfDefault(s.Decode<ILTagUInt8>().Value);

        public static ulong? DecodeNullableILInt(this Stream s) => NullableIfDefault(s.Decode<ILTagILInt>().Value);

        public static ulong? DecodeOptionalILInt(this Stream s) => s.Decode<ILTagILInt>()?.Value;

        public static ILTag[] DecodeSequence(this Stream s) {
            var tagId = s.DecodeTagId();
            return tagId == ILTagId.Sequence ? new ILTagSequence(s).Value : throw new InvalidDataException($"Not a {nameof(ILTagSequence)}");
        }

        public static string DecodeString(this Stream s) => s.Decode<ILTagString>()?.Value;

        public static Dictionary<string, string> DecodeStringDictionary(this Stream s) {
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

        public static ushort DecodeUShort(this Stream s)
            => s.Decode<ILTagUInt16>().Value;

        public static Version DecodeVersion(this Stream s)
            => s.Decode<ILTagVersion>().Value;

        private static T? NullableIfDefault<T>(T value) where T : struct => value.Equals(default(T)) ? null : value;
    }
}