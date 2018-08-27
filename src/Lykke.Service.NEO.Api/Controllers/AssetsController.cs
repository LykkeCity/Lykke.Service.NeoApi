using System.ComponentModel.DataAnnotations;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NEO.Api.Controllers
{
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly INeoService _neoService;

        public AssetsController(INeoService neoService)
        {
            _neoService = neoService;
        }


        //[HttpGet("{assetId}")]
        //public AssetResponse GetAsset(string assetId)
        //{
        //    return Constants.Assets.TryGetValue(assetId, out var asset) ? asset.ToAssetResponse() : null;
        //}

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<Asset>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public PaginationResponse<Asset> Get([Required] string password, [Required]  string name)
        {
            var assets = _neoService.ListAssets(password: password, name: name);

            return PaginationResponse.From("", assets);
        }
    }
}
//}
