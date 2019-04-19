using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.NeoApi.Client
{
    public class NeoClaimTransactionException:Exception
    {
        public ErrorCode ErrorType { get; }

        public NeoClaimTransactionException(ErrorCode code, string message = null, Exception inner = null) 
            : base(message, inner)
        {
            ErrorType = code;
        }

        public enum ErrorCode
        {
            TransactionAlreadyBroadcased,
            ClaimableGasNotAvailiable
        }
    }
}
