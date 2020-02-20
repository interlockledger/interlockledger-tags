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
using System.Collections;
using System.Collections.Generic;

namespace InterlockLedger.Tags
{
    public sealed class SingleEnumerable<T> : IEnumerable<T>
    {
        public SingleEnumerable(T singleElement) => _singleElement = singleElement;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(_singleElement);

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_singleElement);

        private readonly T _singleElement;

        private class Enumerator : IEnumerator<T>
        {
            public Enumerator(T singleElement) {
                _singleElement = singleElement;
                _count = 1;
            }

            T IEnumerator<T>.Current => _value;
            object IEnumerator.Current => _value;

            void IDisposable.Dispose() { }

            bool IEnumerator.MoveNext() => _count-- > 0;

            void IEnumerator.Reset() => _count = 1;

            private readonly T _singleElement;
            private byte _count;
            private T _value => _count == 0 ? _singleElement : default;
        }
    }
}