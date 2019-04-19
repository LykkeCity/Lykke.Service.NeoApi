using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.Contracts
{
    public class BuiltClaimTransactionResponse
    {
        /// <summary>
        /// Claimed gas during transaction
        /// </summary>
        [JsonProperty("claimedGas")]
        public string ClaimedGas { get; set; }

        /// <summary>
        /// All unclaimed gas (inclue claimed during transaction)
        /// </summary>
        [JsonProperty("allGas")]
        public string  AllGas { get; set; }

        /// <summary>
        /// Claim transaction context
        /// </summary>
        [JsonProperty("transactionContext")]
        public string TransactionContext { get; set; }
    }
}
