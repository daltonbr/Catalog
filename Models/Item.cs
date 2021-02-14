using System;

namespace Catalog.Models
{
    // Record types - C#9 feature
    // - Use for immutable objects
    // - With-expressions support
    // - Value-based equality support

    public record Item
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreateDate { get; init; }
    }
}