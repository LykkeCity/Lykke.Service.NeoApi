using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Domain.Addresses;
using Lykke.Service.NEO.Api.Core.Settings;
using Lykke.Service.NEO.Api.Services.Helpers;
using Lykke.Service.NEO.Api.Services.Models;
using Neo;
using Neo.Core;
using Neo.Implementations.Wallets.EntityFramework;
using Neo.Implementations.Wallets.NEP6;
using Neo.Network;
using Neo.Network.RPC;
using Neo.Wallets;
using Newtonsoft.Json;

namespace Lykke.Service.NEO.Api.Services
{
    public class NeoService : INeoService
    {
        protected LocalNode LocalNode { get; private set; }
        private readonly ILog _log; 
        private readonly IAddressRepository _addressRepository;

        public NeoService(ILog log, 
            IAddressRepository addressRepository)
        {
            _log = log;
            _addressRepository = addressRepository;
        }

        public Asset GetAsset(string assetId, string password, string name)
        {
            var walletPath = $"{Settings.Default.Paths.WalletPath}\\{name}.json";
            var wallet = new NEP6Wallet(walletPath);
            var assets = new List<Asset>();
            try
            {
                wallet.Unlock(password);
                //var asset = wallet.GetCoins().Where(p => !p.State.HasFlag(CoinState.Spent))
                //.GroupBy(p => p.Output.AssetId, (k, g) => new
                //{
                //    Asset = Blockchain.Default.GetAssetState(k),
                //    Balance = g.Sum(p => p.Output.Value),
                //    Confirmed = g.Where(p => p.State.HasFlag(CoinState.Confirmed)).Sum(p => p.Output.Value)
                //}).Select(item => new Asset
                //{
                //    AssetId = item.Asset.AssetId,
                //    AssetState = item.Asset,
                //    Balance = item.Balance,
                //    Confirmed = item.Confirmed,
                //    Name = item.Asset.GetName()
                //}).Where(x => x.AssetId == assetId);
                foreach (var item in wallet.GetCoins().Where(p => !p.State.HasFlag(CoinState.Spent))
                .GroupBy(p => p.Output.AssetId, (k, g) => new
                {
                    Asset = Blockchain.Default.GetAssetState(k),
                    Balance = g.Sum(p => p.Output.Value),
                    Confirmed = g.Where(p => p.State.HasFlag(CoinState.Confirmed)).Sum(p => p.Output.Value)
                }))
                {
                    assets.Add(new Asset
                    {
                        AssetId = item.Asset.AssetId,
                        AssetState = item.Asset,
                        Balance = item.Balance,
                        Confirmed = item.Confirmed,
                        Name = item.Asset.GetName()
                    });
                }
            }
            catch (CryptographicException)
            {
                Console.WriteLine($"failed to open file \"{walletPath}\"");
            }

            return null;
        }

        public List<Asset> ListAssets(string password, string name)
        {
            var walletPath = $"{Settings.Default.Paths.WalletPath}\\{name}.json";
            var wallet = new NEP6Wallet(walletPath);
            var assets = new List<Asset>();
            try
            {
                wallet.Unlock(password);
                
                foreach (var item in wallet.GetCoins().Where(p => !p.State.HasFlag(CoinState.Spent))
                .GroupBy(p => p.Output.AssetId, (k, g) => new
                {
                    Asset = Blockchain.Default.GetAssetState(k),
                    Balance = g.Sum(p => p.Output.Value),
                    Confirmed = g.Where(p => p.State.HasFlag(CoinState.Confirmed)).Sum(p => p.Output.Value)
                }))
                {
                    assets.Add(new Asset
                    {
                        AssetId = item.Asset.AssetId,
                        AssetState = item.Asset,
                        Balance = item.Balance,
                        Confirmed = item.Confirmed,
                        Name = item.Asset.GetName()
                    });
                }
            }
            catch (CryptographicException)
            {
                Console.WriteLine($"failed to open file \"{walletPath}\"");
            }

            return assets;
        }

        public bool IsValidAddress(string address)
        {
            UInt160 scriptHash; 
            try
            {                
                //LocalNode.Start(Settings.Default.P2P.Port, Settings.Default.P2P.WsPort);
                //var rpc = new RpcServer(LocalNode);
                //rpc.Start(Settings.Default.RPC.Port, 
                //            Settings.Default.RPC.SslCert, 
                //            Settings.Default.RPC.SslCertPassword);
                
                scriptHash = Wallet.ToScriptHash(address);
            }
            catch
            {
                scriptHash = null;
            }

            return scriptHash != null;
        }

        private static Wallet OpenWallet(string path, string password)
        {
            if (Path.GetExtension(path) == ".db3")
            {
                return UserWallet.Open(path, password);
            }
            else
            {
                var nep6wallet = new NEP6Wallet(path);
                nep6wallet.Unlock(password);
                return nep6wallet;
            }
        }

        public List<Address> ListAddress(string password, string name)
        {
            var walletPath = $"{Settings.Default.Paths.WalletPath}\\{name}.json";
            var wallet = new NEP6Wallet(walletPath);
            var addresses = new List<Address>();
            try
            {
                wallet.Unlock(password);               
                
                addresses.AddRange(wallet.GetAccounts()
                    .Where(p => !p.WatchOnly)
                    .Select(p => p.Contract)
                    .Select(contract => new Address
                    {
                        ContractAddress = contract.Address,
                        IsStandard = contract.IsStandard,
                        Type = contract.IsStandard ? "Standard" : "Nonstandard"
                    }).ToList());
            }
            catch (CryptographicException)
            {
                Console.WriteLine($"failed to open file \"{walletPath}\"");
            }
            return addresses;
        }

        public List<Keys> ListPublicKeys(string password, string name)
        {
            var walletPath = $"{Settings.Default.Paths.WalletPath}\\{name}.json";
            var wallet = new NEP6Wallet(walletPath);
            var publicKeys = new List<Keys>();
            try
            {
                wallet.Unlock(password);
                publicKeys.AddRange(wallet.GetAccounts()
                    .Where(p => p.HasKey)
                    .Select(p => p.GetKey())
                    .Select(key => new Keys()
                    {
                        PublicKey = key.PublicKey
                    }));
            }
            catch (CryptographicException)
            {
                Console.WriteLine($"failed to open file \"{walletPath}\"");
            }

            return publicKeys;
        }

        public async Task<bool> TryCreateBalanceAddressAsync(string address)
        {
            ImportAddress(address);

            return await _addressRepository.CreateBalanceAddressIfNotExistsAsync(address);
        }

        public void ImportAddress(string address)
        {
            var response = JsonRpcHelper.InvokeMethod(RPCOperations.ValidateAddress.GetEnumFieldDescription(), address).ToString();
            var addressInfo = JsonConvert.DeserializeObject<AddressInfo>(response);
            if (!addressInfo.result.isvalid)
            {
                throw new InvalidOperationException($"Invalid NEO address: {address}");
            }             

            //TODO: Import address in TestNet
        }

        public bool ValidateAddressAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return false;
            }
             
            var response = JsonRpcHelper.InvokeMethod(RPCOperations.ValidateAddress.GetEnumFieldDescription(), address).ToString();
            var addressInfo = JsonConvert.DeserializeObject<AddressInfo>(response);
           
            return addressInfo.result.isvalid;
        }

        public async Task<bool> TryDeleteBalanceAddressAsync(string address)
        {
            return await _addressRepository.DeleteBalanceAddressIfExistsAsync(address);
        }
        
    }
}
