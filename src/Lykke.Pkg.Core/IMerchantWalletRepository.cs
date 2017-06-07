using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IMerchantWalletEntity
    {
        string MerchantId { get; set; }
        string Data { get; set; }
    }


    public interface IMerchantWalletRepository
    {
        Task SaveNewAddress(IMerchantWalletEntity merchantWallet);
    }
}
