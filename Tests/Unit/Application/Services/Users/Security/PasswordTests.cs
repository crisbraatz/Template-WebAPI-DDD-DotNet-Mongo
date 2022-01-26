using System;
using Application.Common.Exceptions;
using Application.Services.Users.Security;
using FluentAssertions;
using Xunit;

namespace Unit.Application.Services.Users.Security;

public class PasswordTests
{
    private byte[] _salt;
    private string _hash;
    private const string PlainTextPassword = "Example123";
    private Action _action;

    [Fact]
    public void Should_generate_hash_only()
    {
        _salt = new byte[] {12, 34, 56, 78, 90};

        _hash = Password.GenerateHashOnly(PlainTextPassword, _salt);

        _hash.Should().NotBe(PlainTextPassword).And.HaveLength(44);
    }

    [Fact]
    public void Should_generate_salt_and_hash()
    {
        (_salt, _hash) = Password.GenerateSaltAndHash(PlainTextPassword);

        _salt.Should().HaveCount(16);
        _hash.Should().NotBe(PlainTextPassword).And.HaveLength(44);
    }

    [Fact]
    public void Should_successfully_validate_password_format()
    {
        _action = () => Password.ValidateFormat(PlainTextPassword);

        _action.Should().NotThrow<Exception>();
    }

    [Theory]
    [InlineData("Example")]
    [InlineData("Example123Example123")]
    [InlineData("example123")]
    [InlineData("EXAMPLE123")]
    [InlineData("ExampleE")]
    [InlineData("12312312")]
    [InlineData("Example123!")]
    public void Should_throw_exception_validating_invalid_password_format(string password)
    {
        _action = () => Password.ValidateFormat(password);

        _action.Should().Throw<InvalidPasswordFormatException>().WithMessage("Invalid password format.");
    }
}