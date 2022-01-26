using System;
using Application.Common.Exceptions;
using Application.Services.Users.Security;
using FluentAssertions;
using Xunit;

namespace Unit.Application.Services.Users.Security;

public class EmailTests
{
    private Action _action;

    [Theory]
    [InlineData("example@template.com")]
    [InlineData("example@template.com.br")]
    [InlineData("example@template.com.br.sc")]
    [InlineData("example.example@template.com")]
    [InlineData("example_example@template.com")]
    public void Should_successfully_validate_email_format(string email)
    {
        _action = () => Email.ValidateFormat(email);

        _action.Should().NotThrow<Exception>();
    }

    [Theory]
    [InlineData(".example@template.com")]
    [InlineData("example@template.com.")]
    [InlineData("example@template..com")]
    [InlineData("example@template.c")]
    [InlineData("example@template.company")]
    public void Should_throw_exception_validating_invalid_email_format(string email)
    {
        _action = () => Email.ValidateFormat(email);

        _action.Should().Throw<InvalidEmailFormatException>().WithMessage("Invalid email format.");
    }
}