using Lykke.Service.BlockchainApi.Contract.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.NeoApi.Controllers
{
    public class CapabilitiesController:Controller
    {
        [HttpGet("api/capabilities")]
        public CapabilitiesResponse GetCapabilities()
        {
            return new CapabilitiesResponse
            {
                AreManyInputsSupported = false,
                AreManyOutputsSupported = false,
                IsTransactionsRebuildingSupported = false,
                IsPublicAddressExtensionRequired = false,
                CanReturnExplorerUrl = false,
                IsTestingTransfersSupported = false,
                IsReceiveTransactionRequired = false
            };
        }
    }
}
