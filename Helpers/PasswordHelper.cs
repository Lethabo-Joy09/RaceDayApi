using System.Security.Cryptography;
using System.Text;

namespace RaceDayApi.Helpers
{
    public static class PasswordHelper
    {
        // Hash a plain-text password using SHA256 with a salt
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var saltBytes = Encoding.UTF8.GetBytes("RaceDaySalt2026");
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var combined = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, combined, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combined, saltBytes.Length, passwordBytes.Length);

            var hashedBytes = sha256.ComputeHash(combined);
            return Convert.ToBase64String(hashedBytes);
        }

        // Verify a plain-text password against a stored hash
        public static bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}