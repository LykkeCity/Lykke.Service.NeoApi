using System;
using Lykke.AzureStorage.Tables;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;

namespace Lykke.Service.NeoApi.AzureRepositories.Wallet
{
    internal class WalletBalanceEntity : AzureTableEntity, IWalletBalance
    {
        public string Address { get; set; }
        public decimal Balance { get; set; }
        public DateTime Updated { get; set; }
        public int UpdatedAtBlockHeight { get; set; }

        public static string GeneratePartitionKey(string address)
        {
            return address;
        }

        public static string GenerateRowKey()
        {
            return "";
        }

        public static WalletBalanceEntity Create(IWalletBalance source)
        {
            return new WalletBalanceEntity
            {
                Address = source.Address,
                Balance = source.Balance,
                RowKey = GenerateRowKey(),
                PartitionKey = GeneratePartitionKey(source.Address),
                Updated = source.Updated,
                UpdatedAtBlockHeight = source.UpdatedAtBlockHeight
            };
        }
    }
}
