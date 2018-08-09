using Lykke.Service.NEO.Api.Core.Settings.ServiceSettings;
using Lykke.Service.NEO.Api.Core.Settings.SlackNotifications;

namespace Lykke.Service.NEO.Api.Core.Settings
{
    public class AppSettings
    {
        public NEOApiSettings NEOApiService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
