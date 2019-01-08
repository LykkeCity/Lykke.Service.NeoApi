using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction.Dto;

namespace Lykke.Service.NeoApi.Domain.Repositories.Transaction
{
    public interface IUnconfirmedTransactionRepository
    {
        Task<IEnumerable<IUnconfirmedTransaction>> GetAll();
        Task InsertOrReplace(IUnconfirmedTransaction tx);
        Task DeleteIfExist(params Guid[] operationIds);
    }
}
