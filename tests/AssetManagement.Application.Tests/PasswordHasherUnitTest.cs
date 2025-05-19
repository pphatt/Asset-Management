using System.Text.RegularExpressions;
using AssetManagement.Application.Services;
using AssetManagement.Application.Services.Interfaces;

namespace AssetManagement.Application.Tests
{
    public class PasswordHasherUnitTest
    {
        private readonly IPasswordHasher _passwordHasher;

        public PasswordHasherUnitTest()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_ReturnsNonNullValue()
        {
            // Arrange
            string password = "P@ssw0rd";

            // Act
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);
        }

        [Fact]
        public void HashPassword_ReturnsDifferentValuesForSamePassword()
        {
            // Arrange
            string password = "P@ssw0rd";

            // Act
            string hashedPassword1 = _passwordHasher.HashPassword(password);
            string hashedPassword2 = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotEqual(hashedPassword1, hashedPassword2);
        }

        [Fact]
        public void HashPassword_ReturnsConsistentLength()
        {
            // Arrange
            string shortPassword = "abc";
            string longPassword = "ThisIsAVeryLongPasswordWithMoreThan30Characters";

            // Act
            string shortHash = _passwordHasher.HashPassword(shortPassword);
            string longHash = _passwordHasher.HashPassword(longPassword);

            // Assert
            Assert.Equal(shortHash.Length, longHash.Length);
        }

        [Fact]
        public void HashPassword_ReturnsBase64EncodedString()
        {
            // Arrange
            string password = "Test123!";
            
            // A valid Base64 string matches this pattern
            Regex base64Regex = new Regex(@"^[a-zA-Z0-9\+/]*={0,3}$");

            // Act
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Assert
            Assert.Matches(base64Regex, hashedPassword);
            
            // Should be decodable as Base64
            bool isDecodable = true;
            try 
            {
                Convert.FromBase64String(hashedPassword);
            }
            catch 
            {
                isDecodable = false;
            }
            Assert.True(isDecodable);
        }
        
        [Fact]
        public void HashPassword_HandlesVeryLongPassword()
        {
            // Arrange
            string password = new string('a', 10000); // Very long password

            // Act & Assert (should not throw)
            Exception? exception = Record.Exception(() => _passwordHasher.HashPassword(password));
            Assert.Null(exception);
        }
    }
}
