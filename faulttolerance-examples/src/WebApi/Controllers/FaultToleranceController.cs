using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using WebApi.Policies;
using WebApi.Proxies;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    public class FaultToleranceController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductService _productService;
        private readonly IOrderRepository _orderRepository;
        private readonly PolicyRegistry _policyRegistry;

        public FaultToleranceController(IUserRepository userRepository, IProductService productService,
            IOrderRepository orderRepository, PolicyRegistry policyRegistry)
        {
            _userRepository = userRepository;
            _productService = productService;
            _orderRepository = orderRepository;
            _policyRegistry = policyRegistry;
        }

        [HttpGet("api/user/{id}")]
        public object GetUser(int id)
        {
            try
            {
                 return _userRepository.Get(id);              
            }
            catch (DatabaseTimeoutException)
            {
                Response.StatusCode = 408;
                return "Database timeout";
            }
            catch (BrokenCircuitException)
            {
                Response.StatusCode = 503;
                Response.Headers.Add("Retry-After", "60");
                return null;
            }
        }

        [HttpGet("api/product/{id}")]
        public object GetProduct(int id)
        {
            return _policyRegistry.GroupPolicy()
               .Using("ProductService")
               .GetPolicy()
               .Execute(() => { return _productService.Get(id); });            
        }

        [HttpGet("api/order/{id}")]
        public object GetOrder(int id)
        {
            return _policyRegistry.GroupPolicy()
                .Using("Database")
                .Using("ProductService")
                .GetPolicy()
                .Execute(() =>
                {
                    var product = _productService.Get(id);
                    return _orderRepository.Get(id);
                });            
        }
    }
}