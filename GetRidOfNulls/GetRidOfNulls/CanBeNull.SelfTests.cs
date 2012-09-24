namespace GetRidOfNulls
{
    using NUnit.Framework;

    public static partial class CanBeNull
    {
        private static class SelfTests
        {
            [TestFixture]
            public class Ctor
            {
                [Test]
                public void WorksWithReferenceTypes()
                {
                    var x = new CanBeNull<string>("hello");
                }

                [Test]
                public void AssignsNullValueCorrectly()
                {
                    const string input = "hello";
                    var x = new CanBeNull<string>(input);
                    Assert.That(x.Value, Is.SameAs(input));
                }

                [Test]
                public void AssignsNonNullValueCorrectly()
                {
                    const string input = null;
                    var x = new CanBeNull<string>(input);
                    Assert.That(x.Value, Is.Null);
                }
            }

            [TestFixture]
            public class Create
            {
                [Test]
                public void WorksWithReferenceTypes()
                {
                    var x = NeverNull.Create("hello");
                }

                [Test]
                public void AssignsNullValueCorrectly()
                {
                    const string input = "hello";
                    var x = NeverNull.Create(input);
                    Assert.That(x.Value, Is.SameAs(input));
                }

                [Test]
                public void AssignsNonNullValueCorrectly()
                {
                    const string input = null;
                    var x = CanBeNull.Create(input);
                    Assert.That(x.Value, Is.Null);
                }
            }

            [TestFixture]
            public class ImplicitOperators
            {
                [Test]
                public void ConvertsToCanBeNull()
                {
                    string input = "hello";
                    CanBeNull<string> canBeNull = input;
                    Assert.That(canBeNull.Value, Is.SameAs(input));
                }

                [Test]
                public void ConvertsFromCanBeNull()
                {
                    string input = "hello";
                    CanBeNull<string> canBeNull = CanBeNull.Create(input);

                    string output = canBeNull;
                    Assert.That(output, Is.SameAs(input));
                }
            }

            [TestFixture]
            public class Equals
            {
                [Test]
                public void DelegatesToEncapsulatedValue()
                {
                    var input = new EqualsCapture();
                    var x = CanBeNull.Create(input).Equals(input);
                    Assert.That(input.EqualsCallCount, Is.EqualTo(1));
                }

                [Test]
                public void StillWorksWithNulls()
                {
                    string input = "";
                    var x = CanBeNull.Create(input).Equals("some other string");
                }
            }

            [TestFixture]
            public class ToString
            {
                [Test]
                public void DelegatesToEncapsulatedValue()
                {
                    var input = new EqualsCapture();
                    var x = CanBeNull.Create(input).ToString();
                    Assert.That(input.ToStringCallCount, Is.EqualTo(1));
                }

                [Test]
                public void ReturnsEmptyStringWhenNull()
                {
                    string input = null;
                    var x = CanBeNull.Create(input).ToString();
                    Assert.That(x, Is.EqualTo(""));
                }
            }

            [TestFixture]
            public class GetHashcode
            {
                [Test]
                public void DelegatesToEncapsulatedValue()
                {
                    var input = new EqualsCapture();
                    var x = CanBeNull.Create(input).GetHashCode();
                    Assert.That(input.GetHashCodeCallCount, Is.EqualTo(1));
                }

                [Test]
                public void ReturnsZeroWhenValueIsNull()
                {
                    string input = null;
                    var x = CanBeNull.Create(input).GetHashCode();
                    Assert.That(x, Is.EqualTo(0));
                }
            }

            /// <summary>
            /// To capture calls to delegated Equals method.
            /// </summary>
            private sealed class EqualsCapture
            {
                public int EqualsCallCount = 0;
                public int GetHashCodeCallCount = 0;
                public int ToStringCallCount = 0;

                public override string ToString()
                {
                    this.ToStringCallCount += 1;
                    return base.ToString();
                }

                public override int GetHashCode()
                {
                    this.GetHashCodeCallCount += 1;
                    return base.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    this.EqualsCallCount += 1;
                    return base.Equals(obj);
                }
            }
        }
    }
}
