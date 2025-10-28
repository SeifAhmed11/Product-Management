using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Application.Interfaces;

namespace ProductManagement.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<ProductDto> Items, int TotalCount)> GetAllAsync(string? search, int page, int pageSize, bool includeDeleted)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (entities, total) = await _repository.GetAllAsync(search, page, pageSize, includeDeleted);
            var dtos = entities.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Description = p.Description,
                Price = p.Price
            });
            return (dtos, total);
        }

        public async Task<ProductDto?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var entity = await _repository.GetByIdAsync(id, includeDeleted);
            if (entity == null) return null;

            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Sku = entity.Sku,
                Description = entity.Description,
                Price = entity.Price
            };
        }

        public async Task<ProductDto> AddAsync(ProductCreateUpdateDto dto)
        {
            Validate(dto);

            var exists = await _repository.ExistsSkuAsync(dto.Sku);
            if (exists)
            {
                throw new InvalidOperationException("SKU must be unique");
            }

            var entity = new Product
            {
                Name = dto.Name.Trim(),
                Sku = dto.Sku.Trim(),
                Description = dto.Description,
                Price = dto.Price,
                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.AddAsync(entity);

            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Sku = entity.Sku,
                Description = entity.Description,
                Price = entity.Price
            };
        }

        public async Task<ProductDto?> UpdateAsync(int id, ProductCreateUpdateDto dto)
        {
            Validate(dto);

            var entity = await _repository.GetByIdAsync(id, includeDeleted: true);
            if (entity == null || entity.IsDeleted) return null;

            var skuTaken = await _repository.ExistsSkuAsync(dto.Sku, excludeId: id);
            if (skuTaken)
            {
                throw new InvalidOperationException("SKU must be unique");
            }

            entity.Name = dto.Name.Trim();
            entity.Sku = dto.Sku.Trim();
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);

            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Sku = entity.Sku,
                Description = entity.Description,
                Price = entity.Price
            };
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        private static void Validate(ProductCreateUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required");
            if (dto.Name.Trim().Length > 100)
                throw new ArgumentException("Name max length is 100");
            if (string.IsNullOrWhiteSpace(dto.Sku))
                throw new ArgumentException("SKU is required");
            if (dto.Price <= 0)
                throw new ArgumentException("Price must be greater than 0");
        }
    }
}


