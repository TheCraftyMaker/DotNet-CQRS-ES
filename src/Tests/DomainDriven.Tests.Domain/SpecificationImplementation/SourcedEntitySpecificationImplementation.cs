using System;
using System.Collections.Generic;
using System.Linq;
using DomainDriven.Domain;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainDriven.Tests.Application.SpecificationImplementation
{
    public abstract class SourcedEntitySpecificationImplementation
    {
        private Guid _idValue;
        private int _streamVersion;
        private List<TestEvent> _stream;

        private TestEntity _entityObject;
        private TestEntity _otherEntityObject;

        [TestInitialize]
        public void Setup()
        {
            _idValue = Guid.NewGuid();
            _stream = new List<TestEvent>
            {
                new(Guid.NewGuid().ToString(), 2),
                new(Guid.NewGuid().ToString(), 1)
            };
            _streamVersion = _stream.Count;
        }

        protected void GIVEN_An_Entity()
        {
            _streamVersion = 0;
            _entityObject = new TestEntity(TestId.FromExisting(_idValue.ToString()));
        }

        protected void WHEN_Creating_An_Entity_Object_With_Identifier()
        {
            _streamVersion = 0;
            _entityObject = new TestEntity(TestId.FromExisting(_idValue.ToString()));
        }

        protected void WHEN_Creating_An_Entity_Object_From_Source_Stream()
        {
            _entityObject = new TestEntity(_stream, _streamVersion, TestId.FromExisting(_idValue.ToString()));
        }

        protected void WHEN_Creating_An_Entity_With_The_Same_Identifier()
        {
            _otherEntityObject = new TestEntity(TestId.FromExisting(_idValue.ToString()));
        }

        protected void WHEN_Applying_A_Domain_Event()
        {
            _entityObject.ApplyGateway(_stream.First());
        }

        protected void WHEN_Marking_A_Domain_Event_As_Committed()
        {
            _entityObject.MarkCommittedGateway();
        }

        protected void THEN_Both_Value_Objects_Should_Be_Equal()
        {
            _entityObject.Should().NotBeNull();
            _otherEntityObject.Should().NotBeNull();

            _entityObject.Equals(_otherEntityObject).Should().BeTrue();
        }

        protected void THEN_Hashcode_Of_Both_Entities_Should_Be_The_Same()
        {
            _entityObject.Should().NotBeNull();
            _otherEntityObject.Should().NotBeNull();

            var hashCode = _entityObject.GetHashCode();
            var otherHashCode = _otherEntityObject.GetHashCode();

            hashCode.Should().Be(otherHashCode);
        }

        protected void THEN_The_Identifier_Should_Be_Set_With_The_Correct_Value()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.Id.Value.Should().Be(_idValue);
        }

        protected void THEN_The_Current_Version_Should_Be_Set_With_The_Stream_Version()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.CurrentVersion.Should().Be(_streamVersion);
        }

        protected void THEN_The_Current_Version_Should_Be_Set_With_Plus_One()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.CurrentVersion.Should().Be(_streamVersion + 1);
        }

        protected void THEN_The_Next_Version_Should_Be_Set_With_Stream_Version_Plus_One()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.NextVersion.Should().Be(_streamVersion + 1);
        }

        protected void THEN_The_Current_Version_Should_Be_Set_To_Zero()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.CurrentVersion.Should().Be(0);
        }

        protected void THEN_The_Next_Version_Should_Be_Set_To_One()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.NextVersion.Should().Be(1);
        }

        protected void THEN_The_When_Method_Should_Be_Invoked_As_Many_Times_As_There_Are_Events_In_The_Stream()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.WhenInvocationCount.Should().Be(_stream.Count);
        }

        protected void THEN_The_Event_Should_Be_Added_To_The_Applied_Collection()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.Applied.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x.Equals(_stream.First()));
        }

        protected void THEN_The_Event_Should_Be_Removed_From_The_Applied_Collection()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.Applied.Should()
                .HaveCount(0)
                .And.NotContain(x => x.Equals(_stream.First()));
        }

        protected void THEN_The_When_Method_Should_Be_Invoked_Once()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.WhenInvocationCount.Should().Be(1);
        }

        protected void THEN_The_EnsureValidState_Method_Should_Be_Invoked_Once()
        {
            _entityObject.Should().NotBeNull();
            _entityObject.EnsureValidStateCount.Should().Be(1);
        }

        public class TestEntity : SourcedEntity<TestId, DomainEvent>
        {
            public int WhenInvocationCount { get; set; }
            public int EnsureValidStateCount { get; set; }

            public TestEntity(TestId testId)
                : base(testId)
            {
            }

            public TestEntity(IEnumerable<DomainEvent> stream, int streamVersion, TestId testId)
                : base(stream, streamVersion, testId)
            {
            }

            public void ApplyGateway(DomainEvent @event)
            {
                Apply(@event);
            }

            public void MarkCommittedGateway()
            {
                MarkCommitted();
            }

            public void When(TestEvent @event)
            {
                WhenInvocationCount += 1;
            }

            protected override void EnsureValidState()
            {
                EnsureValidStateCount += 1;
            }
        }

        public class TestEvent : DomainEvent
        {
            public int Version { get; set; }

            public TestEvent(string id, int version)
            {
                Id = id;
                Version = version;
            }
        }
        
        public class TestId : Value<TestId>
        {
            public Guid Value { get; }

            private TestId()
            {
                Value = Guid.NewGuid();
            }

            private TestId(string referenceId)
            {
                if (!Guid.TryParse(referenceId, out var id))
                {
                    throw new ArgumentException("Invalid testId", 
                        nameof(referenceId));
                }
                Value = id;
            }

            public static TestId FromExisting(string referenceId)
            {
                return new(referenceId);
            }
            
            public static implicit operator TestId(string id)
            {
                return new(id);
            }

            public static implicit operator string(TestId testId)
            {
                return testId.Value.ToString();
            }
        }
    }
}