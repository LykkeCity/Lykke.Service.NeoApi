using Autofac;
using AzureStorage.Tables;
using Lykke.Logs;
using Lykke.Service.NeoApi.AzureRepositories.Operation;
using Lykke.Service.NeoApi.AzureRepositories.Transaction;
using Lykke.Service.NeoApi.AzureRepositories.Wallet;
using Lykke.Service.NeoApi.Domain.Repositories.Operation;
using Lykke.Service.NeoApi.Domain.Repositories.Transaction;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.NeoApi.AzureRepositories.Binders
{
    public class AzureRepositoriesModule : Module
    {
        private readonly NeoApiSettings _settings;
        private readonly IReloadingManager<NeoApiSettings> _settingsManager;

        public AzureRepositoriesModule(NeoApiSettings settings, IReloadingManager<NeoApiSettings> settingsManager)
        {
            _settings = settings;
            _settingsManager = settingsManager;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var connString = _settingsManager.Nested(p => p.Db.DataConnString);

            builder.Register(p => new OperationRepository(
                    AzureTableStorage<OperationEntity>.Create(
                        connString,
                        "NeoOperations",
                        p.Resolve<LogFactory>())))
                .As<IOperationRepository>();

            builder.Register(p => new ObservableOperationRepository(
                    AzureTableStorage<ObservableOperationEntity>.Create(
                        connString,
                        "NeoObservableOperations",
                        p.Resolve<LogFactory>())))
                .As<IObservableOperationRepository>();

            builder.Register(p => new UnconfirmedTransactionRepository(
                    AzureTableStorage<UnconfirmedTransactionEntity>.Create(
                        connString,
                        "NeoUnconfirmedTransactions",
                        p.Resolve<LogFactory>())))
                .As<IUnconfirmedTransactionRepository>();
            
            builder.Register(p => new ObservableWalletRepository(
                    AzureTableStorage<ObservableWalletEntity>.Create(
                        connString,
                        "NeoObservableWallets",
                        p.Resolve<LogFactory>())))
                .As<IObservableWalletRepository>();
            
            builder.Register(p => new WalletBalanceRepository(
                    AzureTableStorage<WalletBalanceEntity>.Create(
                        connString,
                        "NeoWalletBalances",
                        p.Resolve<LogFactory>())))
                .As<IWalletBalanceRepository>();
        }
    }
}
