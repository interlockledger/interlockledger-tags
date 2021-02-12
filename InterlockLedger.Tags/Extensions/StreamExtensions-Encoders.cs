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
        public static Stream EncodeAny<T>(this Stream s, T value) where T : ITaggable
            => s.EncodeTag(value?.AsILTag);

        public static Stream EncodeArray<T>(this Stream s, IEnumerable<T> values) where T : class, ITaggableOf<T>
            => s.EncodeTag(new ILTagArrayOfILTag<ILTagExplicit<T>>(values?.Select(v => v.AsTag).ToArray()));

        public static Stream EncodeBool(this Stream s, bool value)
            => s.EncodeTag(value ? ILTagBool.True : ILTagBool.False);

        public static Stream EncodeByte(this Stream s, byte value)
            => s.EncodeTag(new ILTagUInt8(value));

        public static Stream EncodeByteArray(this Stream s, byte[] value)
            => value == null ? s.EncodeNull() : s.EncodeTag(new ILTagByteArray(value));

        public static Stream EncodeByteArray(this Stream s, Span<byte> value)
            => s.EncodeTag(new ILTagByteArray(value));

        public static Stream EncodeColor(this Stream s, InterlockColor value)
            => s.EncodeTag(new ILTagUInt32(value.RGBA));

        public static Stream EncodeDateTimeOffset(this Stream s, DateTimeOffset value)
            => s.EncodeILInt(value.AsMilliseconds());

        public static Stream EncodeDictionary(this Stream s, Dictionary<string, string> dictionary)
            => s.EncodeTag(new ILTagStringDictionary(dictionary));

        public static Stream EncodeILInt(this Stream s, ulong value)
            => s.EncodeTag(new ILTagILInt(value));

        public static Stream EncodeILIntArray(this Stream s, IEnumerable<ulong> values)
            => s.EncodeTag(new ILTagArrayOfILInt(values?.ToArray()));

        public static Stream EncodeInt(this Stream s, int value)
            => s.EncodeTag(new ILTagInt32(value));

        public static Stream EncodeNull(this Stream s) => ILTagNull.Instance.SerializeInto(s);

        public static Stream EncodeOptionalILInt(this Stream s, ulong? optionalValue)
            => (optionalValue.HasValue) ? s.EncodeILInt(optionalValue.Value) : s.EncodeNull();

        public static Stream EncodeOptionalTailingByteArray(this Stream s, byte[] value)
            => value != null ? s.EncodeByteArray(value) : s;

        public static Stream EncodeSequence(this Stream s, IEnumerable<ILTag> values)
            => s.EncodeTag(new ILTagSequence(values?.ToArray()));

        public static Stream EncodeString(this Stream s, string value)
            => s.EncodeTag(new ILTagString(value));

        public static Stream EncodeTag(this Stream s, ILTag tag)
            => tag == null ? s.EncodeNull() : tag.SerializeInto(s);

        public static Stream EncodeTagArray<T>(this Stream s, IEnumerable<T> values) where T : ILTag
            => s.EncodeTag(new ILTagArrayOfILTag<T>(values?.ToArray()));

        public static Stream EncodeUShort(this Stream s, ushort value)
            => s.EncodeTag(new ILTagUInt16(value));

        public static Stream EncodeVersion(this Stream s, Version version)
            => s.EncodeTag(new ILTagVersion(version));
    }
}