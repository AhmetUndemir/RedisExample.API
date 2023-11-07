using RedisExample.API.Models;
using RedisExample.API.Repositories;

namespace RedisExample.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> AddAsync(Product product)
    {
        return await _productRepository.AddAsync(product);
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }
}
