using System;

namespace  Catalog.DTOs
{    
    public record ItemDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreateDate { get; set; }
    }
}