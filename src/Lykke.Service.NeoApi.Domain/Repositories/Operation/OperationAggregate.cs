﻿using System;

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

        public bool IsCashout => IncludeFee == false;

        public DateTime InsertedAt { get; }

        public bool IsClaim { get; set; }

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
            DateTime? detectedOnBlockchain,
            bool isClaim)
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
            IsClaim = isClaim;
        }

        public static OperationAggregate StartNew(
            Guid operationId,
            string fromAddress,
            string toAddress,
            string assetId,
            decimal amount,
            decimal fee,
            bool includeFee,
            bool isClaim = false)
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
                detectedOnBlockchain: null,
                isClaim: isClaim);
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
