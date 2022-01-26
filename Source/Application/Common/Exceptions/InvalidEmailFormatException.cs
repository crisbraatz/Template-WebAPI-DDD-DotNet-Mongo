using System;
using System.Runtime.Serialization;

namespace Application.Common.Exceptions;

[Serializable]
public class InvalidEmailFormatException : Exception
{
    public InvalidEmailFormatException(string message) : base(message)
    {
    }

    protected InvalidEmailFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}