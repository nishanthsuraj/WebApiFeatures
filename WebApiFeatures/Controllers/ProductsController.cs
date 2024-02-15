// Ignore Spelling: Api

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApiFeatures.Db;
using WebApiFeatures.Extensions;
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
        public ActionResult GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = GetSpecificProducts(_context.Products, queryParameters);

            return Ok(products.ToList());
        }

        [HttpGet("async")]
        public async Task<ActionResult> GetAllProductsAsync([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = GetSpecificProducts(_context.Products, queryParameters);

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

        #region Private Methods
        // Pagination Url : https://localhost:7056/api/Products?size=5&page=1
        // Filtering Url: https://localhost:7056/api/Products?MinPrice=20&MaxPrice=50
        // Searching Url: https://localhost:7056/api/Products?ProductName=shirt
        // Sorting Url: https://localhost:7056/api/Products?SortBy=Price&SortOrder=desc
        private IQueryable<Product> GetSpecificProducts(IQueryable<Product> products, ProductQueryParameters queryParameters)
        {
            #region Filtering Criteria
            if (queryParameters.MinPrice != null)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice.Value);
            }
            if (queryParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price <= queryParameters.MaxPrice.Value);
            }
            #endregion

            #region Searching Criteria
            if (!string.IsNullOrEmpty(queryParameters.ProductName))
                products = products.Where(p => p.Name.Contains(queryParameters.ProductName, StringComparison.CurrentCultureIgnoreCase));
            #endregion

            #region Sorting Criteria
            if(!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if(typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(
                        queryParameters.SortBy,
                        queryParameters.SortOrder);
                }
            }
            #endregion

            #region Pagination Criteria
            products = products.Skip(queryParameters.Size * (queryParameters.Page - 1))
                .Take(queryParameters.Size);
            #endregion

            return products;
        }
        #endregion
    }
}
