using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{

    public class MerchantEntity : TableEntity, IMerchantEntity
    {
        public static string GeneratePartitionKey()
        {
            return "M";
        }

        public string MerchantId
        {
            get => RowKey;
            set => RowKey = value;
        }

        public string MerchantName { get; set; }
        public string PublicKey { get; set; }
        public string ApyKey { get; set; }
    }

    public class MerchantRepository : IMerchantRepository
    {
        private readonly INoSQLTableStorage<MerchantEntity> _tableStorage;

        public MerchantRepository(INoSQLTableStorage<MerchantEntity> tableStorage)
        {
            _tableStorage = tableStorage;

        }

        public async Task<IMerchantEntity> GetAsync(string merchantId)
        {
            var pk = TokenEntity.GeneratePartitionKey();
             return await _tableStorage.GetDataAsync(pk, merchantId);
        }

        public async Task<IEnumerable<IMerchantEntity>> GetAllAsync()
        {
            var pk = TokenEntity.GeneratePartitionKey();

            return await _tableStorage.GetDataAsync(pk);
        }
    }
}
