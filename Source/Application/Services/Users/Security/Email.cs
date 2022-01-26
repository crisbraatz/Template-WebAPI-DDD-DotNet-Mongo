using System.Text.RegularExpressions;
using Application.Common.Exceptions;

namespace Application.Services.Users.Security;

public static class Email
{
    public static void ValidateFormat(string email)
    {
        if (!Regex.IsMatch(
                email,
                "^[\\w!#$%&'*+/=?`{|}~^-]+(?:\\.[\\w!#$%&'*+/=?`{|}~^-]+)*@(?:[a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$"))
            throw new InvalidEmailFormatException("Invalid email format.");
    }
}