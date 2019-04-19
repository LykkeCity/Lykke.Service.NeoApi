using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.DomainServices.Blockchain.Contracts
{
    public class GetClaimableResponse
    {
        [JsonProperty("unclaimed")]
        public decimal Unclaimed { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("claimable")]
        public IEnumerable<ClaimableElement> Claimable { get; set; }

        public class ClaimableElement
        {
            [JsonProperty("value")]
            public decimal Value { get; set; }

            [JsonProperty("unclaimed")]
            public float Unclaimed { get; set; }

            [JsonProperty("txid")]
            public string Txid { get; set; }

            [JsonProperty("sys_fee")]
            public double SysFee { get; set; }

            [JsonProperty("start_height")]
            public int StartHeight { get; set; }

            [JsonProperty("n")]
            public uint N { get; set; }

            [JsonProperty("generated")]
            public double Generated { get; set; }

            [JsonProperty("end_height")]
            public int EndHeight { get; set; }
        }
    }
}
