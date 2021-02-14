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

    }
}
