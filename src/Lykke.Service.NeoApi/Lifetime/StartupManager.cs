using System.IO;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Service.NeoApi.Lifetime
{
    public class StartupManager:IStartupManager
    {
        private readonly ILog _log;

        public StartupManager(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }
        public async Task StartAsync()
        {
            _log.Info("Content of protocol.json file", context: File.ReadAllTextAsync("protocol.json"));
        }
    }
}
