using System.Security.Cryptography;
using System.Text;

namespace LearnWellUniversity_CMS.Shared.Utilities;

public static class PasswordGenerator
{
    public static string Generate(int length = 8)
    {
        if (length < 8)
            throw new ArgumentOutOfRangeException(nameof(length), "Password length must be at least 8.");

        StringBuilder builder = new StringBuilder();

        string UpperCaseLetters = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        string LowerCaseLetters = UpperCaseLetters.ToLower();
        string Digits = "0123456789";
        string SpecialCharacters = "~!@#$%^&*";
        string AllCharacters = string.Concat(UpperCaseLetters, LowerCaseLetters, Digits, SpecialCharacters);

        builder.Append(UpperCaseLetters[RandomNumberGenerator.GetInt32(UpperCaseLetters.Length)]);
        builder.Append(LowerCaseLetters[RandomNumberGenerator.GetInt32(LowerCaseLetters.Length)]);
        builder.Append(Digits[RandomNumberGenerator.GetInt32(Digits.Length)]);
        builder.Append(SpecialCharacters[RandomNumberGenerator.GetInt32(SpecialCharacters.Length)]);

        for (int i = 0; i < length - 4; i++)
            builder.Append(AllCharacters[RandomNumberGenerator.GetInt32(AllCharacters.Length)]);

        return builder.ToString();
    }
}
