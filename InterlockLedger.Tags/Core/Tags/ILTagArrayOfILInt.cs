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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterlockLedger.Tags
{
    public class ILTagArrayOfILInt : ILTagExplicit<ulong[]>
    {
        public ILTagArrayOfILInt(object opaqueValue) : this(Elicit(opaqueValue)) {
        }

        public ILTagArrayOfILInt(ulong[] Value) : base(ILTagId.ILIntArray, Value) {
        }

        public ulong this[int i] => Value?[i] ?? 0ul;

        internal ILTagArrayOfILInt(Stream s) : base(ILTagId.ILIntArray, s) {
        }

        protected override ulong[] FromBytes(byte[] bytes) =>
           FromBytesHelper(bytes, s => {
               var length = (int)s.ILIntDecode();
               var result = new ulong[length];
               for (var i = 0; i < length; i++) {
                   result[i] = s.ILIntDecode();
               }
               return result;
           });

        protected override byte[] ToBytes()
            => ToBytesHelper(s => {
                if (Value != null) {
                    s.ILIntEncode((ulong)Value.Length);
                    foreach (var ilint in Value)
                        s.ILIntEncode(ilint);
                }
            });

        private static ulong[] Elicit(object opaqueValue) {
            if (opaqueValue is null)
                throw new ArgumentNullException(nameof(opaqueValue));
            if (opaqueValue is ulong[] values)
                return values;
            if (opaqueValue is IEnumerable<object> items)
                return items.Select(Convert.ToUInt64).ToArray();
            throw new InvalidCastException($"Can't elicit an ulong[] from {opaqueValue.GetType()}");
        }
    }
}
