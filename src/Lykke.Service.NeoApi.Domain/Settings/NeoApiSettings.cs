using System;
using JetBrains.Annotations;

namespace Lykke.Service.NeoApi.Domain.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class NeoApiSettings
    {
        public DbSettings Db { get; set; }

        public string NeoScanUrl { get; set; }

        public string NodeUrl { get; set; }

        public decimal FixedFee { get; set; }
        
        public int ConfirmationLevel { get; set; }

        public TimeSpan UpdateBalancesTimerPeriod { get; set; }

        public TimeSpan DetectTransactionsTimerPeriod { get; set; }
    }
}
