using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Lykke.Service.NEO.Api.Services.Helpers
{
    // NOTE: See http://docs.neo.org/en-us/node/cli/2.7.6/api.html
    public enum RPCOperations
    {
        [Description("importaddress")]
        ImportAddress,
        [Description("dumpprivkey")]
        DumpPrivateKey,
        [Description("getaccountstate")]
        GetAccountState,
        [Description("getapplicationlog")]
        GetApplicationLog,
        [Description("getassetstate")]
        GetAssetState,
        [Description("getbalance")]
        GetBalance,
        [Description("getbestblockhash")]
        GetBestBlockhash,
        [Description("getblock")]
        GetBlock,
        [Description("getblockcount")]
        GetBlockCount,
        [Description("getblockhash")]
        GetBlockHash,
        [Description("getblocksysfee")]
        GetBlocksysfee,
        [Description("getconnectioncount")]
        GetConnectionCount,
        [Description("getcontractstate")]
        GetContractState,
        [Description("getnewaddress")]
        GetNewAddress,
        [Description("getrawmempool")]
        GetRawmempool,
        [Description("getrawtransaction")]
        GetRawTransaction,
        [Description("getstorage")]
        GetStorage,
        [Description("gettxout")]
        Gettxout,
        [Description("getpeers")]
        GetPeers,
        [Description("getvalidators")]
        GetValidators,
        [Description("getversion")]
        GetVersion,
        [Description("invoke")]
        Invoke,
        [Description("invokefunction")]
        InvokeFunction,
        [Description("invokescript")]
        InvokeScript,
        [Description("listaddress")]
        ListAddress,
        [Description("sendfrom")]
        SendFrom,
        [Description("sendrawtransaction")]
        SendRawTransaction,
        [Description("sendtoaddress")]
        SendToAddress,
        [Description("sendmany")]
        SendMany,
        [Description("validateaddress")]
        ValidateAddress
    }
}
