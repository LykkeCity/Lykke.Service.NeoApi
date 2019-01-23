using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.DomainServices.Blockchain.Contracts
{
    public class GetHeightResponse
    {
        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
