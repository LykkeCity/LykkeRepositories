using System;
using System.Collections.Generic;
using Lykke.AzureRepositories.Azure.Tables;
using Lykke.Core;
using Xunit;

namespace Lykke.AzureRepositories.Test
{
    public class ExtentionTests
    {
        private readonly string _key = "test";
        private readonly string _encriptedString = "hfJd1cwIdDe5w8T2pRV1yA==";
        private readonly List<int> _testData = new List<int> {
            5,6,7,8,10
        };

        private static string connectionString =
                //"DefaultEndpointsProtocol=https;AccountName=lkedevsettings;AccountKey=Ztpq2z5ieCo7H5Yp4GUJpWXmIqTrXe25dkJBmlnBp0g8IfrRaVV4H67EjbAFjNC8kbZEMU0TvkFGsMRVrFuvXQ==;EndpointSuffix=core.windows.net"
                "UseDevelopmentStorage=true"
        ;

        [Fact]
        public void EncriptionTest()
        {
            var s = _testData.Encrypt(_key);
            Assert.Equal(Convert.ToBase64String(s), _encriptedString);
        }


        [Fact]
        public void DecriptionTest()
        {
            var s = Convert.FromBase64String(_encriptedString).Decrypt<List<int>>(_key);
            Assert.Equal(5, s.Count);
        }

        [Fact]
        public void TestMerchantId()
        {
            var merchant = new MerchantRepository(new AzureTableStorage<MerchantEntity>(connectionString, "Merchants", null)).GetAsync("BittellerATM-LykkeDev").Result;
            Assert.NotNull(merchant);
        }

        [Fact]
        public void TestMerchantStoreRepo()
        {
            var merchant = new MerchantPayRequestRepository(new AzureTableStorage<MerchantPayRequest>(connectionString, "MerchantPayRequest", null)).GetAllAsync().Result;
            Assert.NotNull(merchant);
        }
    }
}
