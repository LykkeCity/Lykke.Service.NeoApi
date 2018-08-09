using System;
using System.Collections.Generic;
using System.Text;
using Neo;
using Neo.Core;

namespace Lykke.Service.NEO.Api.Core
{
    public class Address
    {
        public string Type { get; set; }
        public bool IsStandard { get; set; }
        public string ContractAddress { get; set; }
    }
}
