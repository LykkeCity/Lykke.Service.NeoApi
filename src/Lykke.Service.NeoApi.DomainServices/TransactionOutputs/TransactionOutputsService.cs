using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Outputs;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;
using NeoModules.NEP6.Helpers;
using NeoModules.NEP6.Transactions;
using NeoModules.Rest.Interfaces;

namespace Lykke.Service.NeoApi.DomainServices.TransactionOutputs
{
    public class TransactionOutputsService : ITransactionOutputsService
    {
        private readonly INeoscanService _neoscanService;
        private readonly ISpentOutputRepository _spentOutputRepository;

        public TransactionOutputsService(INeoscanService neoscanService,
            ISpentOutputRepository spentOutputRepository)
        {
            _neoscanService = neoscanService;
            _spentOutputRepository = spentOutputRepository;
        }

        public async Task<IEnumerable<Coin>> GetUnspentOutputsAsync(string address)
        {
            var blockchainOutputs = await TransactionBuilderHelper.GetUnspent(address, _neoscanService);

            return await FilterSpentCoinsAsync(blockchainOutputs.ToList());
        }

        public async Task CompleteTxOutputs(Guid operationId, NeoModules.NEP6.Transactions.Transaction tx)
        {
            var inputs = tx.Inputs.Select(o => new Output(o)).ToList();
            await _spentOutputRepository.InsertSpentOutputsAsync(operationId, inputs);
        }

        private async Task<IEnumerable<Coin>> FilterSpentCoinsAsync(IList<Coin> coins)
        {
            var spentOutputs = new HashSet<Output>(
                (await _spentOutputRepository.GetSpentOutputsAsync(coins.Select(o => new Output(o.Reference)))),
                Output.TransactionHashNComparer);
            return coins.Where(c => !spentOutputs.Contains(new Output(c.Reference)));
        }
    }
}
