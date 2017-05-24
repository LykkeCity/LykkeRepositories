using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class AccountTokenHistoryEntity : TableEntity, IAccountTokenHistory
    {
        
        public new string PartitionKey
        {
            get { return TokenId; }
            set { TokenId = value; }
        }

        public new string RowKey
        {
            get { return UserName; }
            set { UserName = value; }
        }

        public string TokenId { get; set; }
        public string AccessList { get; set; }
        public string IpList { get; set; }
        public string UserName { get; set; }
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
