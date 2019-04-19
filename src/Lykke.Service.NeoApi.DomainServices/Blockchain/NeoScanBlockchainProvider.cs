using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Lykke.Service.NeoApi.Domain.Services.Blockchain;
using Lykke.Service.NeoApi.Domain.Services.Blockchain.Exceptions;
using Lykke.Service.NeoApi.DomainServices.Blockchain.Contracts;
using NeoModules.Core;
using NeoModules.Core.KeyPair;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.DomainServices.Blockchain
{
    public class NeoScanBlockchainProvider : IBlockchainProvider
    {
        private readonly IFlurlClient _flurlClient;

        public NeoScanBlockchainProvider(IFlurlClient flurlClient)
        {
            _flurlClient = flurlClient;
        }

        public async Task<int> GetHeightAsync()
        {
            return (await GetJson<GetHeightResponse>("/get_height")).Height;
        }

        public async Task<IEnumerable<Coin>> GetUnspentAsync(string address)
        {
            var addressBalance = await GetJson<GetBalanceResponse>($"/get_balance/{address}");

            //from https://github.com/CityOfZion/NeoModules/blob/master/src/NeoModules.NEP6/Helpers/TransactionBuilderHelper.cs#L24
            var coinList = new List<Coin>();
            if (addressBalance.Balances != null)
            {
                coinList.AddRange(from balanceEntry in addressBalance.Balances
                    let child = balanceEntry.Unspents
                    where child?.Count > 0
                    from unspent in balanceEntry.Unspents
                    select new Coin
                    {
                        Output = new TransactionOutput
                        {
                            AssetId = UInt256.Parse(balanceEntry.AssetHash),
                            ScriptHash = address.ToScriptHash(),
                            Value = Fixed8.FromDecimal((decimal)unspent.Value),
                        },
                        Reference = new CoinReference
                        {
                            PrevHash = UInt256.Parse(unspent.TxId),
                            PrevIndex = (ushort)unspent.N,
                        }
                    });
            }

            return coinList;
        }

        public async Task<(string txHash, int blockHeight, string blockHash)?> GetTransactionOrDefaultAsync(string txHash)
        {
            GetTransactionResponse resp;

            try
            {
                resp = await GetJson<GetTransactionResponse>($"/get_transaction/{txHash}");
            }
            catch (FlurlHttpException e) 
                when(e.Call.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return (resp.TxHash, resp.BlockHeight, resp.BlockHash);
        }

        public async Task<(decimal gasAmoumt, IEnumerable<CoinReference> coinReferences)> GetClaimableAsync(string address)
        {
            var resp = await GetJson<GetClaimableResponse>($"/get_claimable/{address}");

            return (resp.Unclaimed, resp.Claimable.Select(p => new CoinReference
            {
                PrevHash = UInt256.Parse(p.Txid),
                PrevIndex = (ushort) p.N
            }).ToList());
        }

        public async Task<decimal> GetUnclaimedAsync(string address)
        {
            var resp = await GetJson<GetUnclaimedResponse>($"/get_unclaimed/{address}");
            return resp.Unclaimed;
        }

        private async Task<T> GetJson<T>(string segment)
        {
            try
            {
                return await _flurlClient.Request(segment).GetJsonAsync<T>();
            }
            catch (FlurlHttpException e)
            {
                var body = await (e.Call.Response?.Content?.ReadAsStringAsync() ?? Task.FromResult(""));
                throw new NeoScanException(e, body);
            }
        }
    }
}
