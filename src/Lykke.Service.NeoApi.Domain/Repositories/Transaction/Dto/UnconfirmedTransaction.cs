using System;

namespace Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto
{
    public class UnconfirmedTransaction : IUnconfirmedTransaction
    {
        public string TxHash { get; set; }
        public Guid OperationId { get; set; }

        public static UnconfirmedTransaction Create(Guid opId, string txHash)
        {
            return new UnconfirmedTransaction
            {
                OperationId = opId,
                TxHash = txHash
            };
        }
    }
}
