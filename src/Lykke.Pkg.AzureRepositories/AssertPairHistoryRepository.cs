using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Entities.Pay;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class AssertPairHistoryEntity : TableEntity, IAssertPairHistoryEntity
    {
        public static string GeneratePartitionKey()
        {
            return "APR";
        }


        public string AssetPair
        {
            get => RowKey;
            set => RowKey = value;
        }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public int Accuracy { get; set; }
        public DateTime StoredTime { get; set; }

        public static AssertPairHistoryEntity Create(AssertPairRate assetPair)
        {
            return new AssertPairHistoryEntity
            {
                AssetPair = assetPair.AssetPair,
                Bid = assetPair.Bid,
                Ask = assetPair.Ask,
                Accuracy = assetPair.Accuracy,
                StoredTime = DateTime.UtcNow,
                ETag = "*"
            };
        }

        public static AssertPairHistoryEntity Create(IAssertPairHistoryEntity assetPairHist)
        {
            return new AssertPairHistoryEntity
            {
                AssetPair = assetPairHist.AssetPair,
                Bid = assetPairHist.Bid,
                Ask = assetPairHist.Ask,
                Accuracy = assetPairHist.Accuracy,
                StoredTime = assetPairHist.StoredTime,
                ETag = "*"
            };
        }
    }

    public class AssertPairHistoryRepository : IAssertPairHistoryRepository
    {
        private readonly INoSQLTableStorage<AssertPairHistoryEntity> _tableStorage;

        public AssertPairHistoryRepository(INoSQLTableStorage<AssertPairHistoryEntity> tableStorage)
        {
            _tableStorage = tableStorage;

        }


        public async Task SaveAssertPairHistoryAsync(IAssertPairHistoryEntity pairHistory)
        {
            var pk = KeyValueEntity.GeneratePartitionKey();
            var item = AssertPairHistoryEntity.Create(pairHistory);
            item.PartitionKey = pk;
            await _tableStorage.InsertOrMergeAsync(item);
        }

        public async Task<IEnumerable<IAssertPairHistoryEntity>> GetAllAsync()
        {
            var pk = KeyValueEntity.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(pk);
        }
    }
}
