using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Persistence;

namespace ProductManagement.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetAllAsync(string? search, int page, int pageSize, bool includeDeleted)
        {
            IQueryable<Product> query = _dbContext.Products.AsQueryable();
            if (includeDeleted) query = query.IgnoreQueryFilters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(p => p.Name.Contains(term) || p.Sku.Contains(term));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<Product?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            IQueryable<Product> query = _dbContext.Products.AsQueryable();
            if (includeDeleted) query = query.IgnoreQueryFilters();
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<bool> ExistsSkuAsync(string sku, int? excludeId = null)
        {
            var q = _dbContext.Products.IgnoreQueryFilters().AsQueryable();
            if (excludeId.HasValue)
            {
                return q.AnyAsync(p => p.Sku == sku && p.Id != excludeId.Value);
            }
            return q.AnyAsync(p => p.Sku == sku);
        }

        public async Task<Product> AddAsync(Product entity)
        {
            _dbContext.Products.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Product entity)
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _dbContext.Products.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null || entity.IsDeleted) return false;
            entity.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}


