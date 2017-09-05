using System;
using Lykke.Core;
using Lykke.AzureRepositories;
using Lykke.AzureRepositories.Azure.Tables;
using Lykke.Core.Azure.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;

namespace Lykke.AzureRepositories.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void KeyValueHistoryRepositoryTest()
        {
            var connectionstring =
                "DefaultEndpointsProtocol=https;AccountName=lkedevmain;AccountKey=l0W0CaoNiRZQIqJ536sIScSV5fUuQmPYRQYohj/UjO7+ZVdpUiEsRLtQMxD+1szNuAeJ351ndkOsdWFzWBXmdw==";
            var traderRepository = new TraderRepository(new AzureTableStorage<TableEntity>(connectionstring, "Traders", null),
                new AzureTableStorage<TraderSettings>(connectionstring, "TraderSettings", null));

            var settings =  traderRepository.GetPropertiyByPhoneNumber("+375447890502", "SmsSettings").Result;
        }
    }
}
