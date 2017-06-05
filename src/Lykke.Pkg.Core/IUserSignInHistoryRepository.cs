using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IUserSignInHistoryEntity
    {
        string UserEmail { get; set; }
        DateTime SignInDate { get; set; }
        string IpAddress { get; set; }
    }


    public interface IUserSignInHistoryRepository
    {
        Task SaveUserLoginHistoryAsync(IUserEntity user, string userIpAddress);
    }
}
