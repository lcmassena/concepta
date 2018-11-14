using Concepta.Application.Domain;

namespace Concepta.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Try get an active user on repository comparing userName and password
        /// </summary>
        /// <param name="username">the username/email</param>
        /// <param name="password">user password</param>
        /// <returns></returns>
        User GetUser(string username, string password);
    }
}
