using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IMerchantWallet
    {
        string MerchantId { get; set; }
        string Data { get; set; }
    }


    public interface IMerchantWalletRepository
    {
        Task SaveNewAddress(IMerchantWallet merchantWallet);
    }
}
