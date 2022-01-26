using System;
using Domain.Common.CustomDateTime;
using NSubstitute;

namespace Unit.Common;

public class MockDateTime : IDateTime
{
    private readonly IDateTime _dateTime;

    public MockDateTime(DateTime dateTime)
    {
        _dateTime = Substitute.For<IDateTime>();
        _dateTime.Today.Returns(dateTime.Date);
        _dateTime.UtcNow.Returns(dateTime);
    }

    public DateTime Today => _dateTime.Today;
    public DateTime UtcNow => _dateTime.UtcNow;
}