using ApplicationCore.Models;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ILoginService
    {
        string GenerateLogin(string firstName, string lastName);
        string GeneratePassword(int length = 8);
        string HashPassword(string password);
        Task<User> LoginUserAsync(string userType, string login, string password);
        bool LoginUserInOfflineMode(User user, string login, string password);
    }
}
