using Microsoft.EntityFrameworkCore;
namespace RedisExample.API.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
             new Product { Id = 1, Name = "Keyboard", Price = 20.00m },
             new Product { Id = 2, Name = "Mouse", Price = 10.00m },
             new Product { Id = 3, Name = "Monitor", Price = 100.00m });

        base.OnModelCreating(modelBuilder);
    }
}
