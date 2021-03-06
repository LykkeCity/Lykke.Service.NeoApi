﻿using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Blockchain;

namespace Lykke.Job.NeoApi.Workflow.PeriodicalHandlers
{
    public class DetectTransactionsPeriodicalHandler:IStartable, IStopable
    {
        private readonly IBlockchainProvider _blockchainProvider;
        private readonly ILog _log;
        private readonly TimerTrigger _timerTrigger;
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;
        private readonly IOperationRepository _operationRepository;
        private readonly IWalletBalanceService _walletBalanceService;
        private readonly IObservableOperationRepository _observableOperationRepository;

        public DetectTransactionsPeriodicalHandler(ILogFactory logFactory,
            TimeSpan timerPeriod, 
            IUnconfirmedTransactionRepository unconfirmedTransactionRepository, 
            IOperationRepository operationRepository, 
            IWalletBalanceService walletBalanceService,
            IObservableOperationRepository observableOperationRepository, 
            IBlockchainProvider blockchainProvider)
        {
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
            _operationRepository = operationRepository;
            _walletBalanceService = walletBalanceService;
            _observableOperationRepository = observableOperationRepository;
            _blockchainProvider = blockchainProvider;

            _log = logFactory.CreateLog(this);

            _timerTrigger = new TimerTrigger(nameof(UpdateBalancesPeriodicalHandler), timerPeriod, logFactory);
            _timerTrigger.Triggered += (trigger, args, token) => Execute();
        }

        public async Task Execute()
        {
            foreach (var unconfirmedTx in await _unconfirmedTransactionRepository.GetAll())
            {
                try
                {
                    await CheckTransaction(unconfirmedTx);
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            }
        }

        private async Task CheckTransaction(IUnconfirmedTransaction unconfirmedTx)
        {
            var operation = await _operationRepository.GetOrDefault(unconfirmedTx.OperationId);
            if (operation == null)
            {
                _log.Error(nameof(DetectTransactionsPeriodicalHandler),
                    message: $"Aggregate for operation {unconfirmedTx.OperationId} not found");

                return;
            }

            var blockchainTx = await _blockchainProvider.GetTransactionOrDefaultAsync(unconfirmedTx.TxHash);

            var isCompleted = blockchainTx?.blockHash != null; //once a tx included in a block means the tx is confirmed by the 7 consensus nodes and cannt be reversed

            var lastBlockHeight = await _blockchainProvider.GetHeightAsync();

            var status = isCompleted
                ? BroadcastStatus.Completed
                : BroadcastStatus.InProgress;

            await _observableOperationRepository.InsertOrReplace(ObervableOperation.Create(operation, status,
                unconfirmedTx.TxHash,
                lastBlockHeight));

            if (isCompleted)
            {
                var fromAddressBalance = await _walletBalanceService.UpdateBalance(operation.FromAddress);
                var toAddressBalance = await _walletBalanceService.UpdateBalance(operation.ToAddress);

                var operationCompletedLoggingContext = new
                {
                    unconfirmedTx.OperationId,
                    unconfirmedTx.TxHash,
                    fromAddressBalance,
                    toAddressBalance
                };

                _log.Info("Transaction detected on blockchain", context: operationCompletedLoggingContext);


                await _unconfirmedTransactionRepository.DeleteIfExist(unconfirmedTx.OperationId);

                operation.OnDetectedOnBlockcain(DateTime.UtcNow);
                await _operationRepository.Save(operation);
            }

        }

        public void Start()
        {
            _log.Info("Starting");

            _timerTrigger.Start();
        }

        public void Dispose()
        {
            _timerTrigger.Dispose();
        }

        public void Stop()
        {
            _timerTrigger.Stop();
        }
    }
}
