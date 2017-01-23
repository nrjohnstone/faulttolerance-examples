using System;
using Microsoft.AspNetCore.Mvc;
using WebApi.Proxies;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    public class FaultToleranceController
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductService _productService;

        public FaultToleranceController(IUserRepository userRepository, IProductService productService)
        {
            _userRepository = userRepository;
            _productService = productService;
        }

        [HttpGet("api/user/{id}")]
        public object User(int id)
        {
            return _userRepository.Get(id);
        }

        [HttpGet("api/product/{id}")]
        public object Product(int id)
        {
            return _productService.Get(id);
        }
    }
}