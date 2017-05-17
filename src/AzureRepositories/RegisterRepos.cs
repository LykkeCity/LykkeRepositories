using Lykke.AzureRepositories.Azure.Tables;
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
               new UserRepository(new AzureTableStorage<UserEntity>(connectionString, "User", log)));

            services.AddSingleton<IServiceTokenRepository>(
              new ServiceTokenRepository(new AzureTableStorage<ServiceTokenEntity>(connectionString, "ServiceToken", log)));
        }
    }

}
