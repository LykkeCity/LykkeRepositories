using System;
using Lykke.Core;
using Lykke.AzureRepositories;
using Lykke.AzureRepositories.Azure.Tables;
using Xunit;

namespace Lykke.AzureRepositories.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void KeyValueHistoryRepositoryTest()
        {
            IKeyValueHistoryRepository repo =
                new KeyValueHistoryRepository(
                    new AzureTableStorage<KeyValueHistory>("DefaultEndpointsProtocol=https;AccountName=lkedevsettings;AccountKey=Ztpq2z5ieCo7H5Yp4GUJpWXmIqTrXe25dkJBmlnBp0g8IfrRaVV4H67EjbAFjNC8kbZEMU0TvkFGsMRVrFuvXQ==", "KeyValueHistory", null));

            repo.SaveKeyValueHistoryAsync("Test", "Test User", "::1");
        }
    }
}
