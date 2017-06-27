using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IMerchantWalletHistoryEntity
    {
        string WalletAddress { get; set; }
        double ChangeRequest { get; set; }
        string UserName { get; set; }
        string UserIpAddress { get; set; }
    }


  

    public interface IMerchantWalletHistoryRepository
    {
        Task SaveNewChangeRequestAsync(string walletAddress, double changeRequest, string userName,
            string userIpAddress);

        Task<IEnumerable<IMerchantWalletHistoryEntity>> GetAllAddressesAsync();

        Task<IMerchantWalletHistoryEntity> GetAddressAsync(string walletAddress);
    }
}
