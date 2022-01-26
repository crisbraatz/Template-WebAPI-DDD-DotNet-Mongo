using System;
using System.Runtime.Serialization;

namespace Application.Common.Exceptions;

[Serializable]
public class InvalidPasswordFormatException : Exception
{
    public InvalidPasswordFormatException(string message) : base(message)
    {
    }

    protected InvalidPasswordFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}