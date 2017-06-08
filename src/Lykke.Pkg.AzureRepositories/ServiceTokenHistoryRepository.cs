using System;
using System.Threading.Tasks;
using Lykke.AzureRepositories.Extentions;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class ServiceTokenHistoryEntity : TableEntity, IServiceTokenHistory
    {


        public string TokenId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        public string UserName { get; set; }
        public string KeyOne { get; set; }
        public string KeyTwo { get; set; }
        public string UserIpAddress { get; set; }
        
    }
    public class ServiceTokenHistoryRepository : IServiceTokenHistoryRepository
    {
        private readonly INoSQLTableStorage<ServiceTokenHistoryEntity> _tableStorage;

        

        public ServiceTokenHistoryRepository(INoSQLTableStorage<ServiceTokenHistoryEntity> tableStorage)
        {
            _tableStorage = tableStorage;
           

        }

        public async Task SaveTokenHistoryAsync(IServiceTokenEntity token, string userName, string userIpAddress)
        {
            var th = new ServiceTokenHistoryEntity
            {
                RowKey = DateTime.UtcNow.StorageString(),
                UserName = userName,
                KeyOne = token.SecurityKeyOne,
                KeyTwo = token.SecurityKeyTwo,
                TokenId = token.RowKey,
                UserIpAddress = userIpAddress
            };

            await _tableStorage.InsertOrMergeAsync(th);
        }
    }
}
