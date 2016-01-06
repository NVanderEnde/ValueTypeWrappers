# ValueTypeWrappers
.NET Library providing abstract classes for deriving domain-meaningful primitive type wrappers that allow for better code re-use and compile-time business logic certainty. The library provides immutable abstract base classes which wrap `T` (specific base classes are provided for strings and TStructs) in such a way that comparison operations are preserved. Implicit conversion between the wrappers and `T` is also provided. Lastly, a validation hook is provided for value-validation at instantiation time.

#Use

Create a domain model which inherits either from `ValueTypeWrappers.StringWrapper` or `ValueTypeWrappers.StructWrapper<T>`. Use `StructWrapper` for true value types such as `bool`s, `DateTime`s, etc - use `StringWrapper` for, well, you know. Another public abstract class is the `TypeWrapper<T>` which has no generic type constraints, if necessary. 

##Validation
If your domain model requires validation, override the `ValidateValue` method - note that this is *called from the constructor of the base class*, so do not use any instance variables from your derived class. If you need to use instance variables from your derived class in your model validation, do not override `ValidateValue` and instead perform your own custom validation in your implementation.

##Implicit conversion
The abstract classes define implicit conversion between themselves and `T`, that is, anything inheriting from `StringWrapper` can implicitly convert itself to a `string`, and `StructWrapper<int>` can implicitly convert itself to an integer. This allows for the value type wrapper classes to be compatibile with methods and operations that require the underlying `T` without additional developer effort.

##Equality comparison
The abstract classes provide overrides for `==` and `.Equals` which mirror the original semantics of the underlying `T`. Thus, comparing two `StructWrapper<T>`s will perform comparison as if they were the underlying `T` types. `GetHashCode` performs the same way, so `HashSets` and other collections will treat the wrappers as if they were `T`s. Two `StructWrapper<int>`s with the same underlying `int` value will collide in a hash set.

#Installation

This library is available via nuget. To install Value Type Wrappers, run the following command in the Package Manager Console

`Install-Package ValueTypeWrappers`