using System;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IJsonDataRepository : IBlobDataRepository
    {
        
    }

    public interface IAccessDataRepository : IBlobDataRepository
    {

    }

    public interface IBlobDataRepository
    {
        Task<string> GetDataAsync();
        Task<Tuple<string, string>> GetDataWithMetaAsync();
        Task UpdateBlobAsync(string json, string userName, string ipAddress);
        string GetETag();
    }


}