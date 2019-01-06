using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.NeoApi.Domain.Repositories.Outputs;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;

namespace Lykke.Service.NeoApi.AzureRepositories.SpentOutputs
{
    public class SpentOutputRepository : ISpentOutputRepository
    {
        private readonly INoSQLTableStorage<SpentOutputEntity> _table;
        private readonly SemaphoreSlim _insertionSemaphore;
        private readonly SemaphoreSlim _deletionSemaphore;

        public SpentOutputRepository(INoSQLTableStorage<SpentOutputEntity> table)
        {
            _table = table;
            _insertionSemaphore = new SemaphoreSlim(1, 8);
            _deletionSemaphore = new SemaphoreSlim(1, 8);
        }

        public async Task InsertSpentOutputsAsync(Guid operationId, IEnumerable<Output> outputs)
        {
            var entities = outputs.Select(o => SpentOutputEntity.Create(o.TransactionHash, o.N, operationId));

            var tasksToAwait = new List<Task>();

            foreach (var group in entities.GroupBy(o => o.PartitionKey))
            {
                await _insertionSemaphore.WaitAsync();
                try
                {
                    tasksToAwait.Add(_table.InsertOrReplaceAsync(group));
                }
                finally
                {
                    _insertionSemaphore.Release(1);
                }
            }

            await Task.WhenAll(tasksToAwait);
        }

        public async Task<IEnumerable<Output>> GetSpentOutputsAsync(IEnumerable<Output> outputs)
        {
            return (await _table.GetDataAsync(outputs.Select(o =>
                new Tuple<string, string>(SpentOutputEntity.GeneratePartitionKey(o.TransactionHash),
                    SpentOutputEntity.GenerateRowKey(o.N)))))
                .Select(p=> new Output(p.TransactionHash, p.N));
        }

        public async Task RemoveOldOutputsAsync(DateTime bound)
        {
            string continuation = null;
            do
            {
                IEnumerable<SpentOutputEntity> outputs;
                (outputs, continuation) = await _table.GetDataWithContinuationTokenAsync(100, continuation);

                var tasksToAwait = new List<Task>();

                foreach (var group in outputs.Where(o => o.Timestamp < bound).GroupBy(o => o.PartitionKey))
                {
                    await _deletionSemaphore.WaitAsync();
                    try
                    {
                        tasksToAwait.Add(_table.DeleteAsync(group));
                    }
                    finally
                    {

                        _deletionSemaphore.Release(1);
                    }
                }

                await Task.WhenAll(tasksToAwait);

            } while (continuation != null);
        }
    }
}
