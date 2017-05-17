using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class UserEntity : TableEntity, IUserEntity
    {
        public static string GeneratePartitionKey()
        {
            return "U";
        }

        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public bool Admin { get; set; }

    }


    public class UserRepository : IUserRepository
    {
        private readonly INoSQLTableStorage<UserEntity> _tableStorage;

        public UserRepository(INoSQLTableStorage<UserEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IUserEntity> GetUserByUserEmail(string userEmail)
        {
            var pk = UserEntity.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(pk, userEmail.ToLower());
        }

        public async Task<IUserEntity> GetUserByUserEmail(string userEmail, string passwordHash)
        {
            var pk = UserEntity.GeneratePartitionKey();
            var result = await _tableStorage.GetDataAsync(pk, userEmail.ToLower());
            if (result == null)
            {
                return null;
            }
            return result.PasswordHash.Equals(passwordHash) ? result : null;
        }

        public async Task<bool> SaveUser(IUserEntity user)
        {
            try
            {
                var te = (UserEntity)user;
                if (te.PartitionKey == null)
                {
                    te.PartitionKey = UserEntity.GeneratePartitionKey();
                }
                await _tableStorage.InsertOrMergeAsync(te);
            }


            catch
            {
                return false;
            }

            return true;
        }

        public async Task<List<IUserEntity>> GetUsers()
        {
            var pk = UserEntity.GeneratePartitionKey();
            return (await _tableStorage.GetDataAsync(pk)).Cast<IUserEntity>().ToList();
        }

        public async Task<bool> RemoveUser(string userEmail)
        {
            try
            {
                await _tableStorage.DeleteAsync(UserEntity.GeneratePartitionKey(), userEmail);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
