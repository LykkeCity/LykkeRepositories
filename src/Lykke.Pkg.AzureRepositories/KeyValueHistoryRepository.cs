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
    public class KeyValueHistory : TableEntity, IKeyValueHistory
    {
        public static string GeneratePartitionKey()
        {
            return "KVH";
        }


        public string KeyValuesSnapshot { get; set; }
        public string UserName { get; set; }
        public string UserIpAddress { get; set; }
    }
    public class KeyValueHistoryRepository : IKeyValueHistoryRepository
    {
        private readonly INoSQLTableStorage<KeyValueHistory> _tableStorage;

        public KeyValueHistoryRepository(INoSQLTableStorage<KeyValueHistory> tableStorage)
        {
            _tableStorage = tableStorage;

        }

        public async Task SaveKeyValueHistoryAsync(string keyValues, string userName, string userIpAddress)
        {
            var th = new KeyValueHistory
            {
                PartitionKey = KeyValueHistory.GeneratePartitionKey(),
                RowKey = DateTime.UtcNow.StorageString(),
                UserName = userName,
                KeyValuesSnapshot = keyValues,
                UserIpAddress = userIpAddress
            };

            await _tableStorage.InsertOrMergeAsync(th);
        }

        
    }
}
