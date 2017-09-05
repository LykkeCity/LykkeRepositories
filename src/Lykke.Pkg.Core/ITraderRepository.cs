using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface ITraderRepository
    {
        Task<string> GetPropertiyByPhoneNumber(string phoneNumber, string propertyName);
        Task<List<string>> GetPropertiesByPhoneNumber(string phoneNumber);
    }
}
