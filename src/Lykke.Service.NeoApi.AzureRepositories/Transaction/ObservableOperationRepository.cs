using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;

namespace Lykke.Service.NeoApi.AzureRepositories.Transaction
{
    internal class ObservableOperationRepository: IObservableOperationRepository
    {
        private readonly INoSQLTableStorage<ObservableOperationEntity> _storage;
        private readonly SemaphoreSlim _deletionSemaphore;

        public ObservableOperationRepository(INoSQLTableStorage<ObservableOperationEntity> storage)
        {
            _storage = storage;
            _deletionSemaphore = new SemaphoreSlim(1, 8);
        }
        

        public async Task InsertOrReplace(IObservableOperation tx)
        {
            await _storage.InsertOrReplaceAsync(ObservableOperationEntity.ByOperationId.Create(tx));
        }

        public async Task DeleteIfExist(params Guid[] operationIds)
        {
            var tasksToAwait = new List<Task>();

            foreach (var operationId in operationIds)
            {
                try
                {
                    await _deletionSemaphore.WaitAsync();

                    tasksToAwait.Add(_storage.DeleteIfExistAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(),
                        ObservableOperationEntity.ByOperationId.GenerateRowKey(operationId)));
                }
                finally
                {
                    _deletionSemaphore.Release(1);
                }
            }

            await Task.WhenAll(tasksToAwait);
        }

        public async Task<IObservableOperation> GetById(Guid opId)
        {
            return await _storage.GetDataAsync(ObservableOperationEntity.ByOperationId.GeneratePartitionKey(),
                UnconfirmedTransactionEntity.GenerateRowKey(opId));
        }
    }
}
