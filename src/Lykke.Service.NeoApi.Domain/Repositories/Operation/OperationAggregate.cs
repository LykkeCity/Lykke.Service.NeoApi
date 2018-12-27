using System;

namespace Lykke.Service.NeoApi.Domain.Repositories.Operation
{
    public class OperationAggregate
    {
        public string Version { get; }

        public Guid OperationId { get; }

        public string FromAddress { get; }

        public string ToAddress { get; }

        public string AssetId { get; }

        public decimal Amount { get; }

        public decimal Fee { get; }

        public bool IncludeFee { get; }

        public DateTime InsertedAt { get; }

        public DateTime? BroadcastedAt { get; private set; }
        public DateTime? DetectedOnBlockchain { get; private set; }

        public bool IsBroadcasted => BroadcastedAt != null;
        public bool IsDetectedOnBlockchain => DetectedOnBlockchain != null;

        public OperationAggregate(string version,
            Guid operationId,
            string fromAddress,
            string toAddress,
            string assetId,
            decimal amount,
            decimal fee,
            bool includeFee,
            DateTime insertedAt,
            DateTime? broadcastedAt,
            DateTime? detectedOnBlockchain)
        {
            Version = version;
            OperationId = operationId;
            FromAddress = fromAddress;
            ToAddress = toAddress;
            AssetId = assetId;
            Amount = amount;
            Fee = fee;
            IncludeFee = includeFee;
            InsertedAt = insertedAt;
            BroadcastedAt = broadcastedAt;
            DetectedOnBlockchain = detectedOnBlockchain;
        }

        public static OperationAggregate StartNew(
            Guid operationId,
            string fromAddress,
            string toAddress,
            string assetId,
            decimal amount,
            decimal fee,
            bool includeFee)
        {
            return new OperationAggregate(version: null,
                operationId: operationId,
                fromAddress: fromAddress,
                toAddress: toAddress, 
                amount:amount, 
                assetId:assetId,
                fee: fee,
                includeFee: includeFee, 
                insertedAt: DateTime.UtcNow, 
                broadcastedAt: null, 
                detectedOnBlockchain: null);
        }

        public void OnBroadcasted(DateTime moment)
        {
            if (!IsBroadcasted)
            {
                BroadcastedAt = moment;
            }
        }

        public void OnDetectedOnBlockcain(DateTime moment)
        {
            if (!IsDetectedOnBlockchain)
            {
                DetectedOnBlockchain = moment;
            }
        }

    }
}
