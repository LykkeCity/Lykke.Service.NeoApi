using System;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.NeoApi.Domain;

namespace Lykke.Service.NeoApi.Helpers
{
    public class MoneyConversionHelper
    {
        public static string ToContract(decimal amount, string assetId)
        {
            return Conversions.CoinsToContract(amount, GetAccuracy(assetId));
        }

        public static decimal FromContract(string input, string assetId)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            return Conversions.CoinsFromContract(input, GetAccuracy(assetId));
        }

        private static int GetAccuracy(string assetId)
        {
            switch (assetId)
            {
                case Constants.Assets.Neo.AssetId:
                    return Constants.Assets.Neo.Accuracy;
                case Constants.Assets.Gas.AssetId:
                    return Constants.Assets.Gas.Accuracy;
                default:
                    throw new ArgumentException("Unknown switch", assetId);
                   
            }
        }
    }
}
