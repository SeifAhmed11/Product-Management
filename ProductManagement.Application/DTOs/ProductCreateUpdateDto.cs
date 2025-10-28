using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Application.DTOs
{
    public class ProductCreateUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public string Sku { get; set; } = default!;

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}


