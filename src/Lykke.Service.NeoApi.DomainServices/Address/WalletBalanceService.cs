using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Address.Exceptions;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;
using Microsoft.WindowsAzure.Storage;
using NeoModules.NEP6.Helpers;
using NeoModules.Rest.Interfaces;

namespace Lykke.Service.NeoApi.DomainServices.Address
{
    public class WalletBalanceService:IWalletBalanceService
    {
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IWalletBalanceRepository _walletBalanceRepository;
        private readonly INeoscanService _neoscanService;
        private readonly ITransactionOutputsService _transactionOutputsService;

        private const int EntityExistsHttpStatusCode = 409;
        private const int EntityNotExistsHttpStatusCode = 404;

        public WalletBalanceService(INeoscanService neoscanService, 
            IObservableWalletRepository observableWalletRepository, 
            IWalletBalanceRepository walletBalanceRepository, 
            ITransactionOutputsService transactionOutputsService)
        {
            _neoscanService = neoscanService;
            _observableWalletRepository = observableWalletRepository;
            _walletBalanceRepository = walletBalanceRepository;
            _transactionOutputsService = transactionOutputsService;
        }

        public async Task<decimal?> UpdateNeoBalance(string address)
        {
            if(await _observableWalletRepository.Get(address) != null)
            {
                var lastBlock = await _neoscanService.GetHeight();

                var balance = (decimal) (await _transactionOutputsService.GetUnspentOutputsAsync(address))
                        .Where(p => p.Output.AssetId == Utils.NeoToken)
                        .ToArray()
                        .Sum(p => p.Output.Value);
                

                if (balance != 0)
                {
                    await _walletBalanceRepository.InsertOrReplace(
                        WalletBalance.Create(address,
                            balance: balance,
                            updatedAtBlock: (int)lastBlock));
                }
                else
                {
                    await _walletBalanceRepository.DeleteIfExist(address);
                }


                return balance;
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
