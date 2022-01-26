using System;

namespace Domain.Common.CustomDateTime;

public class SystemDateTime : IDateTime
{
    public DateTime Today => DateTime.UtcNow.Date;
    public DateTime UtcNow => DateTime.UtcNow;
}