using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;

namespace Lykke.Service.NeoApi.Domain.Repositories.Outputs
{
    public interface ISpentOutputRepository
    {
        Task InsertSpentOutputsAsync(Guid transactionId, IEnumerable<Output> outputs);

        Task<IEnumerable<Output>> GetSpentOutputsAsync(IEnumerable<Output> outputs);

        Task RemoveOldOutputsAsync(DateTime bound);
    }
}
