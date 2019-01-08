using System;
using Common;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Helpers.Transaction
{
    public class WittnessContract
    {
        public string InvocationScript { get; set; }
        public string VerificationScript { get; set; }

        public Witness ToDomain()
        {
            return new Witness
            {
                InvocationScript = !string.IsNullOrEmpty(InvocationScript) ? Convert.FromBase64String(InvocationScript): null,
                VerificationScript = !string.IsNullOrEmpty(VerificationScript) ? Convert.FromBase64String(VerificationScript) : null
            };
        }

        public static WittnessContract FromDomain(Witness source)
        {
            return new WittnessContract
            {
                InvocationScript = source.InvocationScript?.ToBase64(),
                VerificationScript = source.VerificationScript?.ToBase64()
            };
        }
    }
}
