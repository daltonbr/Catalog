using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Collections.Generic;
using Catalog.Models;
using System;
using Catalog.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Controllers
{
    // GET /items
    [ApiController]   
    [Route("items")]
    
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;

        public ItemsController(IItemsRepository repository)
        {
            _repository = repository;
        }

        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync() =>
            (await _repository.GetItemsAsync())
            .Select(item => item.AsDTO());

        // GET /item/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemAsync(Guid id)
        {
            var item = await _repository.GetItemAsync(id);
            if (item == null) return NotFound();
            return Ok(item.AsDTO());
        }

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDTO>> CreateItemAsync(CreateItemDTO itemDTO)
        {           
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDTO.Name,
                Price = itemDTO.Price,
                CreateDate = DateTime.UtcNow
            };
            await _repository.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDTO());            
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDTO itemDTO)
        {
            Item existingItem = await _repository.GetItemAsync(id);
            if (existingItem is null) return NotFound();

            // with-expressions - C#9
            // creates a copy of a record type with the following differences
            Item updatedItem = existingItem with
            {
                Name = itemDTO.Name,
                Price = itemDTO.Price
            };
            await _repository.UpdateItemAsync(updatedItem);
            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            Item existingItem = await _repository.GetItemAsync(id);
            if (existingItem is null) return NotFound();

            await _repository.DeleteItemAsync(id);
            return NoContent();
        }

    }
}
