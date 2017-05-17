using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{

    public class TokenEntity : TableEntity, IToken
    {

        public static string GeneratePartitionKey(){
            return "A";
        }

        public static string GenerateRowKey(string tokenId){
            return tokenId;
        }

        public string AccessList {get;set;}
        public string IpList {get;set;}
    }


    public class TokensRepository : ITokensRepository
    {

        private readonly INoSQLTableStorage<TokenEntity> _tableStorage;

        public TokensRepository(INoSQLTableStorage<TokenEntity> tableStorage)
        {
            _tableStorage = tableStorage;
            
        }
        
        public async Task<IToken> GetAsync(string tokenId)
        {
            var pk = TokenEntity.GeneratePartitionKey();
            var rk = TokenEntity.GenerateRowKey(tokenId);

            return await _tableStorage.GetDataAsync(pk, rk);
        }

        public async Task<IEnumerable<IToken>> GetAllAsync()
        {
            var pk = TokenEntity.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(pk);
        }

        public async Task RemoveTokenAsync(string tokenId)
        {
            var pk = TokenEntity.GeneratePartitionKey();
            await _tableStorage.DeleteAsync(pk, tokenId);
        }

        public async Task SaveTokenAsync(IToken token)
        {
            var ts = token as TokenEntity;
            if (ts == null)
            {
                ts = (TokenEntity) await GetAsync(token.RowKey) ?? new TokenEntity();
                
                ts.ETag = token.ETag;
                ts.AccessList = token.AccessList;
                ts.IpList = token.IpList;
            }
            ts.PartitionKey = TokenEntity.GeneratePartitionKey();
            ts.RowKey = token.RowKey;
            await _tableStorage.InsertOrMergeAsync(ts);
        }
    }


}