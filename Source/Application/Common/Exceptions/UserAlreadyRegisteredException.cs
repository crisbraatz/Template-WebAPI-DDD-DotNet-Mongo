using System;
using System.Runtime.Serialization;

namespace Application.Common.Exceptions;

[Serializable]
public class UserAlreadyRegisteredException : Exception
{
    public UserAlreadyRegisteredException(string message) : base(message)
    {
    }

    protected UserAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}