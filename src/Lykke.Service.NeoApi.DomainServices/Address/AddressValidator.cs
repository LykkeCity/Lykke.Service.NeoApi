using System;
using Lykke.Service.NeoApi.Domain.Services.Address;
using NeoModules.Core.KeyPair;

namespace Lykke.Service.NeoApi.DomainServices.Address
{
    internal class AddressValidator:IAddressValidator
    {
        public bool IsAddressValid(string address)
        {
            try
            {
                address.ToScriptHash();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
