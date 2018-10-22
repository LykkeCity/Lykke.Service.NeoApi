using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.NeoApi.Domain.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public NeoApiSettings NeoApiService { get; set; }
    }
}
