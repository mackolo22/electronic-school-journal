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

        public PersonService(IUniqueIDGenerator uniqueIDGenerator, ITableStorageRepository repository)
        {
            _uniqueIDGenerator = uniqueIDGenerator;
            _repository = repository;
        }

        // TODO: wykorzystać polimorfizm i złączyć te 2 metody w jedną.
        public async Task<Teacher> AddTeacherAsync(string firstName, string lastName, string login, string password)
        {
            long id = _uniqueIDGenerator.GetNextId();
            var teacher = new Teacher(id)
            {
                FirstName = firstName,
                LastName = lastName,
                Login = login,
                Password = password
            };

            await _repository.InsertAsync(teacher);
            return teacher;
        }

        public async Task<Parent> AddParentAsync(string firstName, string lastName, string login, string password)
        {
            long id = _uniqueIDGenerator.GetNextId();
            var parent = new Parent(id)
            {
                FirstName = firstName,
                LastName = lastName,
                Login = login,
                Password = password
            };

            await _repository.InsertAsync(parent);
            return parent;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            var teachers = await _repository.GetAllAsync<Teacher>("Teacher");
            return teachers;
        }
    }
}
