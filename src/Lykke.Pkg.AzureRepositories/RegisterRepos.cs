using System.Collections.Immutable;
using Lykke.AzureRepositories.Azure.Tables;
using Lykke.AzureRepositories.CandleHistory;
using Lykke.AzureRepositories.Dictionaries;
using Lykke.AzureRepositories.Exceptions;
using Lykke.AzureRepositories.Log;
using Lykke.Common.Entities.Dictionaries;
using Lykke.Core;
using Lykke.Core.Azure.Blob;
using Lykke.Core.Log;
using Lykke.Domain.Prices.Repositories;
using Microsoft.Extensions.DependencyInjection;


namespace Lykke.AzureRepositories
{
    public static class RegisterReposExt
    {
        public static void RegisterRepositories(this IServiceCollection services, string connectionString, global::Common.Log.ILog log)
        {
            services.RegisterRepositories(connectionString, new CommonLogAdapter(log));
        }

        public static void RegisterRepositories(this IServiceCollection services, string connectionString, ILog log)
        {
            services.RegisterRepositories(connectionString, connectionString, log);
        }

        public static void RegisterRepositories(this IServiceCollection services, string connectionString, string userConnectionString, global::Common.Log.ILog log)
        {
            RegisterRepositories(services, connectionString, userConnectionString, log == null ? null : new CommonLogAdapter(log));
        }

        public static void RegisterRepositories(this IServiceCollection services, string connectionString, string userConnectionString, ILog log)
        {

            services.AddSingleton<IJsonDataRepository>(
                 new JsonDataRepository(new AzureBlobStorage(connectionString), "settings", "history", "generalsettings.json")
            //new JsonDataRepository(new AzureBlobStorage(connectionString), "history", string.Empty, "generalsettings.json_2017-05-05 14:18:08.873")

            );

            services.AddSingleton<ITokensRepository>(
                new TokensRepository(new AzureTableStorage<TokenEntity>(connectionString, "Tokens", log))
            );

            services.AddSingleton<IKeyValuesRepository>(
                new KeyValuesRepository(new AzureTableStorage<KeyValueEntity>(connectionString, "KeyValues", log)));

            services.AddSingleton<ILockRepository>(
                new LockRepository(new AzureTableStorage<LockEntity>(connectionString, "Lock", log)));

            services.AddSingleton<IUserRepository>(
               new UserRepository(new AzureTableStorage<UserEntity>(userConnectionString, "User", log)));

            services.AddSingleton<IServiceTokenRepository>(
              new ServiceTokenRepository(new AzureTableStorage<ServiceTokenEntity>(connectionString, "ServiceToken", log)));

            services.AddSingleton<IAccountTokenHistoryRepository>(
              new AccountTokenHistoryRepository(new AzureTableStorage<AccountTokenHistoryEntity>(connectionString, "AccessTokenHistory", log)));

            services.AddSingleton<IMarginTradingAssetsRepository>(
                new MarginTradingAssetsRepository(new AzureTableStorage<Dictionaries.MarginTradingAsset>(connectionString, "MarginTradingAssets", log)));

            services.AddSingleton<IMerchantRepository>(
                new MerchantRepository(new AzureTableStorage<MerchantEntity>(connectionString, "Merchants", log)));

            services.AddSingleton<IUserSignInHistoryRepository>(
                new UserSignInHistoryRepository(new AzureTableStorage<UserSignInHistoryEntity>(userConnectionString, "UserSignInHistory", log)));

            services.AddSingleton<IAssertPairHistoryRepository>(
                new AssertPairHistoryRepository(new AzureTableStorage<AssertPairHistoryEntity>(connectionString, "AssertPairHistory", log)));

            services.AddSingleton<IMerchantWalletRepository>(
                new MerchantWalletRepository(new AzureTableStorage<MerchantWalletEntity>(connectionString, "MerchantWallets", log)));

            services.AddSingleton<IMerchantWalletHistoryRepository>(
                new MerchantWalletHistoryRepository(new AzureTableStorage<MerchantWalletHistoryEntity>(connectionString, "MerchantWalletsHistory", log))); 

            services.AddSingleton<IKeyValueHistoryRepository>(
                new KeyValueHistoryRepository(new AzureTableStorage<KeyValueHistory>(connectionString, "KeyValueHistory", log),
                    new AzureBlobStorage(connectionString), "keyvaluehistory"));

            services.AddSingleton<IServiceTokenHistoryRepository>(
                new ServiceTokenHistoryRepository(new AzureTableStorage<ServiceTokenHistoryEntity>(connectionString, "ServiceTokenHistory", log)));

            services.AddSingleton<IAccessDataRepository>(
                new AccessDataRepository(new AzureBlobStorage(connectionString), "access", "accessHistory", "accessHistory.json")
            );

            services.AddSingleton<IMerchantPayRequestRepository>(
                new MerchantPayRequestRepository(new AzureTableStorage<MerchantPayRequest>(connectionString, "MerchantPayRequest", log)));
        }


        public static void RegisterCandleHistoryRepository(this IServiceCollection services, IImmutableDictionary<string, string> assetPairConnectionStrings, 
            global::Common.Log.ILog log)
        {
            services.RegisterCandleHistoryRepository(assetPairConnectionStrings, new CommonLogAdapter(log));
        }

        public static void RegisterCandleHistoryRepository(this IServiceCollection services, IImmutableDictionary<string, string> assetPairConnectionStrings, ILog log)
        {
            services.AddSingleton<ICandleHistoryRepository>(new CandleHistoryRepositoryResolver((assetPair, tableName) =>
            {
                if (!assetPairConnectionStrings.TryGetValue(assetPair, out string assetConnectionString) || string.IsNullOrEmpty(assetConnectionString))
                {
                    throw new ConfigurationException($"Connection string for asset pair '{assetPair}' is not specified.");
                }

                var storage = new AzureTableStorage<CandleTableEntity>(assetConnectionString, tableName, log);

                // Preload table info
                storage.GetDataAsync(assetPair, "1900-01-01").Wait();

                return storage;
            }));
        }
    }
}
