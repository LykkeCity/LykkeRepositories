using System;
using System.Threading.Tasks;

namespace Lykke.Core
{

    public interface IJsonDataRepository
    {
        Task<string> GetDataAsync();
        Task<Tuple<string, string>> GetDataWithMetaAsync();
        Task UpdateBlobAsync(string json, string userName, string ipAddress);
        string GetETag();
    }


}