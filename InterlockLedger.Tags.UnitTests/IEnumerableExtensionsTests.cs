// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
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
public class IEnumerableExtensionsTests
{
    [Test]
    public void EqualTo() {
        Assert.That(((IEnumerable<object>)null).EqualTo(null), "Null should be equal to Null");
        Assert.That(((IEnumerable<object>)null).EqualTo(Enumerable.Empty<object>()), Is.False, "Null should NOT be equal to and Empty Enumeration");
        Assert.That(Enumerable.Empty<object>().EqualTo(Enumerable.Empty<object>()), "Empty Enumeration should be equal to Empty Enumeration");
        Assert.That(new int[] { 1 }.EqualTo(new int[] { 1 }), "Single Member Enumeration should be equal to equivalent Single Member Enumeration");
        Assert.That(new int[] { 1, 2, 3 }.EqualTo(new int[] { 1 }), Is.False, "Multiple Member Enumeration should NOT be equal to Single Member Enumeration");
        Assert.That(new int[] { 1, 2, 3 }.EqualTo(new int[] { 1, 2, 3 }), "Multiple Member Enumeration should be equal to equivalent Multiple Member Enumeration");
        Assert.That(new int[] { 1, 2, 3 }.EqualTo(new int[] { 3, 2, 1 }), Is.False, "Multiple Member Enumeration should NOT be equal to reordered Multiple Member Enumeration");
    }

    [Test]
    public void IfAnyDo() {
        void IfAnyDoTest(IEnumerable<object> values, bool expected, string message) {
            var doneSomething = false;
            var newvalues = values.IfAnyDo(() => doneSomething = true);
            Assert.That(values.EqualTo(newvalues), "output is not the same as input");
            Assert.That(doneSomething, Is.EqualTo(expected), message);
        }
        IfAnyDoTest(null, false, "Shouldn't have done anything for null enumerable");
        IfAnyDoTest(Array.Empty<object>(), false, "Shouldn't have done anything for empty enumerable");
        IfAnyDoTest(new object[] { "something" }, true, "Should have done something for non empty enumerable");
    }

    [Test]
    public void SafeAny() {
        static void SafeAnyTest(IEnumerable<object> values, bool expected, string message) => Assert.That(values.SafeAny(), Is.EqualTo(expected), message);
        SafeAnyTest(null, false, "Shouldn't have returned true for null enumerable");
        SafeAnyTest(Array.Empty<object>(), false, "Shouldn't have returned true for empty enumerable");
        SafeAnyTest(new object[] { "something" }, true, "Should have returned true for non empty enumerable");
    }

    [Test]
    public void SafeCount() {
        static void SafeCountTest(IEnumerable<object> values, int expected, string message) => Assert.That(values.SafeCount(), Is.EqualTo(expected), message);
        SafeCountTest(null, -1, "Should have returned -1 for null enumerable");
        SafeCountTest(Array.Empty<object>(), 0, "Should have returned 0 for empty enumerable");
        SafeCountTest(new object[] { "something" }, 1, "Should have returned 1 for single member enumerable");
    }

    [Test]
    public void SelectSkippingNulls() {
        static void SelectSkippingNullsTest(IEnumerable<string> values, Func<string, string> selector, string message, params string[] expected) => Assert.That(expected.EqualTo(values.SelectSkippingNulls(selector)), message);
        SelectSkippingNullsTest(null, (o) => o, "Should have returned empty enumerable for null enumerable");
        SelectSkippingNullsTest(Array.Empty<string>(), (o) => o, "Should have returned empty enumerable for empty enumerable");
        SelectSkippingNullsTest(new string[] { "something" }, (o) => o, "Should have returned single member enumerable for single member enumerable", "something");
        SelectSkippingNullsTest(new string[] { null, "something" }, (o) => o, "Should have returned single member enumerable for dual member enumerable with one null member", "something");
        SelectSkippingNullsTest(new string[] { "something", "nothing" }, (o) => o == "nothing" ? null : o, "Should have returned single member enumerable for dual member enumerable with one filtered out value", "something");
    }

    [Test]
    public void SafeSkipNulls() {
        static void SkipNullsTest(IEnumerable<string> values, string message, params string[] expected)
            => Assert.That(expected.EqualTo(values.SafeSkipNulls()), message);
        SkipNullsTest(null, "Should have returned empty enumerable for null enumerable");
        SkipNullsTest(Array.Empty<string>(), "Should have returned empty enumerable for empty enumerable");
        SkipNullsTest(new string[] { "something" }, "Should have returned single member enumerable for single member enumerable", "something");
        SkipNullsTest(new string[] { null, "something" }, "Should have returned single member enumerable for dual member enumerable with one null member", "something");
    }

    [Test]
    public void WithDefault() {
        static void WithDefaultTestWithEmptyInputAndNoDefault(IEnumerable<string> values, string message) => Assert.That(_emptyList.EqualTo(values.WithDefault()), message);

        static void WithDefaultTestWithEmptyInputAndNullDefault(IEnumerable<string> values, string message) {
            Assert.That(_emptyList.EqualTo(values.WithDefault((IEnumerable<string>)null)), message);
            Assert.That(_emptyList.EqualTo(values.WithDefault(() => null)), message);
        }
        void WithDefaultTestWithNonEmptyInput(IEnumerable<string> values, string message) {
            Assert.That(values, Is.Not.Null);
            Assert.That(values, Is.Not.Empty);
            Assert.That(values.EqualTo(values.WithDefault()), message);
            Assert.That(values.EqualTo(values.WithDefault("anything")), message);
            Assert.That(values.EqualTo(values.WithDefault((IEnumerable<string>)new string[] { "alternative thing" })), message);
            Assert.That(values.EqualTo(values.WithDefault(() => new string[] { "newly generated thing" })), message);
        }
        void WithDefaultTestWithEmptyInputAndSomeDefault(IEnumerable<string> values, string message, params string[] expected) {
            Assert.That(expected.EqualTo(values.WithDefault(expected)), message);
            Assert.That(expected.EqualTo(values.WithDefault((IEnumerable<string>)expected)), message);
            Assert.That(expected.EqualTo(values.WithDefault(() => expected)), message);
        }
        WithDefaultTestWithNonEmptyInput(new string[] { "something" }, "Should have returned the same non empty enumerable");
        WithDefaultTestWithEmptyInputAndSomeDefault(null, "Should have returned empty enumerable for null enumerable");
        WithDefaultTestWithEmptyInputAndSomeDefault(null, "Should have returned dual member default enumerable enumerable for null enumerable", "anything", "something else");
        WithDefaultTestWithEmptyInputAndSomeDefault(Array.Empty<string>(), "Should have returned empty enumerable for empty enumerable");
        WithDefaultTestWithEmptyInputAndSomeDefault(Array.Empty<string>(), "Should have returned dual member default enumerable for empty enumerable", "anything", "something else");
        WithDefaultTestWithEmptyInputAndNoDefault(null, "Should have returned empty enumerable for null enumerable");
        WithDefaultTestWithEmptyInputAndNoDefault(Array.Empty<string>(), "Should have returned empty enumerable for empty enumerable");
        WithDefaultTestWithEmptyInputAndNullDefault(null, "Should have returned empty enumerable for null enumerable");
        WithDefaultTestWithEmptyInputAndNullDefault(Array.Empty<string>(), "Should have returned empty enumerable for empty enumerable");
    }

    private static readonly IEnumerable<string> _emptyList = Enumerable.Empty<string>();
}