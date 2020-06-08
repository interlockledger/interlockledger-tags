/******************************************************************************************************************************

Copyright (c) 2018-2020 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace InterlockLedger.Tags
{
    public static partial class StreamExtensions
    {
        public static async Task<byte[]> BuildPayloadBytesAsync(this Stream s, ulong tagId) {
            var bytes = await ReadAllBytesAsync(s).ConfigureAwait(false);
            using var ms = new MemoryStream();
            ms.ILIntEncode(tagId);
            ms.ILIntEncode((ulong)bytes.Length);
            ms.WriteBytes(bytes);
            ms.Position = 0;
            return await ms.ReadAllBytesAsync().ConfigureAwait(false);
        }

        public static BaseKeyId DecodeBaseKeyId(this Stream s) => s.Decode<BaseKeyId>();

        public static OwnerId DecodeOwnerId(this Stream s) => s.Decode<OwnerId>();

        public static Stream EncodeInterlockId(this Stream s, InterlockId value) => s.EncodeTag(value);

        public static bool HasBytes(this Stream s) => !(s is null) && s.CanSeek && s.Position < s.Length;

        public static async Task<byte[]> ReadAllBytesAsync(this Stream s) {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            using var buffer = new MemoryStream();
            await s.CopyToAsync(buffer).ConfigureAwait(false);
            return buffer.ToArray();
        }

        public static byte[] ReadBytes(this Stream s, int length) {
            if (s is null || length <= 0)
                return Array.Empty<byte>();
            var bytes = new byte[length];
            var offset = 0;
            var retries = 3;
            while (length > 0) {
                var count = s.Read(bytes, offset, length);
                if (count == length)
                    break;
                if (count == 0) {
                    if (retries-- < 1)
                        throw new TooFewBytesException();
                    Thread.Sleep(100);
                } else
                    retries = 3;
                length -= count;
                offset += count;
            }
            return bytes;
        }

        public static byte[] ReadExactly(this Stream s, int length) {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            var offset = 0;
            var buffer = new byte[length];
            while (offset < length) {
                offset += s.Read(buffer, offset, length - offset);
            }
            return buffer;
        }

        public static byte ReadSingleByte(this Stream s) {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            var bytes = new byte[1];
            var retries = 3;
            while (retries-- > 0) {
                if (s.Read(bytes, 0, 1) == 1)
                    return bytes[0];
                Thread.Sleep(100);
            }
            throw new TooFewBytesException();
        }

        public static Stream WriteBytes(this Stream s, byte[] bytes) {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (bytes?.Length > 0)
                s.Write(bytes, 0, bytes.Length);
            s.Flush();
            return s;
        }

        public static Stream WriteSingleByte(this Stream s, byte value) {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            s.WriteByte(value);
            return s;
        }

        private static byte AsByte(long value) => (byte)(value & 0xFF);

        private static byte AsByteU(ulong value) => (byte)(value & 0xFF);

        private static byte ReadByte(Stream s) {
            var retries = 3;
            while (retries-- > 0) {
                var v = s.ReadByte();
                if (v >= 0)
                    return (byte)v;
                Thread.Sleep(100);
            }
            throw new TooFewBytesException();
        }
    }
}