using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Domain.Services.TransactionOutputs
{
    public interface ITransactionOutputsService
    {
        Task<IEnumerable<Coin>> GetUnspentOutputsAsync(string address);
        Task CompleteTxOutputs(Guid operationId, NeoModules.NEP6.Transactions.Transaction tx);
    }
}
