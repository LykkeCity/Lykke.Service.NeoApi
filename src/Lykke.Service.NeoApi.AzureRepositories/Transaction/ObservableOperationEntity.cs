using System;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.NeoApi.AzureRepositories.Transaction
{
    internal class ObservableOperationEntity : AzureTableEntity, IObservableOperation
    {
        public BroadcastStatus Status { get; set; }
        public Guid OperationId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public DateTime Updated { get; set; }
        public string TxHash { get; set; }
        public int UpdatedAtBlockHeight { get; set; }

        public static ObservableOperationEntity Map(string partitionKey, string rowKey,
            IObservableOperation source)
        {
            return new ObservableOperationEntity
            {
                OperationId = source.OperationId,
                PartitionKey = partitionKey,
                RowKey = rowKey,
                FromAddress = source.FromAddress,
                AssetId = source.AssetId,
                ToAddress = source.ToAddress,
                Status = source.Status,
                Updated = source.Updated,
                TxHash = source.TxHash,
                UpdatedAtBlockHeight = source.UpdatedAtBlockHeight,
                Fee = source.Fee,
                Amount = source.Amount
            };
        }

        public static class ByOperationId 
        {
            public static string GeneratePartitionKey(Guid operationId)
            {
                return operationId.ToString().CalculateHexHash32(3);
            }

            public static string GenerateRowKey(Guid operationId)
            {
                return operationId.ToString();
            }

            public static ObservableOperationEntity Create(IObservableOperation source)
            {
                return Map(GeneratePartitionKey(source.OperationId), GenerateRowKey(source.OperationId), source);
            }
        }
    }
}
