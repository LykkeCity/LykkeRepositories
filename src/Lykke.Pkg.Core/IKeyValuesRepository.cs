using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Core
{
    public interface IKeyValuesRepository
    {
        Task<Dictionary<string, string>> GetAsync();
        Task<IEnumerable<IKeyValueEntity>> GetKeyValueAsync();
        Task<bool> UpdateKeyValueAsync(IEnumerable<IKeyValueEntity> keyValueList);

        Task DeleteKeyValueWithHistoryAsync(string keyValueId, string description, string userName,
            string userIpAddress);
    }

    public interface IKeyValueEntity : IEntity
    {
        string Value { get; set; }
    }
}
