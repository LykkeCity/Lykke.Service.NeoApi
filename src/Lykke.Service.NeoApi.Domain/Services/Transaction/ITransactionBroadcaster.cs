using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;

namespace Lykke.Service.NeoApi.Domain.Services.Transaction
{
    public interface ITransactionBroadcaster
    {
        Task BroadcastTransaction(NeoModules.NEP6.Transactions.Transaction signedTransaction, OperationAggregate aggregate);
    }
}
