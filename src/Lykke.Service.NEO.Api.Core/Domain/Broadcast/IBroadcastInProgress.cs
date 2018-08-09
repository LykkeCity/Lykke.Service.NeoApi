using System;

namespace Lykke.Service.NEO.Api.Core.Domain.Broadcast
{
    public interface IBroadcastInProgress
    {
        Guid OperationId { get; }
        string Hash { get; }
    }
}
