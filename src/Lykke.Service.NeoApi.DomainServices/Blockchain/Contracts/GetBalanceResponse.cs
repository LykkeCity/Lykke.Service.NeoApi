using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.DomainServices.Blockchain.Contracts
{
    public class GetBalanceResponse
    {
        [JsonProperty("balance")]
        public IEnumerable<Balance> Balances { get; set; }

        public class Balance
        {
            [JsonProperty("unspent")]
            public IList<Unspent> Unspents { get; set; }

            [JsonProperty("asset_symbol")]
            public string AssetSymbol { get; set; }

            [JsonProperty("asset_hash")]
            public string AssetHash { get; set; }

            [JsonProperty("asset")]
            public string Asset { get; set; }

            [JsonProperty("amount")]
            public float Amount { get; set; }
        }

        public class Unspent
        {
            [JsonProperty("value")]
            public double Value { get; set; }

            [JsonProperty("txid")]
            public string TxId { get; set; }

            [JsonProperty("n")]
            public uint N { get; set; }
        }
    }
}
