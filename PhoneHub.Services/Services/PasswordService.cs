using Microsoft.Extensions.Options;
using PhoneHub.Core.CustomEntities;
using PhoneHub.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace PhoneHub.Services.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordOptions _options;

        public PasswordService(IOptions<PasswordOptions> options)
        {
            _options = options.Value;
        }

        public string Hash(string password)
        {
            var saltedPassword = _options.SaltKey + password;

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(bytes);
        }

        public bool Verify(string hashedPassword, string plainPassword)
        {
            return hashedPassword == Hash(plainPassword);
        }
    }
}
