using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain;
using Lykke.Service.NeoApi.Domain.Services.Transaction;
using Lykke.Service.NeoApi.Domain.Services.Transaction.Exceptions;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;
using NeoModules.Core;
using NeoModules.NEP6.Transactions;
using NeoModules.Core.KeyPair;
using NeoModules.NEP6.Helpers;
using Utils = NeoModules.NEP6.Helpers.Utils;

namespace Lykke.Service.NeoApi.DomainServices.Transaction
{
    internal class TransactionBuilder:ITransactionBuilder
    {
        private readonly ITransactionOutputsService _transactionOutputsService;

        public TransactionBuilder(ITransactionOutputsService transactionOutputsService)
        {
            _transactionOutputsService = transactionOutputsService;
        }

        public async Task<NeoModules.NEP6.Transactions.Transaction> BuildNeoContractTransactionAsync(string from, string to, decimal amount, bool includeFee, decimal fixedFee)
        {
            if (amount <= fixedFee)
            {
                throw new ArgumentException(
                    $"Passed transaction amount  is less than fixed fee. {amount} <= {fixedFee}", nameof(amount));
            }

            if (includeFee)
            {
                amount -= fixedFee;
            }

            var tx = new ContractTransaction
            {
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new List<TransferOutput>
                {
                    new TransferOutput
                    {
                        AssetId = Utils.NeoToken,
                        ScriptHash = to.ToScriptHash(),
                        Value =  BigDecimal.Parse(amount.ToString("F", CultureInfo.InvariantCulture), 
                            Constants.Assets.Neo.Accuracy)
                    }
                }.Select(p => p.ToTxOutput()).ToArray(),
                Witnesses = new Witness[0]
            };

            var unspentOutputs = await _transactionOutputsService.GetUnspentOutputsAsync(from);

            tx = MakeTransaction(tx, 
                unspentOutputs,
                from.ToScriptHash(), 
                changeAddress: from.ToScriptHash(), 
                fee: Fixed8.FromDecimal(fixedFee));

            return tx;
        }

        //based on  https://github.com/CityOfZion/NeoModules/blob/master/src/NeoModules.NEP6/AccountSignerTransactionManager.cs 
        private T MakeTransaction<T>(T tx, 
            IEnumerable<Coin> unspentOutputs,
            UInt160 from = null, 
            UInt160 changeAddress = null, 
            Fixed8 fee = default(Fixed8)) where T : NeoModules.NEP6.Transactions.Transaction
        {
            fee += tx.SystemFee;
            var payTotal = tx.Outputs.GroupBy(p => p.AssetId, (k, g) => new
            {
                AssetId = k,
                Value = g.Sum(p => p.Value)
            }).ToDictionary(p => p.AssetId);
            if (fee > Fixed8.Zero)
            {
                if (payTotal.ContainsKey(Utils.GasToken))
                {
                    payTotal[Utils.GasToken] = new
                    {
                        AssetId = Utils.GasToken,
                        Value = payTotal[Utils.GasToken].Value + fee
                    };
                }
                else
                {
                    payTotal.Add(Utils.GasToken, new
                    {
                        AssetId = Utils.GasToken,
                        Value = fee
                    });
                }
            }

            var payCoins = payTotal.Select(p => new
            {
                AssetId = p.Key,
                Unspents = FindUnspentCoins(unspentOutputs.ToArray(), p.Key, p.Value.Value)
            }).Select(x => x).ToDictionary(p => p.AssetId);

            var inputSum = payCoins.Values.ToDictionary(p => p.AssetId, p => new
            {
                p.AssetId,
                Value = p.Unspents.Sum(q => q.Output.Value)
            });
            if (changeAddress == null) changeAddress = from; //GetChangeAddress();
            var outputsNew = new List<TransactionOutput>(tx.Outputs);
            foreach (var assetId in inputSum.Keys)
            {
                if (inputSum[assetId].Value > payTotal[assetId].Value)
                {
                    outputsNew.Add(new TransactionOutput
                    {
                        AssetId = assetId,
                        Value = inputSum[assetId].Value - payTotal[assetId].Value,
                        ScriptHash = changeAddress
                    });
                }
            }
            tx.Inputs = payCoins.Values.SelectMany(p => p.Unspents).Select(p => p.Reference).ToArray();
            tx.Outputs = outputsNew.ToArray();
            return tx;
        }

        //based on https://github.com/neo-project/neo/blob/master/neo/Wallets/Wallet.cs#L71
        private static Coin[] FindUnspentCoins(IEnumerable<Coin> unspents, UInt256 assetId, Fixed8 amount)
        {
            var unspentsAsset = unspents.Where(p => p.Output.AssetId == assetId).ToArray();
            var sum = unspentsAsset.Sum(p => p.Output.Value);
            if (sum < amount)
            {
                throw new NotEnoughFundsException($"Not enough funds. Requested: {sum}, Available: {amount}");
            }
            if (sum == amount) return unspentsAsset;
            var unspentsOrdered = unspentsAsset.OrderByDescending(p => p.Output.Value).ToArray();
            var i = 0;
            while (unspentsOrdered[i].Output.Value <= amount)
                amount -= unspentsOrdered[i++].Output.Value;
            if (amount == Fixed8.Zero)
                return unspentsOrdered.Take(i).ToArray();
            else
                return unspentsOrdered.Take(i).Concat(new[] { unspentsOrdered.Last(p => p.Output.Value >= amount) }).ToArray();
        }
    }
}
