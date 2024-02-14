// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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

namespace InterlockLedger.Tags;

[TestFixture]
[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Buggy warning")]
public class IEnumerableExtensionsTests
{
    [Test]
    public void EqualTo() =>
        Assert.Multiple(() => {
            Assert.That(((IEnumerable<object>)null).EqualTo(null), "Null should be equal to Null");
            Assert.That(((IEnumerable<object>)null).EqualTo([]), Is.False, "Null should NOT be equal to and Empty Enumeration");
            Assert.That(Enumerable.Empty<object>().EqualTo([]), "Empty Enumeration should be equal to Empty Enumeration");
            Assert.That(new int[] { 1 }.EqualTo([1]), "Single Member Enumeration should be equal to equivalent Single Member Enumeration");
            Assert.That(new int[] { 1, 2, 3 }.EqualTo([1]), Is.False, "Multiple Member Enumeration should NOT be equal to Single Member Enumeration");
            Assert.That(new int[] { 1, 2, 3 }.EqualTo([1, 2, 3]), "Multiple Member Enumeration should be equal to equivalent Multiple Member Enumeration");
            Assert.That(new int[] { 1, 2, 3 }.EqualTo([3, 2, 1]), Is.False, "Multiple Member Enumeration should NOT be equal to reordered Multiple Member Enumeration");
        });

    [Test]
    public void IfAnyDo() {
        void IfAnyDoTest(IEnumerable<object> values, bool expected, string message) {
            var doneSomething = false;
            var newvalues = values.IfAnyDo(() => doneSomething = true);
            Assert.Multiple(() => {
                Assert.That(values.EqualTo(newvalues), "output is not the same as input");
                Assert.That(doneSomething, Is.EqualTo(expected), message);
            });
        }

        IfAnyDoTest(null, false, "Shouldn't have done anything for null enumerable");
        IfAnyDoTest([], false, "Shouldn't have done anything for empty enumerable");
        IfAnyDoTest(["something"], true, "Should have done something for non empty enumerable");
    }

    [Test]
    public void SafeAny() {
        static void SafeAnyTest(IEnumerable<object> values, bool expected, string message) => Assert.That(values.SafeAny(), Is.EqualTo(expected), message);
        SafeAnyTest(null, false, "Shouldn't have returned true for null enumerable");
        SafeAnyTest([], false, "Shouldn't have returned true for empty enumerable");
        SafeAnyTest(["something"], true, "Should have returned true for non empty enumerable");
    }

    [Test]
    public void SafeCount() {
        static void SafeCountTest(IEnumerable<object> values, int expected, string message) => Assert.That(values.SafeCount(), Is.EqualTo(expected), message);
        SafeCountTest(null, -1, "Should have returned -1 for null enumerable");
        SafeCountTest([], 0, "Should have returned 0 for empty enumerable");
        SafeCountTest(["something"], 1, "Should have returned 1 for single member enumerable");
    }

    [Test]
    public void SelectSkippingNulls() {
        static void SelectSkippingNullsTest(IEnumerable<string> values, Func<string, string> selector, string message, params string[] expected) => Assert.That(expected.EqualTo(values.SelectSkippingNulls(selector)), message);
        SelectSkippingNullsTest(null, (o) => o, "Should have returned empty enumerable for null enumerable");
        SelectSkippingNullsTest([], (o) => o, "Should have returned empty enumerable for empty enumerable");
        SelectSkippingNullsTest(["something"], (o) => o, "Should have returned single member enumerable for single member enumerable", "something");
        SelectSkippingNullsTest([null, "something"], (o) => o, "Should have returned single member enumerable for dual member enumerable with one null member", "something");
        SelectSkippingNullsTest(["something", "nothing"], (o) => o == "nothing" ? null : o, "Should have returned single member enumerable for dual member enumerable with one filtered out value", "something");
    }

    [Test]
    public void SafeSkipNulls() {
        static void SkipNullsTest(IEnumerable<string> values, string message, params string[] expected)
            => Assert.That(expected.EqualTo(values.SkipNulls()), message);
        SkipNullsTest(null, "Should have returned empty enumerable for null enumerable");
        SkipNullsTest([], "Should have returned empty enumerable for empty enumerable");
        SkipNullsTest(["something"], "Should have returned single member enumerable for single member enumerable", "something");
        SkipNullsTest([null, "something"], "Should have returned single member enumerable for dual member enumerable with one null member", "something");
    }

    [Test]
    public void WithDefault() {
        static void WithDefaultTestWithEmptyInputAndNoDefault(IEnumerable<string> values, string message) => Assert.That(_emptyList.EqualTo(values.WithDefault()), message);

        static void WithDefaultTestWithEmptyInputAndNullDefault(IEnumerable<string> values, string message) {
            Assert.Multiple(() => {
                Assert.That(_emptyList.EqualTo(values.WithDefault((IEnumerable<string>)null)), message);
                Assert.That(_emptyList.EqualTo(values.WithDefault(() => null)), message);
            });
        }

        void WithDefaultTestWithNonEmptyInput(IEnumerable<string> values, string message) {
            Assert.That(values, Is.Not.Null);
            Assert.That(values, Is.Not.Empty);
            Assert.Multiple(() => {
                Assert.That(values.EqualTo(values.WithDefault()), message);
                Assert.That(values.EqualTo(values.WithDefault("anything")), message);
                Assert.That(values.EqualTo(values.WithDefault((IEnumerable<string>)["alternative thing"])), message);
                Assert.That(values.EqualTo(values.WithDefault(() => ["newly generated thing"])), message);
            });
        }

        void WithDefaultTestWithEmptyInputAndSomeDefault(IEnumerable<string> values, string message, params string[] expected) {
            Assert.Multiple(() => {
                Assert.That(expected.EqualTo(values.WithDefault(expected)), message);
                Assert.That(expected.EqualTo(values.WithDefault((IEnumerable<string>)expected)), message);
                Assert.That(expected.EqualTo(values.WithDefault(() => expected)), message);
            });
        }

        WithDefaultTestWithNonEmptyInput(["something"], "Should have returned the same non empty enumerable");
        WithDefaultTestWithEmptyInputAndSomeDefault(null, "Should have returned empty enumerable for null enumerable");
        WithDefaultTestWithEmptyInputAndSomeDefault(null, "Should have returned dual member default enumerable enumerable for null enumerable", "anything", "something else");
        WithDefaultTestWithEmptyInputAndSomeDefault([], "Should have returned empty enumerable for empty enumerable");
        WithDefaultTestWithEmptyInputAndSomeDefault([], "Should have returned dual member default enumerable for empty enumerable", "anything", "something else");
        WithDefaultTestWithEmptyInputAndNoDefault(null, "Should have returned empty enumerable for null enumerable");
        WithDefaultTestWithEmptyInputAndNoDefault([], "Should have returned empty enumerable for empty enumerable");
        WithDefaultTestWithEmptyInputAndNullDefault(null, "Should have returned empty enumerable for null enumerable");
        WithDefaultTestWithEmptyInputAndNullDefault([], "Should have returned empty enumerable for empty enumerable");
    }

    private static readonly IEnumerable<string> _emptyList = [];
}