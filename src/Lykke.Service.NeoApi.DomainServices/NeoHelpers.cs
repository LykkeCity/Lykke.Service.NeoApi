using System.Collections.Generic;
using System.Text;
using NeoModules.NEP6.Helpers;

namespace Lykke.Service.NeoApi.DomainServices
{
    public static class NeoHelpers
    {
        public static string ToHexString(this IEnumerable<byte> value)
        {
            var sb = new StringBuilder();
            foreach (byte b in value)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        public static string ToHexString(this NeoModules.NEP6.Transactions.Transaction transaction)
        {
            return Utils.GetHashData(transaction).ToHexString();
        }
    }
}
