using System.Collections.Generic;
using Catalog.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Item>> GetItemsAsync() =>
            await Task.FromResult(_items);

        public async Task<Item> GetItemAsync(Guid id)
        {
            var item = _items.Where(item => item.Id == id).SingleOrDefault();
            return await Task.FromResult(item);
        } 

        public async Task CreateItemAsync(Item item)
        {
            _items.Add(item);
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item)
        {
            Guid id = item.Id;
            int index = _items.FindIndex(existingItem => existingItem.Id == item.Id);
            //if (index == -1) return;
            _items[index] = item;
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var index = _items.FindIndex(existingItem => existingItem.Id == id);
            //if (index == -1) return;
            _items.RemoveAt(index);
            await Task.CompletedTask;
        }
    }
}
