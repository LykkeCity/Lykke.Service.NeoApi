namespace Lykke.Service.NeoApi.DomainServices.Transaction
{
    public class FeeSettings
    {
        public int MaxFreeTransactionSize { get; set; }

        public long FeePerExtraByte { get; set; }
    }
}
