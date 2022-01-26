using System;

namespace Domain.Common.CustomDateTime;

public interface IDateTime
{
    DateTime Today { get; }
    DateTime UtcNow { get; }
}