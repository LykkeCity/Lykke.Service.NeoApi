using Autofac;
using Common;
using Lykke.Common.Log;
using Lykke.Job.NeoApi.Services;
using Lykke.Job.NeoApi.Workflow.PeriodicalHandlers;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Service.NeoApi.Domain.Repositories.Outputs;
using Lykke.Service.NeoApi.Domain.Settings;

namespace Lykke.Job.NeoApi.Modules
{
    public class JobModule : Module
    {
        private readonly NeoApiSettings _settings;

        public JobModule(NeoApiSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<UpdateBalancesPeriodicalHandler>()
                .WithParameter(TypedParameter.From(_settings.UpdateBalancesTimerPeriod))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();


            builder.RegisterType<DetectTransactionsPeriodicalHandler>()
                .WithParameter(TypedParameter.From(_settings.DetectTransactionsTimerPeriod))
                .WithParameter(TypedParameter.From(_settings.ConfirmationLevel))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.Register(p => new RemoveOldSpentOutputsPeriodicalHandler(p.Resolve<ILogFactory>(),
                    _settings.SpentOutputsExpirationTimerPeriod,
                    _settings.SpentOutputsExpiration, p.Resolve<ISpentOutputRepository>()))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
        }
    }
}
