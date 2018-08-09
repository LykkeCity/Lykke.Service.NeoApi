using System.ComponentModel.DataAnnotations;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NEO.Api.Controllers
{
    [Route("api/[controller]")]
    public class KeysController : Controller
    {
        private readonly INeoService _neoService;

        public KeysController(INeoService neoService)
        {
            _neoService = neoService;
        }
        
        [HttpGet]
        public PaginationResponse<Keys> Get([Required] string password, [Required]  string name)
        {
            var publicKeys = _neoService.ListPublicKeys(password: password, name: name);

            return PaginationResponse.From("", publicKeys);
        }
    }
}
//}
