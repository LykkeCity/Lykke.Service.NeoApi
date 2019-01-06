using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;

namespace Lykke.Service.NeoApi.AzureRepositories.Transaction
{
    internal class UnconfirmedTransactionRepository: IUnconfirmedTransactionRepository
    {
        private readonly INoSQLTableStorage<UnconfirmedTransactionEntity> _storage;
        private readonly SemaphoreSlim _deletionSemaphore;

        public UnconfirmedTransactionRepository(INoSQLTableStorage<UnconfirmedTransactionEntity> storage)
        {
            _storage = storage;
            _deletionSemaphore = new SemaphoreSlim(1, 8);
        }

        public async Task<IEnumerable<IUnconfirmedTransaction>> GetAll()
        {
            return await _storage.GetDataAsync();
        }

        public Task InsertOrReplace(IUnconfirmedTransaction tx)
        {
            return _storage.InsertOrReplaceAsync(UnconfirmedTransactionEntity.Create(tx));
        }

        public async Task DeleteIfExist(Guid[] operationIds)
        {
            var tasksToAwait = new List<Task>();

            foreach (var operationId in operationIds)
            {
                await _deletionSemaphore.WaitAsync();
                try
                {
                    tasksToAwait.Add(_storage.DeleteIfExistAsync(UnconfirmedTransactionEntity.GeneratePartitionKey(),
                        UnconfirmedTransactionEntity.GenerateRowKey(operationId)));
                }
                finally
                {
                    _deletionSemaphore.Release(1);
                }
            }

            await Task.WhenAll(tasksToAwait);
        }
    }
}
