using System.Collections.Generic;
using System.Linq;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Helpers.Transaction
{
    public class TransactionContract
    {
        public IEnumerable<CoinReferenceContract> Inputs { get; set; } = new CoinReferenceContract[0];
        public IEnumerable<TransferOutputContract> Outputs { get; set; } = new TransferOutputContract[0];
        public IEnumerable<WittnessContract> Wittnesses { get; set; } = new WittnessContract[0];


        public NeoModules.NEP6.Transactions.Transaction ToDomain()
        {
            return new ContractTransaction
            {
                Attributes = new TransactionAttribute[0],
                Inputs = Inputs.Select(p => p.ToDomain()).ToArray(),
                Outputs = Outputs.Select(p => p.ToDomain().ToTxOutput()).ToArray(),
                Witnesses = Wittnesses.Select(p => p.ToDomain()).ToArray()
            };
        }

        public static TransactionContract FromDomain(NeoModules.NEP6.Transactions.Transaction source)
        {
            return new TransactionContract
            {
                Inputs = source.Inputs.Select(CoinReferenceContract.FromDomain),
                Outputs = source.Outputs.Select(TransferOutputContract.FromDomain),
                Wittnesses = source.Witnesses.Select(WittnessContract.FromDomain),
            };
        }
    }
}
