using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.Helpers
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

        public static bool IsValidContinuationToken(this ModelStateDictionary self, string continuation)
        {
            if (!string.IsNullOrEmpty(continuation))
            {
                try
                {
                    JsonConvert.DeserializeObject<TableContinuationToken>(CommonUtils.HexToString(continuation));
                }
                catch
                {
                    self.AddModelError(nameof(continuation), "Invalid continuation token");

                    return false;
                }
            }

            return true;
        }


        public static bool IsValidTakeParameter(this ModelStateDictionary self, int take)
        {
            if (take <= 0)
            {
                self.AddModelError(nameof(take), "Must be greater than zero");

                return false;
            }

            return true;
        }
    }
}
