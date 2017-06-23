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

        public MerchantPayRequestStatus MerchantPayRequestStatus
        {
            get { return (MerchantPayRequestStatus) Enum.Parse(typeof(MerchantPayRequestStatus), SMerchantPayRequestStatus); }
            set => SMerchantPayRequestStatus = value.ToString();
        }
        public MerchantPayRequestType MerchantPayRequestType
        {
            get { return (MerchantPayRequestType)Enum.Parse(typeof(MerchantPayRequestType), SMerchantPayRequestType); }
            set => SMerchantPayRequestType = value.ToString();
        }
        public MerchantPayRequestNotification MerchantPayRequestNotification
        {
            get { return (MerchantPayRequestNotification)Enum.Parse(typeof(MerchantPayRequestNotification), SMerchantPayRequestNotification); }
            set => SMerchantPayRequestNotification = value.ToString();
        }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string AssetPair { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string ProgressUrl { get; set; }
        public string OrderId { get; set; }

        public string Markup_Percent { get; set; }
        public string Markup_Pips { get; set; }
        public string Markup_FixedFee { get; set; }

        public string SMerchantPayRequestStatus { get; set; }
        public string SMerchantPayRequestType { get; set; }
        public string SMerchantPayRequestNotification { get; set; }

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
                Markup_Percent = request.Markup?.Percent.ToString(CultureInfo.InvariantCulture),
                Markup_Pips = request.Markup?.Pips.ToString(),
                Markup_FixedFee = request.Markup?.FixedFee.ToString(CultureInfo.InvariantCulture),
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

        internal static MerchantPayRequest CreateFull(MerchantPayRequest request)
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
