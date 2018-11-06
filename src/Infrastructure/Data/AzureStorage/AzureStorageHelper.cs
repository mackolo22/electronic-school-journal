using ApplicationCore.Exceptions.AzureStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Data.AzureStorage
{
    public class AzureStorageHelper
    {
        private readonly CloudTableClient _tableClient;
        private readonly IDictionary<string, CloudTable> _tables;

        private static readonly IDictionary<string, string> TableNamesByTypeOfEntity = new Dictionary<string, string>
        {
            { "Administrator",  "UsersTable" },
            { "Student",        "UsersTable" },
            { "Teacher",        "UsersTable" },
            { "Parent",         "UsersTable" },
            { "StudentsClass",  "ClassesTable" }
        };

        public AzureStorageHelper()
        {
            // TODO: zapisywać connnection string w pliku konfiguracyjnym.
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "Electronic school journal", "connectionString.txt");
            string connectionString = File.ReadAllText(filePath);
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            _tables = new Dictionary<string, CloudTable>();
        }

        public async Task<CloudTable> EnsureTableExistenceAndGetReferenceAsync(string typeOfEntity)
        {
            bool tableNameFound = TableNamesByTypeOfEntity.TryGetValue(typeOfEntity, out string tableName);
            if (tableNameFound)
            {
                if (!_tables.ContainsKey(tableName))
                {
                    try
                    {
                        var table = _tableClient.GetTableReference(tableName);
                        await table.CreateIfNotExistsAsync();
                        _tables[tableName] = table;
                    }
                    catch (Exception exception)
                    {
                        throw new TableException($"Nie udało się utworzyć tabeli dla encji typu: {typeOfEntity}", exception);
                    }
                }

                return _tables[tableName];
            }
            else
            {
                throw new TableException($"Nie znaleziono nazwy tabeli dla encji typu: {typeOfEntity}.");
            }
        }
    }
}
