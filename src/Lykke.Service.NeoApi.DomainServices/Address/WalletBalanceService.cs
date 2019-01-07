using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Helpers;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Address.Exceptions;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;
using Microsoft.WindowsAzure.Storage;
using NeoModules.Core;
using NeoModules.NEP6.Helpers;
using NeoModules.Rest.Interfaces;
using NeoModules.Rest.DTOs.NeoScan;

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

                var unspentOutputs = (await _transactionOutputsService.GetUnspentOutputsAsync(address))
                    .Where(p => p.Output.AssetId == Utils.NeoToken)
                    .ToList();

                var blockHeightFromTxHash = new ConcurrentDictionary<UInt256, 
                    NeoModules.Rest.DTOs.NeoScan.Transaction>();

                await unspentOutputs.Select(p => p.Reference.PrevHash).Distinct()
                    .ForEachAsyncSemaphore(8, async txHash =>
                    {
                        blockHeightFromTxHash.TryAdd(txHash, 
                            await _neoscanService.GetTransactionAsync(txHash.ToString().Substring(2)));
                    });

                var validatedUnspentOutputs = unspentOutputs
                    .Where(p =>
                    {
                        var tx = blockHeightFromTxHash[p.Reference.PrevHash];

                        if (tx.BlockHash == null) // unconfirmed tx
                        {
                            return false;
                        }

                        return tx.BlockHeight <= lastBlock;
                    });

                var balance = (decimal) validatedUnspentOutputs
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
            finally
            {
                await _walletBalanceRepository.DeleteIfExist(address);
            }
        }

        public async Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation)
        {
            return await _walletBalanceRepository.GetBalances(take, continuation);
        }
    }
}
