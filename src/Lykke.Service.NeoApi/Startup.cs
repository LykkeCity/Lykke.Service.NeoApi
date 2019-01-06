using JetBrains.Annotations;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using AsyncFriendlyStackTrace;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Logs.Loggers.LykkeSlack;
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

                    logs.Extended = logBuilder =>
                    {
                        logBuilder.AddAdditionalSlackChannel("BlockChainIntegration", opt =>
                        {
                            opt.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Information; // Let it be explicit
                        });

                        logBuilder.AddAdditionalSlackChannel("BlockChainIntegrationImportantMessages", opt =>
                        {
                            opt.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                        });
                    };
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
                options.DefaultErrorHandler = ex => new ErrorResponse {ErrorMessage = ex.ToAsyncString()};
            });
        }
    }
}
