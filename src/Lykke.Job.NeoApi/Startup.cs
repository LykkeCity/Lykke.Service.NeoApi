﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
using Autofac.Core;
using Lykke.Job.NeoApi.Modules;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.MonitoringServiceApiCaller;
using Lykke.Sdk;
using Lykke.Service.NeoApi.AzureRepositories.Binders;
using Lykke.Service.NeoApi.Domain.Settings;
using Lykke.Service.NeoApi.DomainServices.Binders;

namespace Lykke.Job.NeoApi
{
    [PublicAPI]
    public class Startup
    {
        private const string ApiVersion = "v1";
        private const string ApiName = "NeoApiJob API";

        private string _monitoringServiceUrl;
        private ILog _log;
        private IHealthNotifier _healthNotifier;

        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    });

                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration(ApiVersion, ApiName);
                });

                var settingsManager = Configuration.LoadSettings<AppSettings>(options =>
                {
                    options.SetConnString(x => x.SlackNotifications.AzureQueue.ConnectionString);
                    options.SetQueueName(x => x.SlackNotifications.AzureQueue.QueueName);
                    options.SenderName = $"{AppEnvironment.Name} {AppEnvironment.Version}";
                });

                var appSettings = settingsManager.CurrentValue;

                if (appSettings.MonitoringServiceClient != null)
                    _monitoringServiceUrl = appSettings.MonitoringServiceClient.MonitoringServiceUrl;

                services.AddLykkeLogging(
                    settingsManager.ConnectionString(s => s.NeoApiService.Db.LogsConnString),
                    "NeoApiJobLog",
                    appSettings.SlackNotifications.AzureQueue.ConnectionString,
                    appSettings.SlackNotifications.AzureQueue.QueueName,
                    logBuilder =>
                    {
                        logBuilder.AddAdditionalSlackChannel("BlockChainIntegration", opt =>
                        {
                            opt.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                            opt.IncludeHealthNotifications();
                        });

                        logBuilder.AddAdditionalSlackChannel("BlockChainIntegrationImportantMessages", opt =>
                        {
                            opt.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                            opt.IncludeHealthNotifications();
                        });
                    });

                var builder = new ContainerBuilder();
                builder.Populate(services);

                var modules = new IModule[]
                {
                    new JobModule(appSettings.NeoApiService),
                    new CommonServicesModule(settingsManager), 
                    new AzureRepositoriesModule(settingsManager), 
                };

                foreach (var module in modules)
                {
                    builder.RegisterModule(module);
                }

                ApplicationContainer = builder.Build();

                var logFactory = ApplicationContainer.Resolve<ILogFactory>();
                _log = logFactory.CreateLog(this);
                _healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                if (_log == null)
                    Console.WriteLine(ex);
                else
                    _log.Critical(ex);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                    app.UseDeveloperExceptionPage();

                app.UseLykkeForwardedHeaders();
                app.UseLykkeMiddleware(ex => new ErrorResponse {ErrorMessage = ex.ToAsyncString()});

                app.UseMvc();
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
                });
                app.UseSwaggerUI(x =>
                {
                    x.RoutePrefix = "swagger/ui";
                    x.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", ApiVersion);
                });
                app.UseStaticFiles();

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(CleanUp);
            }
            catch (Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                // NOTE: Job not yet recieve and process IsAlive requests here

                await ApplicationContainer.Resolve<IStartupManager>().StartAsync();
                _healthNotifier.Notify("Started");

#if !DEBUG
                await Configuration.RegisterInMonitoringServiceAsync(_monitoringServiceUrl, _healthNotifier);
#endif
            }
            catch (Exception ex)
            {
                _log.Critical(ex);
                throw;
            }
        }

        private async Task StopApplication()
        {
            try
            {
                // NOTE: Job still can recieve and process IsAlive requests here, so take care about it if you add logic here.

                await ApplicationContainer.Resolve<IShutdownManager>().StopAsync();
            }
            catch (Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }

        private void CleanUp()
        {
            try
            {
                // NOTE: Job can't recieve and process IsAlive requests here, so you can destroy all resources
                _healthNotifier?.Notify("Terminating");

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }
    }
}
