using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.NeoApi.Domain.Helpers;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;

namespace Lykke.Service.NeoApi.AzureRepositories.Wallet
{
    internal class WalletBalanceRepository: IWalletBalanceRepository
    {
        private readonly INoSQLTableStorage<WalletBalanceEntity> _storage;

        public WalletBalanceRepository(INoSQLTableStorage<WalletBalanceEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertOrReplace(IWalletBalance balance)
        {
            return _storage.InsertOrReplaceAsync(WalletBalanceEntity.Create(balance));
        }

        public Task DeleteIfExist(string address, string assetId)
        {
            return _storage.DeleteIfExistAsync(WalletBalanceEntity.GeneratePartitionKey(address),
                WalletBalanceEntity.GenerateRowKey(assetId));
        }

        public async Task DeleteIfExist(string address)
        {
            var entites = await _storage.GetDataAsync(WalletBalanceEntity.GeneratePartitionKey(address));

            await entites.ForEachAsyncSemaphore(2, p => DeleteIfExist(p.Address, p.AssetId));
        }

        public async Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation)
        {
            var result = await _storage.GetDataWithContinuationTokenAsync(take, continuation);

            return PaginationResult<IWalletBalance>.Create(result.Entities, result.ContinuationToken);
        }
    }
}
