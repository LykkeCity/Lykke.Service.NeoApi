namespace Lykke.Service.NeoApi.Domain.Services.Address
{
    public interface IAddressValidator
    {
        bool IsAddressValid(string address);
    }
}
