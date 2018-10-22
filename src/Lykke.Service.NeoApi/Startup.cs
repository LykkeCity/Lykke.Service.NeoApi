using JetBrains.Annotations;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Lykke.Service.NeoApi.AzureRepositories.Binders;
using Lykke.Service.NeoApi.Domain.Settings;
using Lykke.Service.NeoApi.DomainServices.Binders;

namespace Lykke.Service.NeoApi
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "NeoApi API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "NeoApiLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.NeoApiService.Db.LogsConnString;
                };

                options.RegisterAdditionalModules = moduleRegistration =>
                {
                    moduleRegistration.RegisterModule<AzureRepositoriesModule>();
                    moduleRegistration.RegisterModule<CommonServicesModule>();
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
