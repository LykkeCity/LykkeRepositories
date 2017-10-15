﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IKeyValueHistory : IEntity
    {
        string KeyValuesSnapshot { get; set; }
        string UserName { get; set; }
        string UserIpAddress { get; set; }
    }

    public interface IKeyValueHistoryRepository
    {
        Task SaveKeyValueHistoryAsync(string keyValues, string userName, string userIpAddress);

        Task DeleteKeyValueHistoryAsync(string keyValueId, string description, string userName,
            string userIpAddress);
    }
}
