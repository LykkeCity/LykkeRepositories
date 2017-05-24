using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IAccountTokenHistory : IEntity
    {
        string TokenId { get; set; }
        string AccessList { get; set; }
        string IpList { get; set; }
        string UserName { get; set; }
        string UserIpAddress { get; set; }
    }

    public interface IAccountTokenHistoryRepository
    {
        Task SaveTokenHistoryAsync(IToken token, string userName, string userIpAddress);
    }
}