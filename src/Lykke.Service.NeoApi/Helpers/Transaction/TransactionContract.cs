using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.NeoApi.Helpers.Transaction.Exceptions;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Helpers.Transaction
{
    public class TransactionContract
    {
        public IEnumerable<CoinReferenceContract> Inputs { get; set; } = new CoinReferenceContract[0];
        public IEnumerable<TransferOutputContract> Outputs { get; set; } = new TransferOutputContract[0];
        public IEnumerable<WittnessContract> Wittnesses { get; set; } = new WittnessContract[0];
        public IEnumerable<CoinReferenceContract> Claims { get; set; } = new CoinReferenceContract[0];


        public NeoModules.NEP6.Transactions.Transaction ToDomain(TransactionType type)
        {
            switch (type)
            {
                case TransactionType.ContractTransaction:
                    return new ContractTransaction
                    {
                        Attributes = new TransactionAttribute[0],
                        Inputs = Inputs.Select(p => p.ToDomain()).ToArray(),
                        Outputs = Outputs.Select(p => p.ToDomain().ToTxOutput()).ToArray(),
                        Witnesses = Wittnesses.Select(p => p.ToDomain()).ToArray()
                    };
                case TransactionType.ClaimTransaction:
                    return new ClaimTransaction()
                    {
                        Attributes = new TransactionAttribute[0],
                        Inputs = Inputs.Select(p => p.ToDomain()).ToArray(),
                        Outputs = Outputs.Select(p => p.ToDomain().ToTxOutput()).ToArray(),
                        Witnesses = Wittnesses.Select(p => p.ToDomain()).ToArray(),
                        Claims = Claims.Select(p => p.ToDomain()).ToArray(),
                    };
                default:
                    throw new InvalidTransactionException($"Unknown switch: {type}");
            }

        }

        public static TransactionContract FromDomain(NeoModules.NEP6.Transactions.Transaction source)
        {
            if (source is ContractTransaction)
            {
                return new TransactionContract
                {
                    Inputs = source.Inputs.Select(CoinReferenceContract.FromDomain),
                    Outputs = source.Outputs.Select(TransferOutputContract.FromDomain),
                    Wittnesses = source.Witnesses.Select(WittnessContract.FromDomain),
                };
            }
            else if (source is ClaimTransaction)
            {
                var claim = (ClaimTransaction) source;
                return new TransactionContract
                {
                    Inputs = claim.Inputs.Select(CoinReferenceContract.FromDomain),
                    Outputs = claim.Outputs.Select(TransferOutputContract.FromDomain),
                    Wittnesses = claim.Witnesses.Select(WittnessContract.FromDomain),
                    Claims = claim.Claims.Select(CoinReferenceContract.FromDomain)
                };
            }
            else
            {
                throw new ArgumentException("Unknown transaction type", nameof(source));
            }
        }
    }
}
