using ApplicationCore.Enums;
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
        private readonly ITableStorageRepository _repository;

        public LoginService(ITableStorageRepository repository)
        {
            _repository = repository;
        }

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
            var sha256 = new SHA256Managed();
            string stringHash = String.Empty;
            byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
            foreach (byte theByte in hash)
            {
                stringHash += theByte.ToString("x2");
            }

            return stringHash;
        }

        public async Task<Administrator> LoginAdministratorAsync(string login, string password)
        {
            var matchingAdministrators = await _repository.GetAllByPropertyAsync<Administrator>(nameof(Administrator), "Login", login);
            var administrator = matchingAdministrators.Where(x => x.Login == login).FirstOrDefault();
            if (administrator != null && administrator.HashedPassword == password)
            {
                return administrator;
            }

            return null;
        }

        public async Task<Parent> LoginParentAsync(string login, string password)
        {
            var matchingParents = await _repository.GetAllByPropertyAsync<Parent>(nameof(Parent), "Login", login);
            var parent = matchingParents.Where(x => x.Login == login).FirstOrDefault();
            if (parent != null && parent.HashedPassword == password)
            {
                return parent;
            }

            return null;
        }

        public async Task<Student> LoginStudentAsync(string login, string password)
        {
            var matchingStudents = await _repository.GetAllByPropertyAsync<Student>(nameof(Student), "Login", login);
            var student = matchingStudents.Where(x => x.Login == login).FirstOrDefault();
            if (student != null && student.HashedPassword == password)
            {
                return student;
            }

            return null;
        }

        public async Task<Teacher> LoginTeacherAsync(string login, string password)
        {
            var matchingTeachers = await _repository.GetAllByPropertyAsync<Teacher>(nameof(Teacher), "Login", login);
            var teacher = matchingTeachers.Where(x => x.Login == login).FirstOrDefault();
            if (teacher != null && teacher.HashedPassword == password)
            {
                return teacher;
            }

            return null;
        }
    }
}
