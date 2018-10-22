using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;

namespace Lykke.Service.NeoApi.Domain.Services.Transaction
{
    public interface ITransactionBroadcaster
    {
        Task BroadcastTransaction(string signedTransaction, OperationAggregate aggregate);
    }
}
