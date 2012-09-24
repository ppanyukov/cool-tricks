namespace GetRidOfNulls
{
    using System;

    public static partial class CanBeNull
    {
        public static CanBeNull<T> Create<T>(T value) where T:class 
        {
            return new CanBeNull<T>(value);
        }
    }

    /// <summary>
    /// Modelled after <see cref="Nullable{T}"/>, except works on reference types.
    /// </summary>
    public struct CanBeNull<T> where T:class 
    {
        private readonly T value;

        public CanBeNull(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get
            {
                return this.value;
            }
        }

        public bool HasValue
        {
            get
            {
                return this.value != null;
            }
        }

        public static implicit operator T(CanBeNull<T> other)
        {
            return other.Value;
        }

        public static implicit operator CanBeNull<T>(T value)
        {
            return new CanBeNull<T>(value);
        }

        public override bool Equals(object obj)
        {
            if(this.HasValue)
            {
                return this.Value.Equals(obj);
            }
            else
            {
                return Object.Equals(obj, null);
            }
        }

        public override int GetHashCode()
        {
            if (this.HasValue)
            {
                return this.Value.GetHashCode();
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            if (this.HasValue)
            {
                return this.Value.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
