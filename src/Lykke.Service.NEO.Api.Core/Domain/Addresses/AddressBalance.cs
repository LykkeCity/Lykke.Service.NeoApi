namespace Lykke.Service.NEO.Api.Core.Domain.Addresses
{
    public class AddressBalance
    {
        public string Address { get; set; } 
        public decimal Balance { get; set; }
        public long BlockTime { get; set; }
    }
}
