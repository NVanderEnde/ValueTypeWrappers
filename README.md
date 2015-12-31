# ValueTypeWrappers
.NET Library providing abstract classes for deriving domain-meaningful primitive type wrappers that allow for better code re-use and compile-time business logic certainty. The library provides immutable abstract base classes which wrap `T` (specific base classes are provided for strings and TStructs) in such a way that comparison operations are preserved. Implicit conversion between the wrappers and `T` is also provided. Lastly, a validation hook is provided for value-validation at instantiation time.

# Intent
One of the cornerstones of domain-driven design (or by another name, "good object-oriented programming") is to have model classes that reflect the types of things the business cares about. We can describe an application's domain model space as *rich* when it allows us to code business logic in a style that mirrors natural language. Frequently, however, model classes will resemble the following:


    public class Customer 
    {
        public int CustomerId { get; set; }
    	public string EmailAddress { get; set; }
        public string CustomerName { get; set; }
        public int CustomerNumber { get; set; }
    }


...where primitive value types are composed into a domain model.

This approach, while easy and somewhat intuitive, has two problems:

- Ambiguity is introduced in method signatures that act on properties of the domain model
- Validation of properties is performed per-model, rather than per-property.

We will investigate both of these problems, and how the ValueTypeWrappers library provides a solution.

## The Problem of Ambiguity

In the `Customer` example above, there are two properties of identical type and similar name: `CustomerId` and `CustomerNumber`. The `CustomerId` property is the *database primary key* of the Customer table row corresponding to the class instance; `CustomerNumber` is a conversational identifier used internally with the business (for example, it could be that David is the 18th customer of our business, and has a `CustomerNumber` of `18`, but due to deletions/migrations/etc. their database PK could be an entirely different value).

Suppose that we are a new developer, and are given a new feature to build. Part of the implementation involves reading a customer's billing history from the database, and we are given an interface to facilitate this, with the following method signature:


    public interface ICustomerRepository
    {
        IEnumerable<BillingLineItem> GetCustomerBillingHistory(int customerIdentifier);
    }


Of the two uniquely identifying integer properties associated with the customer model, which do we use as an argument for this method? Answering this question requires tracking down the implementation of the `GetCustomer` method - and, depending on how abstracted your application architecture is, could require going through several layers of code, and possibly investigating SQL stored-procedures to see what column is queried. 

This not only wastes developer time (and thus, business capital), but can frequently be a source of pernicious errors. Because an incorrect argument assignment will not be discovered until runtime, identifying, tracking down and correcting the issue can be difficult. Trusting parameter *names* is a frequent source of errors like this - while it may seem sensible that a parameter named "customerId" should be given the "CustomerId" property of the corresponding object, there is *no guarantee* that the actual implementation of the methods involved in the operation will be able to successfully use that data.

## The Problem of Validation

Suppose that we have a business requirement to validate customer e-mail addresses<sup>1</sup>. Naturally we will want to ensure that validation errors are caught as quickly as possible. We could do this by putting our validation logic in the constructor of our Customer class:

    public class Customer 
    {
        public Customer(string emailAddress) 
    	{
            if (!emailAddress.Contains("@")) throw new ArgumentException();
    		EmailAddress = emailAddress;
    	}
	
	    public int CustomerId { get; set; }
	    public string EmailAddress { get; set; }
	    public string CustomerName { get; set; }
	    public int CustomerNumber { get; set; }
    }


Excellent. Now we are assured that no customers can exist in our application with an invalid e-mail address. But what if we have another model, say a `SalesRep`, for whom we also want to ensure valid e-mail addresses. We could repeat our constructor logic, but then we have our validation logic in two places, which is bad. We could create something like an `EmailAddressValidator` class to centralize our logic (and call its `.Validate` method from our object constructors), but then we have introduced a dependency in our domain models, which is also bad<sup>2</sup>. 

## A solution

Suppose that instead of the customer model described above, we had a class that looked like this:


    public class Customer 
    {
    	public DatabasePrimaryKey CustomerId { get; set; }	
    	public EmailAddress EmailAddress { get; set; }	
    	public string CustomerName { get; set; }	
    	public ConversationId CustomerNumber { get; set; }	
    }


That is, instead of using primitive types to express the properties of a customer, we've created custom types that capture the relative business meaning of each property. How does this decision affect the two problems outlined above?

### Ambiguity

Given these custom types, our interface will now be able to define behavior as follows:


    public interface ICustomerRepository
    {
    	IEnumerable<BillingLineItem> GetCustomerBillingHistory(DatabasePrimaryKey customerIdentifier);
    }


There is now no ambiguity as to which property we should use to query this data. Indeed, it is now possible to define specific overloads per data type:


    public interface ICustomerRepository
    {
    	IEnumerable<BillingLineItem> GetCustomerBillingHistory(DatabasePrimaryKey customerIdentifier);
    	IEnumerable<BillingLineItem> GetCustomerBillingHistory(ConversationId customerIdentifier);
    }


Whereas before, it was uncertain if the query required the database PK, the conversation ID, or if it could succeed with both.

### Validation

The implementation of the `EmailAddress` class contains its own validation, called in the ctor of the base object<sup>3</sup>:


    public class EmailAddress : StringWrapper
    {
    	public EmailAddress(string value) : base(value)
    	{         
    	}

    	protected override bool ValidateValue(string value)
    	{
     		return value.Contains("@");
    	}
    }


Thus, whenever data of this type is instantiated, the proper validation is ensured to be called - no matter which domain classes contain e-mail address data.

# Caveats

This is, of course, not a magic bullet. It is still possible to introduce ambiguity through inexact type derivation - of course, it is also possible to create a domain space that is *too* precise, and becomes a convoluted mess of wrapper-derived types. However, used properly, this architectural pattern should succeed in its goals of improving code-reuse, domain context expression, and compile-time error discovery.

#### Footnotes

1. I am not suggesting that validating e-mail addresses is a good idea, merely a useful example.
2. I subscribe to the school of thought that domain models should be POCOs, and have no dependencies.
4. See the remarks element of the ValidateValue docstring