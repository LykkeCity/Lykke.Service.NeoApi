using System.Collections.Generic;
using System.Linq;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Service.NeoApi.Domain;
using Lykke.Service.NeoApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NeoApi.Controllers
{
    public class AssetsController: Controller
    {
        private static readonly IReadOnlyList<AssetResponse> Storage = new List<AssetResponse>
        {
            new AssetResponse
            {
                AssetId = Constants.Assets.Neo.AssetId,
                Accuracy = Constants.Assets.Neo.Accuracy, 
                Name = Constants.Assets.Neo.Name,
                Address = null
            },
            new AssetResponse
            {
                AssetId = Constants.Assets.Gas.AssetId,
                Accuracy = Constants.Assets.Gas.Accuracy,
                Name = Constants.Assets.Gas.Name,
                Address = null
            },
        };

        [HttpGet("api/assets")]
        public IActionResult GetPaged([FromQuery] int take, [FromQuery] string continuation)
        {
            if (!ModelState.IsValid || 
                !ModelState.IsValidContinuationToken(continuation) || 
                !ModelState.IsValidTakeParameter(take))
            {
                return BadRequest(ErrorResponseFactory.Create(ModelState));
            }

            return Ok(PaginationResponse.From(null, Storage.Take(take).ToList()));
        }

        [HttpGet("api/assets/{assetId}")]
        public IActionResult GetById(string assetId)
        {
            var asset = Storage.FirstOrDefault(p => p.AssetId == assetId);
            if (asset == null)
            {
                return NoContent();
            }

            return Ok(asset);
        }
    }
}
