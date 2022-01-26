using System;
using Application.Services.Users.Security;
using FluentAssertions;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Unit.Application.Services.Users.Security;

public class TokenTests
{
    [Fact]
    public void Should_generate_jwt()
    {
        const string email = "example@template.com";

        var jwt = Token.GenerateJwt(email);

        jwt.Should().HaveLength(261);
    }

    [Fact]
    public void Should_get_claim_from_authorization()
    {
        StringValues authorization =
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImV4YW1wbGVAdGVtcGxhdGUuY29tIiwibmJmIjoxNjI4MzUzMjQ3LCJleHAiOjE2MjgzNTY4NDcsImlhdCI6MTYyODM1MzI0NywiaXNzIjoiREVGQVVMVEpXVElTU1VFUiIsImF1ZCI6IkRFRkFVTFRKV1RBVURJRU5DRSJ9.zrokqq5IZoSwZLAl9AoNRlhGn1haKuvm_o4urzP08w4";

        var claim = Token.GetClaimFrom(authorization);

        claim.Should().Be("example@template.com");
    }

    [Theory]
    [InlineData("NOTDEFAULTJWTSECURITYKEY", 192)]
    [InlineData("", 168)]
    public void Should_get_security_key(string value, int expectedKeySize)
    {
        Environment.SetEnvironmentVariable("JWT_SECURITY_KEY", value);

        var securityKey = Token.GetSecurityKey();

        securityKey.KeySize.Should().Be(expectedKeySize);
    }
}