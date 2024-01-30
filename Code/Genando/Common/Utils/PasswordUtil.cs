using Common.Constants;
using System.Security.Cryptography;

namespace Common.Utils;

public static class PasswordUtil
{
    #region Properties

    private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string DigitChars = "0123456789";
    private const string SpecialChars = "@$!%*?&";

    #endregion Properties

    #region Methods

    public static string GeneratePassword()
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] randomBytes = new byte[4];
        rng.GetBytes(randomBytes);

        var random = new Random(BitConverter.ToInt32(randomBytes, 0));

        string GenerateRandomString(string chars, int length) =>
            new(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

        string randomPassword =
            GenerateRandomString(UppercaseChars, 1) +
            GenerateRandomString(LowercaseChars, 1) +
            GenerateRandomString(DigitChars, 1) +
            GenerateRandomString(SpecialChars, 1) +
            GenerateRandomString(UppercaseChars + LowercaseChars + DigitChars + SpecialChars, 4);

        return randomPassword;
    }

    public static string HashPassword(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt();

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: SystemConstants.PasswordIteration);

        string passwordWithSalt = $"{hashedPassword}{salt}";

        return passwordWithSalt;
    }

    public static bool VerifyPassword(string password, string hashedPasswordWithSalt)
    {
        string hashedPassword = hashedPasswordWithSalt.Substring(0, 60);

        bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        return isPasswordCorrect;
    }

    #endregion Methods
}