using System;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;

namespace Lykke.Service.NeoApi.AzureRepositories.Operation
{
    public class OperationRepository:IOperationRepository
    {
        private readonly INoSQLTableStorage<OperationEntity> _storage;

        public OperationRepository(INoSQLTableStorage<OperationEntity> storage)
        {
            _storage = storage;
        }

        public Task Insert(OperationAggregate aggregate)
        {
            return _storage.InsertAsync(OperationEntity.FromDomain(aggregate));
        }

        public async Task<OperationAggregate> GetOrDefault(Guid operationId)
        {
            var entity = await _storage.GetDataAsync(
                OperationEntity.GeneratePartitionKey(operationId),
                OperationEntity.GenerateRowKey());

            return entity?.ToDomain();
        }

        public async Task<OperationAggregate> GetOrInsert(Guid operationId, Func<OperationAggregate> factory)
        {
            var rowKey = OperationEntity.GenerateRowKey();
            var partitionKey = OperationEntity.GeneratePartitionKey(operationId);

            return (await _storage.GetOrInsertAsync(partitionKey, rowKey, () => OperationEntity.FromDomain(factory())))
                .ToDomain();
        }

        public Task Save(OperationAggregate aggregate)
        {
            return _storage.ReplaceAsync(OperationEntity.FromDomain(aggregate));
        }
    }
}
