using RedisExample.API.Models;
using RedisExample.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisExample.API.Repositories;

public class ProductRepositoryWithCacheDecorator : IProductRepository
{
    private readonly IProductRepository _productRepository;
    private readonly RedisService _redisService;
    private const string _cacheKey = "products";
    private readonly IDatabase _cacheRepository;

    public ProductRepositoryWithCacheDecorator(IProductRepository productRepository, RedisService redisService, IDatabase cacheRepository)
    {
        _productRepository = productRepository;
        _redisService = redisService;
        _cacheRepository = _redisService.GetDb(0);
    }

    public async Task<Product> AddAsync(Product product)
    {
        var newProduct = await _productRepository.AddAsync(product);

        if (await _cacheRepository.KeyExistsAsync(_cacheKey))
        {
            await _cacheRepository.HashSetAsync(_cacheKey, newProduct.Id, JsonSerializer.Serialize(newProduct));
        }

        return newProduct;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        if (!await _cacheRepository.KeyExistsAsync(_cacheKey))
        {
            return await LoadToCacheFromDbAsync();
        }

        var products = new List<Product>();

        var cacheProducts = await _cacheRepository.HashGetAllAsync(_cacheKey);

        foreach (var cacheProduct in cacheProducts)
        {
            products.Add(JsonSerializer.Deserialize<Product>(cacheProduct.Value));
        }

        return products;
    }

    public async Task<Product> GetByIdAsync(int id)
    {

        if (_cacheRepository.KeyExists(_cacheKey))
        {
            var product = await _cacheRepository.HashGetAsync(_cacheKey, id);
            return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
        }

        var products = await LoadToCacheFromDbAsync();
        
        return products.FirstOrDefault(p => p.Id == id);
    }

    private async Task<List<Product>> LoadToCacheFromDbAsync()
    {
        var products = await _productRepository.GetAllAsync();

        products.ForEach(p =>
        {
            _cacheRepository.HashSetAsync(_cacheKey, p.Id, JsonSerializer.Serialize(p));
        });

        return products;
    }
}
