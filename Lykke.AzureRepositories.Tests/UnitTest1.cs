using System;
using Lykke.Core;
using Lykke.AzureRepositories;
using Lykke.AzureRepositories.Azure.Tables;
using Lykke.Core.Azure.Blob;
using Xunit;

namespace Lykke.AzureRepositories.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void KeyValueHistoryRepositoryTest()
        {
            var connectionstring =
                "DefaultEndpointsProtocol=https;AccountName=lkedevsettings;AccountKey=Ztpq2z5ieCo7H5Yp4GUJpWXmIqTrXe25dkJBmlnBp0g8IfrRaVV4H67EjbAFjNC8kbZEMU0TvkFGsMRVrFuvXQ==";
            IKeyValueHistoryRepository repo =
                new KeyValueHistoryRepository(
                    new AzureTableStorage<KeyValueHistory>(connectionstring, "KeyValueHistory", null),
                    new AzureBlobStorage(connectionstring), "keyvaluehistory");

            repo.SaveKeyValueHistoryAsync("Test", "Test User", "::1").Wait();
        }
    }
}
