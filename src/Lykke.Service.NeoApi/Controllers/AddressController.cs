using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NeoApi.Controllers
{
    public class AddressController:Controller
    {
        private readonly IAddressValidator _addressValidator;

        public AddressController(IAddressValidator addressValidator)
        {
            _addressValidator = addressValidator;
        }

        [HttpGet("api/addresses/{address}/validity")]
        public AddressValidationResponse Validate(string address)
        {
            return new AddressValidationResponse
            {
                IsValid = _addressValidator.IsAddressValid(address)
            };
        }
    }
}
