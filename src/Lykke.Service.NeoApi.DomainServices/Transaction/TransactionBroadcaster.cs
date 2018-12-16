using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;
using Lykke.Service.NeoApi.Domain.Services.Transaction;
using Lykke.Service.NeoApi.Domain.Services.Transaction.Exceptions;
using NeoModules.JsonRpc.Client;
using NeoModules.Rest.Interfaces;
using NeoModules.RPC.Services.Transactions;

namespace Lykke.Service.NeoApi.DomainServices.Transaction
{
    internal class TransactionBroadcaster:ITransactionBroadcaster
    {
        private readonly INeoscanService _neoscanService;
        private readonly NeoSendRawTransaction _neoRawTransactionSender;
        private readonly IObservableOperationRepository _observableOperationRepository;
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;
        private readonly ILog _log;

        public TransactionBroadcaster(NeoSendRawTransaction neoRawTransactionSender, 
            IUnconfirmedTransactionRepository unconfirmedTransactionRepository,
            IObservableOperationRepository observableOperationRepository,
            INeoscanService neoscanService,
            ILogFactory logFactory)
        {
            _neoRawTransactionSender = neoRawTransactionSender;
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
            _observableOperationRepository = observableOperationRepository;
            _neoscanService = neoscanService;
            _log = logFactory.CreateLog(this);
        }

        public async Task BroadcastTransaction(NeoModules.NEP6.Transactions.Transaction signedTransaction, OperationAggregate aggregate)
        {
            var txHash = signedTransaction.Hash.ToString().Substring(2);

            var lastBlockHeight = await _neoscanService.GetHeight();

            try
            {
                var isSuccess = await _neoRawTransactionSender.SendRequestAsync(signedTransaction.ToHexString());

                if (!isSuccess)
                {
                    throw new Exception("Unknown error while broadcasting the tx");
                }
            }
            catch (RpcResponseException e) when (e.RpcError.Code == -501)
            {
                throw new TransactionAlreadyBroadcastedException(e);
            }

            await _observableOperationRepository.InsertOrReplace(ObervableOperation.Create(aggregate,
                BroadcastStatus.InProgress,
                txHash,
                (int)lastBlockHeight));

            await _unconfirmedTransactionRepository.InsertOrReplace(
                UnconfirmedTransaction.Create(aggregate.OperationId, txHash));
        }
        
    }
}
