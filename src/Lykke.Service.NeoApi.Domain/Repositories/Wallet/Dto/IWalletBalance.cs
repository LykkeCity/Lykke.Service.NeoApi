using System;

namespace Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto
{
    public interface IWalletBalance
    {
        string Address { get; }
        string AssetId { get; }
        decimal Balance { get; }
        DateTime Updated { get; }

        int UpdatedAtBlockHeight { get; }
    }
}
