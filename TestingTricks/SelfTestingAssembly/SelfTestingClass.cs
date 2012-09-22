// # Writing self-testing code and testing private methods.
// *(Philip Panyukov, September 2012)*
// 
// This piece of code demonstrates two things:
// 
// 1.  How to embed tests in the same assembly as the code, but without
//     making the tests public.
// 
// 2.  How this trick can also be used to test private methods avoiding
//     reflection and other nastiness.
// 
// In this demo there is one internal class with two private methods.
// The tests are embedded together with the methods they test,
// and NUnit happily recognises them.
// 
// Normally this would be surprising -- NUnit wants the test fixture
// classes to be public. So how does this work if we have test fixtures
// embedded in the internal class?
// 
// The trick is based on the fact that NUnit uses reflection to load
// the types from the assemblies. The list of types includes *all* types,
// regardless whether they are public, internal or private. Then NUnit
// selects types which are "public" and which have "TestFixture" attribute
// on them. And here "public" does not mean "visible to anyone" -- as long
// as the class marked with "TestFixture" attribute is written as 
// `public class`, it will work.
// 
// Now to make the public fixture class invisible to everything except
// NUnit (and reflection), all we need to do is put the test fixture into
// the private inner class. The wrapper is not visible to anything, even
// types within the same assembly. But the test fixture within that wrapper
// is more than visible to NUnit.
// 
// Problem solved!
// 
// Another bonus here is the ability to test private methods. This is based
// on the fact that inner classe have access to private members of outer classes.
// 
// Another problem solved!
//
// Oh, and if it's really undesirable to include the tests
// in some releases, use preprocessor
namespace SelfTestingAssembly
{
// Use preprocessor to exlude tests it this is really required
#if !NO_TESTS
    using NUnit.Framework;
#endif

    /// <summary>
    /// Our class which we want to test.
    /// </summary>
    internal sealed class SelfTestingClass
    {
        /// <summary>
        /// Our private static method we want to test.
        /// </summary>
        private static int AddStuff(int x, int y)
        {
            return x + y;
        }

        /// <summary>
        /// Out instance private method which we also want to test.
        /// </summary>
        private int DivideStuff(int x, int y)
        {
            return x / y;
        }

        // Use preprocessor to exlude tests in some builds if required
#if !NO_TESTS
        /// <summary>
        /// Here is the trick:
        /// 
        /// This inner private class wraps our test fixtures.
        /// By making it private, we hide it from everyone outside of this class.
        /// </summary>
        private static class TestsWrapper
        {
            /// <summary>
            /// The actual test fixture. We exploit the fact that NUnit
            /// cares only that the test fixture is a public non-static class.
            /// It doesn't care that this class is actually hidden
            /// within a private inner class. So all the tests get picked up!
            /// </summary>
            [TestFixture]
            public sealed class SelfTests
            {
                /// <summary>
                /// Inner classes have access to the private members
                /// of outer classes, so we can test the private method here!
                /// </summary>
                [Test]
                public void TestAddStuff()
                {
                    // Yep, this compiles and runs fine.
                    var result = SelfTestingClass.AddStuff(3, 2);
                    Assert.That(result, Is.EqualTo(5));
                }
             
                /// <summary>
                /// The same works for instance methods.
                /// </summary>
                [Test]
                public void TestDivideStuff()
                {
                    var typeUnderTest = new SelfTestingClass();

                    // Yep, this also compiles.
                    var result = typeUnderTest.DivideStuff(10, 2);
                    Assert.That(result, Is.EqualTo(5));
                }
            }
        }
#endif // NO_TEST
    }
}
