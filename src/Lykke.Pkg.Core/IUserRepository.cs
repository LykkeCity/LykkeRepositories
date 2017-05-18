using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Core
{
    /// <summary>
    /// User repository
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="userEmail">email</param>
        /// <returns></returns>
        Task<IUserEntity> GetUserByUserEmail(string userEmail);
        /// <summary>
        /// Get user by email abd passwordHash
        /// </summary>
        /// <param name="userEmail">email</param>
        /// <param name="passwordHash">passwordHash</param>
        /// <returns></returns>
        Task<IUserEntity> GetUserByUserEmail(string userEmail, string passwordHash);
        /// <summary>
        /// Save user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task<bool> SaveUser(IUserEntity user);
        /// <summary>
        /// Get list of all users
        /// </summary>
        /// <returns></returns>
        Task<List<IUserEntity>> GetUsers();
        /// <summary>
        /// REmove user by userEmail
        /// </summary>
        /// <param name="userEmail">userEmail</param>
        /// <returns></returns>
        Task<bool> RemoveUser(string userEmail);
    }

    public interface IUserEntity : IEntity
    {
        string PasswordHash { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        bool Active { get; set; }
        bool Admin { get; set; }
    }
}
