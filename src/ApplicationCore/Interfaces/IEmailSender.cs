using ApplicationCore.Models;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailWithLoginAndPasswordAsync(Person person);
    }
}
