using System;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using WebApi.Repositories;

namespace WebApi.Policies
{
    public class PolicyRegistry
    {
        private readonly CircuitBreakerPolicy _databaseCircuitBreaker;
        private readonly CircuitBreakerPolicy _productServiceCircuitBreaker;

        public PolicyRegistry()
        {
            _databaseCircuitBreaker =
                Policy.Handle<DatabaseTimeoutException>()
                    .CircuitBreaker(exceptionsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (ex, t) => { Console.WriteLine("Database CB Break"); },
                    onReset: () => { Console.WriteLine("Database CB Reset"); });

            _productServiceCircuitBreaker =
                Policy.Handle<RequestTimeoutException>()
                    .CircuitBreaker(exceptionsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (ex, t) => { Console.WriteLine("Product Service CB Break"); },
                    onReset: () => { Console.WriteLine("Product Service CB Reset"); });
        }

        public Policy GetDatabasePolicy()
        {            
            return _databaseCircuitBreaker;
        }

        public Policy GetProductServicePolicy()
        {
            return _productServiceCircuitBreaker;
        }
    }
}
