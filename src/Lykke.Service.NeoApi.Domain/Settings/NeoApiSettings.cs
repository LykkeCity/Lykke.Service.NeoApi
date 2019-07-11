using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.NeoApi.Domain.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class NeoApiSettings
    {
        public DbSettings Db { get; set; }

        [HttpCheck("get_height")]
        public string NeoScanUrl { get; set; }

        public string NodeUrl { get; set; }

        [Optional]
        public int MaxFreeTransactionSize { get; set; } = 1024;

        [Optional]
        public long FeePerExtraByte { get; set; } = 1000;

        public TimeSpan UpdateBalancesTimerPeriod { get; set; }

        public TimeSpan DetectTransactionsTimerPeriod { get; set; }

        public TimeSpan SpentOutputsExpiration { get; set; }
        public TimeSpan SpentOutputsExpirationTimerPeriod { get; set; }
    }
}
