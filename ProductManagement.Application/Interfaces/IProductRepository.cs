using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(string? search, int page, int pageSize, bool includeDeleted);
        Task<Product?> GetByIdAsync(int id, bool includeDeleted = false);
        Task<bool> ExistsSkuAsync(string sku, int? excludeId = null);
        Task<Product> AddAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task<bool> SoftDeleteAsync(int id);
    }
}


