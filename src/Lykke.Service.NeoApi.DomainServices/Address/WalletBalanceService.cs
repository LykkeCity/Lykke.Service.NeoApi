using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Address.Exceptions;
using Microsoft.WindowsAzure.Storage;
using NeoModules.Rest.Interfaces;

namespace Lykke.Service.NeoApi.DomainServices.Address
{
    public class WalletBalanceService:IWalletBalanceService
    {
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IWalletBalanceRepository _walletBalanceRepository;
        private readonly INeoscanService _neoscanService;

        private const int EntityExistsHttpStatusCode = 409;
        private const int EntityNotExistsHttpStatusCode = 404;

        public WalletBalanceService(INeoscanService neoscanService, 
            IObservableWalletRepository observableWalletRepository, 
            IWalletBalanceRepository walletBalanceRepository)
        {
            _neoscanService = neoscanService;
            _observableWalletRepository = observableWalletRepository;
            _walletBalanceRepository = walletBalanceRepository;
        }

        public async Task<decimal?> UpdateBalance(string address)
        {
            if(await _observableWalletRepository.Get(address) != null)
            {
                var lastBlock = await _neoscanService.GetHeight();

                var neoBalance = (await _neoscanService.GetBalanceAsync(address))?
                                 .Balance?
                                 .FirstOrDefault(p => p.Asset == Constants.Assets.Neo.AssetId)?.Amount ?? 0;

                await _walletBalanceRepository.InsertOrReplace(
                    WalletBalance.Create(address,
                        balance: (decimal)neoBalance,
                        updatedAtBlock: (int)lastBlock));

                return (decimal)neoBalance;
            }

            return null;
        }

        public async Task Subscribe(string address)
        {
            try
            {
                await _observableWalletRepository.Insert(ObservableWallet.Create(address));
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == EntityExistsHttpStatusCode)
            {
                throw new WalletAlreadyExistException();
            }
        }

        public async Task Unsubscribe(string address)
        {
            try
            {
                await _observableWalletRepository.Delete(address);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == EntityNotExistsHttpStatusCode)
            {
                throw new WalletNotExistException();
            }

            await _walletBalanceRepository.DeleteIfExist(address);
        }

        public async Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation)
        {
            return await _walletBalanceRepository.GetBalances(take, continuation);
        }
    }
}
