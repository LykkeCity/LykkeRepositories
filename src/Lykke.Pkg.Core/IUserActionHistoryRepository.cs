using System;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IUserActionHistoryEntity
    {
        string UserEmail { get; set; }
        DateTime ActionDate { get; set; }
        string IpAddress { get; set; }
        string ControllerName { get; set; }
        string ActionName { get; set; }
        string Params { get; set; }
    }

    public interface IUserActionHistoryRepository
    {
        Task SaveUserActionHistoryAsync(IUserActionHistoryEntity userActionHistory);
    }
}
