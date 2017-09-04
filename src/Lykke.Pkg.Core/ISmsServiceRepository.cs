using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public enum SmsServiceStatus
    {
        ReadyToSend = 0,
        Sending,
        Sent,
        GotError
    }

    public enum PhoneOperator
    {
        Nexmo = 0,
        Twilio
    }

    public interface ISmsEntity : IEntity
    {
        int SmsServiceStatus { get; set; }
        string DateRow { get; set; }
        string PhoneNumer { get; set; }
        int PhoneOperator { get; set; }
        string RowId { get; set; }
        string ParentRowId { get; set; }
        string Message { get; set; }
        int Atempt { get; set; }
    }

    public interface ISmsServiceRepository
    {
        Task<bool> SaveSmsRequestAsync(ISmsEntity reuqest);
        Task<bool> DeleteSmsRequestAsync(ISmsEntity reuqest);

        Task<IEnumerable<ISmsEntity>> GetSmsRequestsAsync();
        Task<IEnumerable<ISmsEntity>> GetSmsRequestsByStatusAsync(SmsServiceStatus serviceStatus);
        Task<ISmsEntity> GetSmsRequestAsync(string requestId);
    }
}
