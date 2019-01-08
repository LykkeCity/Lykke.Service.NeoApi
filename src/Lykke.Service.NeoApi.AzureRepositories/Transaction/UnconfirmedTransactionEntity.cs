using System;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.NeoApi.AzureRepositories.Transaction
{
    internal class UnconfirmedTransactionEntity : TableEntity, IUnconfirmedTransaction
    {
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }

        public static string GeneratePartitionKey(Guid operationId)
        {
            return operationId.ToString();
        }

        public static string GenerateRowKey()
        {
            return "_";
        }

        public static UnconfirmedTransactionEntity Create(IUnconfirmedTransaction source)
        {
            return new UnconfirmedTransactionEntity
            {
                PartitionKey = GeneratePartitionKey(source.OperationId),
                RowKey = GenerateRowKey(),
                OperationId = source.OperationId,
                TxHash = source.TxHash
            };
        }
    }
}
