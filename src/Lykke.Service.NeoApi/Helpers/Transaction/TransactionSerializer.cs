using System;
using Common;
using Lykke.Service.NeoApi.Helpers.Transaction.Exceptions;
using NeoModules.NEP6.Transactions;
using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.Helpers.Transaction
{
    public static class TransactionSerializer
    {
        public static string Serialize(NeoModules.NEP6.Transactions.Transaction transaction, TransactionType type)
        {
            return TransactionTypeWrapperContract.Create(type, TransactionContract.FromDomain(transaction).ToJson())
                .ToJson().ToBase64();
        }

        public static (NeoModules.NEP6.Transactions.Transaction transaction, TransactionType type) Deserialize(string source)
        {
            try
            {
                var wrapper = source.Base64ToString().DeserializeJson<TransactionTypeWrapperContract>();

                var type = Enum.Parse<TransactionType>(wrapper.Type);

                return (wrapper.Data.DeserializeJson<TransactionContract>().ToDomain(type), type);
            }
            catch (Exception e) when(e is JsonReaderException || e is FormatException)
            {
                throw new InvalidTransactionException();
            }
        }
    }
}
