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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public abstract class ILTagImplicit<T> : ILTag
    {
        [JsonIgnore]
        public override object AsJson => Value;

        public T Value { get; set; }

        protected ILTagImplicit() : base(0) { }

        protected ILTagImplicit(ulong tagId, T value) : base(tagId) => Value = value;

        [SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "We need it")]
        protected ILTagImplicit(Stream s, ulong alreadyDeserializedTagId, Action<ILTag> setup = null) : base(alreadyDeserializedTagId) {
            setup?.Invoke(this);
            Value = DeserializeInner(s);
        }

        [SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "We need it")]
        protected ILTagImplicit(ulong tagId, Stream s) : base(tagId) {
            ValidateTagId(s.ILIntDecode());
            Value = DeserializeInner(s);
        }

        protected abstract T DeserializeInner(Stream s);

        protected void ValidateTagId(ulong decodedTagId) {
            if (decodedTagId != TagId)
                throw new InvalidDataException($"This is not an {GetType().Name}");
        }
    }
}