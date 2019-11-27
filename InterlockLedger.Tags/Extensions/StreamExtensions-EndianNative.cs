/******************************************************************************************************************************

Copyright (c) 2018-2019 InterlockLedger Network
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

using System.IO;

namespace InterlockLedger.Tags
{
    public static partial class StreamExtensions
    {
        public static int BigEndianReadInt(this Stream s)
            => ReadByte(s) + (ReadByte(s) << 8) + (ReadByte(s) << 16) + (ReadByte(s) << 24);

        public static long BigEndianReadLong(this Stream s)
            => ReadByte(s) + ((long)ReadByte(s) << 8) + ((long)ReadByte(s) << 16) + ((long)ReadByte(s) << 24) + ((long)ReadByte(s) << 32) + ((long)ReadByte(s) << 40) + ((long)ReadByte(s) << 48) + ((long)ReadByte(s) << 56);

        public static short BigEndianReadShort(this Stream s)
            => (short)(ReadByte(s) + (ReadByte(s) << 8));

        public static uint BigEndianReadUInt(this Stream s)
            => (uint)(ReadByte(s) + (ReadByte(s) << 8) + (ReadByte(s) << 16) + (ReadByte(s) << 24));

        public static ulong BigEndianReadULong(this Stream s)
            => ReadByte(s) + ((ulong)ReadByte(s) << 8) + ((ulong)ReadByte(s) << 16) + ((ulong)ReadByte(s) << 24) + ((ulong)ReadByte(s) << 32) + ((ulong)ReadByte(s) << 40) + ((ulong)ReadByte(s) << 48) + ((ulong)ReadByte(s) << 56);

        public static ushort BigEndianReadUShort(this Stream s)
            => (ushort)(ReadByte(s) + (ReadByte(s) << 8));

        public static Stream BigEndianWriteInt(this Stream s, int value)
            => s.WriteSingleByte(AsByte(value)).WriteSingleByte(AsByte(value >> 8)).WriteSingleByte(AsByte(value >> 16)).WriteSingleByte(AsByte(value >> 24));

        public static Stream BigEndianWriteLong(this Stream s, long value)
            => s.WriteSingleByte(AsByte(value)).WriteSingleByte(AsByte(value >> 8)).WriteSingleByte(AsByte(value >> 16)).WriteSingleByte(AsByte(value >> 24))
                .WriteSingleByte(AsByte(value >> 32)).WriteSingleByte(AsByte(value >> 40)).WriteSingleByte(AsByte(value >> 48)).WriteSingleByte(AsByte(value >> 56));

        public static Stream BigEndianWriteShort(this Stream s, short value)
            => s.WriteSingleByte(AsByte(value)).WriteSingleByte(AsByte(value >> 8));

        public static Stream BigEndianWriteUInt(this Stream s, uint value)
            => s.WriteSingleByte(AsByte(value)).WriteSingleByte(AsByte(value >> 8)).WriteSingleByte(AsByte(value >> 16)).WriteSingleByte(AsByte(value >> 24));

        public static Stream BigEndianWriteULong(this Stream s, ulong value)
            => s.WriteSingleByte(AsByteU(value)).WriteSingleByte(AsByteU(value >> 8)).WriteSingleByte(AsByteU(value >> 16)).WriteSingleByte(AsByteU(value >> 24))
                .WriteSingleByte(AsByteU(value >> 32)).WriteSingleByte(AsByteU(value >> 40)).WriteSingleByte(AsByteU(value >> 48)).WriteSingleByte(AsByteU(value >> 56));

        public static Stream BigEndianWriteUShort(this Stream s, ushort value)
            => s.WriteSingleByte(AsByte(value)).WriteSingleByte(AsByte(value >> 8));

        public static int LittleEndianReadInteger(this Stream s)
            => (ReadByte(s) << 24) + (ReadByte(s) << 16) + (ReadByte(s) << 8) + ReadByte(s);

        public static long LittleEndianReadLong(this Stream s)
            => (((long)ReadByte(s)) << 56) + (((long)ReadByte(s)) << 48) + (((long)ReadByte(s)) << 40) + (((long)ReadByte(s)) << 32) + (((long)ReadByte(s)) << 24) + (((long)ReadByte(s)) << 16) + (((long)ReadByte(s)) << 8) + ReadByte(s);

        public static short LittleEndianReadShort(this Stream s)
            => (short)((ReadByte(s) << 8) + ReadByte(s));

        public static void LittleEndianWriteInteger(this Stream s, int value)
            => WriteBytes(s, value.ToBytes());

        public static void LittleEndianWriteLong(this Stream s, long value)
            => WriteBytes(s, value.ToBytes());

        public static void LittleEndianWriteShort(this Stream s, short value)
            => WriteBytes(s, value.ToBytes());
    }
}