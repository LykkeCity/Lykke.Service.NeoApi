using System;

namespace Lykke.Service.NeoApi.Domain.Services.Transaction.Exceptions
{
    public class TransactionAlreadyBroadcastedException:Exception
    {
        public TransactionAlreadyBroadcastedException(Exception inner) : base(
            "Node said that transaction already broadcasted", inner)
        {

        }
    }
}
