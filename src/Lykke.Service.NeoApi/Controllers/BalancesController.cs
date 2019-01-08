using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.NeoApi.Domain;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Address.Exceptions;
using Lykke.Service.NeoApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.NeoApi.Controllers
{
    public class BalancesController:Controller
    {
        private readonly IAddressValidator _addressValidator;
        private readonly IWalletBalanceService _balanceService;

        public BalancesController(IAddressValidator addressValidator, IWalletBalanceService balanceService)
        {
            _addressValidator = addressValidator;
            _balanceService = balanceService;
        }

        [HttpPost("api/balances/{address}/observation")]
        public async Task<IActionResult> Subscribe(string address)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest(ErrorResponse.Create("Invalid address"));
            }

            try
            {
                await _balanceService.Subscribe(address);
            }
            catch (WalletAlreadyExistException)
            {

                return Conflict();
            }

            return Ok();
        }

        [HttpDelete("api/balances/{address}/observation")]
        [SwaggerOperation(nameof(Unsubscribe))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 204)]
        public async Task<IActionResult> Unsubscribe(string address)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest(ErrorResponse.Create("Invalid address"));
            }

            try
            {
                await _balanceService.Unsubscribe(address);
            }
            catch (WalletNotExistException)
            {

                return StatusCode((int)HttpStatusCode.NoContent);
            }

            return Ok();
        }
        
        [HttpGet("api/balances/")]
        [SwaggerOperation(nameof(GetBalances))]
        [ProducesResponseType(typeof(PaginationResponse<WalletBalanceContract>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetBalances([FromQuery] int take, [FromQuery] string continuation)
        {
            if (take <= 0)
            {
                ModelState.AddModelError(nameof(take), "Must be greater than zero");
            }

            ModelState.IsValidContinuationToken(continuation);


            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseFactory.Create(ModelState));
            }

            var padedResult = await _balanceService.GetBalances(take, continuation);

            return Ok(PaginationResponse.From(padedResult.Continuation, padedResult.Items.Select(p => new WalletBalanceContract
            {
                Address = p.Address,
                Balance = MoneyConversionHelper.ToContract(p.Balance),
                AssetId = Constants.Assets.Neo.AssetId,
                Block = p.UpdatedAtBlockHeight
            }).ToList().AsReadOnly()));
        }
    }
}
