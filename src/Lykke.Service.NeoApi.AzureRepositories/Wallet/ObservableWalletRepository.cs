using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;

namespace Lykke.Service.NeoApi.AzureRepositories.Wallet
{
    internal class ObservableWalletRepository: IObservableWalletRepository
    {
        private readonly INoSQLTableStorage<ObservableWalletEntity> _storage;

        public ObservableWalletRepository(INoSQLTableStorage<ObservableWalletEntity> storage)
        {
            _storage = storage;
        }

        public async Task Insert(IObservableWallet wallet)
        {
            await _storage.InsertAsync(ObservableWalletEntity.Create(wallet));
        }

        public async Task<IEnumerable<IObservableWallet>> GetAll()
        {
            return await _storage.GetDataAsync();
        }

        public async Task<IPaginationResult<IObservableWallet>> GetPaged(int take, string continuation)
        {
            var result = await _storage.GetDataWithContinuationTokenAsync(take, continuation);

            return PaginationResult<IObservableWallet>.Create(result.Entities, result.ContinuationToken);
        }

        public async Task Delete(string address)
        {
            await _storage.DeleteAsync(ObservableWalletEntity.GeneratePartitionKey(address),
                ObservableWalletEntity.GenerateRowKey());
        }

        public async Task<IObservableWallet> Get(string address)
        {
            return await _storage.GetDataAsync(ObservableWalletEntity.GeneratePartitionKey(address), ObservableWalletEntity.GenerateRowKey());
        }
    }
}
