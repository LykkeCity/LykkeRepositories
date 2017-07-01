using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class BitcoinAggEntity : TableEntity, IBitcoinAggEntity
    {
        public string WalletAddress
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        public string TransactionId
        {
            get => RowKey;
            set => RowKey = value;
        }
        public int BlockNumber { get; set; }
        public double Amount { get; set; }

        public static BitcoinAggEntity Create(IBitcoinAggEntity bitcoinAggEntity)
        {
            return new BitcoinAggEntity
            {
                WalletAddress = bitcoinAggEntity.WalletAddress,
                TransactionId = bitcoinAggEntity.TransactionId,
                BlockNumber = bitcoinAggEntity.BlockNumber,
                Amount = bitcoinAggEntity.Amount,
                ETag = "*"
            };
        }
    }

    public class BitcoinHeightEntity : TableEntity, IBitcoinHeightEntity
    {
        public static string GeneratePartitionKey()
        {
            return "BH";
        }

        public static string GenerateRowKey()
        {
            return "BitcoinHeight";
        }



        public int BitcoinHeight { get; set; }

        public static BitcoinHeightEntity Create(int bitcoinHeight)
        {
            return new BitcoinHeightEntity
            {
                BitcoinHeight = bitcoinHeight,
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                ETag = "*"
            };
        }
    }

    public class BitcoinAggRepository : IBitcoinAggRepository
    {

        private readonly INoSQLTableStorage<BitcoinAggEntity> _aggRepTableStorage;
        private readonly INoSQLTableStorage<BitcoinHeightEntity> _heightTableStorage;

        public BitcoinAggRepository(INoSQLTableStorage<BitcoinAggEntity> aggRepTableStorage, INoSQLTableStorage<BitcoinHeightEntity> heightTableStorage)
        {
            _aggRepTableStorage = aggRepTableStorage;
            _heightTableStorage = heightTableStorage;
        }

        public async Task<IBitcoinAggEntity> GetWalletTransactionAsync(string walletAddress, string transactionId)
        {
            return await _aggRepTableStorage.GetDataAsync(walletAddress, transactionId);
        }

        public async Task<IEnumerable<IBitcoinAggEntity>> GetWalletTransactionsAsync(string walletAddress)
        {
            return await _aggRepTableStorage.GetDataAsync(walletAddress);
        }

        public async Task<IEnumerable<IBitcoinAggEntity>> GetTransactionsAsync()
        {
            return await _aggRepTableStorage.GetDataAsync();
        }

        public async Task SetTransactionAsync(IBitcoinAggEntity bitcoinAgg)
        {
            await _aggRepTableStorage.InsertOrMergeAsync(BitcoinAggEntity.Create(bitcoinAgg));
        }

        public async Task<int> GetNextBlockId()
        {
            var result = await _heightTableStorage.GetDataAsync(BitcoinHeightEntity.GeneratePartitionKey(), BitcoinHeightEntity.GenerateRowKey());
            return result.BitcoinHeight;
        }

        public async Task SetNextBlockId(int blockId)
        {
            await _heightTableStorage.InsertOrMergeAsync(BitcoinHeightEntity.Create(blockId));
        }
    }
}
