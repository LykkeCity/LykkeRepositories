using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class KeyValueEntity : TableEntity, IKeyValueEntity
    {

        public static string GeneratePartitionKey()
        {
            return "K";
        }

        public string Value { get; set; }


    }

    

    public class KeyValuesRepository : IKeyValuesRepository
    {
        private readonly INoSQLTableStorage<KeyValueEntity> _tableStorage;
        private readonly IKeyValueHistoryRepository _history;

        public KeyValuesRepository(INoSQLTableStorage<KeyValueEntity> tableStorage, IKeyValueHistoryRepository history)
        {
            _tableStorage = tableStorage;
            _history = history;
        }
        public async Task<Dictionary<string, string>> GetAsync()
        {
            var entries = await GetKeyValueAsync();
            return entries.Cast<KeyValueEntity>().ToDictionary(itm => itm.RowKey, itm => itm.Value);
        }

        public async Task<IEnumerable<IKeyValueEntity>> GetKeyValueAsync()
        {
            var pk = KeyValueEntity.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(pk);
        }

        public async Task<bool> UpdateKeyValueAsync(IEnumerable<IKeyValueEntity> keyValueList)
        {
            try
            {
                foreach (var tableEntity in keyValueList)
                {
                    var te = (KeyValueEntity) tableEntity;
                    if (te.PartitionKey == null)
                    {
                        te.PartitionKey = KeyValueEntity.GeneratePartitionKey();
                    }
                    await _tableStorage.InsertOrMergeAsync(te);
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task DeleteKeyValueWithHistoryAsync(string keyValueId, string description, string userName, string userIpAddress)
        {
            var kvItem = await _tableStorage.GetDataAsync(KeyValueEntity.GeneratePartitionKey(), keyValueId);
            if (kvItem != null)
            {
                await _tableStorage.DeleteAsync(kvItem);
                await _history.DeleteKeyValueHistoryAsync(keyValueId, description, userName, userIpAddress);
            }
            
        }
    }
}
