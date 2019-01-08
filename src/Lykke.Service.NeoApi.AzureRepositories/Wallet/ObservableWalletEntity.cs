using Lykke.AzureStorage.Tables;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;

namespace Lykke.Service.NeoApi.AzureRepositories.Wallet
{
    internal class ObservableWalletEntity : AzureTableEntity, IObservableWallet
    {
        public string Address { get; set; }

        public static string GeneratePartitionKey(string address)
        {
            return address;
        }

        public static string GenerateRowKey()
        {
            return "";
        }

        public static ObservableWalletEntity Create(IObservableWallet source)
        {
            return new ObservableWalletEntity
            {
                Address = source.Address,
                PartitionKey = GeneratePartitionKey(source.Address),
                RowKey = GenerateRowKey()
            };
        }
    }
}
