using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.NeoApi.AzureRepositories.SpentOutputs
{
    public class SpentOutputEntity : AzureTableEntity, IOutput
    {
        public Guid OperationId { get; set; }

        public string TransactionHash { get; set; }
        public int N { get; set; }

        public static SpentOutputEntity Create(string transactionHash, int n, Guid operationId)
        {
            return new SpentOutputEntity
            {
                PartitionKey = GeneratePartitionKey(transactionHash),
                RowKey = GenerateRowKey(n),
                OperationId = operationId,
                TransactionHash = transactionHash,
                N = n
            };
        }

        public static string GenerateRowKey(int n)
        {
            return n.ToString();
        }

        public static string GeneratePartitionKey(string transactionHash)
        {
            return transactionHash;
        }
    }
}
