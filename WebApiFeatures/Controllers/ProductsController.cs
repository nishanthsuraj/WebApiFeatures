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
        // Pagination Url : https://localhost:7056/api/Products?size=5&page=1
        [HttpGet]
        public ActionResult GetAllProducts([FromQuery] QueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products
               .Skip(queryParameters.Size * (queryParameters.Page - 1))
               .Take(queryParameters.Size);

            return Ok(products.ToList());
        }

        // Pagination Url : https://localhost:7056/api/Products/async?size=5&page=1
        [HttpGet("async")]
        public async Task<ActionResult> GetAllProductsAsync([FromQuery] QueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products
                .Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);

            return Ok(await products.ToListAsync());
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

        /* 
        {
            "sku": "WTF",
            "name": "Suraj Industries",
            "price": 100,
            "categoryId": 1
        }
         */
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


        /* 
        {
            "id": 34
            "sku": "WTF",
            "name": "Suraj Enterprises",
            "price": 100,
            "categoryId": 1
        }
         */
        #region HttpPut
        [HttpPut("{id}")]
        public ActionResult PutProduct(int id, Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}/async")]
        public async Task<ActionResult> PutProductAsync(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
        #endregion

        #region HttpDelete
        [HttpDelete("{id}")]
        public ActionResult<Product> DeleteProduct(int id)
        {
            Product? product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            return product;
        }

        [HttpDelete("{id}/async")]
        public async Task<ActionResult<Product>> DeleteProductAsync(int id)
        {
            Product? product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }
        #endregion

        // https://localhost:7056/api/Products/Delete?ids=1&ids=2&ids=3
        // https://localhost:7056/api/Products/DeleteAsync?ids=1&ids=2&ids=3
        #region Delete using HttpPost
        [HttpPost]
        [Route("Delete")]
        public ActionResult DeleteProducts([FromQuery] int[] ids)
        {
            IList<Product> products = new List<Product>();
            foreach (int id in ids)
            {
                Product? product = _context.Products.Find(id);

                if (product == null)
                    return NotFound();

                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            _context.SaveChanges();

            return Ok(products);
        }

        [HttpPost]
        [Route("Delete/async")]
        public async Task<ActionResult> DeleteProductsAsync([FromQuery] int[] ids)
        {
            IList<Product> products = new List<Product>();
            foreach (int id in ids)
            {
                Product? product = await _context.Products.FindAsync(id);

                if (product == null)
                    return NotFound();

                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }
        #endregion
    }
}
