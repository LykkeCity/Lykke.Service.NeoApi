using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using Flurl;
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
            var resp = await _neoApiBaseUrl
                .AllowHttpStatus(HttpStatusCode.Conflict)
                .AppendPathSegment("/api/transactions/claim").PostJsonAsync(new BuildClaimTransactionRequest
            {
                Address = address,
                OperationId = operationId
            });

            switch (resp.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var buildedTx = (await resp.Content.ReadAsStringAsync())
                        .DeserializeJson<BuiltClaimTransactionResponse>();

                    return (claimedGas: Conversions.CoinsFromContract(buildedTx.ClaimedGas, GasAssetDivisibility),
                        allGas: Conversions.CoinsFromContract(buildedTx.AllGas, GasAssetDivisibility),
                        transactionContext: buildedTx.TransactionContext);
                }
                case HttpStatusCode.Accepted:
                {
                    throw new NeoClaimTransactionException(
                        NeoClaimTransactionException.ErrorCode.ClaimableGasNotAvailiable,
                        $"Resp: {await resp.Content.ReadAsStringAsync()}");
                }

                case HttpStatusCode.Conflict:
                {
                    throw new NeoClaimTransactionException(
                        NeoClaimTransactionException.ErrorCode.TransactionAlreadyBroadcased);
                }

                default:
                     throw new NeoClaimUnknownResponseException(
                         $"Unknown response {resp.StatusCode} : {await resp.Content.ReadAsStringAsync()}");
            }
        }
    }
}
