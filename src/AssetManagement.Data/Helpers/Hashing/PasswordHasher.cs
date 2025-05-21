using System.Security.Cryptography;
using System.Text;

namespace AssetManagement.Data.Helpers.Hashing
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Create the hash using PBKDF2 with 100,000 iterations
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: Encoding.UTF8.GetBytes(password),
                salt: salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA512,
                outputLength: 32);

            // Combine the salt and hash into a single string
            byte[] hashBytes = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);

            // Convert to Base64 string for storage
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string providedPassword, string hashedPassword)
        {
            try
            {
                // Convert the base64 string back to bytes
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                // Extract the salt (first 16 bytes)
                byte[] salt = new byte[16];
                Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

                // Extract the original hash (remaining bytes)
                byte[] originalHash = new byte[hashBytes.Length - 16];
                Buffer.BlockCopy(hashBytes, 16, originalHash, 0, originalHash.Length);

                // Compute the hash of the provided password using the same salt
                byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                    password: Encoding.UTF8.GetBytes(providedPassword),
                    salt: salt,
                    iterations: 100_000,
                    hashAlgorithm: HashAlgorithmName.SHA512,
                    outputLength: 32);

                // Compare the computed hash with the original hash
                return CryptographicOperations.FixedTimeEquals(originalHash, computedHash);
            }
            catch (Exception)
            {
                // If any error occurs (like invalid base64 string), return false
                return false;
            }
        }
    }
}
