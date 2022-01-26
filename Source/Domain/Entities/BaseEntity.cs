using System;
using Domain.Common.CustomDateTime;

namespace Domain.Entities;

public abstract class BaseEntity
{
    private readonly IDateTime _dateTime = new SystemDateTime();

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public string LastUpdatedBy { get; private set; }
    public bool Active { get; private set; }

    protected void SetCreate(string requestedBy)
    {
        Id = Guid.NewGuid();
        CreatedAt = _dateTime.UtcNow;
        CreatedBy = requestedBy;
        LastUpdatedAt = _dateTime.UtcNow;
        LastUpdatedBy = requestedBy;
        Active = true;
    }

    protected void SetInactive(string requestedBy)
    {
        SetUpdate(requestedBy);

        Active = false;
    }

    protected void SetUpdate(string requestedBy)
    {
        LastUpdatedAt = _dateTime.UtcNow;
        LastUpdatedBy = requestedBy;
    }
}