using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Service.NeoApi.Domain.Repositories.Outputs;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;
using Lykke.SettingsReader;

namespace Lykke.Service.NeoApi.AzureRepositories.SpentOutputs
{
    public class SpentOutputRepository : ISpentOutputRepository
    {
        private readonly INoSQLTableStorage<SpentOutputEntity> _table;

        public SpentOutputRepository(INoSQLTableStorage<SpentOutputEntity> table)
        {
            _table = table;
        }

        public Task InsertSpentOutputsAsync(Guid operationId, IEnumerable<Output> outputs)
        {
            var entities = outputs.Select(o => SpentOutputEntity.Create(o.TransactionHash, o.N, operationId));
            return Task.WhenAll(entities.GroupBy(o => o.PartitionKey)
                .Select(group => _table.InsertOrReplaceAsync(group)));
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
            IEnumerable<SpentOutputEntity> outputs = null;
            do
            {
                (outputs, continuation) = await _table.GetDataWithContinuationTokenAsync(100, continuation);
                await Task.WhenAll(outputs.Where(o => o.Timestamp < bound).GroupBy(o => o.PartitionKey)
                    .Select(group => _table.DeleteAsync(group)));
            } while (continuation != null);
        }


        public static ISpentOutputRepository Create(IReloadingManager<string> connectionString,
            ILogFactory logFactory)
        {
            const string tableName = "SpentOutputs";
            var table = AzureTableStorage<SpentOutputEntity>.Create(connectionString,
                tableName, logFactory);

            return new SpentOutputRepository(table);
        }
    }
}
