using System.Collections.Generic;
using System.Threading.Tasks;
using NeoModules.Core;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Domain.Services.Blockchain
{
    public interface IBlockchainProvider
    {
        Task<int> GetHeightAsync();
        Task<IEnumerable<Coin>> GetUnspentAsync(string address);
        Task<(string txHash, int blockHeight, string blockHash)?> GetTransactionOrDefaultAsync(string txHash);
    }
}
