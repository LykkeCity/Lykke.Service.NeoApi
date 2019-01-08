using NeoModules.Core;
using NeoModules.NEP6.Transactions;

namespace Lykke.Service.NeoApi.Helpers.Transaction
{
    public class TransferOutputContract
    {
        public string AssetId { get; set; }

        public string ScriptHash { get; set; }

        public string Value { get; set; }

        public static TransferOutputContract FromDomain(TransactionOutput source)
        {
            return new TransferOutputContract
            {
                AssetId = source.AssetId.ToString(),
                ScriptHash = source.ScriptHash.ToString(),
                Value = source.Value.ToString()
            };
        }

        public TransferOutput ToDomain()
        {
            return new TransferOutput
            {
                AssetId = UInt256.Parse(AssetId),
                ScriptHash = UInt160.Parse(ScriptHash),
                Value = BigDecimal.Parse(Value, 8)
            };
        }
    }
}
