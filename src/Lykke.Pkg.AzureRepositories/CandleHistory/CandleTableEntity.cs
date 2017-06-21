using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Domain.Prices;
using Lykke.Domain.Prices.Contracts;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Lykke.AzureRepositories.CandleHistory
{
    public class CandleTableEntity : ITableEntity
    {
        public CandleTableEntity()
        {
        }

        public CandleTableEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        #region ITableEntity properties

        public string ETag { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        #endregion

        public DateTime DateTime
        {
            get
            {
                // extract from RowKey + Interval from PKey
                if (!string.IsNullOrEmpty(RowKey))
                {
                    return ParseRowKey(RowKey, DateTimeKind.Utc);
                }
                return default(DateTime);
            }
        }

        public PriceType PriceType
        {
            get
            {
                if (!string.IsNullOrEmpty(PartitionKey))
                {
                    if (Enum.TryParse(PartitionKey, out PriceType value))
                    {
                        return value;
                    }
                }
                return PriceType.Unspecified;
            }
        }

        public List<CandleItem> Candles { get; set; } = new List<CandleItem>();

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Candles.Clear();

            EntityProperty property;
            if (properties.TryGetValue("Data", out property))
            {
                var json = property.StringValue;
                if (!string.IsNullOrEmpty(json))
                {
                    Candles.AddRange(JsonConvert.DeserializeObject<List<CandleItem>>(json));
                }
            }
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            // Serialize candles
            var json = JsonConvert.SerializeObject(Candles);

            var dict = new Dictionary<string, EntityProperty>
            {
                {"Data", new EntityProperty(json)}
            };
            return dict;
        }

        public static string GeneratePartitionKey(PriceType priceType)
        {
            return $"{priceType}";
        }

        public static string GenerateRowKey(DateTime date, TimeInterval interval)
        {
            DateTime time;
            switch (interval)
            {
                case TimeInterval.Month:
                    time = new DateTime(date.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    break;

                case TimeInterval.Week:
                    time = DateTimeUtils.GetFirstWeekOfYear(date);
                    break;

                case TimeInterval.Day:
                    time = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    break;

                case TimeInterval.Hour12:
                case TimeInterval.Hour6:
                case TimeInterval.Hour4:
                case TimeInterval.Hour:
                    time = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
                    break;

                case TimeInterval.Min30:
                case TimeInterval.Min15:
                case TimeInterval.Min5:
                case TimeInterval.Minute:
                    time = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0, DateTimeKind.Utc);
                    break;

                case TimeInterval.Sec:
                    time = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, DateTimeKind.Utc);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(interval), interval, null);
            }

            return FormatRowKey(time);
        }

        public void MergeCandles(IEnumerable<IFeedCandle> candles, TimeInterval interval)
        {
            foreach (var candle in candles)
            {
                MergeCandle(candle, interval);
            }
        }

        public void MergeCandles(IEnumerable<CandleItem> candles, TimeInterval interval)
        {
            foreach (var candle in candles)
            {
                MergeCandle(candle, interval);
            }
        }

        public void MergeCandle(IFeedCandle candle, TimeInterval interval)
        {
            // 1. Check if candle with specified time already exist
            // 2. If found - merge, else - add to list
            //
            //var cell = candle.DateTime.GetIntervalCell(interval);
            var tick = candle.DateTime.GetIntervalTick(interval);
            var existingCandle = Candles.FirstOrDefault(ci => ci.Tick == tick);

            if (existingCandle != null)
            {
                // Merge in list
                var mergedCandle = existingCandle
                    .ToCandle(PriceType == PriceType.Bid, DateTime, interval)
                    .MergeWith(candle);

                Candles.Remove(existingCandle);
                Candles.Add(mergedCandle.ToItem(interval));
            }
            else
            {
                // Add to list
                Candles.Add(candle.ToItem(interval));
            }
        }

        public void MergeCandle(CandleItem candle, TimeInterval interval)
        {
            var fc = candle.ToCandle(PriceType == PriceType.Bid, DateTime, interval);
            MergeCandle(fc, interval);
        }

        private static string FormatRowKey(DateTime dateUtc)
        {
            return dateUtc.ToString("s"); // sortable format
        }

        private static DateTime ParseRowKey(string value, DateTimeKind kind)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            return DateTime.SpecifyKind(DateTime.ParseExact(value, "s", System.Globalization.DateTimeFormatInfo.InvariantInfo), kind);
        }
    }
}