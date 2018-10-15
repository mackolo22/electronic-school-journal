using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;

namespace ApplicationCore.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailWithLoginAndPasswordAsync(Person person)
        {
            throw new System.NotImplementedException();
        }
    }
}
