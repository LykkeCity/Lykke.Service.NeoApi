using System.Linq;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NeoApi.Controllers
{
    public class HistoryController:Controller
    {
        private readonly IAddressValidator _addressValidator;

        public HistoryController(IAddressValidator addressValidator)
        {
            _addressValidator = addressValidator;
        }

        [HttpPost("/api/transactions/history/from/{address}/observation")]
        public IActionResult ObserveFrom(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpPost("/api/transactions/history/to/{address}/observation")]
        public IActionResult ObserveTo(
            [FromRoute]string address)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest("Address is invalid");
            }

            return Ok();
        }

        [HttpDelete("/api/transactions/history/from/{address}/observation")]
        public IActionResult DeleteObservationFrom(
            [FromRoute]string address)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest("Address is invalid");
            }

            return Ok();
        }

        [HttpDelete("/api/transactions/history/to/{address}/observation")]
        public IActionResult DeleteObservationTo(
            [FromRoute]string address)
        {
            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest("Address is invalid");
            }

            return Ok();
        }
        
        [HttpGet("/api/transactions/history/from/{address}")]
        public IActionResult GetHistoryFrom(
            [FromRoute]string address,
            [FromQuery]string afterHash,
            [FromQuery]int take)
        {
            if (take <= 0)
            {
                return BadRequest(new ErrorResponse() { ErrorMessage = $"{nameof(take)} must be greater than zero" });
            }

            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest("Address is invalid");
            }

            return Ok(Enumerable.Empty<HistoricalTransactionContract>());
        }

        [HttpGet("/api/transactions/history/to/{address}")]
        public IActionResult GetHistoryTo(
            [FromRoute]string address,
            [FromQuery]string afterHash,
            [FromQuery]int take)
        {
            if (take <= 0)
            {
                return BadRequest(new ErrorResponse() { ErrorMessage = $"{nameof(take)} must be greater than zero" });
            }

            if (!_addressValidator.IsAddressValid(address))
            {
                return BadRequest("Address is invalid");
            }

            return Ok(Enumerable.Empty<HistoricalTransactionContract>());
        }
    }
}
