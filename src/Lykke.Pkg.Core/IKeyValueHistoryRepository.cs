using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IKeyValueHistory : IEntity
    {
        string KeyValues { get; set; }
        string UserName { get; set; }
        string UserIpAddress { get; set; }
    }

    public interface IKeyValueHistoryRepository
    {
        Task SaveTokenHistoryAsync(string keyValues, string userName, string userIpAddress);
    }
}
