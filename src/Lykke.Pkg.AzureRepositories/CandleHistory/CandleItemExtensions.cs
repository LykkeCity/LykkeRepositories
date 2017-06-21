using System;
using Lykke.Domain.Prices;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;

namespace Lykke.AzureRepositories.CandleHistory
{
    internal static class CandleItemExtensions
    {
        public static IFeedCandle ToCandle(this CandleItem candle, bool isBuy, DateTime baseTime, TimeInterval interval)
        {
            if (candle != null)
            {
                return new FeedCandle
                {
                    Open = candle.Open,
                    Close = candle.Close,
                    High = candle.High,
                    Low = candle.Low,
                    IsBuy = isBuy,
                    DateTime = baseTime.AddIntervalTicks(candle.Tick, interval)
                };
            }
            return null;
        }
    }
}