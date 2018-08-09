using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.NEO.Api.Core.Settings
{
    public interface INeoService
    {
        List<Asset> ListAssets(string password, string name);
        List<Address> ListAddress(string password, string name);
        List<Keys> ListPublicKeys(string password, string name);
        Address GetNeoAddress(string address, string name);
    }
}
