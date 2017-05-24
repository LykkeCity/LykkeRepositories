using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class AccountTokenHistoryEntity : TableEntity, IAccountTokenHistory
    {


        public string TokenId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        public string UserName
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public string AccessList { get; set; }
        public string IpList { get; set; }
        public string UserIpAddress { get; set; }
    }
    public class AccountTokenHistoryRepository : IAccountTokenHistoryRepository
    {
        private readonly INoSQLTableStorage<AccountTokenHistoryEntity> _tableStorage;

        public AccountTokenHistoryRepository(INoSQLTableStorage<AccountTokenHistoryEntity> tableStorage)
        {
            _tableStorage = tableStorage;

        }

        public async Task SaveTokenHistoryAsync(IToken token, string userName, string userIpAddress)
        {
            var th = new AccountTokenHistoryEntity
            {
                UserName = userName,
                AccessList = token.AccessList,
                IpList = token.IpList,
                TokenId = token.RowKey,
                UserIpAddress = userIpAddress
            };
            
            await _tableStorage.InsertOrMergeAsync(th);
        }
    }
}
