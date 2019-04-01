using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.NeoApi.Client
{
    public class NeoClaimUnknownResponseException:Exception
    {
        public NeoClaimUnknownResponseException(string message):base(message)
        {

        }
    }
}
