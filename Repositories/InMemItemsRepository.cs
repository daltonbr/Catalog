using System.Collections.Generic;
using Catalog.Models;
using System;
using System.Linq;

namespace Catalog.Repositories
{
    public class InMemItemsRepository : IItemsRepository
    {
        private readonly List<Item> _items =
            new()
            {
                new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreateDate = System.DateTimeOffset.UtcNow },
                new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreateDate = System.DateTimeOffset.UtcNow },
                new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreateDate = System.DateTimeOffset.UtcNow }
            };

        public IEnumerable<Item> GetItems() => _items;

        public Item GetItem(Guid id) => _items.Where(item => item.Id == id).SingleOrDefault();

        public void CreateItem(Item item) => _items.Add(item);

        public void UpdateItem(Item item)
        {
            Guid id = item.Id;
            int index = _items.FindIndex(existingItem => existingItem.Id == item.Id);
            //if (index == -1) return;
            _items[index] = item;
        }

        public void DeleteItem(Guid id)
        {
            var index = _items.FindIndex(existingItem => existingItem.Id == id);
            //if (index == -1) return;
            _items.RemoveAt(index);
        }
    }
}
