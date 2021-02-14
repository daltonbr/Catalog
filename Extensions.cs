using Catalog.DTOs;
using Catalog.Models;

namespace Catalog
{
    public static class Extensions
    {
        public static ItemDTO AsDTO(this Item item)
        {
            return new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                CreateDate = item.CreateDate
            };
        }
    }
}
