using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
   
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

        public float Markup_Percent { get; set; }
        public int Markup_Pips { get; set; }
        public float Markup_FixedFee { get; set; }

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
                Markup_Percent = request.Markup.Percent,
                Markup_Pips = request.Markup.Pips,
                Markup_FixedFee = request.Markup.FixedFee,
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

        internal static MerchantPayRequest CreateFull(IMerchantPayRequest request)
        {
            var result = Create(request);
            result.Markup = new PayFee
            {
                Percent = result.Markup_Percent,
                Pips = result.Markup_Pips,
                FixedFee = result.Markup_FixedFee
            };

            return result;
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
            return (from t in (await _tableStorage.GetDataAsync())
                   select MerchantPayRequest.CreateFull(t)
                   ).ToList();
        }

        public async Task<IEnumerable<IMerchantPayRequest>> GetAllByMerchantIdAsync(string merchantId)
        {
            return (from t in (await _tableStorage.GetDataAsync(merchantId))
                select MerchantPayRequest.CreateFull(t)
            ).ToList();
        }
    }
}
