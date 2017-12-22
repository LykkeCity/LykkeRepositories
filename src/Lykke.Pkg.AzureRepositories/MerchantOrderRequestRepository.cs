using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{

    public class MerchantOrderRequest : TableEntity, IMerchantOrderRequest
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

        public MerchantPayRequestStatus MerchantPayRequestStatus
        {
            get { return (MerchantPayRequestStatus)Enum.Parse(typeof(MerchantPayRequestStatus), SMerchantPayRequestStatus); }
            set => SMerchantPayRequestStatus = value.ToString();
        }

        public MerchantPayRequestNotification MerchantPayRequestNotification
        {
            get { return (MerchantPayRequestNotification)Enum.Parse(typeof(MerchantPayRequestNotification), SMerchantPayRequestNotification); }
            set => SMerchantPayRequestNotification = value.ToString();
        }
        public string SourceAddress { get; set; }
        public string AssetPair { get; set; }
        public string ExchangeAssetId { get; set; }
        public double Amount { get; set; }
        public double OriginAmount { get; set; }
        public double ExchangeRate { get; set; }
        public string AssetId { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string ProgressUrl { get; set; }
        public string OrderId { get; set; }
        public string TransactionDetectionTime { get; set; }
        public string TransactionWaitingTime { get; set; }
        public string Transaction { get; set; }
        public string TransactionStatus { get; set; }

        public string Markup_Percent { get; set; }
        public string Markup_Pips { get; set; }
        public string Markup_FixedFee { get; set; }

        public string SMerchantPayRequestStatus { get; set; }
        public string SMerchantPayRequestNotification { get; set; }

        public MerchantOrderRequest()
        {
            RequestId = Guid.NewGuid().ToString();
        }

        public static MerchantOrderRequest Create(IMerchantOrderRequest request)
        {
            return new MerchantOrderRequest
            {
                MerchantId = request.MerchantId,
                RequestId = request.RequestId,
                TransactionId = request.TransactionId,
                Markup = request.Markup,
                Markup_Percent = request.Markup?.Percent.ToString(CultureInfo.InvariantCulture),
                Markup_Pips = request.Markup?.Pips.ToString(),
                Markup_FixedFee = request.Markup?.FixedFee.ToString(CultureInfo.InvariantCulture),
                MerchantPayRequestStatus = request.MerchantPayRequestStatus,
                MerchantPayRequestNotification = request.MerchantPayRequestNotification,
                SourceAddress = request.SourceAddress,
                AssetPair = request.AssetPair,
                Amount = request.Amount,
                OriginAmount = request.OriginAmount,
                AssetId = request.AssetId,
                SuccessUrl = request.SuccessUrl,
                ErrorUrl = request.ErrorUrl,
                ProgressUrl = request.ProgressUrl,
                OrderId = request.OrderId,
                ETag = "*",
                TransactionDetectionTime = request.TransactionDetectionTime,
                TransactionWaitingTime = request.TransactionWaitingTime,
                ExchangeAssetId = request.ExchangeAssetId,
                ExchangeRate = request.ExchangeRate,
                Transaction = request.Transaction,
                TransactionStatus = request.TransactionStatus

            };
        }

        internal static MerchantOrderRequest CreateFull(MerchantOrderRequest request)
        {
            var result = Create(request);
            float percent, fixedFee;
            int pips;
            float.TryParse(request.Markup_Percent, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out percent);
            float.TryParse(request.Markup_FixedFee, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out fixedFee);
            int.TryParse(request.Markup_Pips, out pips);
            result.Markup = new PayFee
            {
                Percent = percent,
                Pips = pips,
                FixedFee = fixedFee
            };

            return result;
        }
    }


    public class MerchantOrderRequestRepository : IMerchantOrderRequestRepository
    {
        private readonly INoSQLTableStorage<MerchantOrderRequest> _tableStorage;

        public MerchantOrderRequestRepository(INoSQLTableStorage<MerchantOrderRequest> tableStorage)
        {
            _tableStorage = tableStorage;

        }

        public async Task SaveRequestAsync(IMerchantOrderRequest request)
        {
            await _tableStorage.InsertOrMergeAsync(MerchantOrderRequest.Create(request));
        }

        public async Task<IEnumerable<IMerchantOrderRequest>> GetAllAsync()
        {
            return Enumerable.ToList<MerchantOrderRequest>((from t in (await _tableStorage.GetDataAsync())
                                                          select MerchantOrderRequest.CreateFull(t)
            ));
        }

        public async Task<IEnumerable<IMerchantOrderRequest>> GetAllByMerchantIdAsync(string merchantId)
        {
            return Enumerable.ToList<MerchantOrderRequest>((from t in (await _tableStorage.GetDataAsync(merchantId))
                                                          select MerchantOrderRequest.CreateFull(t)
            ));
        }
    }
}