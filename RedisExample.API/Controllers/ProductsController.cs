﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisExample.API.Models;
using RedisExample.API.Repositories;
using RedisExample.Cache;
using StackExchange.Redis;

namespace RedisExample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        private readonly IDatabase _database;

        public ProductsController(IProductRepository productRepository,IDatabase database)
        {
            _productRepository = productRepository;

            _database = database;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
    }
}
