using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.DomainServices.Blockchain.Contracts
{
    public class GetTransactionResponse
    {
        [JsonProperty("txid")]
        public string TxHash { get; set; }

        [JsonProperty("block_height")]
        public int BlockHeight { get; set; }

        [JsonProperty("block_hash")]
        public string BlockHash { get; set; }
    }
}
