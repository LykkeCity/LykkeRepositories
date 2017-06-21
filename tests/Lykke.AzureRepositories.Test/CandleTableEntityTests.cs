using System;
using System.Linq;
using Lykke.AzureRepositories.CandleHistory;
using Lykke.Domain.Prices;
using Lykke.Domain.Prices.Contracts;
using Xunit;

namespace Lykke.AzureRepositories.Test
{
    public class CandleTableEntityTests
    {

        [Fact]
        public void TestsAreCoveringAllIntervals()
        {
            // Tests are written for TimeInterval with 9 values
            Assert.Equal(13, Enum.GetValues(typeof(TimeInterval)).Cast<int>().Count());
        }

        [Fact]
        public void PropertiesParsedFromKeys()
        {
            // Ask
            var entityAsk = new CandleTableEntity("Ask", "2017-05-12T10:00:00");
            Assert.Equal(PriceType.Ask, entityAsk.PriceType);
            Assert.Equal(new DateTime(2017, 5, 12, 10, 0, 0, DateTimeKind.Utc), entityAsk.DateTime);

            // Bid
            var entityBid = new CandleTableEntity("Bid", "2017-05-12T11:05:00");
            Assert.Equal(PriceType.Bid, entityBid.PriceType);
            Assert.Equal(new DateTime(2017, 5, 12, 11, 05, 0, DateTimeKind.Utc), entityBid.DateTime);

            // Mid
            var entityMid = new CandleTableEntity("Mid", "2017-05-12T11:10:00");
            Assert.Equal(PriceType.Mid, entityMid.PriceType);
            Assert.Equal(new DateTime(2017, 5, 12, 11, 10, 0, DateTimeKind.Utc), entityMid.DateTime);
        }

        [Fact]
        public void GenerateRowKey_BasicTests()
        {
            // Sec
            Assert.Equal("2016-12-26T17:59:00", CandleTableEntity.GenerateRowKey(new DateTime(2016, 12, 26, 17, 59, 59), TimeInterval.Sec));

            // Min
            Assert.Equal("2016-12-26T17:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2016, 12, 26, 17, 59, 59), TimeInterval.Minute));

            // Hour
            Assert.Equal("2016-12-26T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2016, 12, 26, 17, 59, 59), TimeInterval.Hour));

            // Week
            Assert.Equal("2016-01-04T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2016, 12, 26), TimeInterval.Week));
            Assert.Equal("2016-01-04T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2016, 12, 27), TimeInterval.Week));
            Assert.Equal("2016-01-04T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2017, 1, 1), TimeInterval.Week));
            Assert.Equal("2017-01-02T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2017, 1, 2), TimeInterval.Week));
            Assert.Equal("2017-01-02T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2017, 1, 30), TimeInterval.Week));
            Assert.Equal("2017-01-02T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2017, 2, 1), TimeInterval.Week));
            Assert.Equal("2017-01-02T00:00:00", CandleTableEntity.GenerateRowKey(new DateTime(2017, 2, 6), TimeInterval.Week));
        }
    }
}