﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NeoApi.Domain;
using Lykke.Service.NeoApi.Domain.Helpers;
using Lykke.Service.NeoApi.Domain.Repositories.Pagination;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Address.Exceptions;
using Lykke.Service.NeoApi.Domain.Services.Blockchain;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;
using Microsoft.WindowsAzure.Storage;
using NeoModules.Core;
using NeoModules.NEP6.Helpers;

namespace Lykke.Service.NeoApi.DomainServices.Address
{
    public class WalletBalanceService:IWalletBalanceService
    {
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IWalletBalanceRepository _walletBalanceRepository;
        private readonly IBlockchainProvider _blockchainProvider;
        private readonly ITransactionOutputsService _transactionOutputsService;
        private readonly ILog _log;

        private const int EntityExistsHttpStatusCode = 409;
        private const int EntityNotExistsHttpStatusCode = 404;

        public WalletBalanceService(IObservableWalletRepository observableWalletRepository, 
            IWalletBalanceRepository walletBalanceRepository, 
            ITransactionOutputsService transactionOutputsService, 
            IBlockchainProvider blockchainProvider,
            ILogFactory logFactory)
        {
            _observableWalletRepository = observableWalletRepository;
            _walletBalanceRepository = walletBalanceRepository;
            _transactionOutputsService = transactionOutputsService;
            _blockchainProvider = blockchainProvider;
            _log = logFactory.CreateLog(this);
        }

        public async Task<decimal?> UpdateBalance(string address)
        {
            if(await _observableWalletRepository.Get(address) != null)
            {
                var lastBlock = await _blockchainProvider.GetHeightAsync();

                var unspentOutputs = (await _transactionOutputsService.GetUnspentOutputsAsync(address))
                    .ToList();

                var blockHeightFromTxHash = new ConcurrentDictionary<UInt256,
                    (string txHash, int blockHeight, string blockHash)>();

                await unspentOutputs.Select(p => p.Reference.PrevHash).Distinct()
                    .ForEachAsyncSemaphore(8, async txHash =>
                    {
                        var tx = await _blockchainProvider.GetTransactionOrDefaultAsync(txHash.ToString().Substring(2));

                        if (tx == null)
                        {
                            throw new InvalidOperationException($"Unable to find transaction with hash {txHash}");
                        }

                        blockHeightFromTxHash.TryAdd(txHash, tx.Value);
                    });

                var validatedUnspentOutputs = unspentOutputs
                    .Where(p =>
                    {
                        var tx = blockHeightFromTxHash[p.Reference.PrevHash];

                        if (tx.blockHash == null) // unconfirmed tx
                        {
                            return false;
                        }

                        return tx.blockHeight <= lastBlock;
                    }).ToList();


                var neoBalance = (decimal)validatedUnspentOutputs
                    .Where(p => p.Output.AssetId == Utils.NeoToken)
                    .Sum(p => p.Output.Value);
                
                var gasBalance = (decimal) validatedUnspentOutputs
                    .Where(p => p.Output.AssetId == Utils.GasToken)
                    .Sum(p => p.Output.Value);

                await Task.WhenAll(UpdateBalanceInRepo(lastBlock, address, neoBalance, Constants.Assets.Neo.AssetId),
                    UpdateBalanceInRepo(lastBlock, address, gasBalance, Constants.Assets.Gas.AssetId));;
            }

            return null;
        }

        private async Task UpdateBalanceInRepo(int lastBlock, string address, decimal balance, string assetId)
        {
            if (balance != 0)
            {
                _log.Info($"[{lastBlock}] Detected balance on {address}: {balance} {assetId}");
                await _walletBalanceRepository.InsertOrReplace(
                    WalletBalance.Create(address,
                        balance: balance,
                        assetId: assetId,
                        updatedAtBlock: lastBlock));
            }
            else
            {
                await _walletBalanceRepository.DeleteIfExist(address, assetId);
            }

        }

        public async Task Subscribe(string address)
        {
            try
            {
                await _observableWalletRepository.Insert(ObservableWallet.Create(address));
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == EntityExistsHttpStatusCode)
            {
                throw new WalletAlreadyExistException();
            }
        }

        public async Task Unsubscribe(string address)
        {
            try
            {
                await _observableWalletRepository.Delete(address);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == EntityNotExistsHttpStatusCode)
            {
                throw new WalletNotExistException();
            }
            finally
            {
                await _walletBalanceRepository.DeleteIfExist(address);
            }
        }

        public async Task<IPaginationResult<IWalletBalance>> GetBalances(int take, string continuation)
        {
            return await _walletBalanceRepository.GetBalances(take, continuation);
        }
    }
}
