using System.Threading.Tasks;
using Lykke.Service.NEO.Api.Core.Settings;
using Lykke.Service.NEO.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NEO.Api.Controllers
{
    [Route("api/[controller]")]
    public class BalanceController : Controller
    {
        private readonly INeoService _neoService;

        public BalanceController(INeoService neoService)
        {
            _neoService = neoService;
        }

        [HttpPost("{address}/observation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromRoute]string address)
        {
            //if (!ModelState.IsValid ||
            //    !ModelState.IsValidAddress(address))
            //{
            //    return BadRequest(ErrorResponseFactory.Create(ModelState));
            //}
            return Ok();

            //if (await _neoService.TryCreateObservableAddressAsync(ObservationCategory.Balance, address))
            //    return Ok();
            //else
            //    return StatusCode(StatusCodes.Status409Conflict);
        }
    }
    }
//}
