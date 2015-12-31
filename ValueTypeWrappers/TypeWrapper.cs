using System;
using System.Diagnostics.CodeAnalysis;

namespace ValueTypeWrappers
{
    /// <summary>
    /// Immutable wrapper around T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TypeWrapper<T>
    {
        public T Value { get; }

        protected TypeWrapper(T value)
        {
            Value = value;
            if (!ValidateValue(value))
            {
                throw new ArgumentException($"Initialization value '{value}' failed validation", nameof(value));
            }
        }

        // Failing to cast will result in an infinite recursion since == will call the override if not downcast
        [SuppressMessage("ReSharper", "RedundantCast.0")]
        public static bool operator ==(TypeWrapper<T> A, TypeWrapper<T> B)
        {
            if ((object)A == null && (object)B == null) return true;
            if ((object)A == null || (object)B == null) return false;
            return A.Equals(B);
        }

        public static bool operator !=(TypeWrapper<T> A, TypeWrapper<T> B)
        {
            return !(A == B);
        }

        /// <summary>
        /// Allows for implicit conversion of a wrapped T to a T
        /// </summary>
        /// <param name="P"></param>
        public static implicit operator T(TypeWrapper<T> P)
        {
            return P.Value;
        }

        /// <summary>
        /// Checks reference equality, then delegates to value equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((TypeWrapper<T>)obj);
        }

        /// <summary>
        /// Performs a value equality check
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(TypeWrapper<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Implement your validation logic against T here.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>This method is called in the constructor of the base class,
        /// So ensure you do not access any instance members of your derived class in the body of this method.
        /// If you need to validate T against an instance member of a derived class, implement it in the constructor of the derived class.</remarks>
        protected virtual bool ValidateValue(T value)
        {
            return true;
        }
    }
}
