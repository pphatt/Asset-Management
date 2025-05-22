using AssetManagement.Data.Helpers.Users;

namespace AssetManagement.Application.Tests.Helpers;

public class UserGenerationHelperTests
{
    // Test for generating a password
    [Fact]
    public void GeneratePassword_WithValidInput_ReturnsCorrectPassword()
    {
        var username = "binhnv";
        var dateOfBirth = new DateTime(1993, 1, 20);
        var password = UserGenerationHelper.GeneratePassword(username, dateOfBirth);
        Assert.Equal("binhnv@20011993", password);
    }

    [Fact]
    public void GeneratePassword_WithUppercaseUsername_ReturnsLowercaseUsernameInPassword()
    {
        var username = "BINHNV";
        var dateOfBirth = new DateTime(1993, 1, 20);
        var password = UserGenerationHelper.GeneratePassword(username, dateOfBirth);
        Assert.Equal("binhnv@20011993", password);
    }

    [Fact]
    public void GeneratePassword_WithSpacesInUsername_RemovesSpaces()
    {
        var username = " binhnv ";
        var dateOfBirth = new DateTime(1993, 1, 20);
        var password = UserGenerationHelper.GeneratePassword(username, dateOfBirth);
        Assert.Equal("binhnv@20011993", password);
    }

    [Fact]
    public void GeneratePassword_WithNullDateOfBirth_ThrowsArgumentException()
    {
        var username = "binhnv";
        DateTime? dateOfBirth = null;
        var ex = Assert.Throws<ArgumentException>(() => UserGenerationHelper.GeneratePassword(username, dateOfBirth));
        Assert.Equal("Date of birth is required to generate password.", ex.Message);
    }

    [Theory]
    [InlineData(1990, 2, 15, "15021990")]
    [InlineData(1985, 11, 7, "07111985")]
    [InlineData(2000, 12, 31, "31122000")]
    public void GeneratePassword_WithDifferentDates_FormatsDatesCorrectly(int year, int month, int day,
        string expectedDobPart)
    {
        var username = "testuser";
        var dob = new DateTime(year, month, day);
        var expected = $"testuser@{expectedDobPart}";
        var actual = UserGenerationHelper.GeneratePassword(username, dob);
        Assert.Equal(expected, actual);
    }

    // Test for generating a staff code
    [Fact]
    public void GenerateStaffCode_WithNoExisting_ReturnsSd0001()
    {
        var codes = new List<string>();
        var result = UserGenerationHelper.GenerateStaffCode(codes);
        Assert.Equal("SD0001", result);
    }

    [Fact]
    public void GenerateStaffCode_WithExistingCodes_ReturnsNextCode()
    {
        var codes = new List<string> { "SD0001", "SD0005" };
        var result = UserGenerationHelper.GenerateStaffCode(codes);
        Assert.Equal("SD0006", result);
    }

    [Fact]
    public void GenerateStaffCode_WithNonSequentialCodes_ReturnsBasedOnHighest()
    {
        var codes = new List<string> { "SD0001", "SD0100", "SD0050" };
        var result = UserGenerationHelper.GenerateStaffCode(codes);
        Assert.Equal("SD0101", result);
    }

    [Fact]
    public void GenerateStaffCode_WithInvalidFormat_IgnoresInvalid()
    {
        var codes = new List<string> { "SD0010", "INVALID", "XY0050" };
        var result = UserGenerationHelper.GenerateStaffCode(codes);
        Assert.Equal("SD0011", result);
    }

    [Fact]
    public void GenerateStaffCode_WithMaxReached_ThrowsException()
    {
        var codes = new List<string> { "SD9999" };
        var ex = Assert.Throws<InvalidOperationException>(() =>
            UserGenerationHelper.GenerateStaffCode(codes));
        Assert.Equal("Maximum number of staff codes reached.", ex.Message);
    }

    [Fact]
    public void GenerateStaffCode_WithLowercasePrefix_StillValid()
    {
        var codes = new List<string> { "sd0020" };
        var result = UserGenerationHelper.GenerateStaffCode(codes);
        Assert.Equal("SD0021", result);
    }

    // Test for generating a username
    [Fact]
    public void GenerateUsername_WithUniqueName_ReturnsBaseUsername()
    {
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", new List<string>());
        Assert.Equal("binhnv", result);
    }

    [Fact]
    public void GenerateUsername_WithExisting_ReturnsWithSuffix()
    {
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", new List<string> { "binhnv" });
        Assert.Equal("binhnv1", result);
    }

    [Fact]
    public void GenerateUsername_WithMultipleExisting_ReturnsCorrectSuffix()
    {
        var usernames = new List<string> { "binhnv", "binhnv1", "binhnv2" };
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", usernames);
        Assert.Equal("binhnv3", result);
    }

    [Fact]
    public void GenerateUsername_WithGap_ReturnsFirstAvailable()
    {
        var usernames = new List<string> { "binhnv", "binhnv2", "binhnv3" };
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", usernames);
        Assert.Equal("binhnv1", result);
    }

    [Fact]
    public void GenerateUsername_WithUppercaseExisting_IsCaseInsensitive()
    {
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", new List<string> { "BINHNV" });
        Assert.Equal("binhnv1", result);
    }

    [Fact]
    public void GenerateUsername_WithMultipleWordLastName_ReturnsInitials()
    {
        var result = UserGenerationHelper.GenerateUsername("Alice", "Johnson Smith Brown", new List<string>());
        Assert.Equal("alicejsb", result);
    }

    [Theory]
    [InlineData("", "Nguyen")]
    [InlineData("Binh", "")]
    public void GenerateUsername_WithMissingParts_Throws(string firstName, string lastName)
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            UserGenerationHelper.GenerateUsername(firstName, lastName, new List<string>()));
        Assert.Equal("First name and last name is required!", ex.ParamName);
    }

    [Fact]
    public void GenerateUsername_WithNormalNames_ReturnsExpectedUsername()
    {
        var usernames = new List<string>();
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", usernames);
        Assert.Equal("binhnv", result);
    }

    [Fact]
    public void GenerateUsername_WithDiacritics_RemovesDiacritics()
    {
        var usernames = new List<string>();
        var result = UserGenerationHelper.GenerateUsername("Đặng", "Văn Bình", usernames);
        Assert.Equal("dangvb", result);
    }

    [Fact]
    public void GenerateUsername_WithSpacesAndCaseInsensitive_HandlesCorrectly()
    {
        var usernames = new List<string> { "binhnv" };
        var result = UserGenerationHelper.GenerateUsername("  Binh  ", "Nguyen Van", usernames);
        Assert.Equal("binhnv1", result);
    }

    [Fact]
    public void GenerateUsername_WithMultipleExistingSuffixes_ReturnsNextAvailable()
    {
        var usernames = new List<string> { "binhnv", "binhnv1", "binhnv2" };
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", usernames);
        Assert.Equal("binhnv3", result);
    }

    [Fact]
    public void GenerateUsername_WithGapInSuffix_ReturnsFirstAvailable()
    {
        var usernames = new List<string> { "binhnv", "binhnv2", "binhnv3" };
        var result = UserGenerationHelper.GenerateUsername("Binh", "Nguyen Van", usernames);
        Assert.Equal("binhnv1", result);
    }

    [Fact]
    public void GenerateUsername_WithSingleWordLastName_ReturnsCorrectInitial()
    {
        var usernames = new List<string>();
        var result = UserGenerationHelper.GenerateUsername("John", "Smith", usernames);
        Assert.Equal("johns", result);
    }

    [Fact]
    public void GenerateUsername_WithMultipleWordLastName_ReturnsAllInitials()
    {
        var usernames = new List<string>();
        var result = UserGenerationHelper.GenerateUsername("Alice", "Johnson Smith Brown", usernames);
        Assert.Equal("alicejsb", result);
    }

    [Fact]
    public void GenerateUsername_WithNullOrEmptyFirstName_Throws()
    {
        var usernames = new List<string>();
        var ex = Assert.Throws<ArgumentNullException>(() =>
            UserGenerationHelper.GenerateUsername("", "Nguyen", usernames));
        Assert.Equal("First name and last name is required!", ex.ParamName);
    }

    [Fact]
    public void GenerateUsername_WithNullOrEmptyLastName_Throws()
    {
        var usernames = new List<string>();
        var ex = Assert.Throws<ArgumentNullException>(
            () => UserGenerationHelper.GenerateUsername("Binh", "", usernames));
        Assert.Equal("First name and last name is required!", ex.ParamName);
    }
}