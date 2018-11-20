using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.NeoApi.Domain;

namespace Lykke.Service.NeoApi.Helpers
{
    public class MoneyConversionHelper
    {
        public static string ToContract(decimal amount)
        {
            return Conversions.CoinsToContract(amount, Constants.Assets.Neo.Accuracy);
        }

        public static decimal FromContract(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            return Conversions.CoinsFromContract(input, Constants.Assets.Neo.Accuracy);
        }
    }
}
