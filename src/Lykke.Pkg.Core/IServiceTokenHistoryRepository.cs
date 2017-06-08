using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IServiceTokenHistory : IEntity
    {
        string TokenId { get; set; }
        string KeyOne { get; set; }
        string KeyTwo { get; set; }
        string UserName { get; set; }
        string UserIpAddress { get; set; }
    }

    public interface IServiceTokenHistoryRepository
    {
        Task SaveTokenHistoryAsync(IServiceTokenEntity token, string userName, string userIpAddress);
    }
}
