using System;
using System.Collections.Generic;
using Polly;
using Polly.CircuitBreaker;
using Polly.Wrap;
using WebApi.Repositories;

namespace WebApi.Policies
{
    public class PolicyRegistry 
    {
        private readonly CircuitBreakerPolicy _productServiceCircuitBreaker;
        private readonly Dictionary<string, Policy> _policies;
        private CircuitBreakerPolicy _databaseAndProductServiceCircuitBreaker;

        public PolicyRegistry()
        {
            _policies = new Dictionary<string, Policy>();
          
            _productServiceCircuitBreaker =
                Policy.Handle<RequestTimeoutException>()
                    .CircuitBreaker(exceptionsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (ex, t) =>
                    {
                        Console.WriteLine("Product Service CB Break");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Product Service CB Reset");
                    });

            _policies.Add("ProductService", _productServiceCircuitBreaker);

            _databaseAndProductServiceCircuitBreaker =
                Policy.Handle<RequestTimeoutException>()
                    .CircuitBreaker(exceptionsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (ex, t) =>
                    {
                        Console.WriteLine("Database & Product CB Break");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Database & Product CB Reset");
                    });

            _policies.Add("Database|ProductService", _databaseAndProductServiceCircuitBreaker);
        }

        public Policy GetDatabasePolicy()
        {            
            return _policies["Database"];
        }

        public Policy GetProductServicePolicy()
        {
            return _productServiceCircuitBreaker;
        }

        public GroupPolicyBuilder GroupPolicy()
        {
            return new GroupPolicyBuilder(this);
        }

        public void Add(Policy policy)
        {
            _policies.Add(policy.PolicyKey, policy);    
        }

        public Policy Get(string policyKey)
        {
            return _policies[policyKey];
        }
    }
}
