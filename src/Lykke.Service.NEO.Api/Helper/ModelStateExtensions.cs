using System;
using Common;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.NEO.Api.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.Service.NEO.Api.Helper
{
    public static class ModelStateExtensions
    {

        public static bool IsValidOperationId(this ModelStateDictionary self, Guid operationId)
        {
            if (operationId == Guid.Empty)
            {
                self.AddModelError(nameof(operationId), "Operation identifier must not be empty GUID");
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsValidContinuation(this ModelStateDictionary self, string continuation)
        {
            if (string.IsNullOrEmpty(continuation))
            {
                return true;
            }

            try
            {
                JsonConvert.DeserializeObject<TableContinuationToken>(Core.Utils.HexToString(continuation));
                return true;
            }
            catch
            {
                self.AddModelError("continuation", "Invalid continuation token");
                return false;
            }
        }

    }
}
