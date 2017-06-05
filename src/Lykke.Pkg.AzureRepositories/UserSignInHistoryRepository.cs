using System;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class UserSignInHistoryEntity : TableEntity, IUserSignInHistoryEntity
    {
        public static string GeneratePartitionKey()
        {
            return "UH";
        }

        public string GetRawKey()
        {
            return SignInDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public string UserEmail { get; set; }

        public DateTime SignInDate { get; set; }

        public string IpAddress { get; set; }
    }

    public class UserSignInHistoryRepository : IUserSignInHistoryRepository
    {

        private readonly INoSQLTableStorage<UserSignInHistoryEntity> _tableStorage;

        public UserSignInHistoryRepository(INoSQLTableStorage<UserSignInHistoryEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task SaveUserLoginHistoryAsync(IUserEntity user, string userIpAddress)
        {
            var uh = new UserSignInHistoryEntity
            {
                PartitionKey = UserSignInHistoryEntity.GeneratePartitionKey(),

                UserEmail = user.RowKey,
                SignInDate = DateTime.UtcNow,
                IpAddress = userIpAddress

            };

            uh.RowKey = uh.GetRawKey();

            await _tableStorage.InsertOrMergeAsync(uh);
        }
    }
}
