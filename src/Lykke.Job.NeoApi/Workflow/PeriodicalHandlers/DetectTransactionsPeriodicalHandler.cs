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
using NeoModules.Rest.Interfaces;

namespace Lykke.Job.NeoApi.Workflow.PeriodicalHandlers
{
    public class DetectTransactionsPeriodicalHandler:IStartable, IStopable
    {
        private readonly INeoscanService _neoscanService;
        private readonly ILog _log;
        private readonly TimerTrigger _timerTrigger;
        private readonly int _confirmationLevel;
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;
        private readonly IOperationRepository _operationRepository;
        private readonly IWalletBalanceService _walletBalanceService;

        public DetectTransactionsPeriodicalHandler(
            INeoscanService neoscanService,
            ILogFactory logFactory,
            TimeSpan timerPeriod, 
            int confirmationLevel, 
            IUnconfirmedTransactionRepository unconfirmedTransactionRepository, 
            IOperationRepository operationRepository, 
            IWalletBalanceService walletBalanceService)
        {
            _neoscanService = neoscanService;
            _confirmationLevel = confirmationLevel;
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
            _operationRepository = operationRepository;
            _walletBalanceService = walletBalanceService;

            _log = logFactory.CreateLog(this);

            _timerTrigger = new TimerTrigger(nameof(UpdateBalancesPeriodicalHandler), timerPeriod, logFactory);
            _timerTrigger.Triggered += (trigger, args, token) => Execute();
        }

        public async Task Execute()
        {
            var lastBlockHeight = await _neoscanService.GetHeight();
            foreach (var unconfirmedTx in await _unconfirmedTransactionRepository.GetAll())
            {
                try
                {
                    await CheckTransaction(unconfirmedTx, (int)lastBlockHeight);
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            }
        }

        private async Task CheckTransaction(IUnconfirmedTransaction unconfirmedTx, int lastBlockHeight)
        {
            var operation = await _operationRepository.GetOrDefault(unconfirmedTx.OperationId);
            if (operation == null)
            {
                _log.Error(nameof(DetectTransactionsPeriodicalHandler),
                    message: $"Aggregate for operation {unconfirmedTx.OperationId} not found");

                return;
            }

            var blockchainTx = await _neoscanService.GetTransactionAsync(unconfirmedTx.TxHash);

            if (blockchainTx == null)
            {
                return;
            }

            var isCompleted = lastBlockHeight - blockchainTx.BlockHeight >= _confirmationLevel;

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
            _log.Info($"Starting {nameof(DetectTransactionsPeriodicalHandler)}");

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