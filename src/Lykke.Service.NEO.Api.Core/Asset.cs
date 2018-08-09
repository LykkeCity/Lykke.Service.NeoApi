using System;
using System.Collections.Generic;
using System.Text;
using Neo;
using Neo.Core;

namespace Lykke.Service.NEO.Api.Core
{
    public class Asset
    {
        public AssetState AssetState { get; set; }
        public Fixed8 Balance { get; set; }
        public Fixed8 Confirmed { get; set; }
        public UInt256 AssetId { get; set; }
        public string Name { get; set; }

    }
}
