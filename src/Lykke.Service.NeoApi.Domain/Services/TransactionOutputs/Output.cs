using System.Collections.Generic;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Domain.Services.TransactionOutputs
{
    public class Output
    {
        private sealed class TransactionHashNEqualityComparer : IEqualityComparer<Output>
        {
            public bool Equals(Output x, Output y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.TransactionHash, y.TransactionHash) && x.N == y.N;
            }

            public int GetHashCode(Output obj)
            {
                unchecked
                {
                    return ((obj.TransactionHash != null ? obj.TransactionHash.GetHashCode() : 0) * 397) ^ obj.N;
                }
            }
        }

        public static IEqualityComparer<Output> TransactionHashNComparer { get; } = new TransactionHashNEqualityComparer();

        public Output(CoinReference outpoint)
        {
            TransactionHash = outpoint.PrevHash.ToString();
            N = outpoint.PrevIndex;
        }

        public string TransactionHash { get; }
        public int N { get; }
    }
}
