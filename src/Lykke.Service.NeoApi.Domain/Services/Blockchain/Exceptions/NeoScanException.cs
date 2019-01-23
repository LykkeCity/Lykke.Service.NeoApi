using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.NeoApi.Domain.Services.Blockchain.Exceptions
{
    public class NeoScanException:Exception
    {
        public NeoScanException(Exception ex, string body) 
            : base($"Error while processing exception within neoscan api: {body}", ex)
        {

        }
    }
}
