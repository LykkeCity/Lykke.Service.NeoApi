using System;
using Autofac;
using Autofac.Builder;
using Flurl.Http.Configuration;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Blockchain;
using Lykke.Service.NeoApi.Domain.Services.Transaction;
using Lykke.Service.NeoApi.Domain.Services.TransactionOutputs;
using Lykke.Service.NeoApi.Domain.Settings;
using Lykke.Service.NeoApi.DomainServices.Address;
using Lykke.Service.NeoApi.DomainServices.Blockchain;
using Lykke.Service.NeoApi.DomainServices.Transaction;
using Lykke.Service.NeoApi.DomainServices.TransactionOutputs;
using Lykke.SettingsReader;
using NeoModules.JsonRpc.Client;
using NeoModules.RPC.Services.Transactions;

namespace Lykke.Service.NeoApi.DomainServices.Binders
{
    public class CommonServicesModule : Module
    {
        private readonly NeoApiSettings _settings;

        public CommonServicesModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue.NeoApiService;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new RpcClient(new Uri(_settings.NodeUrl)))
                .As<IClient>()
                .SingleInstance();

            builder.RegisterType<NeoSendRawTransaction>()
                .AsSelf();
            builder.RegisterInstance(new PerBaseUrlFlurlClientFactory())
                .As<IFlurlClientFactory>()
                .SingleInstance();

            builder.Register(c => new NeoScanBlockchainProvider(c.Resolve<IFlurlClientFactory>().Get(_settings.NeoScanUrl)))
                .As<IBlockchainProvider>()
                .SingleInstance();

            builder.RegisterType<TransactionBroadcaster>()
                .As<ITransactionBroadcaster>();

            builder.RegisterType<TransactionBuilder>()
                .As<ITransactionBuilder>();

            builder.RegisterType<AddressValidator>()
                .As<IAddressValidator>();

            builder.RegisterType<WalletBalanceService>()
                .As<IWalletBalanceService>();

            builder.RegisterType<TransactionOutputsService>()
                .As<ITransactionOutputsService>();

            builder.RegisterInstance(new FeeSettings
            {
                FixedFee = _settings.FixedFee
            }).SingleInstance();
        }
    }
}
