using Domain.Common.CustomDateTime;
using FluentAssertions;
using FluentAssertions.Extensions;
using Unit.Common;
using Xunit;

namespace Unit.Domain.Common.CustomDateTime;

public class SystemDateTimeTests
{
    private readonly IDateTime _dateTime =
        new MockDateTime(1.January(2021).AddHours(1).AddMinutes(2).AddSeconds(3));

    [Fact]
    public void Should_get_date_without_time()
    {
        _dateTime.Today.Should().Be(01.January(2021));
    }

    [Fact]
    public void Should_get_date_with_time()
    {
        _dateTime.UtcNow.Should().Be(01.January(2021).AddHours(1).AddMinutes(2).AddSeconds(3));
    }
}