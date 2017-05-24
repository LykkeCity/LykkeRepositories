using System;
using Lykke.AzureRepositories.Azure.Tables;
using Lykke.AzureRepositories.Log;
using Xunit;


namespace Lykke.AzureRepositories.Test
{
    public class AccountTokenHistoryRepositoryTest
    {
        [Fact]
        public void SaveRowToDb()
        {
            var rep = new AccountTokenHistoryRepository(new AzureTableStorage<AccountTokenHistoryEntity>("DefaultEndpointsProtocol=https;AccountName=lkedevsettings;AccountKey=Ztpq2z5ieCo7H5Yp4GUJpWXmIqTrXe25dkJBmlnBp0g8IfrRaVV4H67EjbAFjNC8kbZEMU0TvkFGsMRVrFuvXQ==;EndpointSuffix=core.windows.net", "AccessTokenHistory", new LogToConsole()));
            rep.SaveTokenHistoryAsync(new TokenEntity
            {
                RowKey = "test@test.test",
                AccessList = "test",
                IpList = "test"
            }, "test@ttt55.test", "testIP").Wait();

            Assert.True(true);
        }
    }
}
