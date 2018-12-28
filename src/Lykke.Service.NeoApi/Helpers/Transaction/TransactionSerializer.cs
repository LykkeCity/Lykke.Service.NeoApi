using System;
using Common;
using Lykke.Service.NeoApi.Helpers.Transaction.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Service.NeoApi.Helpers.Transaction
{
    public static class TransactionSerializer
    {
        public static string Serialize(NeoModules.NEP6.Transactions.Transaction transaction)
        {
            return TransactionContract.FromDomain(transaction).ToJson();
        }

        public static NeoModules.NEP6.Transactions.Transaction Deserialize(string source)
        {
            try
            {
                return source.DeserializeJson<TransactionContract>().ToDomain();
            }
            catch (JsonReaderException e)
            {
                throw new InvalidTransactionException();
            }
        }
    }
}
