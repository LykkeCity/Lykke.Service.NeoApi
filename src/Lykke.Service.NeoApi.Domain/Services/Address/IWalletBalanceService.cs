using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;

namespace Lykke.Service.NeoApi.Domain.Services.Address
{
    public interface IWalletBalanceService
    {
        Task<decimal> UpdateBalance(string address);
        Task Subscribe(string address);
        Task Unsubscribe(string address);
        Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation);
    }
}
