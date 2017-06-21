using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Domain.Prices;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Repositories;

namespace Lykke.AzureRepositories.CandleHistory
{
    public class CandleHistoryRepositoryResolver : ICandleHistoryRepository
    {
        private readonly CreateStorage _createStorage;
        private readonly ConcurrentDictionary<string, CandleHistoryRepository> _repoTable = new ConcurrentDictionary<string, CandleHistoryRepository>();

        public CandleHistoryRepositoryResolver(CreateStorage createStorage)
        {
            _createStorage = createStorage;
        }

        /// <summary>
        /// Insert or merge candle value.
        /// </summary>
        public async Task InsertOrMergeAsync(IFeedCandle feedCandle, string asset, TimeInterval interval, PriceType priceType)
        {
            ValidateAndThrow(asset, interval, priceType);
            var repo = GetRepo(asset, interval);
            try
            {
                await repo.InsertOrMergeAsync(feedCandle, priceType, interval);
            }
            catch
            {
                ResetRepo(asset, interval);
                throw;
            }
        }

        /// <summary>
        /// Insert or merge candle value.
        /// </summary>
        public async Task InsertOrMergeAsync(IEnumerable<IFeedCandle> candles, string asset, TimeInterval interval, PriceType priceType)
        {
            ValidateAndThrow(asset, interval, priceType);
            var repo = GetRepo(asset, interval);
            try
            {
                await repo.InsertOrMergeAsync(candles, priceType, interval);
            }
            catch
            {
                ResetRepo(asset, interval);
                throw;
            }
        }

        /// <summary>
        /// Returns buy or sell candle value for the specified interval in the specified time.
        /// </summary>
        public async Task<IFeedCandle> GetCandleAsync(string asset, TimeInterval interval, PriceType priceType, DateTime dateTime)
        {
            ValidateAndThrow(asset, interval, priceType);
            var repo = GetRepo(asset, interval);
            try
            {
                return await repo.GetCandleAsync(priceType, interval, dateTime);
            }
            catch
            {
                ResetRepo(asset, interval);
                throw;
            }
        }

        /// <summary>
        /// Returns buy or sell candle values for the specified interval from the specified time range.
        /// </summary>
        public async Task<IEnumerable<IFeedCandle>> GetCandlesAsync(string asset, TimeInterval interval, PriceType priceType, DateTime from, DateTime to)
        {
            ValidateAndThrow(asset, interval, priceType);
            var repo = GetRepo(asset, interval);
            try
            {
                return await repo.GetCandlesAsync(priceType, interval, from, to);
            }
            catch
            {
                ResetRepo(asset, interval);
                throw;
            }
        }

        private void ResetRepo(string asset, TimeInterval interval)
        {
            var tableName = interval.ToString().ToLowerInvariant();
            var key = asset.ToLowerInvariant() + "_" + tableName;

            _repoTable[key] = null;
        }

        private CandleHistoryRepository GetRepo(string asset, TimeInterval interval)
        {
            var tableName = interval.ToString().ToLowerInvariant();
            var key = asset.ToLowerInvariant() + "_" + tableName;

            if (!_repoTable.TryGetValue(key, out CandleHistoryRepository repo) || repo == null)
            {
                return _repoTable.AddOrUpdate(
                    key: key,
                    addValueFactory: k => new CandleHistoryRepository(_createStorage(asset, tableName)),
                    updateValueFactory: (k, oldRepo) => oldRepo ?? new CandleHistoryRepository(_createStorage(asset, tableName)));
            }

            return repo;
        }

        private void ValidateAndThrow(string asset, TimeInterval interval, PriceType priceType)
        {
            if (string.IsNullOrEmpty(asset))
            {
                throw new ArgumentNullException(nameof(asset));
            }
            if (interval == TimeInterval.Unspecified)
            {
                throw new ArgumentOutOfRangeException(nameof(interval), "Time interval is not specified");
            }
            if (priceType == PriceType.Unspecified)
            {
                throw new ArgumentOutOfRangeException(nameof(priceType), "Price type is not specified");
            }
        }
    }
}