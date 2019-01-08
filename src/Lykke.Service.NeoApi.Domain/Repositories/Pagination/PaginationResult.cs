using System.Collections.Generic;

namespace Lykke.Service.NeoApi.Domain.Repositories.Pagination
{
    public class PaginationResult<T> : IPaginationResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public string Continuation { get; set; }

        public static IPaginationResult<T> Create(IEnumerable<T> items, string continuation)
        {
            return new PaginationResult<T>
            {
                Continuation = continuation,
                Items = items
            };
        }
    }
}
