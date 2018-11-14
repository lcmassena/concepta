using Concepta.Application.Domain;
using Concepta.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Concepta.Repositories.AzureTables
{
    public class UserRepository : IUserRepository
    {
        public User GetUser(string username, string password)
        {
            return Users.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
        }

        /// <summary>
        /// In memory list, only for POC purpouses
        /// </summary>
        private IEnumerable<User> Users = new List<User>()
        {
            new User() {Password = "Aa234567!", Username = "test1@test2.com" },
            new User() {Password = "1q2w3e4r", Username = "lucas@massena.com.br" },
            new User() {Password = "teste", Username = "teste@massena.com.br" }
        };
    }
}
