using System;
using System.Threading.Tasks;

namespace Lykke.Service.NeoApi.Client
{
    public interface INeoClaimBuilderClient
    {
        //availiable gas, unclaimed gas reference
        //https://docs.neo.org/en-us/faq.html -> What is GAS？How to acquire GAS？
        //https://github.com/PeterLinX/Introduction-to-Neo/blob/master/en/Neo%20Gas.md
        Task<(decimal claimedGas, decimal allGas, string transactionContext)> BuildClaimTransacionAsync(Guid operationId, string address);
    }
}
