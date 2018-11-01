using ApplicationCore.Interfaces;
using Microsoft.WindowsAzure.Storage;
using SnowMaker;
using System;
using System.IO;

namespace ApplicationCore.Services
{
    public class UniqueIDGenerator : IUniqueIDGenerator
    {
        private UniqueIdGenerator _generator;
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _scopeName;
        private const int _batchSize = 10;
        private const int _maxWriteAttempts = 25;

        public UniqueIDGenerator()
        {
            // TODO: czytać to z pliku conf
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "Electronic school journal", "connectionString.txt");
            string connectionString = File.ReadAllText(filePath);
            _connectionString = connectionString;
            _containerName = "peoplecontainer";
            _scopeName = "peopleIds";
            ConnectToStorageAccountAndInitializeGenerator();
        }

        public void ConnectToStorageAccountAndInitializeGenerator()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
            var dataStore = new BlobOptimisticDataStore(cloudStorageAccount, _containerName);
            _generator = new UniqueIdGenerator(dataStore)
            {
                BatchSize = _batchSize,
                MaxWriteAttempts = _maxWriteAttempts
            };
        }

        public long GetNextId()
        {
            var id = _generator.NextId(_scopeName);
            return id;
        }
    }
}
