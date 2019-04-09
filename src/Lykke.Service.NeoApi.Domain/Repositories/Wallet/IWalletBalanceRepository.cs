using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;

namespace Lykke.Service.NeoApi.Domain.Repositories.Wallet
{
    public interface IWalletBalanceRepository
    {
        Task InsertOrReplace(IWalletBalance balance);

        Task DeleteIfExist(string address, string assetId);
        Task DeleteIfExist(string address);
        Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation);
    }
}
