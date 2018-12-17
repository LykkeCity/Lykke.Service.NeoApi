using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.NeoApi.Domain.Services.Transaction.Exceptions
{
    public class NotEnoughFundsException:Exception
    {
        public NotEnoughFundsException(string message) : base(message)
        {

        }
    }
}
