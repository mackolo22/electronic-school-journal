using ApplicationCore.Exceptions;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUsersRepository _usersRepository;

        public LoginService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public string GenerateLogin(string firstName, string lastName)
        {
            if (String.IsNullOrWhiteSpace(firstName) || String.IsNullOrWhiteSpace(lastName))
            {
                throw new LoginException("Imię i nazwisko nie mogą być puste.");
            }
            else
            {
                string login = String.Empty;
                login += firstName.ToLower().RemoveDiacritics();
                login += lastName.ToLower().RemoveDiacritics();
                return login;
            }
        }

        public string GeneratePassword(int length = 8)
        {
            const string availableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            while (0 < length--)
            {
                password.Append(availableChars[random.Next(availableChars.Length)]);
            }

            return password.ToString();
        }

        public string HashPassword(string password)
        {
            string stringHash = String.Empty;
            if (!String.IsNullOrWhiteSpace(password))
            {
                var sha256 = new SHA256Managed();
                byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
                foreach (byte theByte in hash)
                {
                    stringHash += theByte.ToString("x2");
                }
            }

            return stringHash;
        }

        public async Task<User> LoginUserAsync(string userType, string login, string password)
        {
            var users = await _usersRepository.GetAllByPropertyAsync(userType, "Login", login);
            var user = users.FirstOrDefault();
            if (user != null)
            {
                string hashedPassword = HashPassword(password);
                if (user.HashedPassword == hashedPassword)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool LoginUserInOfflineMode(User user, string login, string password)
        {
            string hashedPassword = HashPassword(password);
            if (user.Login == login && user.HashedPassword == hashedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
