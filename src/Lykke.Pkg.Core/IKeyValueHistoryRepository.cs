﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IKeyValueHistory : IEntity
    {
        string KeyValueId { get; set; }
        string NewValue { get; set; }
        string KeyValuesSnapshot { get; set; }
        string UserName { get; set; }
        string UserIpAddress { get; set; }
    }

    public interface IKeyValueHistoryRepository
    {
        Task SaveKeyValueHistoryAsync(string keyValueId, string newValue, string keyValues, string userName,
            string userIpAddress);

        Task SaveKeyValueHistoryAsync(string keyValueId, string newValue, string keyValues, string userName,
            string userIpAddress, DateTime atDate);

        Task DeleteKeyValueHistoryAsync(string keyValueId, string description, string userName,
            string userIpAddress);

        Task DeleteKeyValueHistoryFromDbAsync(string rowKey);

        Task<List<IKeyValueHistory>> GetHistoryByKeyValueAsync(string keyValueId);

        Task<List<IKeyValueHistory>> GetAllAsync();

        Task<Dictionary<string, byte[]>> GetAllBlobAsync();

    }
}
