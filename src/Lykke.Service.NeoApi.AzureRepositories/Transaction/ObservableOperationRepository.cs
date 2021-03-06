﻿using System;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.NeoApi.Domain.Helpers;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;

namespace Lykke.Service.NeoApi.AzureRepositories.Transaction
{
    internal class ObservableOperationRepository: IObservableOperationRepository
    {
        private readonly INoSQLTableStorage<ObservableOperationEntity> _storage;

        public ObservableOperationRepository(INoSQLTableStorage<ObservableOperationEntity> storage)
        {
            _storage = storage;
        }
        

        public async Task InsertOrReplace(IObservableOperation tx)
        {
            await _storage.InsertOrReplaceAsync(ObservableOperationEntity.ByOperationId.Create(tx));
        }

        public async Task DeleteIfExist(params Guid[] operationIds)
        {
            await operationIds.ForEachAsyncSemaphore(8, 
                operationId => _storage.DeleteIfExistAsync(
                    ObservableOperationEntity.ByOperationId.GeneratePartitionKey(operationId),
                    ObservableOperationEntity.ByOperationId.GenerateRowKey()));
        }

        public async Task<IObservableOperation> GetById(Guid opId)
        {
            return await _storage.GetDataAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(opId),
                UnconfirmedTransactionEntity.GenerateRowKey());
        }
    }
}
