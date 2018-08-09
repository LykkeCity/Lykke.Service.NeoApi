using System.ComponentModel.DataAnnotations;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NEO.Api.Controllers
{
    [Route("api/[controller]")]
    public class AssetController : Controller
    {
        private readonly INeoService _neoService;

        public AssetController(INeoService neoService)
        {
            _neoService = neoService;
        }

        //[HttpGet("{assetId}")]
        //public AssetResponse GetAsset(string assetId)
        //{
        //    return Constants.Assets.TryGetValue(assetId, out var asset) ? asset.ToAssetResponse() : null;
        //}

        [HttpGet]
        public PaginationResponse<Asset> Get([Required] string password, [Required]  string name)
        {
            var assets = _neoService.ListAssets(password: password, name: name);

            return PaginationResponse.From("", assets);
        }
    }
}
//}
