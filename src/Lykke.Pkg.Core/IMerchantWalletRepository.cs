using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IMerchantWalletEntity
    {
        string MerchantId { get; set; }
        string WalletAddress { get; set; }
        string Data { get; set; }
        
    }


    public interface IMerchantWalletRepository
    {
        Task SaveNewAddressAsync(IMerchantWalletEntity merchantWallet);

        Task<IEnumerable<IMerchantWalletEntity>> GetAllAddressAsync();

        Task<IEnumerable<IMerchantWalletEntity>> GetAllAddressOfMerchantAsync(string merchantId);
    }
}
