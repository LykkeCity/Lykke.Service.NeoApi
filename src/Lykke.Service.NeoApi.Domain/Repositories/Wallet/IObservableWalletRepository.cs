using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;

namespace Lykke.Service.NeoApi.Domain.Repositories.Wallet
{
    public interface IObservableWalletRepository
    {
        Task Insert(IObservableWallet wallet);
        Task<IEnumerable<IObservableWallet>> GetAll();
        Task<IPaginationResult<IObservableWallet>> GetPaged(int take, string continuation);
        Task Delete(string address);
        Task<IObservableWallet> Get(string address);
    }
}
