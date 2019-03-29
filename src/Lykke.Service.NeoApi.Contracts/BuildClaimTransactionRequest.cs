using System;
using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.Contracts
{
    public class BuildClaimTransactionRequest
    {
        [JsonProperty("operationId")]
        public Guid OperationId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
