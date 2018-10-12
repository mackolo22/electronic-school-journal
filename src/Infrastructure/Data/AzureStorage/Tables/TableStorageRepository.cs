﻿using ApplicationCore.Exceptions.AzureStorage;
using ApplicationCore.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.AzureStorage.Tables
{
    public class TableStorageRepository : ITableStorageRepository
    {
        private readonly AzureStorageHelper _azureStorageHelper;

        public TableStorageRepository(AzureStorageHelper azureStorageHelper)
        {
            _azureStorageHelper = azureStorageHelper;
        }

        public async Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity
        {
            var typeOfEntity = typeof(T);
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(typeOfEntity);
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            try
            {
                var result = await table.ExecuteAsync(retrieveOperation);
                return result as T;
            }
            catch (Exception ex)
            {
                throw new TableException("Pobieranie obiektu z bazy danych nie powiodło się.", ex);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string partitionKey) where T : class, ITableEntity, new()
        {
            var typeOfEntity = typeof(T);
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(typeOfEntity);
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var tableQuery = new TableQuery<T>().Where(filter);
            var entities = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                var tableQuerySegment = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                continuationToken = tableQuerySegment.ContinuationToken;
                entities.AddRange(tableQuerySegment.Results);
            }
            while (continuationToken != null);

            return entities;
        }

        public async Task InsertAsync<T>(T entity) where T : class, ITableEntity
        {
            var typeOfEntity = typeof(T);
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(typeOfEntity);
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
            try
            {
                var tableResult = await table.ExecuteAsync(insertOrReplaceOperation);
            }
            catch (Exception ex)
            {
                throw new TableException("Dodawanie nowego obiektu do bazy danych nie powiodło się.", ex);
            }
        }

        public async Task InsertBatchAsync<T>(IEnumerable<T> entities) where T : class, ITableEntity
        {
            var typeOfEntity = typeof(T);
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(typeOfEntity);
            var batchOperation = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batchOperation.Insert(entity);
            }

            await table.ExecuteBatchAsync(batchOperation);
        }

        public async Task<IEnumerable<T>> GetAllByPropertyAsync<T>(
            string partitionKey,
            string propertyName,
            string propertyValue) where T : class, ITableEntity, new()
        {
            var typeOfEntity = typeof(T);
            var table = await _azureStorageHelper.EnsureTableExistenceAndGetReferenceAsync(typeOfEntity);
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            string propertyFilter = TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, propertyValue);
            string finalFilter = TableQuery.CombineFilters(partitionKeyFilter, TableOperators.And, propertyFilter);
            var tableQuery = new TableQuery<T>().Where(finalFilter);
            var entities = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                var tableQuerySegment = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                continuationToken = tableQuerySegment.ContinuationToken;
                entities.AddRange(tableQuerySegment.Results);
            }
            while (continuationToken != null);

            return entities;
        }
    }
}
