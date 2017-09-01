using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Core;
using Lykke.Core.Azure;
using Lykke.Core.Log;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AzureRepositories
{
    public class SmsEntity : TableEntity, ISmsEntity
    {

        public int SmsServiceStatus { get; set; }
        public string DateRow { get; set; }
        public string PhoneNumer { get; set; }
        public int PhoneOperator { get; set; }
        public string RowId { get; set; }
        public string ParentRowId { get; set; }
        public string Message { get; set; }

        public static SmsEntity Create(ISmsEntity entity)
        {
            return new SmsEntity
            {
                SmsServiceStatus = entity.SmsServiceStatus,
                DateRow = entity.DateRow,
                PhoneNumer = entity.PhoneNumer,
                PhoneOperator = entity.PhoneOperator,
                RowId = entity.RowId,
                ParentRowId = entity.ParentRowId,
                PartitionKey = entity.SmsServiceStatus.ToString(),
                RowKey = entity.DateRow,
                Message = entity.Message
            };
        }
    }
    public class SmsServiceRepository : ISmsServiceRepository
    {
        private readonly INoSQLTableStorage<SmsEntity> _tableStorage;
        private readonly ILog _log;
        private const string Component = "SmsService";
        public SmsServiceRepository(INoSQLTableStorage<SmsEntity> tableStorage, ILog log)
        {
            _tableStorage = tableStorage;
            _log = log;
        }

        public async Task<bool> SaveSmsRequestAsync(ISmsEntity reuqest)
        {
            try
            {
                await _tableStorage.InsertOrMergeAsync(SmsEntity.Create(reuqest));
                return true;
            }
            catch (Exception e)
            {
                await _log.WriteError(Component, "Save sms", null, e, DateTime.UtcNow);
            }
            return false;

        }

        public async Task<bool> DeleteSmsRequestAsync(ISmsEntity reuqest)
        {
            try
            {
                await _tableStorage.DeleteAsync(SmsEntity.Create(reuqest));
                return true;
            }
            catch (Exception e)
            {
                await _log.WriteError(Component, "Delete sms", null, e, DateTime.UtcNow);
            }
            return false;
        }

        public async Task<IEnumerable<ISmsEntity>> GetSmsRequestsAsync()
        {
            return await _tableStorage.GetDataAsync();
        }

        public async Task<IEnumerable<ISmsEntity>> GetSmsRequestsByStatusAsync(SmsServiceStatus serviceStatus)
        {
            return await _tableStorage.GetDataAsync(((int)serviceStatus).ToString());
        }

        public async Task<ISmsEntity> GetSmsRequestAsync(string requestId)
        {
            return (await _tableStorage.GetDataAsync(f=>f.RowId == requestId)).FirstOrDefault();
        }
    }
}
