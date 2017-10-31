using System;
using System.Collections.Generic;
using System.Linq;
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


        public string KeyValueId { get; set; }
        public string NewValue { get; set; }
        public string KeyValuesSnapshot { get; set; }
        public string UserName { get; set; }
        public string UserIpAddress { get; set; }
    }
    public class KeyValueHistoryRepository : IKeyValueHistoryRepository
    {
        private readonly INoSQLTableStorage<KeyValueHistory> _tableStorage;
        private readonly IBlobStorage _blobStorage;
        private readonly string _container;

        public KeyValueHistoryRepository(INoSQLTableStorage<KeyValueHistory> tableStorage,
            IBlobStorage blobStorage, string container)
        {
            _tableStorage = tableStorage;
            _blobStorage = blobStorage;
            _container = container;
        }

        public async Task SaveKeyValueHistoryAsync(string keyValueId, string newValue, string keyValues, string userName, string userIpAddress)
        {
            var th = new KeyValueHistory
            {
                KeyValueId = keyValueId,
                NewValue = newValue,
                PartitionKey = KeyValueHistory.GeneratePartitionKey(),
                RowKey = DateTime.UtcNow.StorageString(),
                UserName = userName,
                UserIpAddress = userIpAddress
            };

            th.KeyValuesSnapshot = $"{th.UserName}_{th.RowKey}_{th.UserIpAddress}";

            await _blobStorage.SaveBlobAsync(_container, th.KeyValuesSnapshot, Encoding.UTF8.GetBytes(keyValues));

            await _tableStorage.InsertOrMergeAsync(th);
        }

        public async Task DeleteKeyValueHistoryAsync(string keyValueId, string description, string userName, string userIpAddress)
        {
            var th = new KeyValueHistory
            {
                KeyValueId = keyValueId,
                PartitionKey = KeyValueHistory.GeneratePartitionKey(),
                RowKey = DateTime.UtcNow.StorageString(),
                UserName = userName,
                UserIpAddress = userIpAddress
            };

            th.KeyValuesSnapshot = $"{th.UserName}_{th.RowKey}_{th.UserIpAddress}";

            await _blobStorage.SaveBlobAsync(_container, th.KeyValuesSnapshot, Encoding.UTF8.GetBytes(description));

            await _tableStorage.InsertOrMergeAsync(th);
        }

        public async Task<List<IKeyValueHistory>> GetHistoryByKeyValueAsync(string keyValueId)
        {
            if (string.IsNullOrEmpty(keyValueId))
            {
                return new List<IKeyValueHistory>();
            }

            var hist = await _tableStorage.GetDataAsync();
            var history = from h in hist
                where keyValueId.Equals(h.KeyValueId)
                orderby h.Timestamp descending
                select (IKeyValueHistory)h;

            return history.ToList();
        }
    }
}
