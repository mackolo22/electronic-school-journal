using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IPersonService
    {
        Task<Teacher> AddTeacherAsync(string firstName, string lastName, string login, string password);
        Task<Parent> AddParentAsync(string firstName, string lastName, string login, string password);
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
    }
}
