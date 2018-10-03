using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Data.AzureStorage
{
    public class AzureStorageHelper
    {
        private readonly CloudTableClient _tableClient;
        private readonly IDictionary<string, CloudTable> _tables;

        public IDictionary<Type, string> TableNamesByTypeOfEntity { get; set; } = new Dictionary<Type, string>
        {
            { typeof(Student),          "PeopleTable" },
            { typeof(Teacher),          "PeopleTable" },
            { typeof(Parent),           "PeopleTable" },
            { typeof(StudentsClass),    "ClassesTable" },
            { typeof(DynamicTableEntity), "ClassesTable" }
        };

        // TODO: poprawić tę metodę gdy zostanie dodany DI container.
        public AzureStorageHelper(/*IConfiguration configuration*/)
        {
            string connectionString = "UseDevelopmentStorage=true";
            //string connectionString = configuration["StorageConfiguration:ConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            _tables = new Dictionary<string, CloudTable>();
        }

        public async Task<CloudTable> EnsureTableExistenceAndGetReferenceAsync(Type typeOfEntity)
        {
            string tableName = String.Empty;
            try
            {
                tableName = TableNamesByTypeOfEntity[typeOfEntity];
            }
            catch (KeyNotFoundException ex)
            {
                throw new TableException($"Nie znaleziono nazwy tabeli dla encji typu: {typeOfEntity.Name}.", ex);
            }

            if (!_tables.ContainsKey(tableName))
            {
                try
                {
                    var table = _tableClient.GetTableReference(tableName);
                    await table.CreateIfNotExistsAsync();
                    _tables[tableName] = table;
                }
                catch (Exception ex)
                {
                    throw new TableException($"Nie udało się utworzyć tabeli dla encji typu: {typeOfEntity.Name}", ex);
                }
            }

            return _tables[tableName];
        }
    }
}
