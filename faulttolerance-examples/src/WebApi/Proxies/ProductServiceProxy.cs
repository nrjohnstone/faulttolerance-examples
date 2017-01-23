using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using WebApi.Policies;
using WebApi.Repositories;

namespace WebApi.Proxies
{
    /// <summary>
    /// Represents an external REST service providing product data
    /// </summary>
    public class ProductServiceProxy : IProductService
    {
        private readonly Policy _productServicePolicy;

        public ProductServiceProxy(PolicyRegistry policyRegistry)
        {
            _productServicePolicy = policyRegistry.GetProductServicePolicy();
        }

        public Product Get(int id)
        {
            if (id == 1)
            {
                return _productServicePolicy.Execute(() => { return new Product(); });
            }

            if (id == 2)
            {
                _productServicePolicy.Execute(() => { throw new RequestTimeoutException(); });
            }

            return null;
        }
    }

    public interface IProductService
    {
        Product Get(int id);
    }

    public class Product
    {
    }
}
