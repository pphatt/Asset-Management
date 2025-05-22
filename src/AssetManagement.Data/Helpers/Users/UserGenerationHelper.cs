using System.Globalization;
using System.Text;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Data.Helpers.Users;

public static class UserGenerationHelper
{
    private static readonly string[] VietnameseSigns = new string[]
    {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
    };

    private static string RemoveSign4VietnameseString(string str)
    {
        for (int i = 1; i < VietnameseSigns.Length; i++)
        {
            for (int j = 0; j < VietnameseSigns[i].Length; j++)
                str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
        }

        return str;
    }

    public static string GenerateUsername(string firstName, string lastName, IEnumerable<string> existingUsernames)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentNullException("First name and last name is required!");

        // Remove diacritics, trim spaces and convert to lowercase
        firstName = RemoveSign4VietnameseString(firstName).Trim().ToLower();
        lastName = RemoveSign4VietnameseString(lastName).Trim();

        // Remove spaces from firstName to handle multiple words
        firstName = string.Join("", firstName.Split(" ", StringSplitOptions.RemoveEmptyEntries));

        // Get the first letter of each word in lastName
        var lastNameInitials = string.Join("", lastName
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w[0].ToString().ToLower()));

        var baseUsername = firstName + lastNameInitials;

        var existingSet = existingUsernames.Select(u => u.ToLower()).ToHashSet();

        if (!existingSet.Contains(baseUsername))
            return baseUsername;

        int suffix = 1;
        string newUsername;
        do
        {
            newUsername = baseUsername + suffix++;
        } while (existingSet.Contains(newUsername));

        return newUsername;
    }

    public static string GenerateStaffCode(IEnumerable<string> existingStaffCodes)
    {
        const string prefix = "SD";
        const int maxLimit = 9999;

        var maxNumber = existingStaffCodes
            .Where(code => code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                           && int.TryParse(code.Substring(2), out _))
            .Select(code => int.Parse(code.Substring(2)))
            .DefaultIfEmpty(0)
            .Max();

        if (maxNumber >= maxLimit)
            throw new InvalidOperationException("Maximum number of staff codes reached.");

        return $"{prefix}{(maxNumber + 1):D4}";
    }

    public static string GeneratePassword(string username, DateTime? dateOfBirth)
    {
        if (dateOfBirth == null)
            throw new ArgumentException("Date of birth is required to generate password.");

        var formattedUsername = username.Trim().ToLower();
        var dobFormatted = dateOfBirth.Value.ToString("ddMMyyyy");

        return $"{formattedUsername}@{dobFormatted}";
    }
}