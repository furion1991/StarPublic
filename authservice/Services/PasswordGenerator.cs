using System.Security.Cryptography;

namespace AuthService.Services;

public static class PasswordGenerator
{
    private static readonly string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private static readonly string Digits = "0123456789";
    private static readonly string SpecialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";
    private static readonly string AllChars = Uppercase + Lowercase + Digits + SpecialChars;
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();


    public static string GeneratePassword(int length = 12)
    {
        if (length < 4) throw new ArgumentException("Password length must be at least 4");

        var password = new char[length];

        password[0] = Uppercase[GetRandomNumber(Uppercase.Length)];
        password[1] = Lowercase[GetRandomNumber(Lowercase.Length)];
        password[2] = Digits[GetRandomNumber(Digits.Length)];
        password[3] = SpecialChars[GetRandomNumber(SpecialChars.Length)];

        for (int i = 4; i < length; i++)
        {
            password[i] = AllChars[GetRandomNumber(AllChars.Length)];
        }

        return new string(password.OrderBy(x => GetRandomNumber(100)).ToArray());
    }

    private static int GetRandomNumber(int max)
    {
        var randomNumber = new byte[1];
        Rng.GetBytes(randomNumber);
        return randomNumber[0] % max;
    }
}

