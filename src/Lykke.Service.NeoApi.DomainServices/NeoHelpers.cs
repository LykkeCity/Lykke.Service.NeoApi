using NeoModules.Core;
using NeoModules.Core.NVM;
using NeoModules.NEP6.Helpers;

namespace Lykke.Service.NeoApi.DomainServices
{
    public static class NeoHelpers
    {
        public static string ToHexString(this NeoModules.NEP6.Transactions.Transaction transaction)
        {
            return transaction.ToArray().ToHexString();
        }
    }
}
