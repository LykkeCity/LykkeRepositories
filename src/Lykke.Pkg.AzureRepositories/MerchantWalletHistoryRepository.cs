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
    public class MerchantWalletHistoryEntity : TableEntity, IMerchantWalletHistoryEntity
    {
        public string WalletAddress
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }
        public double ChangeRequest { get; set; }

        
        public string UserName { get; set; }
        public string UserIpAddress { get; set; }
    }


    public class MerchantWalletHistoryRepository : IMerchantWalletHistoryRepository
    {
        private readonly INoSQLTableStorage<MerchantWalletHistoryEntity> _tableStorage;
        public MerchantWalletHistoryRepository(INoSQLTableStorage<MerchantWalletHistoryEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task SaveNewChangeRequestAsync(string walletAddress, double changeRequest, string userName, string userIpAddress)
        {
            var mwHist = new MerchantWalletHistoryEntity
            {
                WalletAddress = walletAddress,
                RowKey = DateTime.UtcNow.StorageString(),
                ChangeRequest = changeRequest,
                UserName = userName,
                UserIpAddress = userIpAddress
            };
            await _tableStorage.InsertOrMergeAsync(mwHist);
        }

        public async Task<IEnumerable<IMerchantWalletHistoryEntity>> GetAllAddressesAsync()
        {
            return await _tableStorage.GetDataAsync();
        }

        public async Task<IMerchantWalletHistoryEntity> GetAddressAsync(string walletAddress)
        {
            return (await _tableStorage.GetDataAsync(walletAddress)).FirstOrDefault();
        }
    }
}
