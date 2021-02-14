using System.Collections.Generic;
using Catalog.Models;
using System;
using System.Linq;

namespace Catalog.Repositories
{
    public class InMemItemsRepository
    {
        private readonly List<Models.Item> items = 
            new() {
                new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreateDate = System.DateTimeOffset.UtcNow },
                new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreateDate = System.DateTimeOffset.UtcNow },
                new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreateDate = System.DateTimeOffset.UtcNow }
            };

        public IEnumerable<Item> GetItems() => items;

        public Item GetItem(Guid id) => items.Where(item => item.Id == id).SingleOrDefault();
    }
}
