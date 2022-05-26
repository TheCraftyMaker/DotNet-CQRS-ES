using System;
using System.Collections.Generic;
using DomainDriven.Domain;
using FluentAssertions;

namespace DomainDriven.Tests.Application.SpecificationImplementation
{
    public abstract class ValueSpecificationImplementation
    {
        private TestValueObject _valueObject = null!;
        private TestValueObject _otherValueObject = null!;

        protected void GIVEN_A_Value_Object()
        {
            _valueObject = CreateValueObject();
        }

        protected void WHEN_Creating_A_Value_Object_With_The_Same_Property_Values()
        {
            _otherValueObject = CreateValueObject();
        }

        protected void THEN_Both_Value_Objects_Should_Be_Equal()
        {
            _valueObject.Should().NotBeNull();
            _otherValueObject.Should().NotBeNull();

            _valueObject.Equals(_otherValueObject).Should().BeTrue();
            (_valueObject == _otherValueObject).Should().BeTrue();
            (_valueObject != _otherValueObject).Should().BeFalse();
        }

        protected void THEN_Hashcode_Of_Both_Value_Objects_Should_Be_The_Same()
        {
            _valueObject.Should().NotBeNull();
            _otherValueObject.Should().NotBeNull();

            var hashCode = _valueObject.GetHashCode();
            var otherHashCode = _otherValueObject.GetHashCode();

            hashCode.Should().Be(otherHashCode);
        }

        private static TestValueObject CreateValueObject()
        {
            var list = new List<string> { "A", "B", "C", "D", "E", "F" };
            var dict = new Dictionary<int, string>
            {
                {1, "a"},
                {2, "b"},
                {3, "c"},
                {4, "d"},
                {5, "e"},
                {6, "f"},
            };
            return new TestValueObject(2, true, "Test_test-1234%", DateTime.Now.Date, list, dict);
        }

        private class TestValueObject : Value<TestValueObject>
        {
            public int IntegerValue { get; }
            public bool BooleanValue { get; }
            public string StringValue { get; }
            public DateTime DateTimeValue { get; }
            public List<string> CollectionValue { get; }
            public Dictionary<int, string> DictionaryValue { get; }

            public TestValueObject(int integer, bool boolean, string str,
                DateTime dt, List<string> list, Dictionary<int, string> dic)
            {
                IntegerValue = integer;
                BooleanValue = boolean;
                StringValue = str;
                DateTimeValue = dt;
                CollectionValue = list;
                DictionaryValue = dic;
            }
        }
    }
}

