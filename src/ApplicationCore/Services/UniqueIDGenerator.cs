using ApplicationCore.Interfaces;
using Microsoft.WindowsAzure.Storage;
using SnowMaker;
using System;
using System.IO;

namespace ApplicationCore.Services
{
    public class UniqueIDGenerator : IUniqueIDGenerator
    {
        private UniqueIdGenerator _generatorForUsers;
        private UniqueIdGenerator _generatorForMessages;
        private readonly string _connectionString;

        private const string UsersContainer = "userscontainer";
        private const string MessagesContainer = "messagescontainer";
        private const int BatchSize = 10;
        private const int MaxWriteAttempts = 25;

        public UniqueIDGenerator()
        {
            // TODO: czytać to z pliku conf
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = Path.Combine(appDataPath, "Electronic school journal", "connectionString.txt");
            string connectionString = File.ReadAllText(filePath);
            _connectionString = connectionString;
            ConnectToStorageAccountAndInitializeGenerators();
        }

        public void ConnectToStorageAccountAndInitializeGenerators()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(_connectionString);
            var dataStoreForUsers = new BlobOptimisticDataStore(cloudStorageAccount, UsersContainer);
            _generatorForUsers = new UniqueIdGenerator(dataStoreForUsers)
            {
                BatchSize = BatchSize,
                MaxWriteAttempts = MaxWriteAttempts
            };

            var dataStoreForMessages = new BlobOptimisticDataStore(cloudStorageAccount, MessagesContainer);
            _generatorForMessages = new UniqueIdGenerator(dataStoreForMessages)
            {
                BatchSize = BatchSize,
                MaxWriteAttempts = MaxWriteAttempts
            };
        }

        public long GetNextIdForUser()
        {
            string scopeName = "usersIds";
            long id = _generatorForUsers.NextId(scopeName);
            return id;
        }

        public long GetNextIdForMessage()
        {
            string scopeName = "messagesIds";
            long id = _generatorForMessages.NextId(scopeName);
            return id;
        }
    }
}
