using System.Threading.Tasks;

namespace Lykke.Service.NEO.Api.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}