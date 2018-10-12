using ApplicationCore.Enums;
using ApplicationCore.Models;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ILoginService
    {
        string GenerateLogin(string firstName, string lastName);
        string GeneratePassword(int length = 8);
        string HashPassword(string password);
        Task<Student> LoginStudentAsync(string login, string password);
        Task<Teacher> LoginTeacherAsync(string login, string password);
        Task<Parent> LoginParentAsync(string login, string password);
        Task<Administrator> LoginAdministratorAsync(string login, string password);
    }
}
