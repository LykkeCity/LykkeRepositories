using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Core
{
    public interface IServiceTokenRepository
    {

        Task<List<IServiceTokenEntity>> GetAllAsync();
        Task<IServiceTokenEntity> GetAsync(string tokenKey);
        Task<bool> SaveAsync(IServiceTokenEntity token);
        Task<bool> RemoveAsync(string tokenId);
    }

    public interface IServiceTokenEntity : IEntity
    {
        string SecurityKeyOne { get; set; }
        string SecurityKeyTwo { get; set; }
    }
}
