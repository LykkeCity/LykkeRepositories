using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.AzureRepositories.Extentions;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class UserActionHistoryEntity : TableEntity, IUserActionHistoryEntity
    {
        public string UserEmail { get; set; }
        public DateTime ActionDate { get; set; }
        public string IpAddress { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Params { get; set; }

        public static string GeneratePartitionKey()
        {
            return "UAH";
        }

        public string GetRawKey()
        {
            return ActionDate.StorageString();
        }

        public static UserActionHistoryEntity Create(IUserActionHistoryEntity entity)
        {
            return new UserActionHistoryEntity
            {
                UserEmail = entity.UserEmail,
                ActionDate = entity.ActionDate,
                IpAddress = entity.IpAddress,
                ControllerName = entity.ControllerName,
                ActionName = entity.ActionName,
                Params = entity.Params,
                PartitionKey = GeneratePartitionKey(),
                RowKey = entity.ActionDate.StorageString()
            };
        }
    }
    public class UserActionHistoryRepository : IUserActionHistoryRepository
    {
        private readonly INoSQLTableStorage<UserActionHistoryEntity> _tableStorage;
        private readonly IBlobStorage _blobStorage;
        private readonly string _container;

        public UserActionHistoryRepository(INoSQLTableStorage<UserActionHistoryEntity> tableStorage, IBlobStorage blobStorage, string container)
        {
            _tableStorage = tableStorage;
            _blobStorage = blobStorage;
            _container = container;
        }

        public async Task SaveUserActionHistoryAsync(IUserActionHistoryEntity userActionHistory)
        {
            var entity = UserActionHistoryEntity.Create(userActionHistory);
            if (!string.IsNullOrEmpty(entity.Params))
            {
                var parms = entity.Params;
                entity.Params = Guid.NewGuid().ToString();

                if (!string.IsNullOrEmpty(parms))
                {
                    var data = Encoding.UTF8.GetBytes(parms);
                    await _blobStorage.SaveBlobAsync(_container, entity.Params, data);
                }
               
               
            }
           

            await _tableStorage.InsertOrMergeAsync(entity);
        }
    }
}
