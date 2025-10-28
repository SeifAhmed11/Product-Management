using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetAllAsync(string? search, int page, int pageSize, bool includeDeleted);
        Task<ProductDto?> GetByIdAsync(int id, bool includeDeleted = false);
        Task<ProductDto> AddAsync(ProductCreateUpdateDto dto);
        Task<ProductDto?> UpdateAsync(int id, ProductCreateUpdateDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}


