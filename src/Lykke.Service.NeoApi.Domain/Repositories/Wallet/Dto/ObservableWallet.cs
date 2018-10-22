namespace Lykke.Service.NeoApi.Domain.Repositories.Wallet.Dto
{
    public class ObservableWallet:IObservableWallet
    {
        public string Address { get; set; }

        public static ObservableWallet Create(string address)
        {
            return new ObservableWallet
            {
                Address = address
            };
        }
    }
}
