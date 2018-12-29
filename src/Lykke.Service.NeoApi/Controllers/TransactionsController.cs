using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.NeoApi.Domain;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Transaction;
using Lykke.Service.NeoApi.Domain.Services.Transaction.Exceptions;
using Lykke.Service.NeoApi.DomainServices.Transaction;
using Lykke.Service.NeoApi.Helpers;
using Lykke.Service.NeoApi.Helpers.Transaction;
using Lykke.Service.NeoApi.Helpers.Transaction.Exceptions;
using Microsoft.AspNetCore.Mvc;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Controllers
{
    public class TransactionsController:Controller
    {
        private readonly IOperationRepository _operationRepository;
        private readonly IAddressValidator _addressValidator;
        private readonly ITransactionBroadcaster _transactionBroadcaster;
        private readonly FeeSettings _feeSettings;
        private readonly IObservableOperationRepository _observableOperationRepository;
        private readonly ITransactionBuilder _transactionBuilder;

        public TransactionsController(IAddressValidator addressValidator, 
            IOperationRepository operationRepository, 
            ITransactionBroadcaster transactionBroadcaster, 
            FeeSettings feeSettings,
            IObservableOperationRepository observableOperationRepository, 
            ITransactionBuilder transactionBuilder)
        {
            _addressValidator = addressValidator;
            _operationRepository = operationRepository;
            _transactionBroadcaster = transactionBroadcaster;
            _feeSettings = feeSettings;
            _observableOperationRepository = observableOperationRepository;
            _transactionBuilder = transactionBuilder;
        }

        [HttpPost("api/transactions/single")]
        public async Task<IActionResult> BuildSingle([FromBody] BuildSingleTransactionRequest request)
        {
            if (request == null)
            {
                return BadRequest(ErrorResponse.Create("Unable to deserialize request"));
            }

            var amount = MoneyConversionHelper.FromContract(request.Amount);

            if (amount <= 0)
            {
                return BadRequest(ErrorResponse.Create($"Amount can't be less or equal to zero: {amount}"));
            }

            if (amount % 1 != 0)
            {

                return BadRequest($"The minimum unit of NEO is 1 and tokens cannot be subdivided.: {amount}");
            }

            if (request.AssetId != Constants.Assets.Neo.AssetId)
            {

                return BadRequest(ErrorResponse.Create("Invalid assetId"));
            }

            var toAddressValid = _addressValidator.IsAddressValid(request.ToAddress);
            if (!toAddressValid)
            {
                return BadRequest(ErrorResponse.Create("Invalid toAddress"));
            }

            var fromAddressValid = _addressValidator.IsAddressValid(request.FromAddress);
            if (!fromAddressValid)
            {
                return BadRequest(ErrorResponse.Create("Invalid fromAddress"));
            }

            if (request.OperationId == Guid.Empty)
            {
                return BadRequest(ErrorResponse.Create("Invalid operation id (GUID)"));
            }

            var aggregate = await _operationRepository.GetOrInsert(request.OperationId,
                () => OperationAggregate.StartNew(request.OperationId,
                    fromAddress: request.FromAddress,
                    toAddress: request.ToAddress,
                    amount: amount,
                    assetId: request.AssetId,
                    fee: _feeSettings.FixedFee,
                    includeFee: request.IncludeFee));

            if (aggregate.IsBroadcasted)
            {
                return StatusCode(409);
            }

            var fee = aggregate.IsCashout ? _feeSettings.FixedFee : 0;
            var tx = await _transactionBuilder.BuildNeoContractTransactionAsync(request.FromAddress,
                request.ToAddress,
                amount,
                request.IncludeFee,
                fee);


            return Ok(new BuildTransactionResponse
            {
                TransactionContext = TransactionSerializer.Serialize(tx)
            });
        }

        [HttpPost("api/transactions/broadcast")]
        public async Task<IActionResult> BroadcastTransaction([FromBody] BroadcastTransactionRequest request)
        {
            if (request == null)
            {
                return BadRequest(ErrorResponse.Create("Unable to deserialize request"));
            }

            var aggregate = await _operationRepository.GetOrDefault(request.OperationId);

            if (aggregate == null)
            {
                return BadRequest(ErrorResponse.Create($"Operation {request.OperationId} not found"));
            }
            
            Transaction tx;
            try
            {
                tx = TransactionSerializer.Deserialize(request.SignedTransaction);
            }
            catch (InvalidTransactionException)
            {
                return BadRequest(ErrorResponse.Create($"{nameof(request.SignedTransaction)} is invalid"));
            }

            if (aggregate.IsBroadcasted)
            {
                return Conflict();
            }

            try
            {
                await _transactionBroadcaster.BroadcastTransaction(tx, aggregate);
            }
            catch (TransactionAlreadyBroadcastedException)
            {
                return new StatusCodeResult(409);
            }

            aggregate.OnBroadcasted(DateTime.UtcNow);
            await _operationRepository.Save(aggregate);

            return Ok();
        }

        [HttpGet("api/transactions/broadcast/single/{operationId}")]
        public async Task<IActionResult> GetObservableSingleOperation(Guid operationId)
        {
            if (!ModelState.IsValid ||
                !ModelState.IsValidOperationId(operationId))
            {
                return BadRequest(ModelState.ToErrorResponce());
            }

            var result = await _observableOperationRepository.GetById(operationId);

            if (result == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }

            BroadcastedTransactionState MapState(BroadcastStatus status)
            {
                switch (status)
                {
                    case BroadcastStatus.Completed:
                        return BroadcastedTransactionState.Completed;
                    case BroadcastStatus.Failed:
                        return BroadcastedTransactionState.Failed;
                    case BroadcastStatus.InProgress:
                        return BroadcastedTransactionState.InProgress;
                    default:
                        throw new InvalidCastException($"Unknown mapping from {status} ");
                }
            }

            return Ok(new BroadcastedSingleTransactionResponse
            {
                Amount = MoneyConversionHelper.ToContract(result.Amount),
                Fee = MoneyConversionHelper.ToContract(result.Fee),
                OperationId = result.OperationId,
                Hash = result.TxHash,
                Timestamp = result.Updated,
                State = MapState(result.Status),
                Block = result.UpdatedAtBlockHeight
            });
        }

        [HttpDelete("api/transactions/broadcast/{operationId}")]
        public async Task<IActionResult> RemoveObservableOperation(Guid operationId)
        {
            if (!ModelState.IsValid ||
                !ModelState.IsValidOperationId(operationId))
            {
                return BadRequest(ModelState.ToErrorResponce());
            }

            await _observableOperationRepository.DeleteIfExist(operationId);

            return Ok();
        }
    }
}
