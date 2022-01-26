using System;
using System.Runtime.Serialization;

namespace Application.Common.Exceptions;

[Serializable]
public class InvalidPasswordException : Exception
{
    public InvalidPasswordException(string message) : base(message)
    {
    }

    protected InvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}