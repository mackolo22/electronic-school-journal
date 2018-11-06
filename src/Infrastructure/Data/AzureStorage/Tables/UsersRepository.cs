using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.AzureStorage.Tables
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AzureStorageHelper _azureStorageHelper;

        public UsersRepository(AzureStorageHelper azureStorageHelper)
        {
            _azureStorageHelper = azureStorageHelper;
        }

        public async Task<User> GetAsync(string partitionKey, string rowKey)
        {
            try
            {
                var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(partitionKey);
                User user = null;
                if (partitionKey == "Administrator")
                {
                    user = await RetrieveEntityAsync<Administrator>(table, partitionKey, rowKey);
                }
                else if (partitionKey == "Student")
                {
                    user = await RetrieveEntityAsync<Student>(table, partitionKey, rowKey);
                }
                else if (partitionKey == "Parent")
                {
                    user = await RetrieveEntityAsync<Parent>(table, partitionKey, rowKey);
                }
                else if (partitionKey == "Teacher")
                {
                    user = await RetrieveEntityAsync<Teacher>(table, partitionKey, rowKey);
                }
                else
                {
                    throw new TableException("Pobieranie obiektu z bazy danych nie powiodło się. Niepoprawny typ użytkownika.");
                }

                return user;
            }
            catch (Exception exception)
            {
                throw new TableException("Pobieranie obiektu z bazy danych nie powiodło się.", exception);
            }
        }

        private async Task<T> RetrieveEntityAsync<T>(CloudTable table, string partitionKey, string rowKey) where T : class, ITableEntity
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var tableResult = await table.ExecuteAsync(retrieveOperation);
            return tableResult.Result as T;
        }

        public async Task<IEnumerable<User>> GetAllAsync(string partitionKey)
        {
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var users = await GetAllByFilterAsync(partitionKey, filter);
            return users;
        }

        public async Task<IEnumerable<User>> GetAllByPropertyAsync(string partitionKey, string propertyName, string propertyValue)
        {
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            string propertyFilter = TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, propertyValue);
            string finalFilter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, propertyFilter);
            var users = await GetAllByFilterAsync(partitionKey, finalFilter);
            return users;
        }

        private async Task<IEnumerable<User>> GetAllByFilterAsync(string partitionKey, string filter)
        {
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(partitionKey);
            if (partitionKey == "Administrator")
            {
                var administrators = new List<Administrator>();
                var tableQuery = new TableQuery<Administrator>().Where(filter);
                await ExecuteQuerySegmentedAndFillListOfEntitiesAsync(table, tableQuery, administrators);
                return administrators;
            }
            else if (partitionKey == "Student")
            {
                var students = new List<Student>();
                var tableQuery = new TableQuery<Student>().Where(filter);
                await ExecuteQuerySegmentedAndFillListOfEntitiesAsync(table, tableQuery, students);
                return students;
            }
            else if (partitionKey == "Parent")
            {
                var parents = new List<Parent>();
                var tableQuery = new TableQuery<Parent>().Where(filter);
                await ExecuteQuerySegmentedAndFillListOfEntitiesAsync(table, tableQuery, parents);
                return parents;
            }
            else if (partitionKey == "Teacher")
            {
                var teachers = new List<Teacher>();
                var tableQuery = new TableQuery<Teacher>().Where(filter);
                await ExecuteQuerySegmentedAndFillListOfEntitiesAsync(table, tableQuery, teachers);
                return teachers;
            }
            else
            {
                return null;
            }
        }

        private async Task ExecuteQuerySegmentedAndFillListOfEntitiesAsync<T>(CloudTable table, TableQuery<T> tableQuery, List<T> entities)
            where T : class, ITableEntity, new()
        {
            TableContinuationToken continuationToken = null;
            do
            {
                var tableQuerySegment = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                continuationToken = tableQuerySegment.ContinuationToken;
                entities.AddRange(tableQuerySegment.Results);
            }
            while (continuationToken != null);
        }

        public async Task InsertOrReplaceAsync(User user)
        {
            string userType = user.GetType().Name;
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(userType);
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(user);
            try
            {
                var tableResult = await table.ExecuteAsync(insertOrReplaceOperation);
            }
            catch (Exception exception)
            {
                throw new TableException("Dodawanie nowego obiektu do bazy danych nie powiodło się.", exception);
            }
        }

        public async Task InsertBatchAsync(IEnumerable<User> users)
        {
            if (users != null && users.Count() > 0)
            {
                try
                {
                    string userType = users.FirstOrDefault().GetType().Name;
                    var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(userType);
                    var batchOperation = new TableBatchOperation();
                    foreach (var user in users)
                    {
                        batchOperation.Insert(user);
                    }

                    await table.ExecuteBatchAsync(batchOperation);
                }
                catch (Exception exception)
                {
                    throw new TableException("Dodawanie kolekcji obiektów do bazy danych nie powiodło się.", exception);
                }
            }
        }
    }
}
