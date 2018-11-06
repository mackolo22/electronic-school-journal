using ApplicationCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IUsersRepository
    {
        Task<User> GetAsync(string partitionKey, string rowKey);
        Task<IEnumerable<User>> GetAllAsync(string partitionKey);
        Task<IEnumerable<User>> GetAllByPropertyAsync(string partitionKey, string propertyName, string propertyValue);
        Task InsertOrReplaceAsync(User user);
        Task InsertBatchAsync(IEnumerable<User> users);
    }
}
