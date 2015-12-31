using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ValueTypeWrappers.Tests
{
    [TestClass]
    public class ValueTypeWrapperTests
    {
        [TestMethod]
        public void IntegerWrapper_ShouldBeEqualToCorrespondingPrimitive()
        {
            var integer = 24;
            var wrappedInt = new WrappedIntegerExample(integer);
            Assert.IsTrue(wrappedInt == integer);
            Assert.IsTrue(wrappedInt.Value.Equals(integer));
        }

        [TestMethod]
        public void IntegerWrapper_CanBeUsedWhenAnIntegerIsCalledFor()
        {
            var integer = 24;
            var wrappedInt = new WrappedIntegerExample(integer);
            Assert.IsTrue(DidIGetAnInteger(wrappedInt));
        }

        [TestMethod]
        public void StringWrapper_ShouldBeEqualToCorrespondingPrimitive()
        {
            var s = "testemail@test.com";
            var wrappedString = new EmailAddressExample(s);
            Assert.IsTrue(wrappedString == s);
            Assert.IsTrue(wrappedString.Value.Equals(s));
        }

        [TestMethod]
        public void StringWrapper_CanBeUsedWhenAStringIsCalledFor()
        {
            var s = "testemail@test.com";
            var wrappedString = new EmailAddressExample(s);
            Assert.IsTrue(DidIGetAString(wrappedString));
        }

        /// <summary>
        /// In practice, you'll never see this failure at runtime - the compiler will stop you.
        /// This is just to illustrate that the interoperability only goes in one direction.
        /// </summary>
        [TestMethod]
        public void StringWrapper_AssociatedPrimitiveCannotBeUsedWhereWrapperIsCalledFor()
        {
            var s = 24;
            var methodTakingAWrapper =
                GetType()
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(m => m.Name == nameof(DidIGetAStringrWrapper));
            try
            {
                methodTakingAWrapper.Invoke(this, new object[] {s});
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
            }
        }

        /// <summary>
        /// In practice, you'll never see this failure at runtime - the compiler will stop you.
        /// This is just to illustrate that the interoperability only goes in one direction.
        /// </summary>
        [TestMethod]
        public void IntegerWrapper_AssociatedPrimitiveCannotBeUsedWhereWrapperIsCalledFor()
        {
            var integer = 24;
            var methodTakingAWrapper =
                GetType()
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(m => m.Name == nameof(DidIGetAnIntegerWrapper));
            try
            {
                methodTakingAWrapper.Invoke(this, new object[] { integer });
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void StringWrapper_CustomValidationIsCalledOnInstantiation()
        {
            try
            {
                var s = new EmailAddressExample("Foo");
            }
            catch (Exception e)
            { 
                Assert.IsNotNull(e);
                throw;
            }

            try
            {
                var s = new EmailAddressExample("@");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void IntegerWrapper_CustomValidationIsCalledOnInstantiation()
        {
            try
            {
                var i = new WrappedIntegerExample(-1);
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
                throw;
            }

            try
            {
                var i = new WrappedIntegerExample(24);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        private bool DidIGetAStringrWrapper(StringWrapper s)
        {
            return s is StringWrapper;
        }


        private bool DidIGetAnInteger(int integer)
        {
            return integer is int;
        }


        private bool DidIGetAnIntegerWrapper(WrappedIntegerExample integer)
        {
            return integer is WrappedIntegerExample;
        }

        private bool DidIGetAString(string s)
        {
            return s is string;
        }
    }

    internal class WrappedIntegerExample : StructWrapper<int>
    {
        internal WrappedIntegerExample(int value) : base(value)
        {
        }

        protected override bool ValidateValue(int value)
        {
            return value > 0;
        }
    }

    internal class EmailAddressExample : StringWrapper
    {
        internal EmailAddressExample(string value) : base(value)
        {         
        }

        protected override bool ValidateValue(string value)
        {
            return value.Contains("@");
        }
    }
}
