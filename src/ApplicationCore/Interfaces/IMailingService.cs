using ApplicationCore.Models;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IMailingService
    {
        Task SendEmailWithLoginAndPasswordAsync(Person newUser, Person administrator);
        Task SendEmailWithRecoveryCodeAsync(string email, int code);
        Task SendEmailWithNewPasswordAsync(string email, string password);
    }
}
