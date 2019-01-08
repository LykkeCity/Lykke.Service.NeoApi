using System;
using System.Threading.Tasks;

namespace Lykke.Service.NeoApi.Domain.Repositories.Operation
{
    public interface IOperationRepository
    {
        Task<OperationAggregate> GetOrDefault(Guid operationId);

        Task<OperationAggregate> GetOrInsert(Guid operationId, Func<OperationAggregate> factory);

        Task Save(OperationAggregate aggregate);
    }
}
