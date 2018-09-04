using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.NEO.Api.Core.Domain.Operations
{
    public interface IOperationItem
    {
        string FromAddress { get; }
        string ToAddress { get; }
        decimal Amount { get; }
    }
}
