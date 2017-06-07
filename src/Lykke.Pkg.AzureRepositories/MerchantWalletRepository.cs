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
    public class MerchantWallet : TableEntity, IMerchantWallet
    {
        public string MerchantId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        public string Data { get; set; }

        public static MerchantWallet Create(IMerchantWallet merchantWallet)
        {
            return new MerchantWallet
            {
                MerchantId = merchantWallet.MerchantId,
                Data = merchantWallet.Data,
                ETag = "*",
                RowKey = DateTime.UtcNow.StorageString()
            };
        }
    }
    public class MerchantWalletRepository : IMerchantWalletRepository
    {
        private readonly INoSQLTableStorage<MerchantWallet> _tableStorage;

        public MerchantWalletRepository(INoSQLTableStorage<MerchantWallet> tableStorage)
        {
            _tableStorage = tableStorage;

        }

        public async Task SaveNewAddress(IMerchantWallet merchantWallet)
        {
            await _tableStorage.InsertOrMergeAsync(MerchantWallet.Create(merchantWallet));
        }
    }
}
