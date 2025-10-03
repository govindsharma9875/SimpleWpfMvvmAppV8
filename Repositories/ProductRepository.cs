using Microsoft.EntityFrameworkCore;
using SimpleWpfToMvcApp.Web.Data;
using SimpleWpfToMvcApp.Web.Models;

namespace SimpleWpfToMvcApp.Web.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<bool> SKUExistsAsync(string sku, int? excludeId = null);
}

public class ProductRepository(AppDbContext context) : IProductRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SKUExistsAsync(string sku, int? excludeId = null)
    {
        return await _context.Products
            .AnyAsync(p => p.SKU == sku && p.IsActive && (excludeId == null || p.Id != excludeId));
    }
}
