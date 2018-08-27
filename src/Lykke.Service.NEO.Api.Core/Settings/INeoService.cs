using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.NEO.Api.Core.Settings
{
    public interface INeoService
    {
        List<Asset> ListAssets(string password, string name);
        List<Address> ListAddress(string password, string name);
        List<Keys> ListPublicKeys(string password, string name);
        bool IsValidAddress(string address);
        Task<bool> TryCreateBalanceAddressAsync(string address);
        void ImportAddress(string address);
        bool ValidateAddressAsync(string address);
        Task<bool> TryDeleteBalanceAddressAsync(string address);
    }
}
