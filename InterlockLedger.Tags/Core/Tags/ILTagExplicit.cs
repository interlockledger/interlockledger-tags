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

namespace InterlockLedger.Tags
{

    public abstract class ILTagExplicit<T> : ILTagExplicitBase<T>
    {
        protected ILTagExplicit(ulong tagId, T value) : base(tagId, value) {
        }

        protected ILTagExplicit(ulong alreadyDeserializedTagId, Stream s, Action<ILTag> setup = null)
            : base(alreadyDeserializedTagId, s, setup) {
        }

        protected static T FromBytesHelper(byte[] bytes, Func<Stream, T> deserialize) {
            if (bytes == null || bytes.Length == 0)
                return default;
            if (deserialize == null) {
                throw new ArgumentNullException(nameof(deserialize));
            }
            using var s = new MemoryStream(bytes);
            return deserialize(s);
        }

        protected sealed override T DeserializeValueFromStream(Stream s, ulong length)
            => FromBytes(s.ReadBytes((int)length));

        protected abstract T FromBytes(byte[] bytes);

        protected sealed override void SerializeValueToStream(Stream s) => s.WriteBytes(ToBytes());
        protected override ulong GetValueEncodedLength() => (ulong)(ToBytes()?.Length ?? 0);
        protected abstract byte[] ToBytes();
    }
}