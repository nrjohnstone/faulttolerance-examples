using System;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using WebApi.Policies;

namespace WebApi.Repositories
{
    /// <summary>
    /// Represents an external database providing User information
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly Policy _policy;

        public UserRepository(PolicyRegistry policyRegistry)
        {
            _policy = policyRegistry.GetDatabasePolicy();
        }

        public User Get(int id)
        {
            Func<User> query = () => { return null; };
            User user = null;

            if (id == 1)
            {
                query = () => { return new User() {Name = "John Doe"}; };
            }

            if (id == 2)
            {
                query = () => { throw new DatabaseTimeoutException(); };
            }

            user = _policy.Execute(query);

            return user;
        }
    }
}