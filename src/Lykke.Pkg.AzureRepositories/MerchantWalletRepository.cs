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
    public class MerchantWalletEntity : TableEntity, IMerchantWalletEntity
    {
        public string MerchantId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        public string WalletAddress
        {
            get => RowKey;
            set => RowKey = value;
        }

        public string Data { get; set; }

        public static MerchantWalletEntity Create(IMerchantWalletEntity merchantWallet)
        {
            return new MerchantWalletEntity
            {
                MerchantId = merchantWallet.MerchantId,
                Data = merchantWallet.Data,
                ETag = "*"
            };
        }
    }
    public class MerchantWalletRepository : IMerchantWalletRepository
    {
        private readonly INoSQLTableStorage<MerchantWalletEntity> _tableStorage;

        public MerchantWalletRepository(INoSQLTableStorage<MerchantWalletEntity> tableStorage)
        {
            _tableStorage = tableStorage;

        }

        public async Task SaveNewAddressAsync(IMerchantWalletEntity merchantWallet)
        {
            await _tableStorage.InsertOrMergeAsync(MerchantWalletEntity.Create(merchantWallet));
        }

        public async Task<IEnumerable<IMerchantWalletEntity>> GetAllAddressAsync()
        {
           return await _tableStorage.GetDataAsync();
        }

        public async Task<IMerchantWalletEntity> GetAllAddressOfMerchantAsync(string merchantId)
        {
            return (await _tableStorage.GetDataAsync(merchantId)).FirstOrDefault();
        }
    }
}
