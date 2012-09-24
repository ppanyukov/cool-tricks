// P.PANYUKOV, Septempber 2012
//
// Rather than document when parameters can and can't take nulls, and
// rather than document when functions can or can't return nulls, and
// rather than check for nulls in millions of places, and
// rather than use obscure ReSharper attributes to indicate null/not null
// why not use something which can be both enforced and also tell the
// users the requirements/expectations without referring to documentation?
// 
// Use CanBeNull and NeverNull types to do so!
namespace GetRidOfNulls
{
    using System;

    using NUnit.Framework;

    /// <summary>
    /// Non-generic convenience wrapper to create instances by inferring
    /// the type from the type of the argument.
    /// 
    /// Partial for tests.
    /// </summary>
    public static partial class NeverNull
    {
#pragma warning disable 108 // blah blah hides inherited member
// ReSharper disable MemberHidesStaticFromOuterClass
        private static class SelfTests
        {
            [TestFixture]
            public class Ctor
            {
                [Test]
                public void DoesNotThrowWhenValueNotNull()
                {
                    Assert.DoesNotThrow(() => Create("hello"));
                }

                [Test]
                public void WorksWithStructs()
                {
                    Assert.DoesNotThrow(() => Create(DateTime.Now));
                }

                [Test]
                public void ThrowsWhenValueIsNull()
                {
                    Assert.Throws<ArgumentNullException>(() => Create<string>(null));
                }

                [Test]
                public void ValueIsCorrectlyAssigned()
                {
                    const string expectedValue = "some random value";
                    var neverNull = Create(expectedValue);
                    Assert.That(neverNull.Value, Is.SameAs(expectedValue));
                }
            }

            [TestFixture]
            public class ImplicitOpToNeverNull
            {
                [Test]
                public void PreservesObjectIdentityForRefTypes()
                {
                    const string s = "whatever";
                    NeverNull<string> resultOfImplicitOp = s;

                    Assert.That(resultOfImplicitOp.Value, Is.SameAs(s));
                }
            }

            [TestFixture]
            public class ImplicitOpToT
            {
                [Test]
                public void PreservesObjectIdentityForRefTypes()
                {
                    const string s = "whatever";
                    NeverNull<string> neverNull = Create(s);
                    string resultOfImplicitOp = neverNull;
                    Assert.That(resultOfImplicitOp, Is.SameAs(s));
                }
            }

            [TestFixture]
            public class GetHashCode
            {
                [Test]
                public void DelegtesToEncapsulatedValue()
                {
                    const string input = "oehdofd";
                    var expectedValue = input.GetHashCode();
                    var actualValue = Create(input).GetHashCode();
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
            }

            [TestFixture]
            public class Equals
            {
                [Test]
                public void DelegatesToEncapsulatedValue()
                {
                    var input = new EqualsCapture();
                    var neverNull = Create(input);

                    bool b = neverNull.Equals(input);

                    Assert.That(input.EqualsCallCount, Is.EqualTo(1));
                }
            }

            [TestFixture]
            public class ToString
            {
                [Test]
                public void DelegtesToEncapsulatedValue()
                {
                    var input = DateTime.Now;
                    var expectedValue = input.ToString();
                    var actualValue = Create(input).ToString();
                    Assert.That(actualValue, Is.EqualTo(expectedValue));
                }
            }

            /// <summary>
            /// To capture calls to delegated Equals method.
            /// </summary>
            private sealed class EqualsCapture
            {
                public int EqualsCallCount = 0;

                public override bool Equals(object obj)
                {
                    this.EqualsCallCount += 1;
                    return base.Equals(obj);
                }
            }
        }
#pragma warning restore
// ReSharper restore MemberHidesStaticFromOuterClass
    }
}
