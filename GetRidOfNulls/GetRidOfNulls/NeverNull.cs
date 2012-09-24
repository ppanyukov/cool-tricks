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

    /// <summary>
    /// Non-generic convenience wrapper to create instances by inferring
    /// the type from the type of the argument.
    /// 
    /// Partial for tests.
    /// </summary>
    public static partial class NeverNull
    {
        public static NeverNull<T> Create<T>(T value)
        {
            return new NeverNull<T>(value);
        }
    }

    /// <summary>
    /// Encapsulates the given value as the promise to be never null.
    /// Checks the argument at creation time. If constructor succeeded,
    /// this instance can be used as variables, arguments and return values
    /// with the guarantee that the encapsulated value is not null.
    /// </summary>
    public sealed class NeverNull<T>
    {
        private readonly T value;

        /// <summary>
        /// Creates new instance of <see cref="NeverNull{T}"/>. Throws
        /// <see cref="ArgumentNullException"/> if a null value is provided
        /// as the argument.
        /// </summary>
        /// <param name="value">Value to encapsulate.</param>
        public NeverNull(T value)
        {
            // This is the only place we need to check for null!
            // TODO: Check that this works with value types despite warning
            if (value == null)
            {
                const string message = "A null value was given when NeverNull was promised.";
                throw new ArgumentNullException("value", message);
            }

            // Guaranteed to be non-null from here on.
            this.value = value;
        }

        /// <summary>
        /// Gets the encapsulated value. Never returns null, obviously.
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Convenience operator to implicitly create <see cref="NeverNull{T}"/>
        /// instance from <paramref name="value"/> of type <typeparamref name="T"/> 
        /// when <see cref="NeverNull{T}"/> is required as an argument.
        /// </summary>
        /// <example>
        /// <code>
        /// // Function taking NeverNull{string} as argument.
        /// void SomeFun(NeverNull{string} s)
        /// {
        /// }
        /// 
        /// void UseSomeFun()
        /// {
        ///   // Can use explicitly
        ///   var explicit = new NeverNull{string}("hello");
        ///   SomeFun(explicit);
        /// 
        ///   // Can use implicitly
        ///   SomeFun("hello");
        /// }
        /// </code>
        /// </example>
        public static implicit operator NeverNull<T>(T value)
        {
            return new NeverNull<T>(value);
        }

        /// <summary>
        /// Convenience operator to implicitly convert values of type <see cref="NeverNull{T}"/>
        /// to the encapsulated type <typeparamref name="T"/>. Handy for
        /// interoperability with libraries which don't use <see cref="NeverNull{T}"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// // Function returning NeverNull{string}.
        /// NeverNull{string} SomeFun()
        /// {
        /// }
        /// 
        /// // External library function which takes string
        /// void LibraryFun(string s)
        /// {
        /// }
        /// 
        /// // Use without implicit operator
        /// void UsageWithoutImplicitOperator()
        /// {
        ///   // The result is of type NeverNull{string}
        ///   var result = SomeFun();
        /// 
        ///   // Accessing the value
        ///   string s = result.Value;
        /// 
        ///   // This works thanks to implicit operator
        ///   string s2 = result;
        /// 
        ///   // Using with library function
        ///   LibraryFun(result.Value);
        /// 
        ///   // This also works thanks to implicit operator
        ///   LibraryFun(result);
        /// }
        /// 
        /// </code>
        /// </example>
        public static implicit operator T(NeverNull<T> obj)
        {
            return obj.Value;
        }

        /// <summary>
        /// Delegates the call to the encapsulated value.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        /// <summary>
        /// Delegates the call to the encapsulated value.
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.Value.Equals(obj);
        }

        /// <summary>
        /// Delegates the call to the encapsulated value.
        /// </summary>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
