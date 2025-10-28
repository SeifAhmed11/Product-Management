using System;

namespace ProductManagement.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = default!; // unique
        public string Name { get; set; } = default!; // required, max length 100
        public string? Description { get; set; }
        public decimal Price { get; set; } // > 0
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; } // soft delete
    }
}


