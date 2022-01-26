using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Users.Security;

public static class Token
{
    public static string GenerateJwt(string email)
    {
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "DEFAULTJWTAUDIENCE";
        var credentials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256Signature);
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "DEFAULTJWTISSUER";
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = audience,
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = issuer,
            SigningCredentials = credentials,
            Subject = new ClaimsIdentity(new[] {new Claim(nameof(email), email)})
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public static string GetClaimFrom(StringValues authorization)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = (JwtSecurityToken) tokenHandler.ReadToken(authorization.ToString()[7..]);

        return token.Claims.First(claim => claim.Type.ToLowerInvariant() == "email").Value;
    }

    public static SymmetricSecurityKey GetSecurityKey() => new(Encoding.ASCII.GetBytes(
        Environment.GetEnvironmentVariable("JWT_SECURITY_KEY") ?? "DEFAULTJWTSECURITYKEY"));
}