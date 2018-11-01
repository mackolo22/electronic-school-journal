using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IPersonService
    {
        Task<Teacher> AddTeacherAsync(Administrator administrator, string firstName, string lastName, string login, string email, string password, string hashedPassword);
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
    }
}
