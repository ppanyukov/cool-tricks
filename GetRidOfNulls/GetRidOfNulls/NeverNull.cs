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
    /// </summary>
    /// <remarks>
    /// The type is partial for test and extensibility purposes.
    /// </remarks>
    public static partial class NeverNull
    {
        /// <summary>
        /// Creates new instance of <see cref="NeverNull{T}"/> encapsulating the
        /// given value. Throws <see cref="ArgumentNullException"/> if a null value 
        /// is provided as the argument.
        /// </summary>
        /// <param name="value">Value to encapsulate. Must not be null.</param>
        public static NeverNull<T> Create<T>(T value)
        {
            return new NeverNull<T>(value);
        }

        public static NeverNull<T> Create<T>(NeverNull<T> other)
        {
            return new NeverNull<T>(other);
        }
    }

    /// <summary>
    /// Encapsulates the given value as the promise to be never null.
    /// Checks the argument at creation time. If constructor succeeded,
    /// this instance can be used as variables, arguments and return values
    /// with the guarantee that the encapsulated value is not null.
    /// 
    /// DO NOT CALL CONSTRUCTORS ON THIS TYPE DIRECTLY. Use implicit
    /// conversion operators and non-generic <see cref="NeverNull"/>
    /// instead.
    /// 
    /// </summary>
    /// <remarks>
    /// OK, this does not guarantee that the encapsulated value is never
    /// going to be null, because this is a struct and it's always possible
    /// to call the default constructor on it, and it will intialise the 
    /// encapsulated value to its default (which is null for reference types).
    /// 
    /// One solution would be to make it a class, but then it would become a
    /// reference type and it would be possible to use nulls where this type
    /// is expected (thus just shifting the problem of nulls).
    /// 
    /// This type tries to solve two problems: 1) of documenting parameters and return
    /// types without documentation; and 2) reducing the number of NullReferenceException
    /// to a minimum.
    /// 
    /// Looking at the class vs. struct issue, the struct approach will probably work
    /// better: the values of the NeverNull type will never be null themselves, and
    /// the in majority of the cases the encapsulated value will be non-null too.
    /// 
    /// Invoking default constructor or use of arrays (e.g. new NeverNull{string}[10])
    /// are probably going to be rare in real life, especially that there are
    /// implicit conversion operators and non-generic NeverNull type to help
    /// create instances.
    /// 
    /// Even if encapsulated value does happen to be null from time to time,
    /// hopefully it will rare enough for this type to add value, and be easy to 
    /// trace to where the default constructor was used when it happens.
    /// </remarks>
    public struct NeverNull<T>
    {
        private readonly T value;

        /// <summary>
        /// DO NOT CALL DIRECTLY. Use non-generic <see cref="NeverNull"/> instead.
        /// 
        /// Creates new instance of <see cref="NeverNull{T}"/> encapsulating the
        /// given value. Throws <see cref="ArgumentNullException"/> if a null value 
        /// is provided as the argument.
        /// </summary>
        /// <param name="value">Value to encapsulate. Must not be null.</param>
        public NeverNull(T value)
        {
            // This is the only place we need to check for null!
            // TODO: Check that this works with value types despite warning
            if (value == null)
            {
                const string message = "A null value was given when NeverNull was promised.";
                throw new ArgumentNullException("value", message);
            }

            this.value = value;
        }

        /// <summary>
        /// DO NOT CALL DIRECTLY. Use non-generic <see cref="NeverNull"/> instead.
        /// 
        /// Creates new instance of <see cref="NeverNull{T}"/> from another instance
        /// of <see cref="NeverNull{T}"/>. The new instance encapsulates the same value.
        /// </summary>
        /// <param name="other">An instance of <see cref="NeverNull{T}"/> from which to create new instance.</param>
        public NeverNull(NeverNull<T> other) : this(other.Value)
        {
        }

        /// <summary>
        /// Gets the encapsulated value. Never returns null, obviously.
        /// </summary>
        public T Value
        {
            get
            {
                if (this.value == null)
                {
                    throw new NullReferenceException(
                        "Oh snap, the promise of NeverNull is broken. " + 
                        "Did you call default constructor on NeverNull<T>?");
                }
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
