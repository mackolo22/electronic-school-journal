using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUniqueIDGenerator _uniqueIDGenerator;
        private readonly ITableStorageRepository _repository;
        private readonly IMailingService _mailingService;

        public PersonService(
            IUniqueIDGenerator uniqueIDGenerator,
            ITableStorageRepository repository,
            IMailingService mailingService)
        {
            _uniqueIDGenerator = uniqueIDGenerator;
            _repository = repository;
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

            await _repository.InsertOrReplaceAsync(teacher);
            await _mailingService.SendEmailWithLoginAndPasswordAsync(teacher, administrator);
            return teacher;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            var teachers = await _repository.GetAllAsync<Teacher>(nameof(Teacher));
            return teachers;
        }
    }
}
