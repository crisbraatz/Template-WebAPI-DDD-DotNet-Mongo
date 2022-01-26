using System;
using System.Threading;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Unit.Domain.Entities;

public class BaseEntityTests
{
    private DateTime _dateTime;
    private const string CreateProperty = "CreateProperty";
    private const string UpdateProperty = "UpdateProperty";
    private const string CreateRequestedBy = "CreateRequestedBy";
    private const string UpdateRequestedBy = "UpdateRequestedBy";
    private BaseEntityForTests _baseEntityForTests;

    [Fact]
    public void Should_create_an_entity()
    {
        _dateTime = DateTime.UtcNow;

        _baseEntityForTests = new BaseEntityForTests(CreateProperty, CreateRequestedBy);

        _baseEntityForTests.Property.Should().Be(CreateProperty);
        _baseEntityForTests.Id.Should().NotBeEmpty();
        _baseEntityForTests.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _baseEntityForTests.CreatedBy.Should().Be(CreateRequestedBy);
        _baseEntityForTests.LastUpdatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _baseEntityForTests.LastUpdatedBy.Should().Be(CreateRequestedBy);
        _baseEntityForTests.Active.Should().BeTrue();
    }

    [Fact]
    public void Should_inactivate_an_entity()
    {
        _dateTime = DateTime.UtcNow;
        _baseEntityForTests = new BaseEntityForTests(CreateProperty, CreateRequestedBy);
        Thread.Sleep(100);

        _baseEntityForTests.Inactivate(UpdateRequestedBy);

        _baseEntityForTests.Property.Should().Be(CreateProperty);
        _baseEntityForTests.Id.Should().NotBeEmpty();
        _baseEntityForTests.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _baseEntityForTests.CreatedBy.Should().Be(CreateRequestedBy);
        _baseEntityForTests.LastUpdatedAt.Should().BeAfter(_baseEntityForTests.CreatedAt);
        _baseEntityForTests.LastUpdatedBy.Should().Be(UpdateRequestedBy);
        _baseEntityForTests.Active.Should().BeFalse();
    }

    [Fact]
    public void Should_update_an_entity()
    {
        _dateTime = DateTime.UtcNow;
        _baseEntityForTests = new BaseEntityForTests(CreateProperty, CreateRequestedBy);
        Thread.Sleep(100);

        _baseEntityForTests.Update(UpdateProperty, UpdateRequestedBy);

        _baseEntityForTests.Property.Should().Be(UpdateProperty);
        _baseEntityForTests.Id.Should().NotBeEmpty();
        _baseEntityForTests.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.MaxValue);
        _baseEntityForTests.CreatedBy.Should().Be(CreateRequestedBy);
        _baseEntityForTests.LastUpdatedAt.Should().BeAfter(_baseEntityForTests.CreatedAt);
        _baseEntityForTests.LastUpdatedBy.Should().Be(UpdateRequestedBy);
        _baseEntityForTests.Active.Should().BeTrue();
    }

    private class BaseEntityForTests : BaseEntity
    {
        public string Property { get; private set; }

        public BaseEntityForTests(string property, string requestedBy)
        {
            SetCreate(requestedBy);

            Property = property;
        }

        public void Inactivate(string requestedBy)
        {
            SetInactive(requestedBy);
        }

        public void Update(string property, string requestedBy)
        {
            SetUpdate(requestedBy);

            Property = property;
        }
    }
}