using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class TraderSettings : TableEntity
    {
        public string Data { get; set; }
    }

    public class TraderRepository : ITraderRepository
    {
        private readonly INoSQLTableStorage<TableEntity> _tradeTableStorage;
        private readonly INoSQLTableStorage<TraderSettings> _traderSettingsTableStorage;

        public TraderRepository(INoSQLTableStorage<TableEntity> tradeTableStorage, INoSQLTableStorage<TraderSettings> traderSettingsTableStorage)
        {
            _tradeTableStorage = tradeTableStorage;
            _traderSettingsTableStorage = traderSettingsTableStorage;
        }

        private static readonly string TradePrefix = "ClientPhone_";

        private async Task<Guid?> GetTraderId(string phoneNumber)
        {
            var trader = await _tradeTableStorage.GetTopRecordAsync($"{TradePrefix}{phoneNumber}");
            Guid traderId;
            if (trader == null || !Guid.TryParse(trader.RowKey.Replace(TradePrefix, string.Empty), out traderId))
            {
                return null;
            }
            return traderId;
        }

        public async Task<string> GetPropertiyByPhoneNumber(string phoneNumber, string propertyName)
        {
            var traderId = await GetTraderId(phoneNumber);
            if (!traderId.HasValue)
            {
                return string.Empty;
            }
            var traderStore = await _traderSettingsTableStorage.GetDataAsync(traderId.ToString(), propertyName);
            return traderStore == null ? string.Empty : traderStore.Data;
        }

        public async Task<List<string>> GetPropertiesByPhoneNumber(string phoneNumber)
        {
            var result = new List<string>();
            var traderId = await GetTraderId(phoneNumber);
            if (traderId.HasValue)
            {
                result.AddRange(from ts in await _traderSettingsTableStorage.GetDataAsync(traderId.ToString())
                    select ts.Data);
            }

            return result;
        }
    }
}
