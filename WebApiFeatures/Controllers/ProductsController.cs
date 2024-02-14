﻿// Ignore Spelling: Api

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiFeatures.Db;
using WebApiFeatures.Models;

namespace WebApiFeatures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;
        public ProductsController(ShopContext shopContext)
        {
            _context = shopContext;

            _context.Database.EnsureCreated(); // Ensures all the In-memory data is created.
        }

        #region HttpGet
        [HttpGet]
        public ActionResult GetAllProducts()
        {
            return Ok(_context.Products.ToList());
        }

        [HttpGet("async")]
        public async Task<ActionResult> GetAllProductsAsync()
        {
            return Ok(await _context.Products.ToListAsync());
        }

        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            Product? isFound = _context.Products.Find(id);
            if (isFound == null)
            {
                return NotFound();
            }

            return Ok(isFound);
        }

        [HttpGet("{id}/async")]
        public async Task<ActionResult> GetProductAsync(int id)
        {
            Product? isFound = await _context.Products.FindAsync(id);
            if (isFound == null)
            {
                return NotFound();
            }

            return Ok(isFound);
        }
        #endregion

        #region HttpPost
        [HttpPost]
        public ActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPost("async")]
        public async Task<ActionResult> PostProductAsync(Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductAsync", new { id = product.Id }, product);
        }
        #endregion
    }
}
