using System.Security.Cryptography;
using System.Text;

namespace WebApi.Security
{
    public class PasswordHashProvider
    {
        public static string GetSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string GetHash(string password, string salt)
        {
            byte[] srcBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes;

            using (var sha256 = SHA256.Create())
            {
                hashBytes = sha256.ComputeHash(srcBytes);
            }

            return Convert.ToBase64String(hashBytes);
        }
    }
}