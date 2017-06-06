using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Core
{

    public interface IAssertPairHistoryEntity
    {
        string AssetPair { get; set; }
        double Bid { get; set; }
        double Ask { get; set; }
        int Accuracy { get; set; }
        DateTime StoredTime { get; set; }

    }
    public interface IAssertPairHistoryRepository
    {
        Task SaveAssertPairHistoryAsync(IAssertPairHistoryEntity pairHistory);

        Task<IEnumerable<IAssertPairHistoryEntity>> GetAllAsync();
    }
}
