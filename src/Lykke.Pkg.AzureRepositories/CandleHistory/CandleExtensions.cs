using System;
using Lykke.Domain.Prices;
using Lykke.Domain.Prices.Contracts;

namespace Lykke.AzureRepositories.CandleHistory
{
    internal static class CandleExtensions
    {
        public static CandleItem ToItem(this IFeedCandle candle, TimeInterval interval)
        {
            return new CandleItem()
            {
                Open = candle.Open,
                Close = candle.Close,
                High = candle.High,
                Low = candle.Low,
                Tick = candle.DateTime.GetIntervalTick(interval)
            };
        }

        public static string PartitionKey(this IFeedCandle candle, PriceType priceType)
        {
            if (candle == null)
            {
                throw new ArgumentNullException(nameof(candle));
            }
            return CandleTableEntity.GeneratePartitionKey(priceType);
        }

        public static string RowKey(this IFeedCandle candle, TimeInterval interval)
        {
            if (candle == null)
            {
                throw new ArgumentNullException(nameof(candle));
            }
            return CandleTableEntity.GenerateRowKey(candle.DateTime, interval);
        }
    }
}