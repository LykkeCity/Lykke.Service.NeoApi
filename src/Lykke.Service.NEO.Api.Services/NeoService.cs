using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Lykke.Service.NEO.Api.Core;
using Lykke.Service.NEO.Api.Core.Settings;
using Neo;
using Neo.Core;
using Neo.Implementations.Wallets.NEP6;

namespace Lykke.Service.NEO.Api.Services
{
    public class NeoService : INeoService
    {
        public NeoService()
        {

        }

        public List<Asset> ListAssets(string password, string name)
        {
            var walletPath = $"{Settings.Default.Paths.WalletPath}\\{name}.json";
            var wallet = new NEP6Wallet(walletPath);
            var assets = new List<Asset>();
            try
            {
                wallet.Unlock(password);
                assets.AddRange(wallet.GetCoins()
                    .Where(p => !p.State.HasFlag(CoinState.Spent))
                    .GroupBy(p => p.Output.AssetId, (k, g) => new
                    {
                        Asset = Blockchain.Default.GetAssetState(k),
                        Balance = g.Sum(p => p.Output.Value),
                        Confirmed = g.Where(p => p.State.HasFlag(CoinState.Confirmed)).Sum(p => p.Output.Value)
                    })
                    .Select(item => new Asset
                    {
                        AssetId = item.Asset.AssetId,
                        AssetState = item.Asset,
                        Balance = item.Balance,
                        Confirmed = item.Confirmed,
                        Name = item.Asset.GetName()
                    }));
            }
            catch (CryptographicException)
            {
                Console.WriteLine($"failed to open file \"{walletPath}\"");
            }

            return assets;
        }

        public Address GetNeoAddress(string address, string name)
        {
            var walletPath = $"{Settings.Default.Paths.WalletPath}\\{name}.json";
            var wallet = new NEP6Wallet(walletPath);
            var account = wallet.CreateAccount();
            address = account.Address;
            wallet.Save();
            return new Address
            {
                ContractAddress = address
            };
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
                    }));
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
    }
}
