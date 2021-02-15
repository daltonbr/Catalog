using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Collections.Generic;
using Catalog.Models;
using System;
using Catalog.DTOs;
using System.Linq;

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
        public IEnumerable<ItemDTO> GetItems() => _repository.GetItems().Select(item => item.AsDTO());

        // GET /item/{id}
        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetItem(Guid id)
        {
            var item = _repository.GetItem(id);
            if (item == null) return NotFound();
            return Ok(item.AsDTO());
        }

        // POST /items
        [HttpPost]
        public ActionResult<ItemDTO> CreateItem(CreateItemDTO itemDTO)
        {           
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDTO.Name,
                Price = itemDTO.Price,
                CreateDate = DateTime.UtcNow
            };
            _repository.CreateItem(item);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item.AsDTO());            
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDTO itemDTO)
        {
            Item existingItem = _repository.GetItem(id);
            if (existingItem is null) return NotFound();

            // with-expressions - C#9
            // creates a copy of a record type with the following differences
            Item updatedItem = existingItem with
            {                
                Name = itemDTO.Name,
                Price = itemDTO.Price
            };
            _repository.UpdateItem(updatedItem);
            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)        
        {
            Item existingItem = _repository.GetItem(id);
            if (existingItem is null) return NotFound();

            _repository.DeleteItem(id);
            return NoContent();
        }

    }
}
