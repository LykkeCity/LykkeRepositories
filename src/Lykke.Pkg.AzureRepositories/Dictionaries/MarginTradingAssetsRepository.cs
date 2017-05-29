using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Entities.Dictionaries;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories.Dictionaries
{

    public class MarginTradingAsset : TableEntity, IMarginTradingAsset
    {

        public static string GeneratePartitionKey()
        {
            return "MarginTradingAsset";
        }

        
        public string Id {
            get => RowKey;
            set => RowKey = value;
        }
        public string Name { get; set; }
        public string BaseAssetId { get; set; }
        public string QuoteAssetId { get; set; }
        public int Accuracy { get; set; }
        public int LeverageInit { get; set; }
        public int LeverageMaintenance { get; set; }

        public MarginTradingAsset()
        {
            
        }

        public MarginTradingAsset(IMarginTradingAsset marginTradingAsset)
        {
            Id = marginTradingAsset.Id;
            Name = marginTradingAsset.Name;
            BaseAssetId = marginTradingAsset.BaseAssetId;
            QuoteAssetId = marginTradingAsset.QuoteAssetId;
            Accuracy = marginTradingAsset.Accuracy;
            LeverageInit = marginTradingAsset.LeverageInit;
            LeverageMaintenance = marginTradingAsset.LeverageMaintenance;
        }
    }

    public class MarginTradingAssetsRepository : IMarginTradingAssetsRepository
    {

        private readonly INoSQLTableStorage<MarginTradingAsset> _tableStorage;

        public MarginTradingAssetsRepository(INoSQLTableStorage<MarginTradingAsset> tableStorage)
        {
            _tableStorage = tableStorage;

        }


        public async Task<IEnumerable<IMarginTradingAsset>> GetAllAsync()
        {
            var pk = MarginTradingAsset.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(pk);
        }

        public async Task<IEnumerable<IMarginTradingAsset>> GetAllAsync(List<string> instruments)
        {
            var pk = MarginTradingAsset.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(pk, item => instruments.Contains(item.Id));
        }
    }
}
