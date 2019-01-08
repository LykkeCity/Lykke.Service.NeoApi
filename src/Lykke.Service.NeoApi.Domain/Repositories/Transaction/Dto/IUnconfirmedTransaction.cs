using System;

namespace Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto
{
    public interface IUnconfirmedTransaction
    {
        string TxHash { get; }
        Guid OperationId { get; }
    }
}
