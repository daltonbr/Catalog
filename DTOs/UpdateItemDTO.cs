using System.ComponentModel.DataAnnotations;

namespace Catalog.DTOs
{
    public record UpdateItemDTO
    {
        // Guid and CreateDate are server generated            
        [Required]
        public string Name { get; init; }
        [Required]
        [Range(0, int.MaxValue)]
        public decimal Price { get; init; }
    }        
}