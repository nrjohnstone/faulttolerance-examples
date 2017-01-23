using Polly;
using WebApi.Policies;

namespace WebApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Policy _databasePolicy;

        public OrderRepository(PolicyRegistry policyRegistry)
        {
            _databasePolicy = policyRegistry.GetDatabasePolicy();
        }

        public Order Get(int id)
        {
            if (id == 1)
            {
                return _databasePolicy.Execute(() => { return new Order(); });
            }

            if (id == 2)
            {
                _databasePolicy.Execute(() => { throw new DatabaseTimeoutException(); });
            }

            return null;
        }
    }
}