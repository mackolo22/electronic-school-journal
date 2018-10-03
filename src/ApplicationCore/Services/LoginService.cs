using ApplicationCore.Exceptions;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using System;
using System.Text;

namespace ApplicationCore.Services
{
    public class LoginService : ILoginService
    {
        public string GenerateLogin(string firstName, string lastName)
        {
            if (String.IsNullOrWhiteSpace(firstName) || String.IsNullOrWhiteSpace(lastName))
            {
                throw new LoginException("Imię i nazwisko nie mogą być puste.");
            }
            else
            {
                // TODO: Sprawdzenie w bazie czy nie ma już takiego nicku.
                string login = String.Empty;
                login += firstName.ToLower().RemoveDiacritics()[0];
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
    }
}
