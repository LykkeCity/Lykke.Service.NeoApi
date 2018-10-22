using System;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;

namespace Lykke.Service.NeoApi.Domain.Repositories.Transaction
{
    public interface IObservableOperationRepository
    {
        Task InsertOrReplace(IObservableOperation tx);
        Task DeleteIfExist(params Guid[] operationIds);
        Task<IObservableOperation> GetById(Guid opId);
    }
}
