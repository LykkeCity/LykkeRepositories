using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AzureRepositories.Azure.Tables;
using Lykke.AzureRepositories.CandleHistory;
using Lykke.Core.Azure;
using Lykke.Core.Azure.Tables;
using Lykke.Domain.Prices;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace Lykke.AzureRepositories.Test
{
    internal static class CandleExtensions
    {
        public static bool IsEqual(this IFeedCandle candle, IFeedCandle other)
        {
            if (other != null && candle != null)
            {
                return candle.DateTime == other.DateTime
                       && candle.Open == other.Open
                       && candle.Close == other.Close
                       && candle.High == other.High
                       && candle.Low == other.Low
                       && candle.IsBuy == other.IsBuy;
            }
            return false;
        }
    }

    public class CandleHistoryRepositoryTests
    {
        [Fact]
        public void WriteReadSeconds()
        {
            var interval = TimeInterval.Sec;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 3600 * 3) // 3 hours
                select new FeedCandle() { DateTime = baseTime.AddSeconds(i), Open = i, Close = i, High = i, Low = i };

            new CandleHistoryRepository(storage).InsertOrMergeAsync(candlesIn, PriceType.Ask, interval).Wait();

            // Select for 1 minute

            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime.AddHours(1).AddMinutes(1),
                to: baseTime.AddHours(1).AddMinutes(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(60, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1).AddMinutes(1), Open = 3660, Close = 3660, High = 3660, Low = 3660 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1).AddMinutes(1).AddSeconds(59), Open = 3719, Close = 3719, High = 3719, Low = 3719 }));

            // Select for 1 hour

            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime.AddHours(1),
                to: baseTime.AddHours(2)).Result;
            Assert.NotNull(queriedCandles);
            Assert.Equal(3600, queriedCandles.Count());

            // Select for 2 hours

            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime,
                to: baseTime.AddHours(2)).Result;
            Assert.NotNull(queriedCandles);
            Assert.Equal(7200, queriedCandles.Count());

            //  Read / write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddMinutes(1), Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Ask, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Ask, interval, baseTime.AddMinutes(1)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddMinutes(1), Open = 99, Close = 99, High = 99, Low = 60 }));
        }

        [Fact]
        public void WriteReadMinutes()
        {
            var interval = TimeInterval.Minute;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            var repo = new CandleHistoryRepository(storage);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 60 * 24 * 3) // 3 days
                select new FeedCandle() { DateTime = baseTime.AddMinutes(i), Open = i, Close = i, High = i, Low = i };

            repo.InsertOrMergeAsync(candlesIn, PriceType.Bid, interval).Wait();

            // Select for 1 hour
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Bid, interval,
                @from: baseTime.AddHours(1),
                to: baseTime.AddHours(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(60, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), IsBuy = true, Open = 60, Close = 60, High = 60, Low = 60 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1).AddMinutes(59), IsBuy = true, Open = 119, Close = 119, High = 119, Low = 119 }));

            // Select for 1 day
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Bid, interval,
                @from: baseTime.AddDays(1),
                to: baseTime.AddDays(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(60 * 24 * 1, queriedCandles.Count());

            // Read/write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddHours(1), IsBuy = true, Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Bid, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Bid, interval, baseTime.AddHours(1)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), IsBuy = true, Open = 99, Close = 99, High = 99, Low = 60 }));
        }

        [Fact]
        public void WriteRead5Minutes()
        {
            var interval = TimeInterval.Min5;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            var repo = new CandleHistoryRepository(storage);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 12 * 24 * 3) // 3 days
                select new FeedCandle() { DateTime = baseTime.AddMinutes(5 * i), Open = i, Close = i, High = i, Low = i };

            repo.InsertOrMergeAsync(candlesIn, PriceType.Mid, interval).Wait();

            // Select for 1 hour
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Mid, interval,
                @from: baseTime.AddHours(1),
                to: baseTime.AddHours(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(12, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 12, Close = 12, High = 12, Low = 12 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1).AddMinutes(55), Open = 23, Close = 23, High = 23, Low = 23 }));

            // Select for 1 day
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Mid, interval,
                @from: baseTime.AddDays(1),
                to: baseTime.AddDays(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(12 * 24, queriedCandles.Count());

            // Read/write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Mid, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Mid, interval, baseTime.AddHours(1)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 99, Close = 99, High = 99, Low = 12 }));
        }

        [Fact]
        public void WriteRead15Minutes()
        {
            var interval = TimeInterval.Min15;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            var repo = new CandleHistoryRepository(storage);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 4 * 24 * 3) // 3 days
                select new FeedCandle() { DateTime = baseTime.AddMinutes(15 * i), Open = i, Close = i, High = i, Low = i };

            repo.InsertOrMergeAsync(candlesIn, PriceType.Ask, interval).Wait();

            // Select for 1 hour
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime.AddHours(1),
                to: baseTime.AddHours(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(4, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 4, Close = 4, High = 4, Low = 4 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1).AddMinutes(45), Open = 7, Close = 7, High = 7, Low = 7 }));

            // Select for 1 day
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime.AddDays(1),
                to: baseTime.AddDays(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(4 * 24, queriedCandles.Count());

            // Read/write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Ask, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Ask, interval, baseTime.AddHours(1)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 99, Close = 99, High = 99, Low = 4 }));
        }

        [Fact]
        public void WriteRead30Minutes()
        {
            var interval = TimeInterval.Min30;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            var repo = new CandleHistoryRepository(storage);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 2 * 24 * 3) // 3 days
                select new FeedCandle() { DateTime = baseTime.AddMinutes(30 * i), Open = i, Close = i, High = i, Low = i };

            repo.InsertOrMergeAsync(candlesIn, PriceType.Mid, interval).Wait();

            // Select for 1 hour
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Mid, interval,
                @from: baseTime.AddHours(1),
                to: baseTime.AddHours(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(2, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 2, Close = 2, High = 2, Low = 2 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1).AddMinutes(30), Open = 3, Close = 3, High = 3, Low = 3 }));

            // Select for 1 day
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Mid, interval,
                @from: baseTime.AddDays(1),
                to: baseTime.AddDays(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(2 * 24, queriedCandles.Count());

            // Read/write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Mid, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Mid, interval, baseTime.AddHours(1)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddHours(1), Open = 99, Close = 99, High = 99, Low = 2 }));
        }

        [Fact]
        public void WriteReadHours()
        {
            var interval = TimeInterval.Hour;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            var repo = new CandleHistoryRepository(storage);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 24 * 31 * 3) // 3 months
                select new FeedCandle() { DateTime = baseTime.AddHours(i), Open = i, Close = i, High = i, Low = i };

            repo.InsertOrMergeAsync(candlesIn, PriceType.Bid, interval).Wait();

            // Select for 1 day
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Bid, interval,
                @from: baseTime.AddDays(1),
                to: baseTime.AddDays(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(24, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddDays(1), IsBuy = true, Open = 24, Close = 24, High = 24, Low = 24 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddDays(1).AddHours(23), IsBuy = true, Open = 47, Close = 47, High = 47, Low = 47 }));

            // Select for 1 month
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Bid, interval,
                @from: baseTime.AddMonths(1),
                to: baseTime.AddMonths(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(24 * 30, queriedCandles.Count());

            // Read/write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddDays(1), IsBuy = true, Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Bid, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Bid, interval, baseTime.AddDays(1)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddDays(1), IsBuy = true, Open = 99, Close = 99, High = 99, Low = 24 }));
        }

        [Fact]
        public void WriteReadDays()
        {
            var interval = TimeInterval.Day;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 31 * 12 * 3) // > 3 years
                select new FeedCandle() { DateTime = baseTime.AddDays(i), Open = i, Close = i, High = i, Low = i };

            new CandleHistoryRepository(storage).InsertOrMergeAsync(candlesIn, PriceType.Bid, interval).Wait();

            // Select for 1 month
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Bid, interval,
                @from: baseTime.AddMonths(1),
                to: baseTime.AddMonths(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(30, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddMonths(1), IsBuy = true, Open = 31, Close = 31, High = 31, Low = 31 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddMonths(1).AddDays(29), IsBuy = true, Open = 60, Close = 60, High = 60, Low = 60 }));

            // Select for 1 year
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Bid, interval,
                @from: baseTime.AddYears(1),
                to: baseTime.AddYears(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(365, queriedCandles.Count());

            // Read/write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddMonths(1), IsBuy = true, Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Bid, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Bid, interval, baseTime.AddMonths(1)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddMonths(1), IsBuy = true, Open = 99, Close = 99, High = 99, Low = 31 }));
        }

        [Fact]
        public void WriteReadWeeks()
        {
            var interval = TimeInterval.Week;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            DateTime baseTime = new DateTime(2016, 12, 26, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 54 * 3) // > 3 years
                select new FeedCandle() { DateTime = baseTime.AddDays(7 * i), Open = i, Close = i, High = i, Low = i };

            new CandleHistoryRepository(storage).InsertOrMergeAsync(candlesIn, PriceType.Ask, interval).Wait();

            // Select for 1 month
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime,
                to: baseTime.AddDays(28)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(4, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime, Open = 0, Close = 0, High = 0, Low = 0 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddDays(21), Open = 3, Close = 3, High = 3, Low = 3 }));

            // Select for 1 year
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: new DateTime(2017, 12, 25),
                to: new DateTime(2017, 12, 25).AddDays(364)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(52, queriedCandles.Count());

            // Read/write 1 candle
            new CandleHistoryRepository(storage).InsertOrMergeAsync(
                new FeedCandle() { DateTime = baseTime.AddDays(21), Open = 99, Close = 99, High = 99, Low = 99 }, PriceType.Ask, interval).Wait();

            var candle = new CandleHistoryRepository(storage).GetCandleAsync(PriceType.Ask, interval, baseTime.AddDays(21)).Result;
            Assert.True(candle.IsEqual(
                new FeedCandle() { DateTime = baseTime.AddDays(21), Open = 99, Close = 99, High = 99, Low = 3 }));
        }

        [Fact]
        public void WriteReadMonths()
        {
            var interval = TimeInterval.Month;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            var repo = new CandleHistoryRepository(storage);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 12 * 3) // 3 years
                select new FeedCandle() { DateTime = baseTime.AddMonths(i), Open = i, Close = i, High = i, Low = i };

            repo.InsertOrMergeAsync(candlesIn, PriceType.Ask, interval).Wait();

            // Select for 1 year
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime.AddYears(1),
                to: baseTime.AddYears(2)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(12, queriedCandles.Count());

            Assert.True(queriedCandles.First().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddYears(1), Open = 12, Close = 12, High = 12, Low = 12 }));
            Assert.True(queriedCandles.Last().IsEqual(
                new FeedCandle() { DateTime = baseTime.AddYears(1).AddMonths(11), Open = 23, Close = 23, High = 23, Low = 23 }));

            // Select for 1 month
            queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime,
                to: baseTime.AddMonths(1)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(1, queriedCandles.Count());
        }

        [Fact]
        public void CandleCanTake100Characters()
        {
            var interval = TimeInterval.Sec;
            var logger = new LogToMemory();
            var storage = CreateStorage<CandleTableEntity>(logger);
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            var candlesIn = from i in Enumerable.Range(0, 3600) // 1 hour
                select new FeedCandle()
                {
                    DateTime = baseTime.AddSeconds(i),
                    IsBuy = true,
                    Open = 1234567890.123456,
                    Close = 1234567890.123456,
                    High = 1234567890.123456,
                    Low = 1234567890.123456
                };

            new CandleHistoryRepository(storage).InsertOrMergeAsync(candlesIn, PriceType.Ask, interval).Wait();

            // Select all candles
            var queriedCandles = new CandleHistoryRepository(storage).GetCandlesAsync(PriceType.Ask, interval,
                @from: baseTime,
                to: baseTime.AddHours(1)).Result;

            Assert.NotNull(queriedCandles);
            Assert.Equal(3600, queriedCandles.Count());
        }

        /// <summary>
        /// On optimistic concurrency failure, repository retries to make update.
        /// </summary>
        [Fact]
        public void RepositoryHandlesConcurrencyConflictsOnInsert()
        {
            var logger = new LogToMemory();
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            // Clear table
            CreateStorage<CandleTableEntity>(logger, clear: true);

            // Run multiple update tasks simultaneously
            var tasks = new List<Task>();
            for (int t = 0; t < 10; t++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var storage = CreateStorage<CandleTableEntity>(logger, clear: false);
                    var repo = new CandleHistoryRepository(storage);
                    var candle = new FeedCandle() { DateTime = baseTime.AddHours(t), Open = t, Close = t, High = t, Low = t };

                    repo.InsertOrMergeAsync(candle, PriceType.Ask, TimeInterval.Hour).Wait();
                }));
            }
            Task.WhenAll(tasks).Wait();
        }

        [Fact]
        public void RepositoryHandlesConcurrencyConflictsOnUpdate()
        {
            var logger = new LogToMemory();
            DateTime baseTime = new DateTime(2017, 03, 01, 0, 0, 0, DateTimeKind.Utc);

            // Clear table and insert entity
            var storage = CreateStorage<CandleTableEntity>(logger, clear: true);
            var candle = new FeedCandle() { DateTime = baseTime, Open = 0, Close = 0, High = 0, Low = 0 };
            new CandleHistoryRepository(storage).InsertOrMergeAsync(candle, PriceType.Ask, TimeInterval.Hour).Wait();

            // Run multiple update tasks simultaneously
            var tasks = new List<Task>();
            for (int t = 1; t < 11; t++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var repo = new CandleHistoryRepository(CreateStorage<CandleTableEntity>(logger, clear: false));
                    var candlet = new FeedCandle() { DateTime = baseTime.AddHours(t), Open = t, Close = t, High = t, Low = t };
                    repo.InsertOrMergeAsync(candlet, PriceType.Ask, TimeInterval.Hour).Wait();
                }));
            }
            Task.WhenAll(tasks).Wait();
        }

        private INoSQLTableStorage<T> CreateStorage<T>(ILog logger, bool clear = true) where T : class, ITableEntity, new()
        {
            //var table = new AzureTableStorage<T>("UseDevelopmentStorage=true;", "CandlesHistoryTest", logger);
            //if (clear)
            //{
            //    ClearTable(table);
            //}
            //return table;

            //NoSqlTableInMemory does not implement ScanDataAsync method
            return new NoSqlTableInMemory<T>();
        }

        private static void ClearTable<T>(AzureTableStorage<T> table) where T : class, ITableEntity, new()
        {
            var entities = new List<T>();
            do
            {
                entities.Clear();
                table.GetDataByChunksAsync(collection => entities.AddRange(collection)).Wait();
                entities.ForEach(e => table.DeleteAsync(e).Wait());
            } while (entities.Count > 0);
        }
    }
}