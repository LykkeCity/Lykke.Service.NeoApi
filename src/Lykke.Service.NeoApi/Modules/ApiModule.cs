using Autofac;
using Lykke.Sdk;
using Lykke.Service.NeoApi.Lifetime;

namespace Lykke.Service.NeoApi.Modules
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();
        }
    }
}
