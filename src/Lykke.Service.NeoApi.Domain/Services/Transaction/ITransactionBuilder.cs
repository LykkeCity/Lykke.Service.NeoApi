using System.Threading.Tasks;

namespace Lykke.Service.NeoApi.Domain.Services.Transaction
{
    public interface ITransactionBuilder
    {
        Task<NeoModules.NEP6.Transactions.Transaction> BuildNeoContractTransactionAsync(string from, string to, decimal amount, bool includeFee, decimal fixedFee);
    }
}
