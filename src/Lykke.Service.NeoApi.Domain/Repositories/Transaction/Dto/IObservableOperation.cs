using System;

namespace Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto
{
    public interface IObservableOperation 
    {
        BroadcastStatus Status { get; }
        Guid OperationId { get;  }
        string FromAddress { get; }
        string ToAddress { get;  }
        string AssetId { get;  }
        decimal Amount { get; }
        decimal Fee { get; }
        DateTime Updated { get;  }
        string TxHash { get; }
        int UpdatedAtBlockHeight { get; }

    }
}
