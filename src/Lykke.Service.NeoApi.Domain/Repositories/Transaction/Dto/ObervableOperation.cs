using System;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;

namespace Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto
{
    public class ObervableOperation : IObservableOperation
    {
        public Guid OperationId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public bool IncludeFee { get; set; }
        public DateTime Updated { get; set; }
        public BroadcastStatus Status { get; set; }
        public string TxHash { get; set; }
        public int UpdatedAtBlockHeight { get; set; }

        public static ObervableOperation Create(OperationAggregate operation, BroadcastStatus status, string txHash, int updatedAtBlockHeight, DateTime? updated = null)
        {
            return new ObervableOperation
            {
                OperationId = operation.OperationId,
                Amount = operation.Amount,
                AssetId = operation.AssetId,
                FromAddress = operation.FromAddress,
                IncludeFee = operation.IncludeFee,
                ToAddress = operation.ToAddress,
                Status = status,
                TxHash = txHash,
                Updated = updated ?? DateTime.UtcNow,
                Fee = operation.Fee,
                UpdatedAtBlockHeight = updatedAtBlockHeight
            };
        }
    }
}
