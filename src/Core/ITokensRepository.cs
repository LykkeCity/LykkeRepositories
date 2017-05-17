
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Core
{
    public interface IToken : IEntity{
        string IpList {get;set;}
        string AccessList {get;set;}
    }

    public interface ITokensRepository{

        Task<IToken> GetAsync(string tokenId);

        Task<IEnumerable<IToken>> GetAllAsync();
        Task RemoveTokenAsync(string tokenId);
        Task SaveTokenAsync(IToken token);
    }

}