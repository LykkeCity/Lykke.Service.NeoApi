using System;
using System.Threading.Tasks;
using Flurl.Http;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.NeoApi.Contracts;

namespace Lykke.Service.NeoApi.Client
{
    public class NeoClaimBuilderClient: INeoClaimBuilderClient
    {
        private readonly string _neoApiBaseUrl;
        private const int GasAssetDivisibility = 8;

        public NeoClaimBuilderClient(string neoApiBaseUrl)
        {
            _neoApiBaseUrl = neoApiBaseUrl;
        }

        public async Task<(decimal claimedGas, decimal allGas, string transactionContext)> BuildClaimTransacionAsync(Guid operationId, string address)
        {
            var resp = await _neoApiBaseUrl.PostJsonAsync(new BuildClaimTransactionRequest
            {
                Address = address,
                OperationId = operationId
            }).ReceiveJson<BuildedClaimTransactionResponse>();

            return (claimedGas: Conversions.CoinsFromContract(resp.ClaimedGas, GasAssetDivisibility),
                allGas: Conversions.CoinsFromContract(resp.AllGas, GasAssetDivisibility),
                transactionContext: resp.TransactionContext);
        }
    }
}
