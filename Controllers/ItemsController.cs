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
        private readonly InMemItemsRepository repository;

        public ItemsController()
        {
            repository = new InMemItemsRepository();
        }

        // GET /items
        [HttpGet]
        public IEnumerable<Item> GetItems() => repository.GetItems();

        // GET /item/{id}
        [HttpGet("{id}")]
        //public Item GetItem(Guid id) => repository.GetItem(id);
        public ActionResult<Item> GetItem(Guid id)
        {
            var item = repository.GetItem(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

    }
}
