namespace FooLibrary
{
    using NUnit.Framework;

    internal sealed partial class Foo
    {
        private static partial class Tests
        {
            [TestFixture]
            public class SomePrivateMethod
            {
                [Test]
                public void ItWords()
                {
                    var foo = new Foo();
                    var actualResult = foo.SomePrivateMethod("20");
                    Assert.That(actualResult, Is.EqualTo(20));
                }
            }
        }
    }
}
