using System;

namespace Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto
{
    public class WalletBalance:IWalletBalance
    {
        public string Address { get; set; }
        public string AssetId { get; set; }
        public decimal Balance { get; set; }
        public DateTime Updated { get; set; }
        public int UpdatedAtBlockHeight { get; set; }
        public static WalletBalance Create(string address, decimal balance, string assetId, int updatedAtBlock, DateTime? updated = null)
        {
            return new WalletBalance
            {
                Address = address,
                Balance = balance,
                Updated = updated ?? DateTime.UtcNow,
                UpdatedAtBlockHeight = updatedAtBlock,
                AssetId = assetId
            };
        }
    }
}
