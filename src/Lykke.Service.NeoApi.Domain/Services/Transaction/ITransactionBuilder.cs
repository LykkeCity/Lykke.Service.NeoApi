﻿using System.Threading.Tasks;

namespace Lykke.Service.NeoApi.Domain.Services.Transaction
{
    public interface ITransactionBuilder
    {
        Task<(NeoModules.NEP6.Transactions.Transaction tx, decimal fee)> BuildNeoContractTransactionAsync(string from, string to, decimal amount, bool includeFee);
        Task<NeoModules.NEP6.Transactions.Transaction> BuildGasTransactionAsync(string from, string to, decimal amount);
        Task<(NeoModules.NEP6.Transactions.ClaimTransaction tx, decimal availiableGas, decimal unclaimedGas)> BuildClaimTransactions(string address);
    }
}
