using System.Collections.Generic;

namespace Lykke.Service.NeoApi.Domain.Repositories.Pagination
{
    public interface IPaginationResult<T>
    {
        IEnumerable<T> Items { get; }

        string Continuation { get; }
    }
}