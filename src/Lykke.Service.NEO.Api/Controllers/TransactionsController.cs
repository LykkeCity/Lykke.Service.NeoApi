using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Domain.Addresses;
using Lykke.Service.NEO.Api.Core.Domain.Operations;
using Lykke.Service.NEO.Api.Core.Settings;
using Lykke.Service.NEO.Api.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NEO.Api.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly INeoService _neoService;

        public TransactionsController(INeoService neoService)
        {
            _neoService = neoService;
        }

        private async Task<IActionResult> Get<TResponse>(Guid operationId, Func<IOperation, TResponse> toResponse)
        {
            if (!ModelState.IsValid ||
                !ModelState.IsValidOperationId(operationId))
            {
                return BadRequest(ModelState.ToBlockchainErrorResponse());
            }

            var operation = await _neoService.GetOperationAsync(operationId);
            if (operation != null && operation.State != OperationState.Built && operation.State != OperationState.Deleted)
                return Ok(toResponse(operation));
            else
                return NoContent();
        }

        private async Task<IActionResult> Observe(string address, HistoryAddressCategory category)
        {
            if (!ModelState.IsValid ||
                !ModelState.IsValidAddress(_neoService, address))
            {
                return BadRequest(ModelState.ToBlockchainErrorResponse());
            }

            if (await _neoService.TryCreateHistoryAddressAsync(address, category))
                return Ok();
            else
                return StatusCode(StatusCodes.Status409Conflict);
        }

        private async Task<IActionResult> DeleteObservation(string address, HistoryAddressCategory category)
        {
            if (!ModelState.IsValid ||
                !ModelState.IsValidAddress(_neoService, address))
            {
                return BadRequest(ModelState.ToBlockchainErrorResponse());
            }

            if (await _neoService.TryDeleteHistoryAddressAsync(address, category))
                return Ok();
            else
                return NoContent();
        }

        private async Task<IActionResult> GetHistory(string address, string afterHash, int take, HistoryAddressCategory category)
        {
            if (take <= 0)
            {
                ModelState.AddModelError(nameof(take), "Must be greater than zero");
            }

            if (!ModelState.IsValid ||
                !ModelState.IsValidAddress(_neoService, address))
            {
                return BadRequest(ModelState.ToBlockchainErrorResponse());
            }

            var txs = await _neoService.GetHistoryAsync(category, address, afterHash, take);
            return Ok();
            //return Ok(txs
            //    .Select(tx => tx.ToHistoricalContract()) //TODO:
            //    .ToArray());
        }

        //[HttpPost("broadcast")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BlockchainErrorResponse))]
        //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        //[ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorResponse))]
        //public async Task<IActionResult> Broadcast([FromBody]BroadcastTransactionRequest request)
        //{
        //    if (!ModelState.IsValid ||
        //        !ModelState.IsValidRequest(request, _neoService))
        //    {
        //        return BadRequest(ModelState.ToBlockchainErrorResponse());
        //    }

        //    var operation = await _neoService.GetOperationAsync(request.OperationId, false);

        //    if (operation == null)
        //    {
        //        return StatusCode(StatusCodes.Status404NotFound,
        //            ErrorResponse.Create("Transaction must be built beforehand by NEO API (to be successfully broadcasted then)"));
        //    }

        //    if (operation.State != OperationState.Built)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict,
        //            ErrorResponse.Create($"Operation is already {Enum.GetName(typeof(OperationState), operation.State).ToLower()}"));
        //    }

        //    await _neoService.BroadcastAsync(operation.OperationId, request.SignedTransaction);

        //    return Ok();
        //}
    }
}
//}
