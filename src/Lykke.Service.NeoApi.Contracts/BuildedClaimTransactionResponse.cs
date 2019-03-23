using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.Contracts
{
    public class BuildedClaimTransactionResponse
    {
        [JsonProperty("claimedGas")]
        public decimal ClaimedGas { get; set; }

        [JsonProperty("transactionContext")]
        public string TransactionContext { get; set; }
    }
}
