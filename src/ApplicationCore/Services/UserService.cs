using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUniqueIDGenerator _uniqueIDGenerator;
        private readonly IMailingService _mailingService;

        public UserService(
            IUsersRepository usersRepository,
            IUniqueIDGenerator uniqueIDGenerator,
            IMailingService mailingService)
        {
            _usersRepository = usersRepository;
            _uniqueIDGenerator = uniqueIDGenerator;
            _mailingService = mailingService;
        }

        public async Task<Teacher> AddTeacherAsync(
            Administrator administrator,
            string firstName,
            string lastName,
            string login,
            string email,
            string password,
            string hashedPassword)
        {
            long id = _uniqueIDGenerator.GetNextId();
            var teacher = new Teacher(id)
            {
                FirstName = firstName,
                LastName = lastName,
                Login = login,
                Email = email,
                Password = password,
                HashedPassword = hashedPassword
            };

            await _usersRepository.InsertOrReplaceAsync(teacher);
            await _mailingService.SendEmailWithLoginAndPasswordAsync(teacher, administrator);
            return teacher;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            var teachers = await _usersRepository.GetAllAsync(nameof(Teacher));
            return teachers as List<Teacher>;
        }
    }
}
