using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Services.Transaction;
using NeoModules.Core;
using NeoModules.NEP6.Transactions;
using NeoModules.Core.KeyPair;
using NeoModules.NEP6.Helpers;
using NeoModules.Rest.Interfaces;
using Utils = NeoModules.NEP6.Helpers.Utils;

namespace Lykke.Service.NeoApi.DomainServices.Transaction
{
    internal class TransactionBuilder:ITransactionBuilder
    {
        private readonly INeoscanService _restService;

        public TransactionBuilder(INeoscanService restService)
        {
            _restService = restService;
        }

        public Task<string> BuildNeoContractTransactionAsync(string from, string to, decimal amount, bool includeFee, decimal fixedFee)
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
                Outputs = new List<TransferOutput>()
                {
                    new TransferOutput
                    {
                        AssetId = Utils.NeoToken,
                        ScriptHash = to.ToScriptHash(),
                        Value = new BigDecimal(new BigInteger(amount), 8)
                    }
                }.Select(p => p.ToTxOutput()).ToArray(),
                Witnesses = new Witness[0]
            };

            tx = MakeTransaction(tx, from.ToScriptHash(), changeAddress: from.ToScriptHash(), fee: Fixed8.FromDecimal(fixedFee));

            return Task.FromResult(tx.ToHexString());
        }

        //used code from https://github.com/CityOfZion/NeoModules/blob/master/src/NeoModules.NEP6/AccountSignerTransactionManager.cs 
        private T MakeTransaction<T>(T tx, UInt160 from = null, UInt160 changeAddress = null, Fixed8 fee = default(Fixed8)) where T : NeoModules.NEP6.Transactions.Transaction
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

            var payCoins = payTotal.Select(async p => new
            {
                AssetId = p.Key,
                Unspents = await TransactionBuilderHelper.FindUnspentCoins(p.Key, p.Value.Value, from, _restService)
            }).Select(x => x.Result).ToDictionary(p => p.AssetId);

            if (payCoins.Any(p => p.Value.Unspents == null)) return null;

            var inputSum = payCoins.Values.ToDictionary(p => p.AssetId, p => new
            {
                p.AssetId,
                Value = p.Unspents.Sum(q => q.Output.Value)
            });
            if (changeAddress == null) changeAddress = from; //GetChangeAddress();
            List<TransactionOutput> outputsNew = new List<TransactionOutput>(tx.Outputs);
            foreach (UInt256 assetId in inputSum.Keys)
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
    }
}
