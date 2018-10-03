using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ITableStorageRepository
    {
        Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity;
        Task<IEnumerable<T>> GetAllAsync<T>(string partitionKey) where T : class, ITableEntity, new();
        Task<IQueryable<T>> QueryAllAsync<T>() where T : class, ITableEntity;
        Task InsertAsync<T>(T entity) where T : class, ITableEntity;
        Task InsertBatchAsync<T>(IEnumerable<T> entities) where T : class, ITableEntity;
    }
}
