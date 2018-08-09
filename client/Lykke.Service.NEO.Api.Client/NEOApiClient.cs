using System;
using Common.Log;

namespace Lykke.Service.NEO.Api.Client
{
    public class NEOApiClient : INEOApiClient, IDisposable
    {
        private readonly ILog _log;

        public NEOApiClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
