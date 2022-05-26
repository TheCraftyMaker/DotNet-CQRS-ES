using DomainDriven.Tests.Application.SpecificationImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainDriven.Tests.Application
{
    [TestClass]
    public class SourcedEntitySpecification : SourcedEntitySpecificationImplementation
    {
        [TestMethod]
        public void Two_Entities_With_Same_Identifier_Should_Be_Equal()
        {
            GIVEN_An_Entity();
            WHEN_Creating_An_Entity_With_The_Same_Identifier();
            THEN_Both_Value_Objects_Should_Be_Equal();
        }

        [TestMethod]
        public void Two_Entities_With_Same_Identifier_Should_Have_The_Same_Hashcode()
        {
            GIVEN_An_Entity();
            WHEN_Creating_An_Entity_With_The_Same_Identifier();
            THEN_Hashcode_Of_Both_Entities_Should_Be_The_Same();
        }

        [TestMethod]
        public void Creating_An_Entity_With_Identifier_Should_Have_The_Correct_Identifier_Value()
        {
            WHEN_Creating_An_Entity_Object_With_Identifier();
            THEN_The_Identifier_Should_Be_Set_With_The_Correct_Value();
        }

        [TestMethod]
        public void Creating_An_Entity_With_Identifier_Should_Have_The_Correct_Current_Version()
        {
            WHEN_Creating_An_Entity_Object_With_Identifier();
            THEN_The_Current_Version_Should_Be_Set_To_Zero();
        }

        [TestMethod]
        public void Creating_An_Entity_With_Identifier_Should_Have_The_Correct_Next_Version()
        {
            WHEN_Creating_An_Entity_Object_With_Identifier();
            THEN_The_Next_Version_Should_Be_Set_To_One();
        }

        [TestMethod]
        public void Initializing_An_Entity_With_A_Stream_Should_Set_The_Correct_Current_Version()
        {
            WHEN_Creating_An_Entity_Object_From_Source_Stream();
            THEN_The_Current_Version_Should_Be_Set_With_The_Stream_Version();
        }

        [TestMethod]
        public void Initializing_An_Entity_With_A_Stream_Should_Set_The_Correct_Next_Version()
        {
            WHEN_Creating_An_Entity_Object_From_Source_Stream();
            THEN_The_Next_Version_Should_Be_Set_With_Stream_Version_Plus_One();
        }

        [TestMethod]
        public void Initializing_An_Entity_With_A_Stream_Should_Invoke_When_Method_For_Every_Event_In_The_Strea()
        {
            WHEN_Creating_An_Entity_Object_From_Source_Stream();
            THEN_The_When_Method_Should_Be_Invoked_As_Many_Times_As_There_Are_Events_In_The_Stream();
        }

        [TestMethod]
        public void Applying_An_Event_To_Entity_Should_Add_Event_To_Applied_Collection()
        {
            GIVEN_An_Entity();
            WHEN_Applying_A_Domain_Event();
            THEN_The_Event_Should_Be_Added_To_The_Applied_Collection();
        }

        [TestMethod]
        public void Marking_An_Event_Committed_Should_Remove_The_Event_From_Applied_Collection()
        {
            GIVEN_An_Entity();
            WHEN_Applying_A_Domain_Event();
            WHEN_Marking_A_Domain_Event_As_Committed();
            THEN_The_Current_Version_Should_Be_Set_With_Plus_One();
            THEN_The_Event_Should_Be_Removed_From_The_Applied_Collection();
        }

        [TestMethod]
        public void Applying_An_Event_To_Entity_Should_Invoke_When_Method()
        {
            GIVEN_An_Entity();
            WHEN_Applying_A_Domain_Event();
            THEN_The_When_Method_Should_Be_Invoked_Once();
        }

        [TestMethod]
        public void Applying_An_Event_To_Entity_Should_Invoke_EnsureValidState_Method()
        {
            GIVEN_An_Entity();
            WHEN_Applying_A_Domain_Event();
            THEN_The_EnsureValidState_Method_Should_Be_Invoked_Once();
        }
    }
}