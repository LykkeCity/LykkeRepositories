using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Core.Azure;
using Lykke.Domain.Prices;
using Lykke.Domain.Prices.Contracts;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories.CandleHistory
{
    internal sealed class CandleHistoryRepository
    {
        private readonly INoSQLTableStorage<CandleTableEntity> _tableStorage;

        public CandleHistoryRepository(INoSQLTableStorage<CandleTableEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task InsertOrMergeAsync(IFeedCandle candle, PriceType priceType, TimeInterval interval)
        {
            // Get candle table entity
            var partitionKey = CandleTableEntity.GeneratePartitionKey(priceType);
            var rowKey = CandleTableEntity.GenerateRowKey(candle.DateTime, interval);

            var entity = await _tableStorage.GetDataAsync(partitionKey, rowKey) ?? 
                new CandleTableEntity(partitionKey, rowKey);

            // Merge candle
            entity.MergeCandle(candle, interval);

            // Update
            await _tableStorage.InsertOrMergeAsync(entity);
        }

        public async Task InsertOrMergeAsync(IEnumerable<IFeedCandle> candles, PriceType priceType, TimeInterval interval)
        {
            // Group by row
            var groups = candles
                .GroupBy(candle => new { pKey = candle.PartitionKey(priceType), rowKey = candle.RowKey(interval) });

            // Update rows
            foreach (var group in groups)
            {
                await InsertOrMergeAsync(group, group.Key.pKey, group.Key.rowKey, interval);
            }
        }

        private async Task InsertOrMergeAsync(IEnumerable<IFeedCandle> candles, string partitionKey, string rowKey, TimeInterval interval)
        {
            // Read row
            var entity = await _tableStorage.GetDataAsync(partitionKey, rowKey) ?? 
                new CandleTableEntity(partitionKey, rowKey);

            // Merge all candles
            entity.MergeCandles(candles, interval);

            // Update
            await _tableStorage.InsertOrMergeAsync(entity);
        }

        public async Task<IFeedCandle> GetCandleAsync(PriceType priceType, TimeInterval interval, DateTime dateTime)
        {
            if (priceType == PriceType.Unspecified) { throw new ArgumentException(nameof(priceType)); }

            // 1. Get candle table entity
            var partitionKey = CandleTableEntity.GeneratePartitionKey(priceType);
            var rowKey = CandleTableEntity.GenerateRowKey(dateTime, interval);

            CandleTableEntity entity = await _tableStorage.GetDataAsync(partitionKey, rowKey);

            // 2. Find required candle in candle list by tick
            if (entity != null)
            {
                var tick = dateTime.GetIntervalTick(interval);
                var candleItem = entity.Candles.FirstOrDefault(ci => ci.Tick == tick);
                return candleItem.ToCandle(priceType == PriceType.Bid, entity.DateTime, interval);
            }
            return null;
        }

        public async Task<IEnumerable<IFeedCandle>> GetCandlesAsync(PriceType priceType, TimeInterval interval, DateTime from, DateTime to)
        {
            if (priceType == PriceType.Unspecified) { throw new ArgumentException(nameof(priceType)); }

            var partitionKey = CandleTableEntity.GeneratePartitionKey(priceType);
            var rowKeyFrom = CandleTableEntity.GenerateRowKey(from, interval);
            var rowKeyTo = CandleTableEntity.GenerateRowKey(to, interval);

            var query = new TableQuery<CandleTableEntity>();
            var pkeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);

            var rowkeyCondFrom = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, rowKeyFrom);
            var rowkeyCondTo = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, rowKeyTo);
            var rowkeyFilter = TableQuery.CombineFilters(rowkeyCondFrom, TableOperators.And, rowkeyCondTo);

            query.FilterString = TableQuery.CombineFilters(pkeyFilter, TableOperators.And, rowkeyFilter);

            var entities = await _tableStorage.WhereAsync(query);

            var result = from e in entities
                select e.Candles.Select(ci => ci.ToCandle(e.PriceType == PriceType.Bid, e.DateTime, interval));

            return result
                .SelectMany(c => c)
                .Where(c => c.DateTime >= from && c.DateTime < to);
        }
    }
}