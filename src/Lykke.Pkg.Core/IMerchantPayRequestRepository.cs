using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Core;

namespace Lykke.Core
{
    [Flags]
    public enum MerchantPayRequestNotification
    {
        Nothing = 0,
        InProgress = 1,
        Success = 2,
        Error = 4
    }

    public enum MerchantPayRequestStatus
    {
        New,
        InProgress,
        Failed,
        Completed
    }

    public enum MerchantPayRequestType
    {
        Purchase,
        Transfer,
        ExchangeTransfer
    }

    public class PayFee
    {
        float Percent { get; set; }
        int Pips { get; set; }
        float FixedFee { get; set; }
      
    }
    public interface IMerchantPayRequest
    {
        string MerchantId { get; set; }
        string RequestId { get; set; }
        string TransactionId { get; set; }
        PayFee Markup { get; set; }
        MerchantPayRequestStatus MerchantPayRequestStatus { get; set; }
        MerchantPayRequestType MerchantPayRequestType { get; set; }
        MerchantPayRequestNotification MerchantPayRequestNotification { get; set; }
        string SourceAddress { get; set; }
        string DestinationAddress { get; set; }
        string AssetPair { get; set; }
        decimal Amount { get; set; }
        string AssetId { get; set; }
        string SuccessUrl { get; set; }
        string ErrorUrl { get; set; }
        string ProgressUrl { get; set; }
        string OrderId { get; set; }
    }

    public interface IMerchantPayRequestRepository
    {
        Task SaveRequestAsync(IMerchantPayRequest request);
        Task<IEnumerable<IMerchantPayRequest>> GetAllAsync();
        Task<IEnumerable<IMerchantPayRequest>> GetAllByMerchantIdAsync(string merchantId);
    }

}
