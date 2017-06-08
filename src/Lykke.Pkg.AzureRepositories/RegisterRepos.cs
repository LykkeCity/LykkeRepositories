using Lykke.AzureRepositories.Azure.Tables;
using Lykke.AzureRepositories.Dictionaries;
using Lykke.Common.Entities.Dictionaries;
using Lykke.Core;
using Lykke.Core.Azure.Blob;
using Lykke.Core.Log;
using Microsoft.Extensions.DependencyInjection;


namespace Lykke.AzureRepositories
{
    public static class RegisterReposExt
    {
        public static void RegisterRepositories(this IServiceCollection services, string connectionString, ILog log)
        {
            services.RegisterRepositories(connectionString, connectionString, log);
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

            services.AddSingleton<IKeyValueHistoryRepository>(
                new KeyValueHistoryRepository(new AzureTableStorage<KeyValueHistory>(connectionString, "KeyValueHistory", log),
                    new AzureBlobStorage(connectionString), "keyvaluehistory"));

            services.AddSingleton<IServiceTokenHistoryRepository>(
                new ServiceTokenHistoryRepository(new AzureTableStorage<ServiceTokenHistoryEntity>(connectionString, "ServiceTokenHistory", log)));

        }
    }

}
