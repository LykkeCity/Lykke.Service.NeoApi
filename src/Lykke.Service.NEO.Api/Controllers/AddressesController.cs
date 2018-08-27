using System.ComponentModel.DataAnnotations;
using System.Net;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NEO.Api.Controllers
{
    [Route("api/[controller]")]
    public class AddressesController : Controller
    {
        private readonly INeoService _neoService;

        public AddressesController(INeoService neoService)
        {
            _neoService = neoService;
        }
        
        [HttpGet("{address}/validity")]
        [ProducesResponseType(typeof(AddressValidationResponse), (int)HttpStatusCode.OK)]
        public IActionResult GetAddressValidity([FromRoute] string address)
        {
            return Ok(new AddressValidationResponse()
            {
                IsValid = _neoService.IsValidAddress(address: address)
            });
        }

        [HttpGet]
        public PaginationResponse<Address> Get([Required] string password, [Required] string name)
        {
            var addresses = _neoService.ListAddress(password: password, name: name);

            return PaginationResponse.From("", addresses);
        }
    }
}
//}
