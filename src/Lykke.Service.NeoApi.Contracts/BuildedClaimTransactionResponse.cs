using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.Contracts
{
    public class BuildedClaimTransactionResponse
    {
        //availiable gas, unclaimed gas reference
        //https://docs.neo.org/en-us/faq.html -> What is GAS？How to acquire GAS？
        //https://github.com/PeterLinX/Introduction-to-Neo/blob/master/en/Neo%20Gas.md

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
