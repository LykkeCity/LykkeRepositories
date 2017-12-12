using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Core
{

    public interface IMerchantOrderRequest
    {
        string MerchantId { get; set; }
        string RequestId { get; set; }
        string TransactionId { get; set; }
        PayFee Markup { get; set; }
        MerchantPayRequestStatus MerchantPayRequestStatus { get; set; }
        MerchantPayRequestNotification MerchantPayRequestNotification { get; set; }
        string SourceAddress { get; set; }
        string AssetPair { get; set; }
        string AssetId { get; set; }
        string ExchangeAssetId { get; set; }
        double Amount { get; set; }
        double OriginAmount { get; set; }
        double ExchangeRate { get; set; }
        string SuccessUrl { get; set; }
        string ErrorUrl { get; set; }
        string ProgressUrl { get; set; }
        string OrderId { get; set; }
        string TransactionDetectionTime { get; set; }
        string TransactionWaitingTime { get; set; }
    }

    public interface IMerchantOrderRequestRepository
    {
        Task SaveRequestAsync(IMerchantOrderRequest request);
        Task<IEnumerable<IMerchantOrderRequest>> GetAllAsync();
        Task<IEnumerable<IMerchantOrderRequest>> GetAllByMerchantIdAsync(string merchantId);
    }
}