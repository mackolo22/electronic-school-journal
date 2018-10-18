using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ITableStorageRepository
    {
        Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity;
        Task<IEnumerable<T>> GetAllAsync<T>(string partitionKey) where T : class, ITableEntity, new();
        Task<IEnumerable<T>> GetAllByPropertyAsync<T>(string partitionKey, string propertyName, string propertyValue) where T : class, ITableEntity, new();
        Task InsertOrReplaceAsync<T>(T entity) where T : class, ITableEntity;
        Task InsertBatchAsync<T>(IEnumerable<T> entities) where T : class, ITableEntity;
    }
}
