using System.Threading.Tasks;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Service.NEO.Api.Core.Settings;
using Lykke.Service.NEO.Api.Helper;
using Lykke.Service.NEO.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.Service.NEO.Api.Controllers
{
    [Route("api/[controller]")]
    public class BalancesController : Controller
    {
        private readonly INeoService _neoService;

        public BalancesController(INeoService neoService)
        {
            _neoService = neoService;
        }
              

        [HttpPost("{address}/observation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromRoute]string address)
        {
            if (!ModelState.IsValid ||
                !ModelState.IsValidAddress(_neoService, address))
            {
                return BadRequest(ModelState.ToBlockchainErrorResponse());
            }

            if (await _neoService.TryCreateBalanceAddressAsync(address))
                return Ok();
            else
                return StatusCode(StatusCodes.Status409Conflict);
        }

        [HttpDelete("{address}/observation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> Delete([FromRoute]string address)
        {
            if (!ModelState.IsValid ||
                !ModelState.IsValidAddress(_neoService, address))
            {
                return BadRequest(ModelState.ToBlockchainErrorResponse());
            }

            if (await _neoService.TryDeleteBalanceAddressAsync(address))
                return Ok();
            else
                return NoContent();
        }
    }
} 
