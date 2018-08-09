namespace Lykke.Service.NEO.Api.Core.Domain.Balance
{
    public interface IBalancePositive
    {
        string Address { get;  }
        decimal Amount { get; set; }
        long Block { get; set; }
    }
}
