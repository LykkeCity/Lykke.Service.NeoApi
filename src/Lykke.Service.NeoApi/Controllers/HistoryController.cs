using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NeoApi.Controllers
{
    public class HistoryController:Controller
    {
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
            return Ok();
        }

        [HttpDelete("/api/transactions/history/from/{address}/observation")]
        public IActionResult DeleteObservationFrom(
            [FromRoute]string address)
        {
            return Ok();
        }

        [HttpDelete("/api/transactions/history/to/{address}/observation")]
        public IActionResult DeleteObservationTo(
            [FromRoute]string address)
        {
            return Ok();
        }
        
        [HttpGet("/api/transactions/history/from/{address}")]
        public IActionResult GetHistoryFrom(
            [FromRoute]string address,
            [FromQuery]string afterHash,
            [FromQuery]int take)
        {
            return Ok();
        }

        [HttpGet("/api/transactions/history/to/{address}")]
        public IActionResult GetHistoryTo(
            [FromRoute]string address,
            [FromQuery]string afterHash,
            [FromQuery]int take)
        {
            return Ok();
        }
    }
}
