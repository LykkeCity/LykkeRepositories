using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class PayFee : Core.PayFee
    {
        public float Percent { get; set; }
        public int Pips { get; set; }
        public float FixedFee { get; set; }
    }

    public class MerchantPayRequest : TableEntity, IMerchantPayRequest
    {
        public string MerchantId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        public string RequestId
        {
            get => RowKey;
            set => RowKey = value;
        }
        public string TransactionId { get; set; }
        public Core.PayFee Markup { get; set; }
        public MerchantPayRequestStatus MerchantPayRequestStatus { get; set; }
        public MerchantPayRequestType MerchantPayRequestType { get; set; }
        public MerchantPayRequestNotification MerchantPayRequestNotification { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string AssetPair { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string ProgressUrl { get; set; }
        public string OrderId { get; set; }

        public MerchantPayRequest()
        {
            RequestId = Guid.NewGuid().ToString();
        }

        public static MerchantPayRequest Create(IMerchantPayRequest request)
        {
            return new MerchantPayRequest
            {
                MerchantId = request.MerchantId,
                RequestId = request.RequestId,
                TransactionId = request.TransactionId,
                Markup = request.Markup,
                MerchantPayRequestStatus = request.MerchantPayRequestStatus,
                MerchantPayRequestType = request.MerchantPayRequestType,
                MerchantPayRequestNotification = request.MerchantPayRequestNotification,
                SourceAddress = request.SourceAddress,
                DestinationAddress = request.DestinationAddress,
                AssetPair = request.AssetPair,
                Amount = request.Amount,
                AssetId = request.AssetId,
                SuccessUrl = request.SuccessUrl,
                ErrorUrl = request.ErrorUrl,
                ProgressUrl = request.ProgressUrl,
                OrderId = request.OrderId,
                ETag = "*"
            };
        }
    }

    public class MerchantPayRequestRepository : IMerchantPayRequestRepository
    {
        private readonly INoSQLTableStorage<MerchantPayRequest> _tableStorage;

        public MerchantPayRequestRepository(INoSQLTableStorage<MerchantPayRequest> tableStorage)
        {
            _tableStorage = tableStorage;

        }

        public async Task SaveRequestAsync(IMerchantPayRequest request)
        {
            await _tableStorage.InsertOrMergeAsync(MerchantPayRequest.Create(request));
        }

        public async Task<IEnumerable<IMerchantPayRequest>> GetAllAsync()
        {
            return await _tableStorage.GetDataAsync();
        }

        public async Task<IEnumerable<IMerchantPayRequest>> GetAllByMerchantIdAsync(string merchantId)
        {
            return await _tableStorage.GetDataAsync(merchantId);
        }
    }
}
