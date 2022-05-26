using DomainDriven.Tests.Application.SpecificationImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainDriven.Tests.Application
{
    [TestClass]
    public class ValueSpecification : ValueSpecificationImplementation
    {
        [TestMethod]
        public void Two_Value_Objects_With_Same_Property_Values_Should_Be_Equal()
        {
            GIVEN_A_Value_Object();
            WHEN_Creating_A_Value_Object_With_The_Same_Property_Values();
            THEN_Both_Value_Objects_Should_Be_Equal();
        }

        [TestMethod]
        public void Two_Value_Objects_With_Same_Property_Values_Should_Have_The_Same_Hashcode()
        {
            GIVEN_A_Value_Object();
            WHEN_Creating_A_Value_Object_With_The_Same_Property_Values();
            THEN_Hashcode_Of_Both_Value_Objects_Should_Be_The_Same();
        }
    }
}