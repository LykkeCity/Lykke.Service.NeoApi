using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.DomainServices.Blockchain.Contracts
{
    public class GetUnclaimedResponse
    {
        [JsonProperty("unclaimed")]
        public decimal Unclaimed { get; set; }
    }
}
