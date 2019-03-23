using System;
using Lykke.AzureStorage.Tables;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;

namespace Lykke.Service.NeoApi.AzureRepositories.Operation
{
    public class OperationEntity:AzureTableEntity
    {
        public static string GeneratePartitionKey(Guid operationId)
        {
            return operationId.ToString();
        }

        public static string GenerateRowKey()
        {
            return "";
        }

        public Guid OperationId { get; set; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string AssetId { get; set; }

        public decimal Amount { get; set; }

        public decimal Fee { get; set; }

        public bool IncludeFee { get; set; }

        public DateTime InsertedAt { get; set; }

        public DateTime? BroadcastedAt { get; set; }

        public DateTime? DetectedOnBlockchain { get; set; }

        public bool IsClaim { get; set; }

        public static OperationEntity FromDomain(OperationAggregate aggregate)
        {
            return new OperationEntity
            {
                ETag = string.IsNullOrEmpty(aggregate.Version) ? "*" : aggregate.Version,
                Fee = aggregate.Fee,
                Amount = aggregate.Amount,
                AssetId = aggregate.AssetId,
                DetectedOnBlockchain = aggregate.DetectedOnBlockchain,
                BroadcastedAt = aggregate.BroadcastedAt,
                FromAddress = aggregate.FromAddress,
                IncludeFee = aggregate.IncludeFee,
                InsertedAt = aggregate.InsertedAt,
                OperationId = aggregate.OperationId,
                ToAddress = aggregate.ToAddress,
                PartitionKey = GeneratePartitionKey(aggregate.OperationId),
                RowKey = GenerateRowKey()
            };
        }

        public OperationAggregate ToDomain()
        {
            return new OperationAggregate(version: ETag,
                operationId: OperationId,
                fromAddress: FromAddress,
                toAddress: ToAddress,
                amount:Amount,
                fee: Fee,
                assetId: AssetId,
                includeFee: IncludeFee,
                insertedAt: InsertedAt,
                broadcastedAt: BroadcastedAt,
                detectedOnBlockchain: DetectedOnBlockchain,
                isClaim: IsClaim);
        }
    }
}
