using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Data.AzureStorage.Tables
{
    public class ClassesRepository : IClassesRepository
    {
        private readonly AzureStorageHelper _azureStorageHelper;

        public ClassesRepository(AzureStorageHelper azureStorageHelper)
        {
            _azureStorageHelper = azureStorageHelper;
        }

        public async Task<StudentsClass> GetAsync(string partitionKey, string rowKey)
        {
            try
            {
                var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync("StudentsClass");
                var retrieveOperation = TableOperation.Retrieve<StudentsClass>(partitionKey, rowKey);
                var tableResult = await table.ExecuteAsync(retrieveOperation);
                return tableResult.Result as StudentsClass;
            }
            catch (Exception exception)
            {
                throw new TableException("Pobieranie obiektu z bazy danych nie powiodło się.", exception);
            }
        }

        public async Task InsertOrReplaceAsync(StudentsClass studentsClass)
        {
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync("StudentsClass");
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(studentsClass);
            try
            {
                var tableResult = await table.ExecuteAsync(insertOrReplaceOperation);
            }
            catch (Exception exception)
            {
                throw new TableException("Dodawanie nowego obiektu do bazy danych nie powiodło się.", exception);
            }
        }
    }
}
