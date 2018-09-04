using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.Service.NEO.Api.Helper
{
    public static class ModelStateExtensions
    {
        public static bool IsValidAddress(this ModelStateDictionary self, INeoService neoService, string address)
        {
            if (neoService.ValidateAddressAsync(address))
            {
                return true;
            }
            else
            {
                self.AddModelError(nameof(address), $"{address} is not a valid NEO transparent (t-) address");
                return false;
            }
        }

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

        public static BlockchainErrorResponse ToBlockchainErrorResponse(this ModelStateDictionary self)
        {
            var response = new BlockchainErrorResponse
            {
                ModelErrors = new Dictionary<string, List<string>>(),
                ErrorCode = BlockchainErrorCode.Unknown
            };

            foreach (var state in self)
            {
                var messages = state.Value.Errors
                    .Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                    .Select(e => e.ErrorMessage)
                    .Concat(state.Value.Errors
                        .Where(e => string.IsNullOrWhiteSpace(e.ErrorMessage))
                        .Select(e => e.Exception.Message))
                    .ToList();

                if (messages.Any())
                {
                    response.ModelErrors.Add(state.Key, messages);
                }
            }

            return response;
        }

    }
}
