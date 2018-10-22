using System.Threading.Tasks;

namespace Lykke.Service.NeoApi.Domain.Services.Transaction
{
    public interface ITransactionBuilder
    {
        Task<string> BuildNeoContractTransactionAsync(string from, string to, decimal amount, bool includeFee, decimal fixedFee);
    }
}
