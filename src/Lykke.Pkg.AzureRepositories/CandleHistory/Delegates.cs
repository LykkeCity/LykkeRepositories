using Lykke.Core.Azure;

namespace Lykke.AzureRepositories.CandleHistory
{
    public delegate INoSQLTableStorage<CandleTableEntity> CreateStorage(string asset, string tableName);
}