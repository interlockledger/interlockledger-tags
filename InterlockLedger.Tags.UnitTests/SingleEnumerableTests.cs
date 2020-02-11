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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace InterlockLedger.Tags
{
    public class SingleEnumerableTests
    {
        private const string _niceString = "Nice";

        [Test]
        public void ForEach() {
            int count = 0;
            foreach (var i in new SingleEnumerable<int>(2)) {
                Assert.AreEqual(2, i);
                count++;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ForEachOnNullStringElement() {
            int count = 0;
            foreach (var s in new SingleEnumerable<string>(null)) {
                Assert.IsNull(s);
                count++;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ForEachOnStringElement() {
            int count = 0;
            foreach (var s in new SingleEnumerable<string>(_niceString)) {
                Assert.AreEqual(_niceString, s);
                count++;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void UseIEnumerable() {
            static void TestIteration(IEnumerable en) {
                var e = en.GetEnumerator();
                Assert.NotNull(e);
                Assert.AreEqual(0, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(2, e.Current);
                Assert.IsFalse(e.MoveNext());
                Assert.AreEqual(0, e.Current);
                e.Reset();
            }
            IEnumerable e = new SingleEnumerable<int>(2);
            Assert.NotNull(e);
            TestIteration(e);
            TestIteration(e);
        }

        [Test]
        public void UseIEnumerableOfT() {
            static void TestIteration(IEnumerable<int> en) {
                var e = en.GetEnumerator();
                Assert.NotNull(e);
                Assert.AreEqual(0, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(2, e.Current);
                Assert.IsFalse(e.MoveNext());
                Assert.AreEqual(0, e.Current);
                e.Reset();
            }
            IEnumerable<int> e = new SingleEnumerable<int>(2);
            Assert.NotNull(e);
            TestIteration(e);
            TestIteration(e);
        }

        [Test]
        public void UseIEnumerator() {
            static void TestIteration(IEnumerator e) {
                Assert.AreEqual(0, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(2, e.Current);
                Assert.IsFalse(e.MoveNext());
                Assert.AreEqual(0, e.Current);
            }
            var e = ((IEnumerable)new SingleEnumerable<int>(2)).GetEnumerator();
            Assert.NotNull(e);
            TestIteration(e);
            e.Reset();
            TestIteration(e);
        }

        [Test]
        public void UseIEnumeratorOfT() {
            static void TestIteration(IEnumerator<int> e) {
                Assert.AreEqual(0, e.Current);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(2, e.Current);
                Assert.IsFalse(e.MoveNext());
                Assert.AreEqual(0, e.Current);
            }
            var e = ((IEnumerable<int>)new SingleEnumerable<int>(2)).GetEnumerator();
            Assert.NotNull(e);
            TestIteration(e);
            e.Reset();
            TestIteration(e);
        }

        [Test]
        public void UseParallelIEnumeratorsOfT() {
            static void TestIteration(IEnumerator<int> e, IEnumerator<int> e2) {
                Assert.NotNull(e);
                Assert.NotNull(e2);
                Assert.AreEqual(0, e.Current, nameof(e));
                Assert.AreEqual(0, e2.Current, nameof(e2));
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(2, e.Current, nameof(e));
                Assert.AreEqual(0, e2.Current, nameof(e2));
                Assert.IsFalse(e.MoveNext());
                Assert.AreEqual(0, e.Current, nameof(e));
                Assert.IsTrue(e2.MoveNext());
                Assert.AreEqual(2, e2.Current, nameof(e2));
                Assert.IsFalse(e2.MoveNext());
                Assert.AreEqual(0, e2.Current, nameof(e2));
                Assert.AreEqual(0, e.Current, nameof(e));
            }
            IEnumerable<int> e = new SingleEnumerable<int>(2);
            TestIteration(e.GetEnumerator(), e.GetEnumerator());
        }
    }
}