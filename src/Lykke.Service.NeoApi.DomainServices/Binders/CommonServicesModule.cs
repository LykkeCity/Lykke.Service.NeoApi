using System;
using Autofac;
using Lykke.Service.NeoApi.Domain.Services.Address;
using Lykke.Service.NeoApi.Domain.Services.Transaction;
using Lykke.Service.NeoApi.Domain.Settings;
using Lykke.Service.NeoApi.DomainServices.Address;
using Lykke.Service.NeoApi.DomainServices.Transaction;
using NeoModules.JsonRpc.Client;
using NeoModules.Rest.Interfaces;
using NeoModules.Rest.Services;
using NeoModules.RPC.Services.Transactions;

namespace Lykke.Service.NeoApi.DomainServices.Binders
{
    public class CommonServicesModule : Module
    {
        private readonly NeoApiSettings _settings;

        public CommonServicesModule(NeoApiSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new RpcClient(new Uri(_settings.NodeUrl)))
                .As<IClient>()
                .SingleInstance();

            builder.RegisterType<NeoSendRawTransaction>()
                .AsSelf();

            builder.RegisterInstance(new NeoScanRestService(_settings.NeoScanUrl))
                .As<INeoscanService>()
                .SingleInstance();

            builder.RegisterType<TransactionBroadcaster>()
                .As<ITransactionBroadcaster>();

            builder.RegisterType<TransactionBuilder>()
                .As<ITransactionBuilder>();

            builder.RegisterType<AddressValidator>()
                .As<IAddressValidator>();

            builder.RegisterType<WalletBalanceService>()
                .As<IWalletBalanceService>();

            builder.RegisterInstance(new FeeSettings
            {
                FixedFee = _settings.FixedFee
            }).SingleInstance();
        }
    }
}
