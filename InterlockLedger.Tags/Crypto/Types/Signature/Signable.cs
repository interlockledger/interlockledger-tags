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
    public interface ISignable
    {
        ILTag ResolveSigned(ushort version, Stream s);
    }

    public abstract class Signable<T> : VersionedValue<T>, ISignable where T : Signable<T>, new()
    {
        public Signable() : base(0, 0) {
        }

        public ILTag ResolveSigned(ushort version, Stream s) => new SignedValue<T>(version, (T)this, s).AsPayload;

        public SignedValue<T> SignWith(ISigningContext context) {
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            return new SignedValue<T>((T)this, context.Key.SignWithId(AsPayload.EncodedBytes).AsSingle());
        }

        protected Signable(ulong tagId, ushort version) : base(tagId, version) {
        }
    }
}
