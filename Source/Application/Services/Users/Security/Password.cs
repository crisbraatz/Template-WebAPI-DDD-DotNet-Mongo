using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Application.Services.Users.Security;

public static class Password
{
    public static string GenerateHashOnly(string password, byte[] salt) => GenerateHash(password, salt);

    public static (byte[] salt, string hash) GenerateSaltAndHash(string password)
    {
        var salt = GenerateSalt();
        var hash = GenerateHash(password, salt);

        return (salt, hash);
    }

    public static void ValidateFormat(string password)
    {
        if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,16}$"))
            throw new InvalidPasswordFormatException("Invalid password format.");
    }

    private static string GenerateHash(string password, byte[] salt) =>
        Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));

    private static byte[] GenerateSalt()
    {
        var salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        return salt;
    }
}