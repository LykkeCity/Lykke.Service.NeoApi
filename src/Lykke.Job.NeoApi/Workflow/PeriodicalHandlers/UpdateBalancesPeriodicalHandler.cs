using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.NeoApi.Domain.Repositories.Wallet;
using Lykke.Service.NeoApi.Domain.Services.Address;

namespace Lykke.Job.NeoApi.Workflow.PeriodicalHandlers
{
    public class UpdateBalancesPeriodicalHandler:IStartable, IStopable
    {
        private readonly IObservableWalletRepository _observableWalletRepository;
        private readonly IWalletBalanceService _walletBalanceService;
        private readonly ILog _log;
        private readonly TimerTrigger _timerTrigger;

        public UpdateBalancesPeriodicalHandler(
            ILogFactory logFactory,
            TimeSpan timerPeriod, 
            IWalletBalanceService walletBalanceService, 
            IObservableWalletRepository observableWalletRepository)
        {
            _walletBalanceService = walletBalanceService;
            _observableWalletRepository = observableWalletRepository;

            _log = logFactory.CreateLog(this);

            _timerTrigger = new TimerTrigger(nameof(UpdateBalancesPeriodicalHandler), timerPeriod, logFactory);
            _timerTrigger.Triggered += (trigger, args, token) => Execute();
        }

        public async Task Execute()
        {
            foreach (var wallet in await _observableWalletRepository.GetAll())
            {
                try
                {
                    await _walletBalanceService.UpdateBalance(wallet.Address);
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            }
        }

        public void Start()
        {
            _log.Info($"Starting {nameof(UpdateBalancesPeriodicalHandler)}");

            _timerTrigger.Start();
        }

        public void Dispose()
        {
            _timerTrigger.Dispose();
        }

        public void Stop()
        {
            _timerTrigger.Stop();
        }
    }
}
