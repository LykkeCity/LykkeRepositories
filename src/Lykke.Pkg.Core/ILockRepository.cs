using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Core
{
    public interface ILockRepository
    {
        Task<ILockEntity> GetJsonPageLockAsync();
        Task SetJsonPageLockAsync(string userName, string ipAddress);
        Task ResetJsonPageLockAsync();
    }

    public interface ILockEntity: IEntity
    {
        DateTime DateTime { get; set; }
        string UserName { get; set; }
        string IpAddress { get; set; }
    }
}
