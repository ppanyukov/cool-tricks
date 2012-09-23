namespace FooLibrary
{
    using NUnit.Framework;

    internal sealed partial class Foo
    {
        private static partial class Tests
        {
            [TestFixture]
            public class StringToNumber
            {
                [Test]
                public void ItWords()
                {
                    var foo = new Foo();
                    var actualResult = foo.StringToNumber("10");
                    Assert.That(actualResult, Is.EqualTo(10));
                }
            }
        }
    }
}
