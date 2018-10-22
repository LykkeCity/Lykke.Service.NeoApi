using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;
using Lykke.Service.NeoApi.Domain.Services.Transaction;
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

        public TransactionBroadcaster(NeoSendRawTransaction neoRawTransactionSender, 
            IUnconfirmedTransactionRepository unconfirmedTransactionRepository,
            IObservableOperationRepository observableOperationRepository,
            INeoscanService neoscanService)
        {
            _neoRawTransactionSender = neoRawTransactionSender;
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
            _observableOperationRepository = observableOperationRepository;
            _neoscanService = neoscanService;
        }

        public async Task BroadcastTransaction(string signedTransaction, OperationAggregate aggregate)
        {
            //TODO validata transaction parameters (throw ex) and get tx hash

            var txHash = "TODO hash!";

            var lastBlockHeight = await _neoscanService.GetHeight();
            
            //TODO handle transaction already broadcasted error and return proper status code on TransactionAlreadyBroadcastedException
            await _neoRawTransactionSender.SendRequestAsync(signedTransaction);

            await _observableOperationRepository.InsertOrReplace(ObervableOperation.Create(aggregate,
                BroadcastStatus.InProgress,
                txHash,
                (int)lastBlockHeight));

            await _unconfirmedTransactionRepository.InsertOrReplace(
                UnconfirmedTransaction.Create(aggregate.OperationId, txHash));
        }
        
    }
}
