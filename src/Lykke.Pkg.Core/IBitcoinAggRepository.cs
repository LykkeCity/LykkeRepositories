using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IBitcoinAggEntity
    {
        string WalletAddress { get; set; }
        string TransactionId { get; set; }
        int BlockNumber { get; set; }
        double Amount { get; set; }

    }

    public interface IBitcoinHeightEntity
    {
        int BitcoinHeight { get; set; }
    }

    public interface IBitcoinAggRepository
    {
        Task<IBitcoinAggEntity> GetWalletTransactionAsync(string walletAddress, string transactionId);
        Task<IEnumerable<IBitcoinAggEntity>> GetWalletTransactionsAsync(string walletAddress);
        Task<IEnumerable<IBitcoinAggEntity>> GetTransactionsAsync();
        Task SetTransactionAsync(IBitcoinAggEntity bitcoinAgg);
        Task<int> GetNextBlockId();
        Task SetNextBlockId(int blockId);
    }
}
