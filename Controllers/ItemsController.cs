using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using System.Collections.Generic;
using Catalog.Models;
using System;

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
        public IEnumerable<Item> GetItems() => _repository.GetItems();

        // GET /item/{id}
        [HttpGet("{id}")]
        //public Item GetItem(Guid id) => repository.GetItem(id);
        public ActionResult<Item> GetItem(Guid id)
        {
            var item = _repository.GetItem(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

    }
}
