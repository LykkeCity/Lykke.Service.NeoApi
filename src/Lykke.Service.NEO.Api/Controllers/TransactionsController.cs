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
    public class TransactionsController : Controller
    {
        private readonly INeoService _neoService;

        public TransactionsController(INeoService neoService)
        {
            _neoService = neoService;
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
